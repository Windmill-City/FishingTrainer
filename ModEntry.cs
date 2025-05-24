namespace FishingTrainer;

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;

public class ModEntry : Mod
{
    public ModConfig Config = new ModConfig();

    public override void Entry(IModHelper helper)
    {
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

        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => "Example checkbox",
            tooltip: () => "An optional description shown as a tooltip to the player.",
            getValue: () => Config.ExampleBoolean,
            setValue: value => Config.ExampleBoolean = value
        );
        configMenu.AddTextOption(
            mod: ModManifest,
            name: () => "Example string",
            getValue: () => Config.ExampleString,
            setValue: value => Config.ExampleString = value
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => "Example number",
            getValue: () => Config.ExampleNumber,
            setValue: value => Config.ExampleNumber = value
        );
    }
}
