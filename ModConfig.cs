using FishingTrainer;
using StardewModdingAPI.Utilities;

public sealed class ModConfig
{

    public KeybindList OpenFishingGame { get; set; } = KeybindList.Parse("F8");
    public KeybindList ResetFishingGame { get; set; } = KeybindList.Parse("F9");

    private string _previousFishId = "";
    public string PreviousFishId
    {
        get => _previousFishId;
        set
        {
            if (value != _previousFishId)
            {
                _previousFishId = value;
                ModEntry.Instance!.Helper.WriteConfig(ModEntry.Config);
            }
        }
    }

    public bool ResetOnCaught = false;

    public float TimeToPauseOnNoAction = 0; // Seconds
    public bool ShowDebugHints = false;

    public int FishingLevel = 10;

    public bool hasTreasure = false;
    public bool hasDeluxeBait = false;
    public bool hasTreasureHunter = false;
    public bool hasBlessingOfWaters = false;

    public int TrapBobber = 0;
    public int CorkBobber = 0;
    public int LeadBobber = 0;
    public int BarbedHook = 0;

    public bool ReelSound = false;
    public bool CaughtSound = false;
    public bool EscapeSound = false;
}