using FishingTrainer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

class FishingGame : IClickableMenu
{
    public static Texture2D? Background;

    public const int TickRate = 60; // 60 ticks/sec

    public bool isPaused = true;
    public int PauseTimer = 0;

    public bool isPerfect = true;

    public FloaterSinker FloaterSinker;
    public Fish Fish;
    public Reel Reel;
    public Catch Catch;
    public Bobber Bobber;
    public BobberBar BobberBar;
    public Treasure Treasure;
    public BarbedHook BarbedHook;

    public bool isPressed
    {
        get
        {
            var Input = ModEntry.Instance!.Helper.Input;

            if (Input.GetState(SButton.MouseLeft) == SButtonState.Held)
            {
                // Position BobberBar
                var X_BB = xPositionOnScreen + 64;
                var Y_BB = yPositionOnScreen + 32;

                var Cursor = Input.GetCursorPosition().ScreenPixels;

                // in BobberBar Area?
                return Cursor.X >= X_BB && Cursor.X <= X_BB + 38 * 4
                        && Cursor.Y >= Y_BB && Cursor.Y <= Y_BB + 150 * 4;
            }

            if (Game1.isOneOfTheseKeysDown(Game1.oldKBState, Game1.options.useToolButton))
            {
                return true;
            }

            if (Game1.options.gamepadControls
                && (Game1.oldPadState.IsButtonDown(Buttons.X) || Game1.oldPadState.IsButtonDown(Buttons.A)))
            {
                return true;
            }

            return false;
        }
    }

    public bool BobberInBar
    {
        get
        {
            float BobberTop = Bobber.Position;
            float BobberBottom = Bobber.Position + Bobber.Height;

            float BobberBarTop = BobberBar.Position;
            float BobberBarBottom = BobberBar.Position + BobberBar.Height;

            return BobberBottom <= BobberBarBottom && BobberTop >= BobberBarTop;
        }
    }

    public bool TreasureInBar
    {
        get
        {
            if (Treasure.isHidden || Treasure.isCaught) return false;

            float TreasureTop = Treasure.Position;
            float TreasureBottom = Treasure.Position + Treasure.Height;

            float BobberBarTop = BobberBar.Position;
            float BobberBarBottom = BobberBar.Position + BobberBar.Height;

            return TreasureBottom <= BobberBarBottom && TreasureTop >= BobberBarTop;
        }
    }

    public FishingGame() : base(0, 0, 320 * 4, 180 * 4)
    {
        var Content = ModEntry.Instance!.Helper.GameContent;

        // Texture
        if (Background is null)
            Background = Content.Load<Texture2D>("LooseSprites\\troutDerbyLetterBG");

        // Components
        FloaterSinker = new FloaterSinker(this);
        BarbedHook = new BarbedHook(this);
        Fish = new Fish(this);
        Reel = new Reel(this);
        Catch = new Catch(this);
        Bobber = new Bobber(this);
        BobberBar = new BobberBar(this);
        Treasure = new Treasure(this);

        // Cleanup
        behaviorBeforeCleanup += BeforeExitMenu;

        // Hide HUD
        Game1.displayHUD = false;

        Reposition();
    }

    private void BeforeExitMenu(IClickableMenu menu)
    {
        StopSound();
        Game1.displayHUD = true;
    }

    private void StopSound()
    {
        Reel.ReelSound?.Stop(AudioStopOptions.Immediate);
        Reel.UnReelSound?.Stop(AudioStopOptions.Immediate);
    }

    public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
    {
        Reposition();
    }

    private void Reposition()
    {
        xPositionOnScreen = (int)((Game1.viewport.Width - width * Game1.options.uiScale) / 2);
        yPositionOnScreen = (int)((Game1.viewport.Height - height * Game1.options.uiScale) / 2);
    }

    public override void update(GameTime time)
    {
        onTick();

        if (isPaused) return;

        FloaterSinker.onTick();

        Bobber.onTick();
        BobberBar.onTick();

        Fish.onTick();
        Reel.onTick();
        Catch.onTick();
        Treasure.onTick();

        if (Catch.isCaught && ModEntry.Config.ResetOnCaught) Reset();
    }

    public void onTick()
    {
        // Check if Reset key is pressed
        if (ModEntry.Config.ResetFishingGame.JustPressed())
        {
            Reset();
        }

        // if TimeToPauseOnNoAction is zero, then disable the pause function
        if (ModEntry.Config.TimeToPauseOnNoAction == 0)
        {
            if (isPressed) isPaused = false;
            return;
        }

        if (!isPressed)
        {
            if (PauseTimer > 0)
            {
                PauseTimer--;
            }
            else
            {
                isPaused = true;
                StopSound();
            }
        }
        else
        {
            isPaused = false;
            PauseTimer = (int)(ModEntry.Config.TimeToPauseOnNoAction * TickRate);
        }
    }

    public void Reset()
    {
        isPaused = true;
        isPerfect = true;

        Bobber.Reset();
        BobberBar.Reset();

        Treasure.Reset();

        Catch.Reset();
        Fish.Reset();
    }

    public override void draw(SpriteBatch b)
    {
        var X = xPositionOnScreen;
        var Y = yPositionOnScreen;

        // Background
        b.Draw(Background, new Vector2(X, Y), new Rectangle(0, 0, 320, 180), Color.White, 0.0f, new Vector2(0, 0), 4f, SpriteEffects.None, 0.99f);

        // Position BobberBar
        var X_BB = X + 64;
        var Y_BB = Y + 32;

        // BobberBar
        b.Draw(Game1.mouseCursors, new Vector2(X_BB, Y_BB), new Rectangle(644, 1999, 38, 150), Color.White, 0.0f, new Vector2(0, 0), 4f, SpriteEffects.None, 0.98f);

        // Progress LeftTop
        var X_PL = X_BB + 32 * 4;
        var Y_PT = Y_BB + 2 * 4;

        // Catch Progress
        b.Draw(Game1.staminaRect, new Rectangle(X_PL, Y_PT + (int)(144 * 4 * (1f - Catch.CatchProgress)), 4 * 4, (int)(144 * 4 * Catch.CatchProgress)), Utility.getRedToGreenLerpColor(Catch.CatchProgress));

        // Bar LeftTop
        var X_BL = X_BB + 17 * 4;
        var Y_BT = Y_BB + 3 * 4;

        //BobberBar
        var BarShake = BobberInBar || TreasureInBar ? Vector2.Zero : BobberBar.Shaker.Shake;
        var BarColor = BobberInBar || TreasureInBar ? Color.White : (Color.White * 0.25f * ((float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 100.0), 2) + 2f));
        b.Draw(Game1.mouseCursors, new Vector2(X_BL, Y_BT + (int)BobberBar.Position) + BarShake, new Rectangle(682, 2078, 9, 2), BarColor, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);
        b.Draw(Game1.mouseCursors, new Vector2(X_BL, Y_BT + (int)BobberBar.Position + 8) + BarShake, new Rectangle(682, 2081, 9, 1), BarColor, 0f, Vector2.Zero, new Vector2(4f, BobberBar.Height - 16), SpriteEffects.None, 0.89f);
        b.Draw(Game1.mouseCursors, new Vector2(X_BL, Y_BT + (int)BobberBar.Position + BobberBar.Height - 8) + BarShake, new Rectangle(682, 2085, 9, 2), BarColor, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.89f);

        if (ModEntry.Config.ShowDebugHints)
        {
            // BobberBar Top
            b.Draw(Game1.staminaRect, new Rectangle(X_BL, Y_BT + (int)BobberBar.Position, 9 * 4, 4), Color.Red);
            // BobberBar Bottom
            b.Draw(Game1.staminaRect, new Rectangle(X_BL, Y_BT - 4 + (int)(BobberBar.Position + BobberBar.Height), 9 * 4, 4), Color.White);
        }

        if (!Treasure.isHidden && !Treasure.isCaught)
        {
            // Treasure
            var TreasureShake = TreasureInBar ? Treasure.Shaker.Shake : Vector2.Zero;
            b.Draw(Game1.mouseCursors, new Vector2(X_BL, Y_BT + Treasure.Position) + TreasureShake, new Rectangle(638, 1865, 20, 24), Color.White, 0f, new Vector2(2f, 0f), 2f, SpriteEffects.None, 0.85f);

            if (ModEntry.Config.ShowDebugHints)
            {
                // Treasure Top
                b.Draw(Game1.staminaRect, new Rectangle(X_BL, Y_BT + (int)Treasure.Position, 9 * 4, 4), Color.Red);
                // Treasure Bottom
                b.Draw(Game1.staminaRect, new Rectangle(X_BL, Y_BT - 4 + (int)(Treasure.Position + Treasure.Height), 9 * 4, 4), Color.White);
            }

            if (Treasure.CatchProgress > 0f)
            {
                b.Draw(Game1.staminaRect, new Rectangle(X_BL + 2, Y_BT - 8 + (int)Treasure.Position, 8 * 4, 8), Color.DimGray * 0.5f);
                b.Draw(Game1.staminaRect, new Rectangle(X_BL + 2, Y_BT - 8 + (int)Treasure.Position, (int)(Treasure.CatchProgress * 8 * 4), 8), Color.Orange);
            }
        }

        //Bobber
        var BobberShake = BobberInBar ? Bobber.Shaker.Shake : Vector2.Zero;
        b.Draw(Game1.mouseCursors, new Vector2(X_BL, Y_BT + Bobber.Position) + BobberShake, new Rectangle(614, 1840, 20, 20), Color.White, 0f, new Vector2(1f, 1f), 2f, SpriteEffects.None, 0.88f);

        if (ModEntry.Config.ShowDebugHints)
        {
            // Bobber Top
            b.Draw(Game1.staminaRect, new Rectangle(X_BL, Y_BT + (int)Bobber.Position, 9 * 4, 4), Color.Red);
            // Bobber Bottom
            b.Draw(Game1.staminaRect, new Rectangle(X_BL, Y_BT - 4 + (int)(Bobber.Position + Bobber.Height), 9 * 4, 4), Color.White);
        }

        // Reel
        b.Draw(Game1.mouseCursors, new Vector2(X_BB + 5.5f * 4, Y_BB + 128.5f * 4), new Rectangle(257, 1990, 5, 10), Color.White, Reel.ReelRotation, new Vector2(2.5f, 9.5f), 4f, SpriteEffects.None, 0.9f);

        var X_Info = X + 256;
        var Y_Info = Y + 32;

        // Sonar Background
        b.Draw(Game1.mouseCursors_1_6, new Vector2(X_Info, Y_Info), new Rectangle(227, 6, 29, 24), Color.White, 0f, new Vector2(10f, 0f), 4f, SpriteEffects.None, 0.88f);
        // Fish Object
        Fish.Content.fishObject.drawInMenu(b, new Vector2(X_Info, Y_Info + 16), 1);

        X_Info -= 16;
        Y_Info += 128;

        // Fish Display Name
        b.DrawString(Game1.dialogueFont, Fish.Content.fishObject.DisplayName, new Vector2(X_Info, Y_Info), Color.Black);
        Y_Info += 40;
        // Fish Motion Type
        switch (Fish.Content.motionType)
        {
            case MotionType.Mixed:
                b.DrawString(Game1.dialogueFont, I18n.ExBobberBar_MotionType_Mixed(), new Vector2(X_Info, Y_Info), Color.DarkBlue);
                break;
            case MotionType.Dart:
                b.DrawString(Game1.dialogueFont, I18n.ExBobberBar_MotionType_Dart(), new Vector2(X_Info, Y_Info), Color.DarkBlue);
                break;
            case MotionType.Smooth:
                b.DrawString(Game1.dialogueFont, I18n.ExBobberBar_MotionType_Smooth(), new Vector2(X_Info, Y_Info), Color.DarkBlue);
                break;
            case MotionType.Floater:
                b.DrawString(Game1.dialogueFont, I18n.ExBobberBar_MotionType_Floater(), new Vector2(X_Info, Y_Info), Color.DarkBlue);
                break;
            case MotionType.Sinker:
                b.DrawString(Game1.dialogueFont, I18n.ExBobberBar_MotionType_Sinker(), new Vector2(X_Info, Y_Info), Color.DarkBlue);
                break;
        }
        Y_Info += 40;
        // Fish Difficulty
        b.DrawString(Game1.dialogueFont, I18n.ExBobberBar_Difficulty(Bobber.Difficulty, Fish.Content.difficulty), new Vector2(X_Info, Y_Info), Color.Black);
        Y_Info += 40;
        // Fish Size
        b.DrawString(Game1.dialogueFont, I18n.ExBobberBar_Size(Fish.Size, Fish.Content.minSize, Fish.Content.maxSize), new Vector2(X_Info, Y_Info), Color.Black);
        Y_Info += 40;

        // Perfect
        b.DrawString(Game1.dialogueFont, I18n.ExBobberBar_Perfect(), new Vector2(X_Info, Y_Info), isPerfect ? Color.Orange : Color.Black);
        Y_Info += 40;

        // Status
        b.DrawString(Game1.dialogueFont, !isPaused ? I18n.ExBobberBar_Running() : I18n.ExBobberBar_Paused(), new Vector2(X_Info, Y_Info), !isPaused ? Color.DarkGreen : Color.DarkRed);
        Y_Info += 40 * 3;

        // Chance Veer
        b.DrawString(Game1.dialogueFont, I18n.ExBobberBar_Chance_Veer($"{Bobber.ChanceVeer:P}"), new Vector2(X_Info, Y_Info), Color.Black);
        Y_Info += 40;
        // Chance Dart
        b.DrawString(Game1.dialogueFont, I18n.ExBobberBar_Chance_Dart($"{Bobber.ChanceDart:P}"), new Vector2(X_Info, Y_Info), Color.Black);
        Y_Info += 40;
        // Chance QuickDart
        b.DrawString(Game1.dialogueFont, I18n.ExBobberBar_Chance_QuickDart($"{Bobber.ChanceQuickDart:P}"), new Vector2(X_Info, Y_Info), Color.Black);

        drawMouse(b);
    }
}