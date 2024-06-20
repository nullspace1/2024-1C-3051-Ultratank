using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;
using WarSteel.Scenes.SceneProcessors;

public class LightRender : GameObjectRenderer
{

    private Light _light;

    private bool _depth;

    public LightRender(Light light, bool depth) : base(ContentRepoManager.Instance().GetEffect("Light"))
    {
        _light = light;
        _depth = depth;
    }

    public override void Draw(GameObject gameObject, Scene scene)
    {
        if (gameObject.HasTag("skybox")) return;
        foreach (var m in gameObject.Model.GetMeshes())
        {
            foreach (var p in m.MeshParts)
                p.Effect = _effect;


            Matrix world = gameObject.Model.GetPartTransform(m, gameObject.Transform);
            _effect.Parameters["World"].SetValue(world);
            _effect.Parameters["WorldViewProjection"].SetValue(world * scene.Camera.View * scene.Camera.Projection);
            _effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(world)));
            _effect.Parameters["LightPosition"].SetValue(_light.Transform.AbsolutePosition);
            _effect.Parameters["LightColor"].SetValue(_light.Color.ToVector3());
            _effect.Parameters["Depth"].SetValue(_depth);

            m.Draw();
        }
    }

}