public class FloaterSinker
{
    public const float DeltaPerTick = 0.01f;
    public const float AccelerationMax = 1.5f;

    public FishingGame Context;

    private float _acceleration = 0;
    public float Acceleration
    {
        get => _acceleration;
        set => _acceleration = Math.Clamp(value, -AccelerationMax, AccelerationMax);
    }

    public FloaterSinker(FishingGame game)
    {
        Context = game;
    }

    public void Reset()
    {
        Acceleration = 0;
    }

    public void onTick()
    {
        switch (Context.Fish.Item.Type)
        {
            case MotionType.Sinker:
                Acceleration += DeltaPerTick;
                break;
            case MotionType.Floater:
                Acceleration -= DeltaPerTick;
                break;
            default:
                Acceleration = 0;
                break;
        }
    }
}