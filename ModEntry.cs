namespace FishingTrainer;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using SpaceShared.APIs;
using StardewValley;

public class ModEntry : Mod
{
    public static ModEntry? Instance;
    public ModConfig Config = new ModConfig();

    public override void Entry(IModHelper helper)
    {
        Instance = this;
        I18n.Init(helper.Translation);
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.Input.ButtonsChanged += OnButtonsChanged;
    }

    private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (Config.OpenMiniGame.JustPressed())
        {
            if (Game1.activeClickableMenu == null)
            {
                Game1.activeClickableMenu = new ExBobberBar();
            }
            else
            {
                if (Game1.activeClickableMenu is ExBobberBar)
                {
                    Game1.activeClickableMenu.exitThisMenu();
                }
            }
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        Game1.activeClickableMenu = new ExBobberBar();

        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
        {
            Monitor.Log("Generic Mod Config Menu not installed! Skip intergration...");
            return;
        }

        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config)
        );

        configMenu.AddKeybindList(
            mod: ModManifest,
            name: I18n.Config_OpenMiniGame,
            tooltip: I18n.Config_OpenMiniGame_ToolTip,
            getValue: () => Config.OpenMiniGame,
            setValue: value => Config.OpenMiniGame = value
        );
    }
}
