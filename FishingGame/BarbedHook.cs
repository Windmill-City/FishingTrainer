using FishingTrainer;

class BarbedHook
{
    public FishingGame Context;

    public BarbedHook(FishingGame game)
    {
        Context = game;
    }

    public void onTick()
    {
        if (Context.BobberInBar)
        {
            for (int i = 0; i < ModEntry.Config.BarbedHook; i++)
            {
                var acceleration = i > 0 ? 0.05f : 0.2f;

                var Bobber = Context.Bobber;
                var BobberBar = Context.BobberBar;

                var midBar = BobberBar.Position + BobberBar.Height / 2;
                var midBobber = Bobber.Position + Bobber.Height / 2;

                // is bobber over the bar?
                BobberBar.Velocity += midBobber < midBar ? -acceleration : acceleration;

                // bar acceleration modifer
                var modifier = i > 0 ? 0.9f : 0.5f;
                BobberBar.Acceleration *= modifier;
            }
        }
    }
}