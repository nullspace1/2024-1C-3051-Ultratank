using Microsoft.Xna.Framework;
using WarSteel.Managers;
using WarSteel.UIKit;

public class TertiaryBtn : Button
{
    public TertiaryBtn(string text) : base(ContentRepoManager.Instance().GetTexture("UI/tertiary-btn"), 0.5f, text, Color.LightBlue) { }
}