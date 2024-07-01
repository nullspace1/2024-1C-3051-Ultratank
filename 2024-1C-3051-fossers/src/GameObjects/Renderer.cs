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

namespace WarSteel.Common.Shaders
{
    public class Renderer
    {
        private readonly Texture2D _texture;
        private readonly Color _color;
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

        public void DrawDefault(GameObject gameObject, Scene scene)
        {
            _effect.CurrentTechnique = _effect.Techniques["Default"];
            var lightProcessor = scene.GetSceneProcessor<LightProcessor>();

            Draw(gameObject, scene, _effect, lightProcessor, applyTextureParameters: true);
        }

        public void DrawLight(GameObject gameObject, Scene scene, Light light, bool depth)
        {
            if (gameObject.HasTag("skybox")) return;

            _effect.CurrentTechnique = _effect.Techniques["LightPass"];
            var lightProcessor = scene.GetSceneProcessor<LightProcessor>();

            Draw(gameObject, scene, _effect, lightProcessor, light, depth);
        }

        public void DrawShadowMap(GameObject gameObject, Matrix lightViewProjection, float farPlaneDistance)
        {
            foreach (var m in gameObject.Model.GetMeshes())
            {
                foreach (var p in m.MeshParts)
                    p.Effect = _shadowEffect;

                Matrix world = gameObject.Model.GetPartWorld(m);
                _shadowEffect.Parameters["WorldViewProjection"].SetValue(gameObject.Model.GetPartTransform(m) * world * lightViewProjection);
                _shadowEffect.Parameters["FarPlaneDistance"].SetValue(farPlaneDistance);

                m.Draw();
            }
        }

        private void Draw(GameObject gameObject, Scene scene, Effect effect, LightProcessor lightProcessor, Light light = null, bool depth = false, bool applyTextureParameters = false)
        {
            foreach (var m in gameObject.Model.GetMeshes())
            {
                foreach (var p in m.MeshParts)
                    p.Effect = effect;

                Matrix world = gameObject.Model.GetPartWorld(m);

                effect.Parameters["World"].SetValue(world);
                effect.Parameters["Bone"].SetValue(gameObject.Model.GetPartTransform(m));
                effect.Parameters["WorldCameraViewProjection"].SetValue(world * scene.Camera.View * scene.Camera.Projection);
                effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(gameObject.Model.GetPartTransform(m) * world)));
                effect.Parameters["WorldLightViewProjection"].SetValue(world * lightProcessor.LightViewProjection);
                effect.Parameters["ImpactCount"].SetValue(gameObject.Model.ImpactCount);
                effect.Parameters["ImpactPoints"].SetValue(gameObject.Model.GetImpactPositions(m));
                effect.Parameters["ImpactVelocities"].SetValue(gameObject.Model.GetImpactVelocities(m));

                if (applyTextureParameters)
                {
                    if (_texture == null)
                    {
                        effect.Parameters["Color"].SetValue(_color.ToVector3());
                        effect.Parameters["HasTexture"].SetValue(false);
                    }
                    else
                    {
                        effect.Parameters["Texture"].SetValue(_texture);
                        effect.Parameters["HasTexture"].SetValue(true);
                    }

                    effect.Parameters["ShadowDepth"].SetValue(lightProcessor.ShadowMapRenderTarget);
                    effect.Parameters["TextureSize"].SetValue(lightProcessor.ShadowMapSize);
                    effect.Parameters["DiffuseCoefficient"].SetValue(0.5f);
                    effect.Parameters["FarPlaneDistance"].SetValue(lightProcessor.FarPlaneDistance);
                    effect.Parameters["LightDirection"].SetValue(lightProcessor.LightDirection);
                }

                if (light != null)
                {
                    effect.Parameters["LightPosition"].SetValue(light.Transform.AbsolutePosition);
                    effect.Parameters["LightColor"].SetValue(light.Color.ToVector3());
                    effect.Parameters["Depth"].SetValue(depth);
                }

                m.Draw();
            }
        }
    }
}
