
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;
using WarSteel.Scenes.SceneProcessors;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace WarSteel.Common.Shaders;

public class Default : GameObjectRenderer
{

    private Texture2D _texture;
    private Color _color;

    public Default(Texture2D texture) : base(ContentRepoManager.Instance().GetEffect("Default"))
    {
        _texture = texture;
    }

    public Default( Color color) : base(ContentRepoManager.Instance().GetEffect("Default"))
    {
        _color = color;
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