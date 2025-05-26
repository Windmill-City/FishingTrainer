using FishingTrainer;
using StardewValley;

class Catch
{
    public const float ProgressIncPerTick = 0.002f;

    public FishingGame Context;

    public bool isCaught;
    public bool isPerfect;

    private float _catchProgress;
    public float CatchProgress
    {
        get => _catchProgress;
        set
        {
            _catchProgress = Math.Clamp(value, 0, 1);
        }
    }

    public float ProgressDecPerTick
    {
        get
        {
            // Treasure Hunter keep the progress while treasure in bar
            if (ModEntry.Config.hasTreasureHunter && Context.TreasureInBar) return 0;

            var decPerTick = 0.003f;

            // Trap Bobber Reduction
            var reduction = 0.001f;
            for (int i = 0; i < ModEntry.Config.TrapBobber; i++)
            {
                decPerTick -= reduction;
                reduction /= 2;
            }
            decPerTick = -Math.Max(0.001f, decPerTick);

            // Blessing Of Waters Reduction
            if (ModEntry.Config.hasBlessingOfWaters) decPerTick *= 0.5f;

            return decPerTick;
        }
    }

    public Catch(FishingGame game)
    {
        Context = game;
        Reset();
    }

    public void Reset()
    {
        isCaught = false;
        isPerfect = true;
        CatchProgress = 0.3f;
    }

    public void onTick()
    {
        var prvCatchProgress = CatchProgress;

        CatchProgress += Context.BobberInBar ? ProgressIncPerTick : ProgressDecPerTick;

        // just Zero
        if (CatchProgress == 0 && prvCatchProgress > 0)
        {
            if (ModEntry.Config.EscapeSound)
                Game1.playSound("fishEscape");
        }

        if (CatchProgress == 1)
        {
            // minor reset - for better experience in non-full reset mode
            CatchProgress = 0.3f;
            isPerfect = true;

            isCaught = true;

            if (ModEntry.Config.CaughtSound)
                Game1.playSound("jingle1");
        }

        // perfect conditions
        if (Context.BobberInBar) return;
        if (ModEntry.Config.hasTreasureHunter && Context.TreasureInBar) return;

        isPerfect = false;
    }
}