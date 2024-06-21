using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;
using WarSteel.Scenes.SceneProcessors;
using Vector3 = Microsoft.Xna.Framework.Vector3;
namespace WarSteel.Common.Shaders;

public class Renderer
{

    private readonly Texture2D _texture;
    private Color _color;
    private List<Vector3> _impactPoints = new();

    private readonly Effect _effect;
    private readonly Effect _shadowEffect;

    public Renderer(Texture2D texture)
    {
        _texture = texture;
        _effect = ContentRepoManager.Instance().GetEffect("Shader");
        _shadowEffect = ContentRepoManager.Instance().GetEffect("Shadow");
    }

    public Renderer(Color color)
    {
        _color = color;
        _effect = ContentRepoManager.Instance().GetEffect("Shader");
        _shadowEffect = ContentRepoManager.Instance().GetEffect("Shadow");
    }

    public void AddImpact(Vector3 position)
    {
        if (_impactPoints.Count < 5)
        {
            _impactPoints.Add(position);
        }
    }

    public void DrawDefault(GameObject gameObject, Scene scene)
    {

        _effect.CurrentTechnique = _effect.Techniques["Default"];

        LightProcessor lightProcessor = scene.GetSceneProcessor<LightProcessor>();

        foreach (var m in gameObject.Model.GetMeshes())
        {
            foreach (var p in m.MeshParts)
                p.Effect = _effect;


            Matrix world = gameObject.Model.GetPartTransform(m, gameObject.Transform);

            _effect.Parameters["World"].SetValue(world);
            _effect.Parameters["CameraViewProjection"].SetValue(scene.Camera.View * scene.Camera.Projection);
            _effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(world)));
            _effect.Parameters["LightViewProjection"].SetValue(lightProcessor.LightViewProjection);
            _effect.Parameters["LightDirection"].SetValue(lightProcessor.LightDirection);
            _effect.Parameters["ShadowDepth"].SetValue(lightProcessor.ShadowMapRenderTarget);
            _effect.Parameters["TextureSize"].SetValue(lightProcessor.ShadowMapSize);
            _effect.Parameters["DiffuseCoefficient"].SetValue(1f);
            _effect.Parameters["FarPlaneDistance"].SetValue(lightProcessor.FarPlaneDistance);
            _effect.Parameters["ImpactCount"].SetValue(_impactPoints.Count);
            _effect.Parameters["ImpactPoints"].SetValue(_impactPoints.Select(i => gameObject.Transform.LocalToWorldPosition(i)).ToArray());

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

    public void DrawLight(GameObject gameObject, Scene scene, Light light, bool depth)
    {

        _effect.CurrentTechnique = _effect.Techniques["LightPass"];

        LightProcessor lightProcessor = scene.GetSceneProcessor<LightProcessor>();

        if (gameObject.HasTag("skybox")) return;
        foreach (var m in gameObject.Model.GetMeshes())
        {
            foreach (var p in m.MeshParts)
                p.Effect = _effect;


            Matrix world = gameObject.Model.GetPartTransform(m, gameObject.Transform);
            
            _effect.Parameters["World"].SetValue(world);
            _effect.Parameters["CameraViewProjection"].SetValue(scene.Camera.View * scene.Camera.Projection);
            _effect.Parameters["LightViewProjection"].SetValue(lightProcessor.LightViewProjection);
            _effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(world)));
            _effect.Parameters["LightPosition"].SetValue(light.Transform.AbsolutePosition);
            _effect.Parameters["LightColor"].SetValue(light.Color.ToVector3());
            _effect.Parameters["Depth"].SetValue(depth);

            m.Draw();
        }
    }

    public void DrawShadowMap(GameObject gameObject, Matrix lightViewProjection, float farPlaneDistance)
    {

        foreach (var modelMesh in gameObject.Model.GetMeshes())
        {
            foreach (var p in modelMesh.MeshParts)
                p.Effect = _shadowEffect;

            Matrix world = gameObject.Model.GetPartTransform(modelMesh, gameObject.Transform);

            _shadowEffect.Parameters["WorldViewProjection"].SetValue(world * lightViewProjection);
            _shadowEffect.Parameters["FarPlaneDistance"].SetValue(farPlaneDistance);

            modelMesh.Draw();
        }

    }



}