using System;
using System.Collections.Generic;

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

    public static uint NOTHING      = 0x00000000;

    public static uint BLOCKED      = 0x00000001;
    public static uint ROOM         = 0x00000002;
    public static uint CORRIDOR     = 0x00000004;
    //                                0x00000008;
    public static uint PERIMETER    = 0x00000010;
    public static uint ENTRANCE     = 0x00000020;
    public static uint ROOM_ID      = 0x0000FFC0;

    public static uint ARCH         = 0x00010000;
    public static uint DOOR         = 0x00020000;
    public static uint LOCKED       = 0x00040000;
    public static uint TRAPPED      = 0x00080000;
    public static uint SECRET       = 0x00100000;
    public static uint PORTC        = 0x00200000;
    public static uint STAIR_DN     = 0x00400000;
    public static uint STAIR_UP     = 0x00800000;

    public static uint LABEL        = 0xFF000000;

    public static uint OPENSPACE    = ROOM | CORRIDOR;
    public static uint DOORSPACE    = ARCH | DOOR | LOCKED | TRAPPED | SECRET | PORTC;
    public static uint ESPACE       = ENTRANCE | DOORSPACE | 0xFF000000;
    public static uint STAIRS       = STAIR_DN | STAIR_UP;

    public static uint BLOCK_ROOM   = BLOCKED | ROOM;
    public static uint BLOCK_CORR   = BLOCKED | PERIMETER | CORRIDOR;
    public static uint BLOCK_DOOR   = BLOCKED | DOORSPACE;

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
    public int last_room_id;
    public SortedList<int, Dictionary<string, object>> rooms;
    public List<Dictionary<string, object>> doors;
    public List<Dictionary<string, object>> stairs;
    public Dictionary<string, int> connect;
    public Random rnd;

    public dungeon(DungeonOptions opts)
    {
        get_opts(opts);
        create_dungeon();
    }

    protected void get_opts(DungeonOptions opts)
    {
        seed = opts.Seed;
        n_rows = opts.NRows;
        n_cols = opts.NCols;
        dungeon_layout = opts.DungeonLayout;
        room_min = opts.RoomMin;
        room_max = opts.RoomMax;
        room_layout = opts.RoomLayout;
        corridor_layout = opts.CorridorLayout;
        remove_deadends = opts.RemoveDeadends;
        add_stairs = opts.AddStairs;
    }

    private void create_dungeon()
    {
        n_i = n_rows / 2;
        n_j = n_cols / 2;
        n_rows = n_i * 2;
        n_cols = n_j * 2;
        max_row = n_rows - 1;
        max_col = n_cols - 1;
        n_rooms = 0;

        int max = room_max;
        int min = room_min;
        room_base = (min + 1) / 2;
        room_radix = ((max - min) / 2) + 1;

        rooms = new SortedList<int, Dictionary<string, object>>();
        connect = new Dictionary<string,int>();
        stairs = new List<Dictionary<string, object>>();
        doors = new List<Dictionary<string,object>>();

        init_cells();
        emplace_rooms();
        open_rooms();
        label_rooms();
        corridors();
        if(add_stairs > 0)
            emplace_stairs();
        clean_dungeon();
    }

    private void init_cells()
    {
        cell = new uint[n_rows+1][];

        for(int r = 0; r <= n_rows; r++)
        {
            cell[r] = new uint[n_cols+1];
            for(int c = 0; c <= n_cols; c++)
            {
                cell[r][c] = NOTHING;
            }
        }
        rnd = new Random(seed);

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
        int center_r = n_rows / 2;
        int center_c = n_cols / 2;

        for (int r = 0; r <= n_rows; r++)
        {
            for (int c = 0; c <= n_cols; c++)
            {
                double d = Math.Sqrt( Math.Pow(r - center_r, 2) + Math.Pow(c - center_c, 2));
                if (d > center_c)
                    cell[r][c] = BLOCKED;
            }
        }
    }

    private void mask_cells(int[][] mask)
    {
        float r_x = mask.Length * 1.0f / (n_rows + 1);
        float c_x = mask[0].Length * 1.0f / (n_cols + 1);

        for (int r = 0; r <= n_rows; r++)
        {
            for (int c = 0; c <= n_cols; c++)
            {
                if (mask[(int)(r * r_x)][(int)(c * c_x)] == 0)
                    cell[r][c] = BLOCKED;
            }
        }
    }

    private void emplace_rooms()
    {
        if (room_layout == "Packed")
            pack_rooms();
        else
            scatter_rooms();
    }

    private void pack_rooms()
    {
        for(int i = 0; i < n_i; i++)
        {
            int r = (i * 2) + 1;
            for(int j = 0; j < n_j; j++)
            {
                int c = (j * 2) + 1;

                if ((cell[r][c] & ROOM) != NOTHING) continue;
                if ((i == 0 || j == 0) && (rnd.Next(2) == 1)) continue;

                Dictionary<string, int> proto = new Dictionary<string,int>() { {"i",i}, {"j",j} };
                emplace_room(proto);
            }
        }
    }

    private void scatter_rooms()
    {
        int n_r = alloc_rooms();
        for (int i = 0; i < n_r; i++)
            emplace_room();
    }

    private int alloc_rooms()
    {
        int dungeon_area = n_cols * n_rows;
        int room_area = room_max * room_max;
        int n_r = dungeon_area / room_area;

        return n_r;
    }

    private void emplace_room(Dictionary<string, int> proto = null)
    {
        if(n_rooms == 999) return;

        if(proto == null)
            proto = new Dictionary<string,int>();

        int r,c;

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // room position and size

        set_room(proto);

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // room boundaries

        int r1 = ( proto["i"]                    * 2) + 1;
        int c1 = ( proto["j"]                    * 2) + 1;
        int r2 = ((proto["i"] + proto["height"]) * 2) - 1;
        int c2 = ((proto["j"] + proto["width"] ) * 2) - 1;

        if (r1 < 1 || r2 > max_row) return;
        if (c1 < 1 || c2 > max_col) return;

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // check for collisions with existing rooms

        Dictionary<string, int> hit = sound_room(r1,c1,r2,c2);
        if (hit.ContainsKey("blocked")) return;
        int n_hits = hit.Count;
        int room_id;

        if (n_hits == 0)
        {
            room_id = n_rooms + 1;
            n_rooms = room_id;
        }
        else
        {
            return;
        }
        last_room_id = room_id;

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // emplace room

        for (r = r1; r <= r2; r++)
        {
            for (c = c1; c <= c2; c++)
            {
                if ((cell[r][c] & ENTRANCE) != NOTHING)
                {
                    cell[r][c] &= ~ ESPACE;
                }
                else if ((cell[r][c] & PERIMETER) != NOTHING)
                {
                    cell[r][c] &= ~ PERIMETER;
                }
                cell[r][c] |= ROOM | (uint)(room_id << 6);
            }
        }
        int height = ((r2 - r1) + 1) * 10;
        int width = ((c2 - c1) + 1) * 10;

        Dictionary<string, object> room_data = new Dictionary<string, object>()
        {
            { "id", room_id },
            { "row", r1 },
            { "col", c1 },
            { "north", r1 },
            { "south", r2 },
            { "west", c1 },
            { "east", c2 },
            { "height", height },
            { "width", width },
            { "area", height * width }
            
        };
        rooms.Add(room_id, room_data);

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // block corridors from room boundary
        // check for door openings from adjacent rooms

        for (r = r1 - 1; r <= r2 + 1; r++)
        {
            if ((cell[r][c1 - 1] & (ROOM | ENTRANCE)) == NOTHING)
            {
                cell[r][c1 - 1] |= PERIMETER;
            }
            if ((cell[r][c2 + 1] & (ROOM | ENTRANCE)) == NOTHING)
            {
                cell[r][c2 + 1] |= PERIMETER;
            }
        }
        for (c = c1 - 1; c <= c2 + 1; c++)
        {
            if ((cell[r1 - 1][c] & (ROOM | ENTRANCE)) == NOTHING)
            {
                cell[r1 - 1][c] |= PERIMETER;
            }
            if ((cell[r2 + 1][c] & (ROOM | ENTRANCE)) == NOTHING)
            {
                cell[r2 + 1][c] |= PERIMETER;
            }
        }
    }

    private void set_room(Dictionary<string,int> proto)
    {
        int i_base = room_base;
        int radix = room_radix;

        if(!proto.ContainsKey("height"))
        {
            if (proto.ContainsKey("i"))
            {
                int a = n_i - i_base - proto["i"];
                if (a < 0) a = 0;
                int r = (a < radix) ? a : radix;

                proto.Add("height", rnd.Next(r) + i_base);
            }
            else
            {
                proto.Add("height", rnd.Next(radix) + i_base);
            }
        }
        if(!proto.ContainsKey("width"))
        {
            if (proto.ContainsKey("j"))
            {
                int a = n_j - i_base - proto["j"];
                if (a < 0) a = 0;
                int r = (a < radix) ? a : radix;

                proto.Add("width", rnd.Next(r) + i_base);
            }
            else
            {
                proto.Add("width", rnd.Next(radix) + i_base);
            }
        }
        
        if(!proto.ContainsKey("i"))
        {
            proto.Add("i", rnd.Next(n_i - proto["height"]));
        }
        if(!proto.ContainsKey("j"))
        {
            proto.Add("j", rnd.Next(n_j - proto["width"]));
        }
    }

    private Dictionary<string,int> sound_room(int r1,int c1,int r2,int c2)
    {
        Dictionary<string, int> hit = new Dictionary<string,int>();

        for (int r = r1; r <= r2; r++)
        {
            for (int c = c1; c <= c2; c++)
            {
                if ((cell[r][c] & BLOCKED) != NOTHING)
                {
                    hit.Add("blocked", 1);
                    return hit;
                }
                if ((cell[r][c] & ROOM) != NOTHING)
                {
                    uint id = (cell[r][c] & ROOM_ID) >> 6;

                    if(hit.ContainsKey(id.ToString()))
                        hit[id.ToString()]++;
                    else
                        hit.Add(id.ToString(), 1);
                }
            }
        }
        return hit;
    }

    private void open_rooms()
    {
        for (int id = 1; id <= n_rooms; id++)
        {
            open_room(rooms[id]);
        }
        connect.Clear();
    }

    private void open_room(Dictionary<string, object> room)
    {
        List<Dictionary<string, object>> list = door_sills(room);
        if(list == null || list.Count == 0) return;
        int n_opens = alloc_opens(room);

        for (int i = 0; i < n_opens; i++)
        {
            Dictionary<string, object> sill = null;
            int door_r = 0;
            int door_c = 0;
            uint door_cell = 0;
            bool doContinue = false;
            do
            {
                doContinue = false;
                do
                {
                    sill = Splice(list, rnd.Next(list.Count));
                    if (sill == null)
                        goto next;
                    door_r = (int)sill["door_r"];
                    door_c = (int)sill["door_c"];
                    door_cell = cell[door_r][door_c];
                }
                while ((door_cell & DOORSPACE) != NOTHING);

                int out_id = 0;
                if (sill.ContainsKey("out_id"))
                {
                    out_id = (int)sill["out_id"];
                    string strConnect = Math.Min((int)room["id"], out_id).ToString() + "," + Math.Max((int)room["id"], out_id).ToString();

                    if (connect.ContainsKey(strConnect))
                    {
                        connect[strConnect]++;
                        doContinue = true;
                    }
                    else
                        connect.Add(strConnect, 1);
                }
            }
            while (doContinue);

            int open_r = (int)sill["sill_r"];
            int open_c = (int)sill["sill_c"];
            string open_dir = (string)sill["dir"];

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            // open door

            for (int x = 0; x < 3; x++)
            {
                int r = open_r + (di[open_dir] * x);
                int c = open_c + (dj[open_dir] * x);

                cell[r][c] &= ~ PERIMETER;
                cell[r][c] |= ENTRANCE;
            }
            uint door_type = set_door_type();
            Dictionary<string, object> door =  new Dictionary<string,object>() { {"row", door_r}, {"col", door_c} };

            if (door_type == ARCH)
            {
                cell[door_r][door_c] |= ARCH;
                door.Add("key", "arch"); door.Add("type", "Archway");
            }
            else if (door_type == DOOR)
            {
                cell[door_r][door_c] |= DOOR;
                cell[door_r][door_c] |= (Convert.ToUInt32('o') << 24);
                door.Add("key", "open"); door.Add("type", "Unlocked Door");
            }
            else if (door_type == LOCKED)
            {
                cell[door_r][door_c] |= LOCKED;
                cell[door_r][door_c] |= (Convert.ToUInt32('x') << 24);
                door.Add("key", "lock"); door.Add("type", "Locked Door");
            }
            else if (door_type == TRAPPED)
            {
                cell[door_r][door_c] |= TRAPPED;
                cell[door_r][door_c] |= (Convert.ToUInt32('t') << 24);
                door.Add("key", "trap"); door.Add("type", "Trapped Door");
            }
            else if (door_type == SECRET)
            {
                cell[door_r][door_c] |= SECRET;
                cell[door_r][door_c] |= (Convert.ToUInt32('s') << 24);
                door.Add("key", "secret"); door.Add("type", "Secret Door");
            }
            else if (door_type == PORTC)
            {
                cell[door_r][door_c] |= PORTC;
                cell[door_r][door_c] |= (Convert.ToUInt32('#') << 24);
                door.Add("key", "portc"); door.Add("type", "Portcullis");
            }

            if(sill.ContainsKey("out_id"))
                door.Add("out_id", (int)sill["out_id"]);
           
            if(!room.ContainsKey("door"))
                room.Add("door", new Dictionary<string, List<Dictionary<string, object>>>());
            Dictionary<string, List<Dictionary<string, object>>> d  = (Dictionary<string, List<Dictionary<string, object>>>)room["door"];
            if (!d.ContainsKey(open_dir))
                d.Add(open_dir, new List<Dictionary<string, object>>());
            d[open_dir].Add(door);
        }
        next: ;
    }

    private List<Dictionary<string, object>> door_sills(Dictionary<string, object> room)
    {
        List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

        if ((int)room["north"] >= 3)
        {
            for (int c = (int)room["west"]; c <= (int)room["east"]; c += 2)
            {
                Dictionary<string, object> sill = check_sill(room, (int)room["north"], c, "north");
                if (sill != null)
                    list.Add(sill);
            }
        }
        if ((int)room["south"] <= (n_rows - 3))
        {
            for (int c = (int)room["west"]; c <= (int)room["east"]; c += 2)
            {
                Dictionary<string, object> sill = check_sill(room, (int)room["south"], c, "south");
                if (sill != null)
                    list.Add(sill);
            }
        }
        if ((int)room["west"] >= 3)
        {
            for (int r = (int)room["north"]; r <= (int)room["south"]; r += 2)
            {
                Dictionary<string, object> sill = check_sill(room, r, (int)room["west"], "west");
                if (sill != null)
                    list.Add(sill);
            }
        }
        if ((int)room["east"] <= (n_cols - 3))
        {
            for (int r = (int)room["north"]; r <= (int)room["south"]; r += 2)
            {
                Dictionary<string, object> sill = check_sill(room, r, (int)room["east"], "east");
                if (sill != null)
                    list.Add(sill);
        }
        }
        return shuffle(list);
    }

    private Dictionary<string, object> check_sill(Dictionary<string, object> room, int sill_r, int sill_c, string dir)
    {
        int door_r = sill_r + di[dir];
        int door_c = sill_c + dj[dir];
        uint door_cell = cell[door_r][door_c];
        if ((door_cell & PERIMETER) == NOTHING) return null;
        if ((door_cell & BLOCK_DOOR) != NOTHING) return null;
        int out_r  = door_r + di[dir];
        int out_c  = door_c + dj[dir];
        uint out_cell = cell[out_r][out_c];
        if ((out_cell & BLOCKED) != NOTHING) return null;

        Dictionary<string, object> ret = new Dictionary<string, object>()
        {
            {"sill_r",    sill_r},
            {"sill_c",    sill_c},
            {"dir",       dir},
            {"door_r",    door_r},
            {"door_c",    door_c}
        };

        if((out_cell & ROOM) != NOTHING)
        {
            int out_id = (int)(out_cell & ROOM_ID) >> 6;
            if (out_id == (int)room["id"])
                return null;
            ret.Add("out_id", out_id);
        }
        
        return ret;
    }

    private List<T> shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }

    private int alloc_opens(Dictionary<string, object> room)
    {
        int room_h = (((int)room["south"] - (int)room["north"]) / 2) + 1;
        int room_w = (((int)room["east"] - (int)room["west"]) / 2) + 1;
        int flumph = (int)Math.Sqrt(room_w * room_h);
        int n_opens = flumph + rnd.Next(flumph);

        return n_opens;
    }

    private uint set_door_type()
    {
        int i = rnd.Next(110);

        if (i < 15)
            return ARCH;
        else if (i < 60)
            return DOOR;
        else if (i < 75)
            return LOCKED;
        else if (i < 90)
            return TRAPPED;
        else if (i < 100)
            return SECRET;
        else
            return PORTC;
    }

    public T Splice<T>(List<T> Source, int Start)
    {
        if (Source.Count == 0)
            return default(T);

        T retVal = Source[Start];
        Source.RemoveRange(Start, 1);
        return retVal;
    }

    private void label_rooms()
    {
        for (int id = 1; id <= n_rooms; id++)
        {
            Dictionary<string, object> room = rooms[id];
            string label = room["id"].ToString();
            int len = label.Length;
            int label_r = ((int)room["north"] + (int)room["south"]) / 2;
            int label_c = (((int)room["west"] + (int)room["east"] - len) / 2) + 1;

            for (int c = 0; c < len; c++)
            {
                char ch = label[c];
                cell[label_r][label_c + c] |= (Convert.ToUInt32(ch) << 24);
            }
        }
    }

    private void corridors()
    {
        for (int i = 1; i < n_i; i++)
        {
            int r = (i * 2) + 1;
            for (int j = 1; j < n_j; j++)
            {
                int c = (j * 2) + 1;

                if ((cell[r][c] & CORRIDOR) != NOTHING) continue;
                tunnel(i,j);
            }
        }
    }

    private void tunnel(int i, int j, string last_dir = null)
    {
        string[] dirs = tunnel_dirs(last_dir);

        foreach(string dir in dirs)
        {
            if (open_tunnel(i,j,dir))
            {
                int next_i = i + di[dir];
                int next_j = j + dj[dir];

                tunnel(next_i,next_j,dir);
            }
        }
    }

    private string[] tunnel_dirs(string last_dir)
    {
        int p = corridor_layouts[corridor_layout];
        List<string> dirs = shuffle(new List<string>(dj.Keys));

        if (last_dir != null && p > 0 && rnd.Next(100) < p)
                dirs.InsertRange(0, new string[] { last_dir });
        
        return dirs.ToArray();
    }

    private bool open_tunnel(int i, int j, string dir)
    {
        int this_r = (i * 2) + 1;
        int this_c = (j * 2) + 1;
        int next_r = ((i + di[dir]) * 2) + 1;
        int next_c = ((j + dj[dir]) * 2) + 1;
        int mid_r = (this_r + next_r) / 2;
        int mid_c = (this_c + next_c) / 2;

        if (sound_tunnel(mid_r,mid_c,next_r,next_c))
        {
            return delve_tunnel(this_r,this_c,next_r,next_c);
        }
        else
        {
            return false;
        }
    }

    private bool sound_tunnel(int mid_r, int mid_c, int next_r, int next_c)
    {
        if (next_r < 0 || next_r > n_rows) return false;
        if (next_c < 0 || next_c > n_cols) return false;
        
        int r1 = Math.Min(mid_r,next_r);
        int r2 = Math.Max(mid_r, next_r);
        int c1 = Math.Min(mid_c, next_c);
        int c2 = Math.Max(mid_c, next_c);

        for (int r = r1; r <= r2; r++)
        {
            for (int c = c1; c <= c2; c++)
            {
                if ((cell[r][c] & BLOCK_CORR) != NOTHING) return false;
            }
        }

        return true;
    }

    private bool delve_tunnel(int this_r, int this_c, int next_r, int next_c)
    {
        int r1 = Math.Min(this_r, next_r);
        int r2 = Math.Max(this_r, next_r);
        int c1 = Math.Min(this_c, next_c);
        int c2 = Math.Max(this_c, next_c);

        for (int r = r1; r <= r2; r++)
        {
            for (int c = c1; c <= c2; c++)
            {
                cell[r][c] &= ~ ENTRANCE;
                cell[r][c] |= CORRIDOR;
            }
        }
        return true;
    }

    private void emplace_stairs()
    {
        int n = add_stairs;
        if(n == 0) return;
        List<Dictionary<string, object>> list = stair_ends();
        if(list.Count == 0) return;

        for (int i = 0; i < n; i++)
        {
            Dictionary<string, object> stair = Splice(list,rnd.Next(list.Count));
            //if(stair == null) return;
            int r = (int)stair["row"];
            int c = (int)stair["col"];
            int type = (i < 2) ? i : rnd.Next(2);

            if (type == 0)
            {
                cell[r][c] |= STAIR_DN;
                cell[r][c] |= (Convert.ToUInt32('d') << 24);
                stair.Add("key", "down");
            }
            else
            {
                cell[r][c] |= STAIR_UP;
                cell[r][c] |= (Convert.ToUInt32('u') << 24);
                stair.Add("key","up");
            }
            stairs.Add(stair);
        }
    }

    private List<Dictionary<string, object>> stair_ends()
    {
        List<Dictionary<string, object>> list = new List<Dictionary<string,object>>();

        //ROW:
        for (int i = 0; i < n_i; i++)
        {
            int r = (i * 2) + 1;
            //COL:
            for (int j = 0; j < n_j; j++)
            {
                int c = (j * 2) + 1;

                if((cell[r][c] & CORRIDOR) == NOTHING) continue;
                if((cell[r][c] & STAIRS) != NOTHING) continue;

                foreach(string dir in stair_end.Keys)
                {
                    if (check_tunnel(r,c,stair_end[dir]))
                    {
                        Dictionary<string, object> end = new Dictionary<string, object>() { { "row", r }, { "col", c } };
                        int[] n = stair_end[dir]["next"][0];
                        end.Add("next_row", (int)end["row"] + n[0]);
                        end.Add("next_col", (int)end["col"] + n[1]);

                        list.Add(end);

                        break;
                    }
                }
            }
        }
        return list;
    }

    private bool check_tunnel(int r, int c, Dictionary<string, int[][]>check)
    {
        int[][] list;

        if (check.ContainsKey("corridor"))
        {
            list = check["corridor"];
            foreach(int[] p in list)
            {
                if((cell[r+p[0]][c+p[1]] & CORRIDOR) == NOTHING)
                    return false;
            }
        }
        if (check.ContainsKey("walled"))
        {
            list = check["walled"];
            foreach(int[] p in list)
            {
                if((cell[r+p[0]][c+p[1]] & OPENSPACE) != NOTHING)
                    return false;
            }
        }
        return true;
    }

    private void clean_dungeon()
    {
        if (remove_deadends > 0)
        {
            removeDeadends();
        }
        fix_doors();
        empty_blocks();
    }

    private void removeDeadends()
    {
        int p = remove_deadends;

        collapse_tunnels(p, close_end);
    }

    private void collapse_tunnels(int p, Dictionary<string, Dictionary<string, int[][]>> xc)
    {
        if(p == 0) return;
        
        bool all = (p == 100);

        for (int i = 0; i < n_i; i++)
        {
            int r = (i * 2) + 1;
            for(int j = 0; j < n_j; j++)
            {
                int c = (j * 2) + 1;

                if((cell[r][c] & OPENSPACE) == NOTHING) continue;
                if((cell[r][c] & STAIRS) != NOTHING) continue;
                if( (all || (rnd.Next(100)) < p) == false) continue;

                collapse(r,c,xc);
            }
        }
    }

    private void collapse(int r, int c, Dictionary<string, Dictionary<string, int[][]>> xc)
    {
        if((cell[r][c] & OPENSPACE) == NOTHING)
        {
            return;
        }
        foreach(string dir in xc.Keys)
        {
            if (check_tunnel(r,c, xc[dir]))
            {
                foreach(int[] p in xc[dir]["close"])
                {
                    cell[r+p[0]][c+p[1]] = NOTHING;
                }
                if(xc[dir].ContainsKey("open"))
                {
                    int[] p = xc[dir]["open"][0];
                    cell[r+p[0]][c+p[1]] |= CORRIDOR;
                }
                if (xc[dir].ContainsKey("recurse"))
                {
                    int[] p = xc[dir]["recurse"][0];
                    collapse(r+p[0], c+p[1], xc);
                }
            }
        }
    }

    private void fix_doors()
    {
        try
        {
            bool[][] fix = new bool[n_rows + 1][];
            for (int r = 0; r <= n_rows; r++)
            {
                fix[r] = new bool[n_cols + 1];
                for (int c = 0; c <= n_cols; c++)
                    fix[r][c] = false;
            }

            foreach (Dictionary<string, object> room in rooms.Values)
            {
                List<string> dirs = new List<string>( ((Dictionary<string, List<Dictionary<string, object>>>)room["door"]).Keys );
                foreach (string dir in dirs)
                {
                    List<Dictionary<string, object>> shiny = new List<Dictionary<string, object>>();
                    List<Dictionary<string, object>> doors = ((Dictionary<string, List<Dictionary<string, object>>>)room["door"])[dir];
                    foreach (Dictionary<string, object> door in doors)
                    {
                        int door_r = (int)door["row"];
                        int door_c = (int)door["col"];
                        uint door_cell = cell[door_r][door_c];
                        if ((door_cell & OPENSPACE) == NOTHING) continue;

                        if (fix[door_r][door_c])
                        {
                            shiny.Add(door);
                        }
                        else
                        {
                            int out_id;
                            if (door.ContainsKey("out_id"))
                            {
                                out_id = (int)door["out_id"];
                                string out_dir = opposite[dir];

                                Dictionary<string, object> out_room = rooms[out_id];
                                if (!out_room.ContainsKey("door"))
                                    out_room.Add("door", new Dictionary<string, List<Dictionary<string, object>>>());
                                Dictionary<string, List<Dictionary<string, object>>> d = (Dictionary<string, List<Dictionary<string, object>>>)out_room["door"];
                                if (!d.ContainsKey(out_dir))
                                    d.Add(out_dir, new List<Dictionary<string, object>>());
                                d[out_dir].Add(door);
                            }
                            shiny.Add(door);
                            fix[door_r][door_c] = true;
                        }
                    }
                    if (shiny.Count > 0)
                    {
                        doors = shiny;
                        doors.AddRange(shiny);
                    }
                    else
                    {
                        ((Dictionary<string, List<Dictionary<string, object>>>)room["door"]).Remove(dir);
                    }
                }
            }
        }
        catch
        {
            throw;
        }
    }

    private void empty_blocks()
    {
        for(int r = 0; r <= n_rows; r++)
            for(int c = 0; c <= n_cols; c++)
                if((cell[r][c] & BLOCKED) != NOTHING)
                    cell[r][c] = NOTHING;
    }
}
