using Microsoft.Xna.Framework.Audio;
using StardewValley;

class Reel
{
    public FishingGame Context;
    public ICue? ReelSound = null;
    public ICue? UnReelSound = null;
    public float ReelRotation = 0;
    public bool isButtonPressed = false;
    public bool hasBend = false;

    public Reel(FishingGame game)
    {
        Context = game;
    }

    public void onTick()
    {
        if (Context.isPressed && !hasBend)
        {
            Game1.playSound("fishingRodBend");
        }
        hasBend = Context.isPressed;

        if (Context.BobberInBar)
        {
            ReelRotation += (float)(Math.PI / 8f);
            UnReelSound?.Stop(AudioStopOptions.Immediate);
            if (ReelSound == null || ReelSound.IsStopped || ReelSound.IsStopping || !ReelSound.IsPlaying)
            {
                Game1.playSound("fastReel", out ReelSound);
            }
        }
        else
        {
            var BobberBar = Context.BobberBar;
            var len = Math.Abs(Context.Bobber.Position - (BobberBar.Position + BobberBar.Height / 2));
            ReelRotation -= (float)Math.PI / Math.Max(10f, 200f - len);
            ReelSound?.Stop(AudioStopOptions.Immediate);
            if (UnReelSound == null || UnReelSound.IsStopped)
            {
                Game1.playSound("slowReel", 600, out UnReelSound);
            }
        }
    }
}