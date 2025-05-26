using FishingTrainer;

class Fish
{
    public const int TimePerFishSizeReduction = 800;

    public FishingGame Context;

    public FishItem Item;

    public int FishSizeReductionTimer;


    public Fish(FishingGame game)
    {
        Context = game;
        Item = FishItems.GetPreviousFishItem();
        Reset();
    }

    public void Reset()
    {
        FishSizeReductionTimer = TimePerFishSizeReduction;
        Item.RandomSize();
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
                Item.Size -= 1;
                FishSizeReductionTimer = TimePerFishSizeReduction;
            }
        }

    }
}