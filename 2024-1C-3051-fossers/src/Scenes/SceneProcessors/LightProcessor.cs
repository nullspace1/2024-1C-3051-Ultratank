using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Entities;
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

    private float _projectionTextureRatio = 6;

    public float FarPlaneDistance = 20000;
    public float NearPlaneDistance = 1;

    private float lightDistance = 30000;

    public Vector3 LightDirection;



    public LightProcessor(GraphicsDevice device, Color color, Vector3 direction)
    {
        _device = device;
        LightViewProjection = Matrix.CreateLookAt(Vector3.Normalize(direction) * lightDistance, Vector3.Zero, Vector3.UnitX) * Matrix.CreateOrthographic(ShadowMapSize * _projectionTextureRatio, ShadowMapSize * _projectionTextureRatio , NearPlaneDistance, 100000);
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

    }

    public void Update(Scene scene, GameTime time)
    {

        _device.SetRenderTarget(ShadowMapRenderTarget);
        _device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, new Color(0, 0, 0, 1), 1, 0);

        foreach (var o in scene.GetGameObjects())
        {
            o.Draw(scene, new Shadow(LightViewProjection, NearPlaneDistance, FarPlaneDistance));
        }

        _device.SetRenderTarget(null);
        _device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, new Color(0, 0, 0, 1), 1, 0);
    }

    public void Draw(Scene scene)
    {

         _device.SetRenderTarget(_dynamicLightsRenderTarget);
        _device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1, 0);

        _device.BlendState = BlendState.Opaque;

        foreach (var o in scene.GetGameObjects())
        {
            foreach (var l in _lights)
            {
                o.Draw(scene, new LightRender(l, true));
            }
        }

        _device.BlendState = BlendState.Additive;

        foreach (var o in scene.GetGameObjects())
        {
            foreach (var l in _lights)
            {
                o.Draw(scene, new LightRender(l, false));
            }
        }

        _device.SetRenderTarget(null);
        _device.BlendState = BlendState.AlphaBlend;

        scene.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        scene.SpriteBatch.Draw(_dynamicLightsRenderTarget, Vector2.Zero, Color.White * 0.2f); // Adjust transparency (0.6f for 60% opacity)
        scene.SpriteBatch.End(); 

        scene.ResetGraphicsDevice();

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
