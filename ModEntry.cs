namespace FishingTrainer;

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;

using StardewValley;

public class ModEntry : Mod
{
    public static ModEntry? Instance;
    public static ModConfig Config = new ModConfig();

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
                Game1.activeClickableMenu = new FishingGame();
            }
            else
            {
                if (Game1.activeClickableMenu is FishingGame)
                {
                    Game1.activeClickableMenu.exitThisMenu();
                }
            }
        }
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        Game1.activeClickableMenu = new FishingGame();

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
            name: () => I18n.Config_OpenFishingGame(),
            tooltip: () => I18n.Config_OpenFishingGame_ToolTip(),
            getValue: () => Config.OpenMiniGame,
            setValue: value => Config.OpenMiniGame = value
        );

        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => I18n.Config_TimeToPauseNoAction(),
            tooltip: () => I18n.Config_TimeToPauseNoAction_ToolTip(),
            getValue: () => Config.TimeToPauseOnNoAction,
            setValue: value => Config.TimeToPauseOnNoAction = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => I18n.Config_ShowDebugHint(),
            tooltip: () => I18n.Config_ShowDebugHint_ToolTip(),
            getValue: () => Config.ShowDebugHints,
            setValue: value => Config.ShowDebugHints = value
        );

        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => I18n.Config_FishingLevel(),
            tooltip: () => I18n.Config_FishingLevel_ToolTip(),
            getValue: () => Config.FishingLevel,
            setValue: value => Config.FishingLevel = value
        );

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => I18n.Config_HasTreasure(),
            tooltip: () => "",
            getValue: () => Config.hasTreasure,
            setValue: value => Config.hasTreasure = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => I18n.Config_HasDeluxeBait(),
            tooltip: () => I18n.Config_HasDeluxeBait_ToolTip(),
            getValue: () => Config.hasDeluxeBait,
            setValue: value => Config.hasDeluxeBait = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => I18n.Config_HasTreasureHunter(),
            tooltip: () => I18n.Config_HasTreasureHunter_ToolTip(),
            getValue: () => Config.hasTreasureHunter,
            setValue: value => Config.hasTreasureHunter = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => I18n.Config_HasBlessingOfWaters(),
            tooltip: () => I18n.Config_HasBlessingOfWaters_ToolTip(),
            getValue: () => Config.hasBlessingOfWaters,
            setValue: value => Config.hasBlessingOfWaters = value
        );

        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => I18n.Config_TrapBobber(),
            tooltip: () => I18n.Config_TrapBobber_ToolTip(),
            getValue: () => Config.TrapBobber,
            setValue: value => Config.TrapBobber = value
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => I18n.Config_CorkBobber(),
            tooltip: () => I18n.Config_CorkBobber_ToolTip(),
            getValue: () => Config.CorkBobber,
            setValue: value => Config.CorkBobber = value
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => I18n.Config_LeadBobber(),
            tooltip: () => I18n.Config_LeadBobber_ToolTip(),
            getValue: () => Config.LeadBobber,
            setValue: value => Config.LeadBobber = value
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => I18n.Config_BarbedHook(),
            tooltip: () => I18n.Config_BarbedHook_ToolTip(),
            getValue: () => Config.BarbedHook,
            setValue: value => Config.BarbedHook = value
        );
    }
}
