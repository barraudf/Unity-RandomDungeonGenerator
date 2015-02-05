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

    public static uint NOTHING     = 0x00000000;

    public static uint BLOCKED = 0x00000001;
    public static uint ROOM = 0x00000002;
    public static uint CORRIDOR = 0x00000004;
    //                 0x00000008;
    public static uint PERIMETER = 0x00000010;
    public static uint ENTRANCE = 0x00000020;
    public static uint ROOM_ID = 0x0000FFC0;

    public static uint ARCH = 0x00010000;
    public static uint DOOR = 0x00020000;
    public static uint LOCKED = 0x00040000;
    public static uint TRAPPED = 0x00080000;
    public static uint SECRET = 0x00100000;
    public static uint PORTC = 0x00200000;
    public static uint STAIR_DN = 0x00400000;
    public static uint STAIR_UP = 0x00800000;

    public static uint LABEL = 0xFF000000;

    public static uint OPENSPACE = ROOM | CORRIDOR;
    public static uint DOORSPACE = ARCH | DOOR | LOCKED | TRAPPED | SECRET | PORTC;
    public static uint ESPACE = ENTRANCE | DOORSPACE | 0xFF000000;
    public static uint STAIRS = STAIR_DN | STAIR_UP;

    public static uint BLOCK_ROOM = BLOCKED | ROOM;
    public static uint BLOCK_CORR = BLOCKED | PERIMETER | CORRIDOR;
    public static uint BLOCK_DOOR = BLOCKED | DOORSPACE;

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
    public SortedList<int, Dictionary<string, int>> rooms;
    public Dictionary<string, int> connect;

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
        room_layout = "Packed";
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

        rooms = new SortedList<int, Dictionary<string, int>>();
        connect = new Dictionary<string,int>();

        init_cells();
        emplace_rooms();
        open_rooms();
        label_rooms();
        corridors();
        //$dungeon = &emplace_stairs($dungeon) if ($dungeon->{'add_stairs'});
        //$dungeon = &clean_dungeon($dungeon);
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
                if ((i == 0 || j == 0) && Random.value > 0.5) continue;

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
        int n_r = (int)dungeon_area / room_area;
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

        Dictionary<string, int> room_data = new Dictionary<string,int>()
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

                proto.Add("height", Random.Range(0, r) + i_base);
            }
            else
            {
                proto.Add("height", Random.Range(0, radix) + i_base);
            }
        }
        if(!proto.ContainsKey("width"))
        {
            if (proto.ContainsKey("j"))
            {
                int a = n_j - i_base - proto["j"];
                if (a < 0) a = 0;
                int r = (a < radix) ? a : radix;

                proto.Add("width", Random.Range(0, r) + i_base);
            }
            else
            {
                proto.Add("width", Random.Range(0, radix) + i_base);
            }
        }
        
        if(!proto.ContainsKey("i"))
        {
            proto.Add("i", Random.Range(0, n_i - proto["height"]));
        }
        if(!proto.ContainsKey("j"))
        {
            proto.Add("j", Random.Range(0, n_j - proto["width"]));
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

    private void open_room(Dictionary<string, int> room)
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
                    sill = Splice(list, Random.Range(0, list.Count));
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
                    string strConnect = Math.Min(room["id"], out_id).ToString() + "," + Math.Max(room["id"], out_id).ToString();

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
           
            //push(@{ $room->{'door'}{$open_dir} },$door) if ($door);
        }
        next: ;
    }

    private List<Dictionary<string, object>> door_sills(Dictionary<string, int> room)
    {
        List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

        if (room["north"] >= 3)
        {
            for (int c = room["west"]; c <= room["east"]; c += 2)
            {
                Dictionary<string, object> sill = check_sill(room, room["north"], c, "north");
                if (sill != null)
                    list.Add(sill);
            }
        }
        if (room["south"] <= (n_rows - 3))
        {
            for (int c = room["west"]; c <= room["east"]; c += 2)
            {
                Dictionary<string, object> sill = check_sill(room, room["south"], c, "south");
                if (sill != null)
                    list.Add(sill);
            }
        }
        if (room["west"] >= 3)
        {
            for (int r = room["north"]; r <= room["south"]; r += 2)
            {
                Dictionary<string, object> sill = check_sill(room, r, room["west"], "west");
                if (sill != null)
                    list.Add(sill);
            }
        }
        if (room["east"] <= (n_cols - 3))
        {
            for (int r = room["north"]; r <= room["south"]; r += 2)
            {
                Dictionary<string, object> sill = check_sill(room, r, room["east"], "east");
                if (sill != null)
                    list.Add(sill);
        }
        }
        return shuffle(list);
    }

    private Dictionary<string, object> check_sill(Dictionary<string, int> room, int sill_r, int sill_c, string dir)
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
            if (out_id == room["id"])
                return null;
            ret.Add("out_id", out_id);
        }
        
        return ret;
    }

    private static List<T> shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }

    private int alloc_opens(Dictionary<string, int> room)
    {
        int room_h = ((room["south"] - room["north"]) / 2) + 1;
        int room_w = ((room["east"] - room["west"]) / 2) + 1;
        int flumph = (int)Math.Sqrt(room_w * room_h);
        int n_opens = flumph + Random.Range(0, flumph);

        return n_opens;
    }

    private uint set_door_type()
    {
        int i = Random.Range(0, 110);

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

    public Dictionary<string, object> Splice(List<Dictionary<string, object>> Source, int Start)
    {
        if (Source.Count == 0)
            return null;

        Dictionary<string, object> retVal = Source[Start];
        Source.RemoveRange(Start, 1);
        return retVal;
    }

    private void label_rooms()
    {
        for (int id = 1; id <= n_rooms; id++)
        {
            Dictionary<string, int> room = rooms[id];
            string label = room["id"].ToString();
            int len = label.Length;
            int label_r = (int)(room["north"] + room["south"]) / 2;
            int label_c = (int)((room["west"] + room["east"] - len) / 2) + 1;

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

        if (last_dir != null && p > 0 && Random.Range(0, 100) < p)
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
        int r1 = Math.Min(this_r,next_r);
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
}
