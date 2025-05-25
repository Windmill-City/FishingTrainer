using FishingTrainer;
using StardewValley;
using StardewValley.Characters;

class Catch
{
    public const float ProgressIncPerTick = 0.002f;

    public FishingGame Context;

    public bool isCaught;

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
        CatchProgress = 0.3f;
    }

    public void onTick()
    {
        var prvCatchProgress = CatchProgress;

        CatchProgress += Context.BobberInBar ? ProgressIncPerTick : ProgressDecPerTick;

        // Just Zero
        if (CatchProgress == 0 && prvCatchProgress > 0)
        {
            Game1.playSound("fishEscape");
        }

        if (CatchProgress == 1)
        {
            // minor reset - for better experience in non-full reset mode
            CatchProgress = 0.3f;
            Context.isPerfect = true;

            isCaught = true;
            Game1.playSound("jingle1");
        }
    }
}