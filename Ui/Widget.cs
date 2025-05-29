using FishingTrainer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

public class TextureSlice
{
    public readonly Texture2D Texture;
    public readonly Rectangle Source;
    public readonly Vector2 Origin;
    public readonly float Scale;

    public TextureSlice(Texture2D texture, Rectangle src, Vector2 origin, float scale)
    {
        Texture = texture;
        Source = src;
        Origin = origin;
        Scale = scale;
    }

    public void Draw(SpriteBatch b, Vector2 position, Color color, float rotation = 0)
    {
        b.Draw(Texture, position, Source, color, rotation, Origin, Scale, SpriteEffects.None, 0);
    }

    public void Draw(SpriteBatch b, Vector2 position, Color color, Vector2 scale)
    {
        b.Draw(Texture, position, Source, color, 0, Origin, scale, SpriteEffects.None, 0);
    }
}

public abstract class Widget
{
    public int X;
    public int Y;
    public int Width;
    public int Height;

    public bool inWidgetArea
    {
        get
        {
            var Input = ModEntry.Instance!.Helper.Input;
            var Cursor = Input.GetCursorPosition().GetScaledScreenPixels();
            return Cursor.X >= X && Cursor.Y >= Y && Cursor.X <= X + Width && Cursor.Y <= Y + Height;
        }
    }

    public abstract void onTick();

    public abstract void Draw(SpriteBatch b);

    public static void Rect(SpriteBatch b, int x, int y, int width, int height, Color color)
    {
        b.Draw(Game1.staminaRect, new Rectangle(x, y, width, height), color);
    }

    public static int Text(SpriteBatch b, SpriteFont font, float x, float y, string text, Color color)
    {
        b.DrawString(font, text, new Vector2(x, y), color);
        return deltaX(font, text);
    }

    public static int deltaX(SpriteFont font, string text)
    {
        return (int)Math.Ceiling(font.MeasureString(text).X);
    }
}