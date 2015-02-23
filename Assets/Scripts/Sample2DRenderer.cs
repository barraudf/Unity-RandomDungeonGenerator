using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Sample2DRenderer : MonoBehaviour
{
    public int Seed = System.Environment.TickCount;
    public int NRows = 39;
    public int NCols = 39;
    public EnumDungeonLayouts DungeonLayout = EnumDungeonLayouts.None;
    public int RoomMin = 3;
    public int RoomMax = 9;
    public string RoomLayout = "Packed";
    public string CorridorLayout = "Straight";
    public int RemoveDeadends = 100;
    public int AddStairs = 0;
    public int TileResolution = 12;

    private MeshFilter _MeshFilter;
    //private MeshCollider _MeshCollider;
    private MeshRenderer _MeshRenderer;

    public Sprite DoorSprite;
    public Sprite EmptySprite;
    public Sprite RoomSprite;

    public MeshRenderer MeshRenderer
    {
        get { return _MeshRenderer; }
    }

	// Use this for initialization
	void Start ()
    {
        dungeon d = GenerateDungeon();
        BuildMesh(d);
	}

    public void InitializeComponents()
    {
        //_MeshCollider = GetComponent<MeshCollider>();
        _MeshFilter = GetComponent<MeshFilter>();
        _MeshRenderer = GetComponent<MeshRenderer>();
    }

    public dungeon GenerateDungeon()
    {
        DungeonOptions opts = new DungeonOptions()
        {
            Seed = Seed,
            NRows = NRows,
            NCols = NCols,
            RoomMin = RoomMin,
            RoomMax = RoomMax,
            RoomLayout = RoomLayout,
            CorridorLayout = CorridorLayout,
            RemoveDeadends = RemoveDeadends,
            AddStairs = AddStairs
        };

        switch(DungeonLayout)
        {
            case EnumDungeonLayouts.Box:
                opts.DungeonLayout = new BoxLayout();
                break;
            case EnumDungeonLayouts.Cross:
                opts.DungeonLayout = new CrossLayout();
                break;
            case EnumDungeonLayouts.Round:
                opts.DungeonLayout = new RoundLayout();
                break;
            default:
                opts.DungeonLayout = null;
                break;
        }

        return new dungeon(opts);
    }

    public void PrintMap(dungeon d)
    {
        string map = string.Empty;
        for (int r = 0; r <= d.n_rows; r++)
        {
            if (map.Length > 0)
                map += "\n";
            for (int c = 0; c <= d.n_cols; c++)
            {
                //if ((d.cell[r][c] & dungeon.PERIMETER) != dungeon.NOTHING)
                //    map += "0";
                if ((d.cell[r][c] & dungeon.DOORSPACE) != dungeon.NOTHING)
                    map += "-";
                else if ((d.cell[r][c] & dungeon.ROOM) != dungeon.NOTHING)
                    map += "#";
                else if ((d.cell[r][c] & dungeon.CORRIDOR) != dungeon.NOTHING)
                    map += "#";
                else
                    map += " ";
            }
        }
        Debug.Log(map);
    }

    public void BuildMesh(dungeon d)
    {
        int NumTiles = (d.n_rows + 1) * (d.n_cols + 1);
        int NumTris = NumTiles * 2;

        int SizeX = d.n_cols + 1;
        int SizeY = d.n_rows + 1;
        int VSizeX = SizeX + 1;
        int VSizeY = SizeY + 1;
        int NumVerts = VSizeX * VSizeY;

        Vector3[] Vertices = new Vector3[NumVerts];
        Vector3[] Normals = new Vector3[NumVerts];
        Vector2[] UV = new Vector2[NumVerts];

        int[] Triangles = new int[NumTris * 3];

        for (int y = 0; y < VSizeY; y++)
            for (int x = 0; x < VSizeX; x++)
            {
                int SquareIndex = y * VSizeX + x;
                Vertices[SquareIndex] = new Vector3(x, y, 0);
                Normals[SquareIndex] = Vector3.up;
                UV[SquareIndex] = new Vector2(x * 1.0f / SizeX, y * 1.0f / SizeY);
            }
        
        for (int y = 0; y < SizeY; y++)
            for (int x = 0; x < SizeX; x++)
            {
                int SquareIndex = y * SizeX + x;
                int TriOffset = SquareIndex * 6;

                Triangles[TriOffset + 0] = y * VSizeX + x +          0;
                Triangles[TriOffset + 1] = y * VSizeX + x + VSizeX + 0;
                Triangles[TriOffset + 2] = y * VSizeX + x + VSizeX + 1;

                Triangles[TriOffset + 3] = y * VSizeX + x +          0;
                Triangles[TriOffset + 4] = y * VSizeX + x + VSizeX + 1;
                Triangles[TriOffset + 5] = y * VSizeX + x +          1;
            }

        Mesh Mesh = new Mesh();
        Mesh.vertices = Vertices;
        Mesh.triangles = Triangles;
        Mesh.normals = Normals;
        Mesh.uv = UV;

        if (_MeshFilter == null)
            InitializeComponents();
        _MeshFilter.mesh = Mesh;

        BuildTexture(d);
    }

    protected void BuildTexture(dungeon d)
    {
        int TexWidth = (d.n_cols + 1) * TileResolution;
        int TexHeight = (d.n_rows + 1) * TileResolution;

        Texture2D Texture = new Texture2D(TexWidth, TexHeight);

        for (int r = 0; r <= d.n_rows; r++)
        {
            for (int c = 0; c <= d.n_cols; c++)
            {
                Sprite s;
                if ((d.cell[r][c] & dungeon.DOORSPACE) != dungeon.NOTHING)
                    s = DoorSprite;
                else if ((d.cell[r][c] & dungeon.ROOM) != dungeon.NOTHING)
                    s = RoomSprite;
                else if ((d.cell[r][c] & dungeon.CORRIDOR) != dungeon.NOTHING)
                    s = RoomSprite;
                else
                    s = EmptySprite;

                DrawCell(Texture, c * TileResolution, r * TileResolution, s);
            }
        }

        Texture.filterMode = FilterMode.Point;
        Texture.wrapMode = TextureWrapMode.Clamp;
        Texture.Apply();

        if (_MeshRenderer == null)
            InitializeComponents();
        _MeshRenderer.sharedMaterials[0].mainTexture = Texture;
    }

    protected void DrawCell(Texture2D destTexture, int x, int y, Sprite cellSprite)
    {
        try
        {
            Color[] cell = cellSprite.texture.GetPixels((int)cellSprite.textureRect.x, (int)cellSprite.textureRect.y, (int)cellSprite.textureRect.width, (int)cellSprite.textureRect.height);
            destTexture.SetPixels(x, y, (int)cellSprite.textureRect.width, (int)cellSprite.textureRect.height, cell);
        }
        catch
        {
            throw;
        }
    }
}
