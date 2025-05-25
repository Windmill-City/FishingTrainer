using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public abstract class Widget
{
    // X and Y
    public Vector2 Position;
    // Width and Height
    public Vector2 Bounds;

    public abstract void render(SpriteBatch b);
}