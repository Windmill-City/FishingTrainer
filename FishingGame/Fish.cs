using FishingTrainer;

class Fish
{
    public const int TimePerFishSizeReduction = 800;

    public FishingGame Context;

    public FishObject Obj;

    public int FishSizeReductionTimer;


    public Fish(FishingGame game)
    {
        Context = game;
        Obj = FishContent.GetDefaultFishObject();
        Reset();
    }

    public void Reset()
    {
        FishSizeReductionTimer = TimePerFishSizeReduction;
        Obj.RandomSize();
    }

    public void onTick()
    {
        if (Context.BobberInBar) return;
        if (ModEntry.Config.hasTreasureHunter && Context.TreasureInBar) return;

        if (FishSizeReductionTimer > 0)
        {
            FishSizeReductionTimer--;
            if (FishSizeReductionTimer == 0)
            {
                Obj.Size -= 1;
                FishSizeReductionTimer = TimePerFishSizeReduction;
            }
        }

    }
}