using System;
using UnityEngine;
using System.Collections.Generic;

using Random = UnityEngine.Random;

public class dungeon
{
    protected static Dictionary<string, int[][]> dungeon_layouts = new Dictionary<string, int[][]>()
    {
        { "Box", new int[][] { new int[] { 1, 1, 1 }, new int[] { 1, 0, 1 }, new int[] { 1, 1, 1 } }},
        { "Cross", new int[][] { new int[] { 0, 1, 0 }, new int[] { 1, 1, 1 }, new int[] { 0, 1, 0 } }}
    };

    protected static Dictionary<string, int> corridor_layouts = new Dictionary<string, int>()
    {
        { "Labyrinth", 0 },
        { "Bent", 50 },
        { "Straight", 100 }
    };

    protected static Dictionary<string, int> di = new Dictionary<string, int>()
    {
        { "north", -1 },
        { "south", 1 },
        { "west", 0 },
        { "east", 0 }
    };
    protected static Dictionary<string, int> dj = new Dictionary<string, int>()
    {
        { "north", 0 },
        { "south", 0 },
        { "west", -1 },
        { "east", 1 }
    };

    protected static Dictionary<string, string> opposite = new Dictionary<string, string>()
    {
        { "north", "south" },
        { "south", "north" },
        { "west", "east" },
        { "east", "west" }
    };

    protected static Dictionary<string, Dictionary<string, int[][]>> stair_end = new Dictionary<string, Dictionary<string, int[][]>>()
    {
        { "north", new Dictionary<string, int[][]>()
            {
                { "walled", new int[][] { new int[] {1,-1}, new int[] {0,-1}, new int[] {-1,-1}, new int[] {-1,0}, new int[] {-1,1}, new int[] {0,1}, new int[] {1,1} }},
                { "corridor", new int[][] { new int[] {0,0}, new int[] {1,0}, new int[] {2,0} }},
                { "stair", new int[][] { new int[] {0,0} }},
                { "next", new int[][] { new int[] {1,0} }}
            }
        },
        { "south", new Dictionary<string, int[][]>()
            {
                { "walled", new int[][] { new int[] {-1,-1}, new int[] {0,-1}, new int[] {1,-1}, new int[] {1,0}, new int[] {1,1}, new int[] {0,1}, new int[] {-1,1} }},
                { "corridor", new int[][] { new int[] {0,0}, new int[] {-1,0}, new int[] {-2,0} }},
                { "stair", new int[][] { new int[] {0,0} }},
                { "next", new int[][] { new int[] {-1,0} }}
            }
        },
        { "west", new Dictionary<string, int[][]>()
            {
                { "walled", new int[][] { new int[] {-1,1}, new int[] {-1,0}, new int[] {-1,-1}, new int[] {0,-1}, new int[] {1,-1}, new int[] {1,0}, new int[] {1,1} }},
                { "corridor", new int[][] { new int[] {0,0}, new int[] {0,1}, new int[] {0,2} }},
                { "stair", new int[][] { new int[] {0,0} }},
                { "next", new int[][] { new int[] {0,1} }}
            }
        },
        { "east", new Dictionary<string, int[][]>()
            {
                { "walled", new int[][] { new int[] {-1,-1}, new int[] {-1,0}, new int[] {-1,1}, new int[] {0,1}, new int[] {1,1}, new int[] {1,0}, new int[] {1,-1} }},
                { "corridor", new int[][] { new int[] {0,0}, new int[] {0,-1}, new int[] {0,-2} }},
                { "stair", new int[][] { new int[] {0,0} }},
                { "next", new int[][] { new int[] {0,-1} }}
            }
        }
    };

    protected static Dictionary<string, Dictionary<string, int[][]>> close_end = new Dictionary<string, Dictionary<string, int[][]>>()
    {
        { "north", new Dictionary<string, int[][]>()
            {
                { "walled", new int[][] { new int[] {0,-1}, new int[] {1,-1}, new int[] {1,0}, new int[] {1,1}, new int[] {0,1} }},
                { "close", new int[][] { new int[] {0,0} }},
                { "recurse", new int[][] { new int[] {-1,0} }}
            }
        },
        { "south", new Dictionary<string, int[][]>()
            {
                { "walled", new int[][] { new int[] {0,-1}, new int[] {-1,-1}, new int[] {-1,0}, new int[] {-1,1}, new int[] {0,1} }},
                { "close", new int[][] { new int[] {0,0} }},
                { "recurse", new int[][] { new int[] {1,0} }}
            }
        },
        { "west", new Dictionary<string, int[][]>()
            {
                { "walled", new int[][] { new int[] {-1,0}, new int[] {-1,1}, new int[] {0,1}, new int[] {1,1}, new int[] {1,0} }},
                { "close", new int[][] { new int[] {0,0} }},
                { "recurse", new int[][] { new int[] {0,-1} }}
            }
        },
        { "east", new Dictionary<string, int[][]>()
            {
                { "walled", new int[][] { new int[] {-1,0}, new int[] {-1,-1}, new int[] {0,-1}, new int[] {1,-1}, new int[] {1,0} }},
                { "close", new int[][] { new int[] {0,0} }},
                { "recurse", new int[][] { new int[] {0,1} }}
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

    public int seed;
    public int n_rows;
    public int n_cols;
    public string dungeon_layout;
    public int room_min;
    public int room_max;
    public string room_layout;
    public string corridor_layout;
    public int remove_deadends;
    public int add_stairs;
    public string map_style;
    public int cell_size;

    public int n_i;
    public int n_j;
    public int max_row;
    public int max_col;
    public int n_rooms;
    public int room_base;
    public int room_radix;
    public uint[][] cell;

    public dungeon()
    {
        get_opts();
        create_dungeon();
    }

    protected void get_opts()
    {
        seed = System.Environment.TickCount;
        n_rows = 39;
        n_cols = 39;
        dungeon_layout = "None";
        room_min = 3;
        room_max = 9;
        room_layout = "Scattered";
        corridor_layout = "Bent";
        remove_deadends = 50;
        add_stairs = 2;
        map_style = "Standard";
        cell_size = 18;
    }

    private void create_dungeon()
    {
        n_i = (int)n_rows / 2;
        n_j = (int)n_cols / 2;
        n_rows = n_i * 2;
        n_cols = n_j * 2;
        max_row = n_rows - 1;
        max_col = n_cols - 1;
        n_rooms = 0;

        int max = room_max;
        int min = room_min;
        room_base = (int)((min + 1) / 2);
        room_radix = (int)((max - min) / 2) + 1;

        init_cells();
        //$dungeon = &emplace_rooms($dungeon);
        //$dungeon = &open_rooms($dungeon);
        //$dungeon = &label_rooms($dungeon);
        //$dungeon = &corridors($dungeon);
        //$dungeon = &emplace_stairs($dungeon) if ($dungeon->{'add_stairs'});
        //$dungeon = &clean_dungeon($dungeon);
    }

    private void init_cells()
    {
        for(int r = 0; r <= n_rows; r++)
        {
            for(int c = 0; c <= n_cols; c++)
            {
                cell[r][c] = NOTHING;
            }
        }
        Random.seed = seed;

        if (dungeon_layouts.ContainsKey(dungeon_layout))
        {
            mask_cells(dungeon_layouts[dungeon_layout]);
        }
        else if (dungeon_layout == "Round")
        {
            round_mask();
        }
    }

    private void round_mask()
    {
        int center_r = (int)n_rows / 2;
        int center_c = (int)n_cols / 2;

        for (int r = 0; r <= n_rows; r++)
        {
            for (int c = 0; c <= n_cols; c++)
            {
                float d = Mathf.Sqrt( Mathf.Pow(r - center_r, 2) + Mathf.Pow(c - center_c, 2));
                if (d > center_c)
                    cell[r][c] = BLOCKED;
            }
        }
    }

    private void mask_cells(int[][] p)
    {
        int r_x = (p.Length / (n_rows + 1));
        int c_x = (p[0].Length / (n_cols + 1));

        for (int r = 0; r <= n_rows; r++)
        {
            for (int c = 0; c <= n_cols; c++)
            {
                if(p[r * r_x][c * c_x] == 1)
                cell[r][c] = BLOCKED;
            }
        }
    }
}
