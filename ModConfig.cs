using StardewModdingAPI.Utilities;

public sealed class ModConfig
{
    public KeybindList OpenFishingGame { get; set; } = KeybindList.Parse("F8");
    public KeybindList ResetFishingGame { get; set; } = KeybindList.Parse("F9");

    public float TimeToPauseOnNoAction = 3; // Seconds
    public bool ShowDebugHints = false;

    public int FishingLevel = 0;

    public bool hasTreasure = true;
    public bool hasDeluxeBait = false;
    public bool hasTreasureHunter = false;
    public bool hasBlessingOfWaters = false;

    public int TrapBobber = 0;
    public int CorkBobber = 0;
    public int LeadBobber = 0;
    public int BarbedHook = 0;
}