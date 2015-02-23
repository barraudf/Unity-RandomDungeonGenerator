using System;
using System.Collections;

public class BoxLayout : ArrayBasedLayout
{
    protected override void SetMask(ref int[][] mask)
    {
        mask = new int[][]
        {
            new int[] { 1, 1, 1 },
            new int[] { 1, 0, 1 },
            new int[] { 1, 1, 1 }
        };
    }
}
