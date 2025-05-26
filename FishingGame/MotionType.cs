public enum MotionType
{
    Mixed,
    Dart,
    Smooth,
    Sinker,
    Floater,
}

public static class MotionTypeExtension
{
    public static MotionType asMotionType(this string motionType)
    {
        switch (motionType)
        {
            case "mixed":
                return MotionType.Mixed;
            case "dart":
                return MotionType.Dart;
            case "smooth":
                return MotionType.Smooth;
            case "floater":
                return MotionType.Floater;
            case "sinker":
                return MotionType.Sinker;
            default:
                throw new FormatException(motionType);
        }
    }
}