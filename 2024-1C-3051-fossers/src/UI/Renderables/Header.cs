using Microsoft.Xna.Framework;

namespace WarSteel.UIKit;

public class Header : TextUI
{
    public Header(string text) : base(text,"tenada/Tenada", 1f, Color.White)
    {
    }

    public Header(string text, Color color) : base(text,"tenada/Tenada", 1f, color)
    {
    }

}