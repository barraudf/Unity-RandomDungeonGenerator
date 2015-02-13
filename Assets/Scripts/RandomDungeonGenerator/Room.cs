using System;
using System.Collections.Generic;

public class Room
{
    public Dictionary<EnumDirections, List<Door>> Doors;
    public int ID;
    public int Row;
    public int Col;
    public int Height;
    public int Width;
    public int North;
    public int West;
    public int South;
    public int East;
    public int Area;

    public Room()
    {
        Doors = new Dictionary<EnumDirections, List<Door>>();
        foreach (EnumDirections dir in Enum.GetValues(typeof(EnumDirections)))
            Doors.Add(dir, new List<Door>());
    }

    //public int North
    //{
    //    get { return Row; }
    //}
    
    //public int West
    //{
    //    get { return Col; }
    //}

    //public int South
    //{
    //    get { return Row + Height; }
    //}

    //public int East
    //{
    //    get { return Col + Width; }
    //}

    //public int Area
    //{
    //    get { return Width * Height; }
    //}
}
