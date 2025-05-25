using FishingTrainer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore.UI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Extensions;
using StardewValley.Menus;

class Shaker
{
    public int Min;
    public int Max;
    public float SmoothFactor;

    public Vector2 Shake
    {
        get =>
            new Vector2(
                Game1.random.Next(Min, Max),
                Game1.random.Next(Min, Max)
            ) / SmoothFactor;
    }

    public Shaker(int min, int max, float smoothFactor)
    {
        Min = min;
        Max = max;
        SmoothFactor = smoothFactor;
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
            default:
                Acceleration = 0;
                break;
        }
    }
}

class Bobber
{
    public static Shaker Shaker = new Shaker(-10, 11, 10f);
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

class BobberBar
{
    public static Shaker Shaker = new Shaker(-10, 11, 10f);
    public const int BobberBarBaseHeight = 96;
    public const int BobberBarIncPerLevel = 8;
    public const int PositionMax = 141 * 4;
    public const float Acceleration = 0.25f;
    public int Height = BobberBarBaseHeight;
    private float _position;
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

    public BobberBar()
    {
        Position = PositionMax;
    }

    public void onTick()
    {
    }
}

class Treasure
{
    public static Shaker Shaker = new Shaker(-3, 2, 1f);
    public const int Height = 24 * 2;
    public const int PositionMax = 141 * 4 - Height;
    public const int PositionMin = 8;
    public bool isHidden = false;
    public bool isCaught = false;
    public float CatchProgress = 0.3f;
    public float appearTimer = 0;
    private float _position = 0;
    public float Position
    {
        get => _position;
        set
        {
            _position = Math.Clamp(value, PositionMin, PositionMax);
        }
    }

    public Treasure()
    {
        Position = PositionMax;
    }
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
    public const int ChallengeFishMax = 3;
    public bool hasChallengeBait = false;
    private int challengeFish;
    public int ChallengeFish
    {
        get => challengeFish;
        set
        {
            challengeFish = Math.Clamp(value, 0, ChallengeFishMax);
        }
    }
}

class Catch
{
    public float ProgressIncPerTick = 0.002f;
    public float ProgressDecPerTick = 0.003f;
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
    public float ReelRotation = 0;
}


class ExBobberBar : IClickableMenu
{
    public RootElement Ui;
    public int TrapBobberCount = 0;
    public int CorkBobberCount = 0;
    public bool isPerfect = true;
    public bool showDebugHint = true;
    public bool hasBlessingOfWaters = false;
    public int FishingLevel = 0;
    public Fish Fish = new Fish();
    public Reel Reel = new Reel();
    public Catch Catch = new Catch();
    public Bobber Bobber = new Bobber();
    public Treasure Treasure = new Treasure();
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
    public bool TreasureInBar
    {
        get
        {
            float TreasureTop = Treasure.Position;
            float TreasureBottom = Treasure.Position + Treasure.Height;

            float BobberBarTop = BobberBar.Position;
            float BobberBarBottom = BobberBar.Position + BobberBar.Height;

            return TreasureBottom <= BobberBarBottom && TreasureTop >= BobberBarTop;
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

        // Progress LeftTop
        var X_PL = X + 32 * 4;
        var Y_PT = Y + 2 * 4;

        // Catch Progress
        b.Draw(Game1.staminaRect, new Rectangle(X_PL, Y_PT + (int)(144 * 4 * (1f - Catch.CatchProgress)), 4 * 4, (int)(144 * 4 * Catch.CatchProgress)), Utility.getRedToGreenLerpColor(Catch.CatchProgress));

        // Bar LeftTop
        var X_BL = X + 17 * 4;
        var Y_BT = Y + 3 * 4;

        //BobberBar
        var BarShake = BobberInBar || TreasureInBar ? Vector2.Zero : BobberBar.Shaker.Shake;
        var BarColor = BobberInBar || TreasureInBar ? Color.White : (Color.White * 0.25f * ((float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 100.0), 2) + 2f));
        b.Draw(Game1.mouseCursors, new Vector2(X_BL, Y_BT + (int)BobberBar.Position) + BarShake, new Rectangle(682, 2078, 9, 2), BarColor, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);
        b.Draw(Game1.mouseCursors, new Vector2(X_BL, Y_BT + (int)BobberBar.Position + 8) + BarShake, new Rectangle(682, 2081, 9, 1), BarColor, 0f, Vector2.Zero, new Vector2(4f, BobberBar.Height - 16), SpriteEffects.None, 0.89f);
        b.Draw(Game1.mouseCursors, new Vector2(X_BL, Y_BT + (int)BobberBar.Position + BobberBar.Height - 8) + BarShake, new Rectangle(682, 2085, 9, 2), BarColor, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);

        if (showDebugHint)
        {
            // BobberBar Top
            b.Draw(Game1.staminaRect, new Rectangle(X_BL, Y_BT + (int)BobberBar.Position, 9 * 4, 4), Color.Red);
            // BobberBar Bottom
            b.Draw(Game1.staminaRect, new Rectangle(X_BL, Y_BT - 4 + (int)(BobberBar.Position + BobberBar.Height), 9 * 4, 4), Color.White);
        }

        //Bobber
        var BobberShake = BobberInBar ? Bobber.Shaker.Shake : Vector2.Zero;
        b.Draw(Game1.mouseCursors, new Vector2(X_BL, Y_BT + Bobber.Position) + BobberShake, new Rectangle(614, 1840, 20, 20), Color.White, 0f, new Vector2(1f, 1f), 2f, SpriteEffects.None, 0.88f);

        if (showDebugHint)
        {
            // Bobber Top
            b.Draw(Game1.staminaRect, new Rectangle(X_BL, Y_BT + (int)Bobber.Position, 9 * 4, 4), Color.Red);
            // Bobber Bottom
            b.Draw(Game1.staminaRect, new Rectangle(X_BL, Y_BT - 4 + (int)(Bobber.Position + Bobber.Height), 9 * 4, 4), Color.White);
        }

        if (!Treasure.isHidden && !Treasure.isCaught)
        {
            // Treasure
            var TreasureShake = TreasureInBar ? Treasure.Shaker.Shake : Vector2.Zero;
            b.Draw(Game1.mouseCursors, new Vector2(X_BL, Y_BT + Treasure.Position) + TreasureShake, new Rectangle(638, 1865, 20, 24), Color.White, 0f, new Vector2(2f, 0f), 2f, SpriteEffects.None, 0.85f);

            if (showDebugHint)
            {
                // Treasure Top
                b.Draw(Game1.staminaRect, new Rectangle(X_BL, Y_BT + (int)Treasure.Position, 9 * 4, 4), Color.Red);
                // Treasure Bottom
                b.Draw(Game1.staminaRect, new Rectangle(X_BL, Y_BT - 4 + (int)(Treasure.Position + Treasure.Height), 9 * 4, 4), Color.White);
            }

            if (Treasure.CatchProgress > 0f)
            {
                b.Draw(Game1.staminaRect, new Rectangle(X_BL + 2, Y_BT - 8 + (int)Treasure.Position, 8 * 4, 8), Color.DimGray * 0.5f);
                b.Draw(Game1.staminaRect, new Rectangle(X_BL + 2, Y_BT - 8 + (int)Treasure.Position, (int)(Treasure.CatchProgress * 8 * 4), 8), Color.Orange);
            }
        }

        // Reel
        b.Draw(Game1.mouseCursors, new Vector2(X + 5 * 4, Y + 129 * 4), new Rectangle(257, 1990, 5, 10), Color.White, Reel.ReelRotation, new Vector2(2f, 10f), 4f, SpriteEffects.None, 0.9f);

        drawMouse(b);

        Game1.EndWorldDrawInUI(b);
    }
}