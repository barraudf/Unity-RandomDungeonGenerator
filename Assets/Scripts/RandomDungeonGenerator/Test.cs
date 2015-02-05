using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        dungeon d = new dungeon();
        PrintMap(d);
	}

    protected void PrintMap(dungeon d)
    {
        string map = string.Empty;
        for (int r = 0; r <= d.n_rows; r++)
        {
            if (map.Length > 0)
                map += "\n";
            for (int c = 0; c <= d.n_cols; c++)
            {
                if ((d.cell[r][c] & dungeon.PERIMETER) != dungeon.NOTHING)
                    map += "0";
                else if ((d.cell[r][c] & dungeon.DOORSPACE) != dungeon.NOTHING)
                    map += "#";
                else if ((d.cell[r][c] & dungeon.ROOM) != dungeon.NOTHING)
                    map += ".";
                else if ((d.cell[r][c] & dungeon.CORRIDOR) != dungeon.NOTHING)
                    map += ".";
                else
                    map += " ";
            }
        }
        Debug.Log(map);
    }
}
