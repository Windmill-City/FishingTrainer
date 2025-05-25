using Microsoft.Xna.Framework;
using StardewValley;

class Shaker
{
    public int Min;
    public int Max;
    public float SmoothFactor;

    public Vector2 Shake
    {
        get =>
            new Vector2(
                Game1.random.Next(Min, Max),
                Game1.random.Next(Min, Max)
            ) / SmoothFactor;
    }

    public Shaker(int min, int max, float smoothFactor)
    {
        Min = min;
        Max = max;
        SmoothFactor = smoothFactor;
    }
}