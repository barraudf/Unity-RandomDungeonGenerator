using System;
using System.Collections;

public class RoundLayout : BaseLayout
{
    public override void MaskCells(dungeon dungeon)
    {
        int center_r = dungeon.n_rows / 2;
        int center_c = dungeon.n_cols / 2;

        for (int r = 0; r <= dungeon.n_rows; r++)
        {
            for (int c = 0; c <= dungeon.n_cols; c++)
            {
                double d = Math.Sqrt(Math.Pow(r - center_r, 2) + Math.Pow(c - center_c, 2));
                if (d > center_c)
                    dungeon.cell[r][c] = dungeon.BLOCKED;
            }
        }
    }
}
