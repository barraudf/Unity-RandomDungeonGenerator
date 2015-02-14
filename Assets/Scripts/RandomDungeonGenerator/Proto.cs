using System;

public class Proto
{
    private int _I;
    private int _J;
    private int _Height;
    private int _Width;

    private bool _IIsSet = false;
    private bool _JIsSet = false;
    private bool _HeightIsSet = false;
    private bool _WidthIsSet = false;

    public int I
    {
        get { return _I; }
        set { _I = value; _IIsSet = true; }
    }

    public int J
    {
        get { return _J; }
        set { _J = value; _JIsSet = true; }
    }

    public int Height
    {
        get { return _Height; }
        set { _Height = value; _HeightIsSet = true; }
    }

    public int Width
    {
        get { return _Width; }
        set { _Width = value; _WidthIsSet = true; }
    }

    public bool IIsSet { get { return _IIsSet; } }
    public bool JIsSet { get { return _JIsSet; } }
    public bool HeightIsSet { get { return _HeightIsSet; } }
    public bool WidthIsSet { get { return _WidthIsSet; } }
}
