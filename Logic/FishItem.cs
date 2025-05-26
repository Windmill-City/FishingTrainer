using FishingTrainer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

public class FishItem
{
    private Item Internal;

    public string ItemId => Internal.ItemId;

    public int Difficulty;
    public MotionType Type;

    public string DisplayName => Internal.DisplayName;

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
        get => Internal.Quality;
        set => Internal.Quality = value;
    }

    public bool isBossFish
    {
        get
        {
            bool bossFish;
            Internal.TryGetTempData("IsBossFish", out bossFish);
            return bossFish;
        }
    }

    public FishItem(Item fishObject, MotionType motionType, int difficulty, int minSize, int maxSize)
    {
        Internal = fishObject;
        Type = motionType;
        Difficulty = difficulty;
        SizeMin = minSize;
        SizeMax = maxSize;
    }

    public void RandomSize()
    {
        var RandomSizeBase = Game1.random.NextDouble();

        Size = (int)(SizeMin + (SizeMax - SizeMin) * RandomSizeBase) + 1;
        Internal.Quality = (!(RandomSizeBase < 0.33)) ? (RandomSizeBase < 0.66 ? 1 : 2) : 0;
    }

    public void Draw(SpriteBatch b, Vector2 position)
    {
        Internal.drawInMenu(b, position, 1f);
    }
}

public static class FishItems
{
    public static Dictionary<MotionType, List<FishItem>> GetFishItems()
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
                Value = new FishItem(
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

    public static FishItem GetDefaultFishItem()
    {
        return GetFishItems()[MotionType.Smooth].First();
    }

    public static FishItem GetPreviousFishItem()
    {
        return GetFishItems()
        .Values
        .SelectMany(i => i)
        .Where(i => i.ItemId == ModEntry.Config.PreviousFishId)
        .FirstOrDefault(GetDefaultFishItem());
    }
}