using FishingTrainer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

public class FishObject
{
    private Item Obj;

    public string ItemId => Obj.ItemId;

    public int Difficulty;
    public MotionType Type;

    public string DisplayName => Obj.DisplayName;

    private int _size = 0;
    public int Size
    {
        get => _size;
        set => _size = Math.Clamp(value, SizeMin, SizeMax);
    }

    public int SizeMin;
    public int SizeMax;

    public int Quality
    {
        get => Obj.Quality;
        set => Obj.Quality = value;
    }

    public bool isBossFish
    {
        get
        {
            bool bossFish;
            Obj.TryGetTempData("IsBossFish", out bossFish);
            return bossFish;
        }
    }

    public FishObject(Item fishObject, MotionType motionType, int difficulty, int minSize, int maxSize)
    {
        Obj = fishObject;
        Type = motionType;
        Difficulty = difficulty;
        SizeMin = minSize;
        SizeMax = maxSize;
    }

    public void RandomSize()
    {
        var RandomSizeBase = Game1.random.NextDouble();

        Size = (int)(SizeMin + (SizeMax - SizeMin) * RandomSizeBase) + 1;
        Obj.Quality = (!(RandomSizeBase < 0.33)) ? (RandomSizeBase < 0.66 ? 1 : 2) : 0;
    }

    public void Draw(SpriteBatch b, Vector2 position)
    {
        Obj.drawInMenu(b, position, 1f);
    }
}

public static class FishContent
{
    public static Dictionary<MotionType, List<FishObject>> GetFishContents()
    {
        return DataLoader.Fish(Game1.content)
        .Select(kvp =>
        {
            var rawData = kvp.Value.Split('/').ToList();
            rawData.Insert(0, kvp.Key);
            return rawData;
        })
        .Where(i => i[2] != "trap") // filter object catch by Crab Pot
        .Select(rawData =>
        {
            MotionType type = rawData[3].asMotionType();
            return new
            {
                Key = type,
                Value = new FishObject(
                    ItemRegistry.Create(rawData[0]),
                    type,
                    Convert.ToInt32(rawData[2]),
                    Convert.ToInt32(rawData[4]),
                    Convert.ToInt32(rawData[5])
                )
            };
        })
        .GroupBy(i => i.Key)
        .ToDictionary(g => g.Key, g => g.Select(i => i.Value).OrderBy(i => i.Difficulty).ToList());
    }

    public static FishObject GetDefaultFishObject()
    {
        return GetFishContents()[MotionType.Smooth].First();
    }

    public static FishObject GetPreviousFishObject()
    {
        return GetFishContents()
        .Values
        .SelectMany(i => i)
        .Where(i => i.ItemId == ModEntry.Config.PreviousFishId)
        .FirstOrDefault(GetDefaultFishObject());
    }
}