using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Managers;

namespace WarSteel.UIKit;

public class Image : UIRenderer
{
    private Texture2D _texture;
    private Color _color;
    private Vector2 _origin;
    private float _scale;

    public Image(string texture, float scale = 1.0f)
    {
        _texture = ContentRepoManager.Instance().GetTexture(texture);
        _color = Color.White;
        _origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
        _scale = scale;
    }

    public Image(string texture, Color color, float scale = 1.0f)
    {
        _texture = ContentRepoManager.Instance().GetTexture(texture);
        _color = color;
        _origin = new Vector2(_texture.Width / 2, _texture.Height / 2);
        _scale = scale;
    }

    public void Draw(SpriteBatch spriteBatch, UI ui)
    {
        float scaleX = ui.Width / _texture.Width;
        float scaleY = ui.Height / _texture.Height;

        Vector2 position = new(ui.Position.X, ui.Position.Y);

        spriteBatch.Draw(_texture, position, null, _color, 0f, _origin, new Vector2(scaleX, scaleY), SpriteEffects.None, 0f);
    }
}