using System;
using UnityEngine;
using System.Collections.Generic;

public class dungeon
{
    public Dictionary<string, int[,]> dungeon_layout = new Dictionary<string, int[,]>()
    {
        { "Box", new int[,] { { 1, 1, 1 }, { 1, 0, 1 }, { 1, 1, 1 } }},
        { "Cross", new int[,] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } }}
    };
    
    public Dictionary<string, int> corridor_layout = new Dictionary<string, int>()
    {
        { "Labyrinth", 0 },
        { "Bent", 50 },
        { "Straight", 100 }
    };

    public Dictionary<string, int> di = new Dictionary<string, int>()
    {
        { "north", -1 },
        { "south", 1 },
        { "west", 0 },
        { "east", 0 }
    };
    public Dictionary<string, int> dj = new Dictionary<string, int>()
    {
        { "north", 0 },
        { "south", 0 },
        { "west", -1 },
        { "east", 1 }
    };

    public Dictionary<string, string> opposite = new Dictionary<string, string>()
    {
        { "north", "south" },
        { "south", "north" },
        { "west", "east" },
        { "east", "west" }
    };

    public Dictionary<string, Dictionary<string, int[,]>> stair_end = new Dictionary<string, Dictionary<string, int[,]>>()
    {
        { "north", new Dictionary<string, int[,]>()
            {
                { "walled", new int[,] { {1,-1}, {0,-1}, {-1,-1}, {-1,0}, {-1,1}, {0,1}, {1,1} }},
                { "corridor", new int[,] { {0,0}, {1,0}, {2,0} }},
                { "stair", new int[,] { {0,0} }},
                { "next", new int[,] { {1,0} }}
            }
        },
        { "south", new Dictionary<string, int[,]>()
            {
                { "walled", new int[,] { {-1,-1}, {0,-1}, {1,-1}, {1,0}, {1,1}, {0,1}, {-1,1} }},
                { "corridor", new int[,] { {0,0}, {-1,0}, {-2,0} }},
                { "stair", new int[,] { {0,0} }},
                { "next", new int[,] { {-1,0} }}
            }
        },
        { "west", new Dictionary<string, int[,]>()
            {
                { "walled", new int[,] { {-1,1}, {-1,0}, {-1,-1}, {0,-1}, {1,-1}, {1,0}, {1,1} }},
                { "corridor", new int[,] { {0,0}, {0,1}, {0,2} }},
                { "stair", new int[,] { {0,0} }},
                { "next", new int[,] { {0,1} }}
            }
        },
        { "east", new Dictionary<string, int[,]>()
            {
                { "walled", new int[,] { {-1,-1}, {-1,0}, {-1,1}, {0,1}, {1,1}, {1,0}, {1,-1} }},
                { "corridor", new int[,] { {0,0}, {0,-1}, {0,-2} }},
                { "stair", new int[,] { {0,0} }},
                { "next", new int[,] { {0,-1} }}
            }
        }
    };

    public Dictionary<string, Dictionary<string, int[,]>> close_end = new Dictionary<string, Dictionary<string, int[,]>>()
    {
        { "north", new Dictionary<string, int[,]>()
            {
                { "walled", new int[,] { {0,-1}, {1,-1}, {1,0}, {1,1}, {0,1} }},
                { "close", new int[,] { {0,0} }},
                { "recurse", new int[,] { {-1,0} }}
            }
        },
        { "south", new Dictionary<string, int[,]>()
            {
                { "walled", new int[,] { {0,-1}, {-1,-1}, {-1,0}, {-1,1}, {0,1} }},
                { "close", new int[,] { {0,0} }},
                { "recurse", new int[,] { {1,0} }}
            }
        },
        { "west", new Dictionary<string, int[,]>()
            {
                { "walled", new int[,] { {-1,0}, {-1,1}, {0,1}, {1,1}, {1,0} }},
                { "close", new int[,] { {0,0} }},
                { "recurse", new int[,] { {0,-1} }}
            }
        },
        { "east", new Dictionary<string, int[,]>()
            {
                { "walled", new int[,] { {-1,0}, {-1,-1}, {0,-1}, {1,-1}, {1,0} }},
                { "close", new int[,] { {0,0} }},
                { "recurse", new int[,] { {0,1} }}
            }
        }
    };

    protected static uint NOTHING     = 0x00000000;

    protected static uint BLOCKED = 0x00000001;
    protected static uint ROOM = 0x00000002;
    protected static uint CORRIDOR = 0x00000004;
    //                 0x00000008;
    protected static uint PERIMETER = 0x00000010;
    protected static uint ENTRANCE = 0x00000020;
    protected static uint ROOM_ID = 0x0000FFC0;

    protected static uint ARCH = 0x00010000;
    protected static uint DOOR = 0x00020000;
    protected static uint LOCKED = 0x00040000;
    protected static uint TRAPPED = 0x00080000;
    protected static uint SECRET = 0x00100000;
    protected static uint PORTC = 0x00200000;
    protected static uint STAIR_DN = 0x00400000;
    protected static uint STAIR_UP = 0x00800000;

    protected static uint LABEL = 0xFF000000;

    protected static uint OPENSPACE = ROOM | CORRIDOR;
    protected static uint DOORSPACE = ARCH | DOOR | LOCKED | TRAPPED | SECRET | PORTC;
    protected static uint ESPACE = ENTRANCE | DOORSPACE | 0xFF000000;
    protected static uint STAIRS = STAIR_DN | STAIR_UP;

    protected static uint BLOCK_ROOM = BLOCKED | ROOM;
    protected static uint BLOCK_CORR = BLOCKED | PERIMETER | CORRIDOR;
    protected static uint BLOCK_DOOR = BLOCKED | DOORSPACE;

    public dungeon()
    {
        
    }
}
