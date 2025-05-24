using FishingTrainer;
using Force.DeepCloner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore.UI;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Menus;

abstract class Shakeable
{
    public Vector2 Shake
    {
        get =>
            new Vector2(Game1.random.Next(-10, 11), Game1.random.Next(-10, 11)) / 10f;
    }
}

enum MotionType
{
    Mixed,
    Dart,
    Smooth,
    Sink,
    Floater,
}

class FloaterSinker
{
    public const float AccelerationPerTick = 0.01f;
    public const float AccelerationMax = 1.5f;
    private float acceleration = 0;
    public float Acceleration
    {
        get => acceleration;
        set => acceleration = Math.Clamp(value, -AccelerationMax, AccelerationMax);
    }

    public void onTick(MotionType motionType)
    {
        switch (motionType)
        {
            case MotionType.Sink:
                Acceleration += AccelerationPerTick;
                break;
            case MotionType.Floater:
                Acceleration -= AccelerationPerTick;
                break;
        }
    }
}

class Bobber : Shakeable
{
    public const int Height = 20 * 2;
    public const int PositionMax = 141 * 4 - Height;
    public const float PositionCloseMargin = 3f;
    public const float DartPositionMin = 50;
    public const float DartPositionMax = 100;
    public const float VelocitySmoothingFactor = 5f;
    public MotionType motionType = MotionType.Smooth;
    public float difficulty = 30;
    public bool isSlidingOrStill = true;
    public float Velocity = 0;
    public float Acceleration = 0;
    public FloaterSinker floaterSinker = new FloaterSinker();
    public bool CloseToTarget
    {
        get => Math.Abs(Position - TargetPosition) <= PositionCloseMargin;
    }
    public double ChanceVeer
    {
        get
        {
            if (motionType == MotionType.Smooth)
                return difficulty * 20 / 4000f;
            else// Mixed/Dart/Floater/Sinker
                return difficulty / 4000f;
        }
    }
    public double ChanceDart
    {
        get
        {
            if (motionType == MotionType.Smooth)
                return 0;
            else// Mixed/Dart/Floater/Sinker
                return difficulty / 2000f;
        }
    }
    public double ChanceQuickDart
    {
        get
        {
            if (motionType == MotionType.Mixed)
                return difficulty / 1000f;
            else// Smooth/Dart/Floater/Sinker
                return 0;
        }
    }
    public bool RandomVeer
    {
        get
        {
            // Smooth type doesn't veers when it is moving
            if (motionType == MotionType.Smooth && !isSlidingOrStill)
                return false;
            else
                return Game1.random.NextDouble() < ChanceVeer;
        }
    }
    public bool RandomDart
    {
        // Dart when bobber moving slowly
        get => isSlidingOrStill && Game1.random.NextDouble() < ChanceDart;
    }
    public bool RandomQuickDart
    {
        get => Game1.random.NextDouble() < ChanceQuickDart;
    }
    public float RandomVeerPosition
    {
        get
        {
            float spaceBottom = PositionMax - Position;
            float spaceAbove = Position;

            // 10% ~ 45%
            float randomScale = Game1.random.Next(10, 45);
            // 10% ~ 99%
            float deltaScale = Math.Min(randomScale + difficulty, 99f) / 100f;
            // Move to top or bottom
            float deltaPosition = Game1.random.Next((int)-spaceAbove, (int)spaceBottom);

            return Position + deltaPosition * deltaScale;
        }
    }
    public float RandomDartPosition
    {
        get
            => Position
            + (Game1.random.NextBool() ? -1 : 1)
            * Game1.random.Next((int)DartPositionMin, (int)DartPositionMax);
    }
    public float RandomQuickDartPosition
    {
        get
            => Position
            + (Game1.random.NextBool() ? -1 : 1)
            * Game1.random.Next((int)DartPositionMin, (int)DartPositionMax + (int)difficulty * 2);
    }
    public float RandomAcceleration
    {
        get
        {
            float force = TargetPosition - Position;

            float reduction;
            reduction = Game1.random.Next(10, 30);
            reduction += Math.Max(0, 100 - difficulty);

            return force / reduction;
        }
    }
    private float _targetPosition = 0;
    public float TargetPosition
    {
        get => _targetPosition;
        set => _targetPosition = Math.Clamp(value, 0, PositionMax);
    }
    private float _position = PositionMax;
    public float Position
    {
        get => _position;
        set => _position = Math.Clamp(value, 0, PositionMax);
    }

    public void onTick()
    {
        // accumulate the extra acceleration
        floaterSinker.onTick(motionType);

        if (RandomVeer)
        {
            isSlidingOrStill = false;
            TargetPosition = RandomVeerPosition;
        }

        if (RandomDart)
        {
            isSlidingOrStill = false;
            TargetPosition = RandomDartPosition;
        }

        if (RandomQuickDart)
        {
            isSlidingOrStill = false;
            TargetPosition = RandomQuickDartPosition;
        }

        if (CloseToTarget)
        {
            // The bobber keeps its speed when it get close to the
            // target position, so it behaves like sliding
            isSlidingOrStill = true;
        }
        else
        {
            // if not keeping sliding or still, do update acceleration and speed
            if (!isSlidingOrStill)
            {
                Acceleration = RandomAcceleration;
                // linear leap speed smoothing algorithm
                Velocity += (Acceleration - Velocity) / VelocitySmoothingFactor;
            }
        }
        Position += Velocity + floaterSinker.Acceleration;
    }
}

class BobberBar : Shakeable
{
    public const int PositionMax = 141 * 4;
    public const float Acceleration = 0.25f;
    public int Height = 400;
    private float _position = 0;
    public float Position
    {
        get => _position;
        set
        {
            _position = Math.Clamp(value, 0, PositionMax - Height);
        }
    }
    private float _velocity = 0;
    public float Velocity
    {
        get => _velocity;
        set
        {
            _velocity = value;
        }
    }

    public void onTick()
    {

    }
}

class Treasure : Shakeable
{
    public bool hasCaught = false;
    public bool hasTreasure = true;
    public float position = 0;
    public float catchProgress = 0;
    public float scale = 0;
    public float appearTimer = 0;
}

class Fish
{
    public const int TimePerFishSizeReduction = 800;
    public Item? fishObject = null;
    public int quality = 0;
    public int size = 0;
    public int minSize = 0;
    public int maxSize = 0;
    public int fishSizeReductionTimer = TimePerFishSizeReduction;
}

class ChallengeBait
{
    public const int ChallengeFishes = 3;
    public bool hasChallengeBait = false;
    public int fishesLeft = ChallengeFishes;
}

class Catch
{
    public const float ProgressIncPerTick = 0.002f;
    public int TrapBobberCount = 0;
    public bool isPerfect = true;
    public float ProgressDecPerTick
    {
        get
        {
            switch (TrapBobberCount)
            {
                case 0:
                    return 0.003f; // Reduction = 0.000f
                case 1:
                    return 0.002f; // Reduction = 0.001f
                case 2:
                    return 0.0015f;// Reduction = 0.0015f
                default:
                    return 0.001f; // Reduction = 0.002f
            }
        }
    }
    private float _catchProgress = 1f;
    public float CatchProgress
    {
        get => _catchProgress;
        set
        {
            _catchProgress = Math.Clamp(value, 0, 1);
        }
    }
}


class Reel
{
    public ICue? reelSound = null;
    public ICue? unReelSound = null;
    public float reelRotation = 0;
}


class ExBobberBar : IClickableMenu
{
    public RootElement Ui;
    public bool hasBlessingOfWaters = false;
    public Fish Fish = new Fish();
    public Catch Catch = new Catch();
    public Bobber Bobber = new Bobber();
    public BobberBar BobberBar = new BobberBar();
    public bool BobberInBar
    {
        get
        {
            float BobberTop = Bobber.Position;
            float BobberBottom = Bobber.Position + Bobber.Height;

            float BobberBarTop = BobberBar.Position;
            float BobberBarBottom = BobberBar.Position + BobberBar.Height;

            return BobberBottom <= BobberBarBottom && BobberTop >= BobberBarTop;
        }
    }
    public SparklingText? PerfectText = null;

    public ExBobberBar() : base(0, 0, 320 * 4, 180 * 4)
    {
        Ui = new RootElement();

        var Content = ModEntry.Instance!.Helper.GameContent;

        var Background = new Image
        {
            Texture = Content.Load<Texture2D>("LooseSprites\\troutDerbyLetterBG"),
            TexturePixelArea = new Rectangle(0, 0, 320, 180),
            Scale = 4,
        };
        Ui.AddChild(Background);

        var BobberBarUi = new Image
        {
            Texture = Content.Load<Texture2D>("LooseSprites\\Cursors"),
            TexturePixelArea = new Rectangle(644, 1999, 38, 150),
            Scale = 4,
            LocalPosition = new Vector2(64, 32),
        };
        Ui.AddChild(BobberBarUi);

        Reposition();
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        Reposition();
    }

    private void Reposition()
    {
        Ui.LocalPosition = new Vector2(Ui.Width - 320 * 4, Ui.Height - 180 * 4) / 2;
    }

    public override void update(GameTime time)
    {
        Ui.Update();

        Bobber.onTick();
        BobberBar.onTick();
    }

    public override void draw(SpriteBatch b)
    {
        Game1.StartWorldDrawInUI(b);

        Ui.Draw(b);

        // Position BobberBarUi
        var X = (int)Ui.Position.X + 64;
        var Y = (int)Ui.Position.Y + 32;

        // Catch Progress
        b.Draw(Game1.staminaRect, new Rectangle(X + 32 * 4, Y + 2 * 4 + (int)(144 * 4 * (1f - Catch.CatchProgress)), 4 * 4, (int)(144 * 4 * Catch.CatchProgress)), Utility.getRedToGreenLerpColor(Catch.CatchProgress));

        //BobberBar
        var BarShake = BobberInBar ? Vector2.Zero : BobberBar.Shake;
        var BarColor = BobberInBar ? Color.White : (Color.White * 0.25f * ((float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 100.0), 2) + 2f));
        b.Draw(Game1.mouseCursors, new Vector2(X + 17 * 4, Y + 3 * 4 + (int)BobberBar.Position) + BarShake, new Rectangle(682, 2078, 9, 2), BarColor, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);
        b.Draw(Game1.mouseCursors, new Vector2(X + 17 * 4, Y + 3 * 4 + (int)BobberBar.Position + 8) + BarShake, new Rectangle(682, 2081, 9, 1), BarColor, 0f, Vector2.Zero, new Vector2(4f, BobberBar.Height - 16), SpriteEffects.None, 0.89f);
        b.Draw(Game1.mouseCursors, new Vector2(X + 17 * 4, Y + 3 * 4 + (int)BobberBar.Position + BobberBar.Height - 8) + BarShake, new Rectangle(682, 2085, 9, 2), BarColor, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);
        // BobberBar Top
        b.Draw(Game1.staminaRect, new Rectangle(X + 17 * 4, Y + 3 * 4 + (int)BobberBar.Position, 4 * 9, 4), Color.GreenYellow);
        // BobberBar Bottom
        b.Draw(Game1.staminaRect, new Rectangle(X + 17 * 4, Y + 3 * 4 + (int)(BobberBar.Position + BobberBar.Height), 4 * 9, 4), Color.OrangeRed);

        //Bobber
        var BobberShake = BobberInBar ? Bobber.Shake : Vector2.Zero;
        b.Draw(Game1.mouseCursors, new Vector2(X + 17 * 4, Y + 3 * 4 + Bobber.Position) + BobberShake, new Rectangle(614, 1840, 20, 20), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0.88f);
        // Bobber Top
        b.Draw(Game1.staminaRect, new Rectangle(X + 17 * 4, Y + 3 * 4 + (int)Bobber.Position, 4 * 9, 4), Color.Green);
        // Bobber Bottom
        b.Draw(Game1.staminaRect, new Rectangle(X + 17 * 4, Y + 3 * 4 + (int)(Bobber.Position + Bobber.Height), 4 * 9, 4), Color.Orange);

        drawMouse(b);

        Game1.EndWorldDrawInUI(b);
    }
}