using FishingTrainer;
using Force.DeepCloner;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;

public class UiFishSelector : Widget
{
    public const int RowHeight = 40;

    public List<FishItem> Fishes;
    public FishItem ActiveFish;

    public event EventHandler? OnFishSelected;

    public int ScrollMax => Math.Max(0, Fishes.Count - Height / RowHeight);

    private int _scrollPos = 0;
    public int ScrollPos
    {
        get => _scrollPos;
        set
        {
            _scrollPos = Math.Clamp(value, 0, ScrollMax);
        }
    }

    public UiFishSelector()
    {
        Width = 400;
        Height = RowHeight * 15;

        Fishes = FishItems.GetFishItems();
        ActiveFish = FishItems.GetPreviousFishItem();
    }

    public override void Draw(SpriteBatch b)
    {
        int X_Offset = X;
        int Y_Offset = Y;
        foreach (var fish in Fishes.Skip(ScrollPos))
        {
            fish.Draw(b, new Vector2(X_Offset, Y_Offset - 16), 0.5f);

            var deltaX = 48f;
            // DisplayName
            deltaX += Text(b, X_Offset + deltaX, Y_Offset, fish.DisplayName, Color.Black);
            // Difficulty
            deltaX += Text(b, X_Offset + deltaX, Y_Offset, $"({fish.Difficulty})", Color.DarkMagenta);
            // MotionType
            Text(b, X_Offset + deltaX, Y_Offset, fish.Type.ToString(), Color.DarkBlue);

            Y_Offset += RowHeight;
            if (Y_Offset + RowHeight > Y + Height) break;
        }
    }

    public override void onTick()
    {
        var Input = ModEntry.Instance!.Helper.Input;

        if (Input.GetState(SButton.MouseLeft) == SButtonState.Held)
        {
            var Cursor = Input.GetCursorPosition().ScreenPixels;
            Log.Debug($"Cursor: {Cursor.X}, {Cursor.Y}");

            // is in selector area?
            if (Cursor.X >= X && Cursor.Y >= Y && Cursor.X <= X + Width && Cursor.Y <= Y + Height)
            {
                var Index = (int)(ScrollPos + (Cursor.Y - Y) / RowHeight);
                Log.Debug($"Index: {Index}, Count: {Fishes.Count}");

                if (Index < Fishes.Count)
                {
                    ActiveFish = Fishes[Index].DeepClone();
                    OnFishSelected?.Invoke(this, new EventArgs());
                }
            }
        }
    }
}