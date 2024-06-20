using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;
namespace WarSteel.Common.Shaders;

public class Dent : GameObjectRenderer
{
    private Texture2D _texture;
    private Color _color;
    private List<Vector3> _impacts;

    public Dent(Texture2D texture, List<Vector3> impacts) : base(ContentRepoManager.Instance().GetEffect("Dent"))
    {
        _texture = texture;
        _impacts = impacts;
    }

    public Dent(Color color, List<Vector3> impacts) : base(ContentRepoManager.Instance().GetEffect("Dent"))
    {
        _color = color;
        _impacts = impacts;
    }

    public override void Draw(GameObject gameObject, Scene scene)
    {
        foreach (var m in gameObject.Model.GetMeshes())
        {
            foreach (var p in m.MeshParts)
            {

                p.Effect = _effect;
            }

            Matrix world = gameObject.Model.GetPartTransform(m, gameObject.Transform);

            _effect.Parameters["WorldViewProjection"].SetValue(world * scene.Camera.View * scene.Camera.Projection);
            _effect.Parameters["ImpactRadius"].SetValue(10);
            _effect.Parameters["Impacts"].SetValue(_impacts.ToArray());

            if (_texture == null)
            {
                _effect.Parameters["Color"].SetValue(_color.ToVector3());
                _effect.Parameters["HasTexture"].SetValue(false);
            }
            else
            {
                _effect.Parameters["Texture"].SetValue(_texture);
                _effect.Parameters["HasTexture"].SetValue(true);
            }
            m.Draw();
        }
    }
}