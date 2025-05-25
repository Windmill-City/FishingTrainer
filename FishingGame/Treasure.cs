using FishingTrainer;
using StardewValley;

class Treasure
{
    public static Shaker Shaker = new Shaker(-3, 2, 1f);

    public const int Height = 24 * 2;
    public const int PositionMax = 141 * 4 - Height;
    public const int PositionMin = 8;
    public const float ProgressIncPerTick = 0.0135f;
    public const float ProgressDecPerTick = -0.01f;

    public FishingGame Context;

    public bool isHidden;
    public bool isCaught;

    public int RandomAppearTime => Game1.random.Next(60, 60 * 3);
    public float AppearTimer;

    private float _position;
    public float Position
    {
        get => _position;
        set
        {
            _position = Math.Clamp(value, PositionMin, PositionMax);
        }
    }

    public float RandomPosition
    {
        get
        {
            var Bar = Context.BobberBar;
            if (Bar.Position > BobberBar.PositionMax / 2)
            {
                // generate above the bar
                return Game1.random.Next(PositionMin, (int)(Bar.Position - Height));
            }
            else
            {
                // generate below the bar
                return Game1.random.Next((int)(Bar.Position + Bar.Height), PositionMax);
            }
        }
    }

    private float _catchProgress;
    public float CatchProgress
    {
        get => _catchProgress;
        set
        {
            _catchProgress = Math.Clamp(value, 0, 1);
        }
    }

    public Treasure(FishingGame game)
    {
        Context = game;
        Reset();
    }

    public void Reset()
    {
        isHidden = true;
        isCaught = false;
        CatchProgress = 0;
        AppearTimer = RandomAppearTime;
    }

    public void onTick()
    {
        if (!ModEntry.Config.hasTreasure) return;

        if (isHidden)
        {
            if (AppearTimer > 0)
            {
                AppearTimer--;
            }
            else
            {
                Position = RandomPosition;
                isHidden = false;
                Game1.playSound("dwop");
            }
        }

        if (!isHidden && !isCaught)
        {
            CatchProgress += Context.TreasureInBar ? ProgressIncPerTick : ProgressDecPerTick;
            if (CatchProgress == 1)
            {
                isCaught = true;
                Game1.playSound("newArtifact");
            }
        }
    }
}