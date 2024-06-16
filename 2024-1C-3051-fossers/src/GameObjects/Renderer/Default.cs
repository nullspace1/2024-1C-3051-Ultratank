
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
    private const int LIGHTSOURCE_LIMIT = 15;
    private float _ambientCoefficient;
    private float _diffuseCoefficient;
    private Texture2D _texture;
    private Color _color;

    public Default(float ambient, float diffuse, Texture2D texture) : base(ContentRepoManager.Instance().GetEffect("Default"))
    {
        _ambientCoefficient = ambient;
        _diffuseCoefficient = diffuse;
        _texture = texture;
    }

    public Default(float ambient, float diffuse, Color color) : base(ContentRepoManager.Instance().GetEffect("Default"))
    {
        _ambientCoefficient = ambient;
        _diffuseCoefficient = diffuse;
        _color = color;
    }

    public override void Draw(GameObject gameObject, Scene scene)
    {

        _effect.CurrentTechnique = _effect.Techniques["ActualDrawing"];
        LoadEffectParams(scene);

        foreach (var m in gameObject.Model.Meshes)
        {
            foreach (var p in m.MeshParts)
            {
                
                p.Effect = _effect;
            }
            Matrix world = gameObject.Transform.LocalToWorldMatrix(m.ParentBone.Transform);
                _effect.Parameters["World"].SetValue(world);
                _effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(world)));
            m.Draw();
        }
        

    }

    protected void LoadEffectParams(Scene scene)
    {
        if (scene.GetSceneProcessor<LightProcessor>() == null)
        {
            return;
        }

        List<LightSource> sources = scene.GetSceneProcessor<LightProcessor>().GetLightSources();

        Vector3[] colors = new Vector3[LIGHTSOURCE_LIMIT];
        Vector3[] positions = new Vector3[LIGHTSOURCE_LIMIT];

        for (int i = 0; i < Math.Min(sources.Count, LIGHTSOURCE_LIMIT); i++)
        {
            Vector3 sourceColor = sources[i].Color.ToVector3();
            Vector3 sourcePosition = sources[i].Position;
            colors[i] = sourceColor;
            positions[i] = sourcePosition;
        }

        _effect.Parameters["View"].SetValue(scene.GetCamera().View);
        _effect.Parameters["Projection"].SetValue(scene.GetCamera().Projection);
        _effect.Parameters["AmbientLight"].SetValue(scene.GetSceneProcessor<LightProcessor>().GetAmbientColor().ToVector3());
        _effect.Parameters["AmbientCoefficient"].SetValue(_ambientCoefficient);
        _effect.Parameters["LightSourcePositions"].SetValue(positions);
        _effect.Parameters["LightSourceColors"].SetValue(colors);
        _effect.Parameters["DiffuseCoefficient"].SetValue(_diffuseCoefficient);
        _effect.Parameters["AmbientLightDirection"].SetValue(scene.GetSceneProcessor<LightProcessor>().GetAmbientLightDirection());
        _effect.Parameters["ShadowMap"].SetValue(scene.GetSceneProcessor<LightProcessor>().GetRenderTarget());
        _effect.Parameters["LightViewProjection"].SetValue(scene.GetSceneProcessor<LightProcessor>().GetLightViewProjection());

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
    }
}
