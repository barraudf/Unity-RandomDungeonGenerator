public enum EnumDirections
{
    North,
    South,
    West,
    East
}

public static class EnumDirectionsExtensions
{
    public static EnumDirections Opposite(this EnumDirections dir)
    {
        switch(dir)
        {
            case EnumDirections.East: return EnumDirections.West;
            case EnumDirections.North: return EnumDirections.South;
            case EnumDirections.South: return EnumDirections.North;
            case EnumDirections.West:
            default: return EnumDirections.East;
        }
    }
}