using FishingTrainer;
using StardewValley;

class BobberBar
{
    public static Shaker Shaker = new Shaker(-10, 11, 10f);

    public const int BobberBarBaseHeight = 96;
    public const int BobberBarIncPerLevel = 8;
    public const int PositionMax = 141 * 4;
    public const float AccelerationBase = 0.25f;

    public FishingGame Context;

    public float Velocity;
    public float Acceleration;

    public bool AtTop => Position == 0;
    public bool AtBottom => Position == PositionMax - Height;

    private float _position;
    public float Position
    {
        get => _position;
        set
        {
            _position = Math.Clamp(value, 0, PositionMax - Height);
        }
    }

    public int Height
    {
        get
        {
            var height = BobberBarBaseHeight;
            // Fishing level
            height += ModEntry.Config.FishingLevel * 8;
            // Cork Bobber
            height += ModEntry.Config.CorkBobber * 24;
            // Deluxe Bait
            height += ModEntry.Config.hasDeluxeBait ? 12 : 0;
            return height;
        }
    }

    public BobberBar(FishingGame game)
    {
        Context = game;
        Reset();
    }

    public void Reset()
    {
        Velocity = Acceleration = 0;
        _position = PositionMax - Height;
    }

    public void onTick()
    {
        Acceleration = Context.isPressed ? -AccelerationBase : AccelerationBase;

        // if key pressed when the bar at top/bottom, then zero its velocity
        if (Context.isPressed && (AtTop || AtBottom))
        {
            Velocity = 0;
        }

        if (Context.BobberInBar)
        {
            Acceleration *= 0.5f;
        }

        Context.BarbedHook.onTick();

        Velocity += Acceleration;

        var prvPosition = Position;
        var prvInBar = Context.BobberInBar || Context.TreasureInBar;

        Position += Velocity;

        // just leave
        if (prvInBar && !(Context.BobberInBar || Context.TreasureInBar))
        {
            Game1.playSound("tinyWhip");
        }

        var justHitTop = AtTop && prvPosition - _position > 0;
        var justHitBottom = AtBottom && prvPosition - _position < 0;

        if (justHitTop || justHitBottom)
        {
            Velocity = -Velocity * 2f / 3f;
            Game1.playSound("shiny4");
        }

        if (justHitBottom && ModEntry.Config.LeadBobber > 0)
        {
            Velocity *= ModEntry.Config.LeadBobber * 0.1f;
        }
    }
}