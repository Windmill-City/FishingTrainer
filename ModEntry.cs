namespace FishingTrainer;

using StardewModdingAPI;
using StardewModdingAPI.Events;

using SpaceShared.APIs;

public class ModEntry : Mod
{
    public ModConfig Config = new ModConfig();

    public override void Entry(IModHelper helper)
    {
        I18n.Init(helper.Translation);
        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
    }
    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config)
        );
    }
}
