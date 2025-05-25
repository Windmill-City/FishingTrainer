using FishingTrainer;
using StardewValley;
using StardewValley.Extensions;

class Bobber
{
    public static Shaker Shaker = new Shaker(-10, 11, 10f);

    public FishingGame Context;

    public const int Height = 20 * 2;
    public const int PositionMax = 141 * 4 - Height;
    public const float PositionCloseMargin = 3f;
    public const float DartPositionMin = 50;
    public const float DartPositionMax = 100;
    public const float VelocitySmoothingFactor = 5f;

    public MotionType motionType => Context.Fish.Content.motionType;
    public bool isSlidingOrStill = true;
    public float Velocity = 0;
    public float Acceleration = 0;

    public bool CloseToTarget
    {
        get => Math.Abs(Position - TargetPosition) <= PositionCloseMargin;
    }

    public double ChanceVeer
    {
        get
        {
            if (motionType == MotionType.Smooth)
                return Difficulty * 20 / 4000f;
            else// Mixed/Dart/Floater/Sinker
                return Difficulty / 4000f;
        }
    }

    public double ChanceDart
    {
        get
        {
            if (motionType == MotionType.Smooth)
                return 0;
            else// Mixed/Dart/Floater/Sinker
                return Difficulty / 2000f;
        }
    }

    public double ChanceQuickDart
    {
        get
        {
            if (motionType == MotionType.Mixed)
                return Difficulty / 1000f;
            else// Smooth/Dart/Floater/Sinker
                return 0;
        }
    }

    public bool RandomVeer
    {
        get
        {
            // Smooth type doesn't veers when it is moving
            if (motionType == MotionType.Smooth && !isSlidingOrStill)
                return false;
            else
                return Game1.random.NextDouble() < ChanceVeer;
        }
    }

    public bool RandomDart
    {
        // Dart when bobber moving slowly
        get => isSlidingOrStill && Game1.random.NextDouble() < ChanceDart;
    }

    public bool RandomQuickDart
    {
        get => Game1.random.NextDouble() < ChanceQuickDart;
    }

    public float RandomVeerPosition
    {
        get
        {
            float spaceBottom = PositionMax - Position;
            float spaceAbove = Position;

            // 10% ~ 45%
            float randomScale = Game1.random.Next(10, 45);
            // 10% ~ 99%
            float deltaScale = Math.Min(randomScale + Difficulty, 99f) / 100f;
            // Move to top or bottom
            float deltaPosition = Game1.random.Next((int)-spaceAbove, (int)spaceBottom);

            return Position + deltaPosition * deltaScale;
        }
    }

    public float RandomDartPosition
    {
        get
            => Position
            + (Game1.random.NextBool() ? -1 : 1)
            * Game1.random.Next((int)DartPositionMin, (int)DartPositionMax);
    }
    public float RandomQuickDartPosition
    {
        get
            => Position
            + (Game1.random.NextBool() ? -1 : 1)
            * Game1.random.Next((int)DartPositionMin, (int)DartPositionMax + (int)Difficulty * 2);
    }

    public float RandomAcceleration
    {
        get
        {
            float force = TargetPosition - Position;

            float reduction;
            reduction = Game1.random.Next(10, 30);
            reduction += Math.Max(0, 100 - Difficulty);

            return force / reduction;
        }
    }

    private float _targetPosition = 0;
    public float TargetPosition
    {
        get => _targetPosition;
        set => _targetPosition = Math.Clamp(value, 0, PositionMax);
    }

    private float _position = PositionMax;
    public float Position
    {
        get => _position;
        set => _position = Math.Clamp(value, 0, PositionMax);
    }

    private float _difficulty = 30;
    public float Difficulty
    {
        get
        {
            return ModEntry.Config.hasBlessingOfWaters ? _difficulty / 2 : _difficulty;
        }
        set => _difficulty = value;
    }

    public Bobber(FishingGame Bar)
    {
        Context = Bar;
    }

    public void Reset()
    {
        isSlidingOrStill = true;
        Position = PositionMax;
        Velocity = Acceleration = 0;
    }

    public void onTick()
    {
        if (RandomVeer)
        {
            isSlidingOrStill = false;
            TargetPosition = RandomVeerPosition;
        }

        if (RandomDart)
        {
            isSlidingOrStill = false;
            TargetPosition = RandomDartPosition;
        }

        if (RandomQuickDart)
        {
            isSlidingOrStill = false;
            TargetPosition = RandomQuickDartPosition;
        }

        if (CloseToTarget)
        {
            // The bobber keeps its speed when it get close to the
            // target position, so it behaves like sliding
            isSlidingOrStill = true;
        }
        else
        {
            // if not keeping sliding or still, do update acceleration and speed
            if (!isSlidingOrStill)
            {
                Acceleration = RandomAcceleration;
                // linear leap speed smoothing algorithm
                Velocity += (Acceleration - Velocity) / VelocitySmoothingFactor;
            }
        }

        Position += Velocity + Context.FloaterSinker.Acceleration;
    }
}