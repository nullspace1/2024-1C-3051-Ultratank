using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Managers;

namespace WarSteel.UIKit;

public class TextUI : UIRenderer
{
    public string Text;
    private Color _color;
    private SpriteFont _font;
    private float _fontSize;

    public TextUI(string text, string font, float fontSize, Color color)
    {
        Text = text;
        _color = color;
        _font = ContentRepoManager.Instance().GetSpriteFont(font);
        _fontSize = fontSize;
    }

    public void Draw(SpriteBatch spriteBatch, UI ui)
    {
        Vector2 center = ui.Position;
        Vector2 textSize = _font.MeasureString(Text) * _fontSize;
        Vector2 textPosition = new(center.X - textSize.X / 2, center.Y - textSize.Y / 2);
        spriteBatch.DrawString(_font, Text, textPosition, _color, 0f, Vector2.Zero, _fontSize, SpriteEffects.None, 0f);
    }
}