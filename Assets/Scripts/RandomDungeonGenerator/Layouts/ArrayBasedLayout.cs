using System;
using System.Collections;

public abstract class ArrayBasedLayout : BaseLayout
{
    private int[][] _Mask;

    public int[][] Mask
    {
        get { return _Mask; }
    }

    protected abstract void SetMask(ref int[][] mask);

    public ArrayBasedLayout()
        : base()
    {
        SetMask(ref _Mask);
    }

    public override void MaskCells(dungeon dungeon)
    {
        if(Mask.Length == 0 || Mask[0].Length == 0)
            return;

        int h = Mask.Length;
        int w = Mask[0].Length;

        float r_x = h * 1.0f / (dungeon.n_rows + 1);
        float c_x = w * 1.0f / (dungeon.n_cols + 1);

        for (int r = 0; r <= dungeon.n_rows; r++)
            for (int c = 0; c <= dungeon.n_cols; c++)
                if (Mask[(int)(r * r_x)][(int)(c * c_x)] == 0)
                    dungeon.cell[r][c] = dungeon.BLOCKED;
    }
}
