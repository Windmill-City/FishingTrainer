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
        Width = 0;
        Height = RowHeight * 15;

        Fishes = FishItems.GetFishItems();
        ActiveFish = FishItems.GetPreviousFishItem();

        ScrollPos = Fishes.IndexOf(ActiveFish) - 7;

        // layout
        foreach (var fish in Fishes)
        {
            Width = Math.Max(deltaX($"{fish.DisplayName}({fish.Difficulty}){fish.Type.asString()}"), Width);
        }
        Width += 48; // icon
        Width += 32; // padding
    }

    public override void Draw(SpriteBatch b)
    {
        int X_Offset = X;
        int Y_Offset = Y;
        foreach (var fish in Fishes.Skip(ScrollPos))
        {
            // icon
            fish.Draw(b, new Vector2(X_Offset, Y_Offset - 16), 0.5f);

            int deltaX = 48;
            // DisplayName
            var nameColor = fish.Equals(ActiveFish) ? Color.Green : (fish.isBossFish ? Color.OrangeRed : Color.Black);
            deltaX += Text(b, X_Offset + deltaX, Y_Offset, fish.DisplayName, nameColor);
            // Difficulty
            deltaX += Text(b, X_Offset + deltaX, Y_Offset, $"({fish.Difficulty})", Color.DarkMagenta);
            // MotionType
            Text(b, X_Offset + deltaX, Y_Offset, fish.Type.asString(), Color.DarkBlue);

            Y_Offset += RowHeight;
            if (Y_Offset + RowHeight > Y + Height) break;
        }
    }

    public override void onTick()
    {
        var Input = ModEntry.Instance!.Helper.Input;

        if (Input.GetState(SButton.MouseLeft) == SButtonState.Pressed)
        {
            var Cursor = Input.GetCursorPosition().ScreenPixels;

            // is in selector area?
            if (Cursor.X >= X && Cursor.Y >= Y && Cursor.X <= X + Width && Cursor.Y <= Y + Height)
            {
                var Index = (int)(ScrollPos + (Cursor.Y - Y) / RowHeight);
                if (Index < Fishes.Count)
                {
                    ActiveFish = Fishes[Index].DeepClone();

                    ModEntry.Config.PreviousFishId = ActiveFish.ItemId;

                    OnFishSelected?.Invoke(this, new EventArgs());
                }
            }
        }
    }
}