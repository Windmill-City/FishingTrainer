using FishingTrainer;
using StardewValley;

class Fish
{
    public const int TimePerFishSizeReduction = 800;

    public FishingGame Context;

    public FishData Content;

    public int FishSizeReductionTimer;

    private int _size = 0;
    public int Size
    {
        get => _size;
        set
        {
            _size = Math.Clamp(value, Content.minSize, Content.maxSize);
        }
    }

    public Fish(FishingGame game)
    {
        Context = game;

        List<FishData>? fishData;
        FishContent.GetFishContents().TryGetValue(MotionType.Smooth, out fishData);
        Content = fishData!.First();

        Reset();
    }

    public void Reset()
    {
        FishSizeReductionTimer = TimePerFishSizeReduction;

        var RandomSizeBase = Game1.random.NextDouble();

        Size = (int)(Content.minSize + (Content.maxSize - Content.minSize) * RandomSizeBase) + 1;
        Content.fishObject!.Quality = (!(RandomSizeBase < 0.33)) ? (RandomSizeBase < 0.66 ? 1 : 2) : 0;
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
                Size -= 1;
                FishSizeReductionTimer = TimePerFishSizeReduction;
            }
        }

        Context.isPerfect = false;
    }
}