using FishingTrainer;

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

    public static string asString(this MotionType motionType)
    {
        switch (motionType)
        {
            case MotionType.Mixed:
                return I18n.MotionType_Mixed();
            case MotionType.Dart:
                return I18n.MotionType_Dart();
            case MotionType.Smooth:
                return I18n.MotionType_Smooth();
            case MotionType.Floater:
                return I18n.MotionType_Floater();
            case MotionType.Sinker:
                return I18n.MotionType_Sinker();
            default:
                throw new NotImplementedException();
        }
    }
}