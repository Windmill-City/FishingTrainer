using FishingTrainer;
using StardewValley;

class FishData
{
    public Item fishObject;
    public MotionType motionType;
    public int difficulty;
    public int minSize;
    public int maxSize;

    public FishData(Item fishObject, MotionType motionType, int difficulty, int minSize, int maxSize)
    {
        this.fishObject = fishObject;
        this.motionType = motionType;
        this.difficulty = difficulty;
        this.minSize = minSize;
        this.maxSize = maxSize;
    }
}

class FishContent
{
    public static Dictionary<MotionType, List<FishData>> GetFishContents()
    {
        var Monitor = ModEntry.Instance!.Monitor;

        Dictionary<string, string> dictionary = DataLoader.Fish(Game1.content);

        var FishContents = new Dictionary<MotionType, List<FishData>>();

        foreach ((var fishId, var raw) in dictionary)
        {
            Log.Debug($"{fishId}: {raw}");

            string[] rawContent = raw.Split('/');

            // fish catch by Crab Pot
            if (rawContent[1].ToLower() == "trap") continue;

            MotionType? motionType = rawContent[2].ToLower().asMotionType();
            if (motionType is null)
            {
                Log.Warn($"Unknown MotionType: {rawContent[2]} for Fish: {raw}");
                continue;
            }

            var Content =
                FishContents.GetValueOrDefault((MotionType)motionType, new List<FishData>());
            Content.Add(new FishData
            (
                ItemRegistry.Create(fishId),
                (MotionType)motionType,
                Convert.ToInt32(rawContent[1]),
                Convert.ToInt32(rawContent[3]),
                Convert.ToInt32(rawContent[4])
            ));
            FishContents[(MotionType)motionType] = Content;
        }

        foreach (var key in FishContents.Keys)
        {
            FishContents[key] = FishContents[key]
                .OrderBy(item => item.difficulty).ToList();
        }

        return FishContents;
    }
}