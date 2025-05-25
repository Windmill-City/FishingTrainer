using FishingTrainer;
using StardewModdingAPI;
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
            Monitor.Log($"{fishId}: {raw}", LogLevel.Debug);

            string[] rawContent = raw.Split('/');

            // fish catch by trap
            if (rawContent[1].ToLower() == "trap") continue;

            MotionType motionType;
            switch (rawContent[2].ToLower())
            {
                case "mixed":
                    motionType = MotionType.Mixed;
                    break;
                case "dart":
                    motionType = MotionType.Dart;
                    break;
                case "smooth":
                    motionType = MotionType.Smooth;
                    break;
                case "floater":
                    motionType = MotionType.Floater;
                    break;
                case "sinker":
                    motionType = MotionType.Sinker;
                    break;
                default:
                    Monitor.Log(
                        $"Unknown MotionType: {rawContent[2]} for Fish: {raw}",
                        LogLevel.Warn
                    );
                    continue;
            }

            var Content = FishContents.GetValueOrDefault(motionType, new List<FishData>());
            Content.Add(new FishData
            (
                ItemRegistry.Create(fishId),
                motionType,
                Convert.ToInt32(rawContent[1]),
                Convert.ToInt32(rawContent[3]),
                Convert.ToInt32(rawContent[4])
            ));
            FishContents[motionType] = Content;
        }

        foreach (var key in FishContents.Keys)
        {
            FishContents[key] = FishContents[key]
                .OrderBy(item => item.difficulty).ToList();
        }

        return FishContents;
    }
}