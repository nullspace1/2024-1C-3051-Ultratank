using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.Samples.Geometries;
using WarSteel.Common;
using WarSteel.Entities;
using WarSteel.Managers;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;


namespace WarSteel.Scenes.SceneProcessors;

class LightProcessor : ISceneProcessor
{

    private GraphicsDevice _device;

    private List<Light> _lights = new();

    public Color Color;

    public Matrix LightViewProjection;

    private RenderTarget2D _dynamicLightsRenderTarget;

    public RenderTarget2D ShadowMapRenderTarget;

    public int ShadowMapSize = 2048;

    private float _projectionTextureRatio = 10;

    public float FarPlaneDistance = 60000;
    public float NearPlaneDistance = 1;

    private float lightDistance = 30000;

    public Vector3 LightDirection;

    private FullScreenQuad screenQuad;

    private Effect bloomEffect;



    public LightProcessor(GraphicsDevice device, Color color, Vector3 direction)
    {
        _device = device;
        LightViewProjection = Matrix.CreateLookAt(Vector3.Normalize(direction) * lightDistance, Vector3.Zero, Vector3.UnitY) * Matrix.CreateOrthographic(ShadowMapSize * _projectionTextureRatio, ShadowMapSize * _projectionTextureRatio, NearPlaneDistance, FarPlaneDistance);
        Color = color;
        LightDirection = direction;
    }

    public void Initialize(Scene scene)
    {

        _dynamicLightsRenderTarget = new RenderTarget2D(
             _device,
             _device.Viewport.Width,
             _device.Viewport.Height,
             false, // Mipmap
             SurfaceFormat.Color, // Surface format for color rendering
             DepthFormat.Depth24, // Depth buffer format
             0, // Mip levels
             RenderTargetUsage.PlatformContents
         );

        ShadowMapRenderTarget = new RenderTarget2D(scene.GraphicsDeviceManager.GraphicsDevice, ShadowMapSize, ShadowMapSize, false,
                SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);

        screenQuad = new(_device);

        bloomEffect = ContentRepoManager.Instance().GetEffect("Bloom");

    }

    public void Update(Scene scene, GameTime time)
    {

        _device.SetRenderTarget(ShadowMapRenderTarget);
        _device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, new Color(0, 0, 0, 1), 1, 0);

        foreach (var o in scene.GetGameObjects())
        {
            o.Renderer.DrawShadowMap(o, LightViewProjection, FarPlaneDistance);
        }

        _device.SetRenderTarget(ContentRepoManager.Instance().GlobalRenderTarget);
    }

    public void Draw(Scene scene)
    {
        _device.SetRenderTarget(_dynamicLightsRenderTarget);
        _device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0);

        _device.BlendState = BlendState.Opaque;

        var visibleObjects = scene.GetVisibleObjects(scene.Camera.View * scene.Camera.Projection);
        List<Light> visibleLights = GetVisibleLights(scene);

        foreach (var light in visibleLights)
        {
            _device.BlendState = BlendState.Opaque;

            foreach (var obj in visibleObjects)
            {
                obj.Renderer.DrawLight(obj, scene, light, true);
            }

            _device.BlendState = BlendState.Additive;

            foreach (var obj in visibleObjects)
            {
                obj.Renderer.DrawLight(obj, scene, light, false);
            }

            _device.BlendState = BlendState.AlphaBlend;
            bloomEffect.Parameters["Screen"].SetValue(_dynamicLightsRenderTarget);

            var pos = _device.Viewport.Project(light.Transform.AbsolutePosition, scene.Camera.Projection, scene.Camera.View, Matrix.Identity);
            var lightScreenPos = new Vector2(pos.X / _device.Viewport.Width, pos.Y / _device.Viewport.Height);
            var distance = 1 / (1 - pos.Z) * 0.0001f;

            bloomEffect.Parameters["LightScreenPosition"].SetValue(lightScreenPos);
            bloomEffect.Parameters["LightColor"].SetValue(light.Color.ToVector3());
            bloomEffect.Parameters["Distance"].SetValue(distance);
            screenQuad.Draw(bloomEffect);

        }

        _device.SetRenderTarget(ContentRepoManager.Instance().GlobalRenderTarget);
        _device.BlendState = BlendState.AlphaBlend;

        scene.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        scene.SpriteBatch.Draw(_dynamicLightsRenderTarget, Vector2.Zero, Color.White * 0.2f);
        scene.SpriteBatch.End();

        scene.ResetGraphicsDevice();
    }

    private List<Light> GetVisibleLights(Scene scene)
    {
        var visibleLights = new List<Light>();

        var frustum = new BoundingFrustum(scene.Camera.View * scene.Camera.Projection);
        foreach (var light in _lights)
        {
            if (frustum.Contains(new BoundingSphere(light.Transform.Position, 100)) != ContainmentType.Disjoint)
            {
                visibleLights.Add(light);
            }
        }

        return visibleLights;
    }

    public void AddLight(Light light)
    {
        _lights.Add(light);
    }

    public void RemoveLight(Light light)
    {
        _lights.Remove(light);
    }

}

public class Light
{

    public Transform Transform;
    public Color Color;
    public Light(Transform transform, Color color)
    {
        Transform = transform;
        Color = color;
    }

    public Light(Vector3 position, Color color)
    {
        Transform = new Transform
        {
            Position = position
        };
        Color = color;
    }

}
