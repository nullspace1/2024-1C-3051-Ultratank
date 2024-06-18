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

    private Dictionary<Light, Texture2D> _textures = new();

    private RenderTarget2D auxRenderTarget;

    public LightProcessor(GraphicsDevice device)
    {
        _device = device;
    }


    public void Initialize(Scene scene)
    {
        auxRenderTarget = new RenderTarget2D(
             _device,
             _device.Viewport.Width,
             _device.Viewport.Height,
             false, // Mipmap
             SurfaceFormat.Color, // Surface format for color rendering
             DepthFormat.Depth16, // Depth buffer format
             0, // Mip levels
             RenderTargetUsage.PlatformContents
         );
    }

    public void Update(Scene scene, GameTime time)
    {
    }

    public void Draw(Scene scene)
    {

        _textures.Clear();
        _lights.ForEach(l => _textures.Add(l, l.GetDepthMap(_device, scene)));

        foreach (var light in _lights)
        {
            _textures[light] = light.GetDepthMap(_device, scene);
        }

        _device.SetRenderTarget(auxRenderTarget);

        _device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, new Color(0, 0, 0, 1), 1, 0);

        _device.BlendState = BlendState.Opaque;

        foreach (var o in scene.GetGameObjects())
        {
            foreach (var l in _lights)
            {
                o.Draw(scene, new LightRender(l, _textures[l], true));
            }
        }

        _device.BlendState = BlendState.Additive;

        foreach (var o in scene.GetGameObjects())
        {
            foreach (var l in _lights)
            {


                o.Draw(scene, new LightRender(l, _textures[l], false));

            }
        }

        _device.SetRenderTarget(null);

        _device.BlendState = BlendState.AlphaBlend;

        scene.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        scene.SpriteBatch.Draw(auxRenderTarget, Vector2.Zero, Color.White * 0.3f); // Adjust transparency (0.8f for 80% opacity)
        scene.SpriteBatch.End();

        // Reset graphics device state if needed
        scene.ResetGraphicsDevice();

    }

    public void AddLight(Light light)
    {
        light.renderTarget = new RenderTarget2D(_device, 2048, 2048, false,
                SurfaceFormat.Single, DepthFormat.Depth16, 0, RenderTargetUsage.PlatformContents);
        _lights.Add(light);
    }

    public void RemoveLight(Light light)
    {
        light.renderTarget.Dispose();
        _lights.Remove(light);
    }



}

public class Light
{

    public Transform Transform;
    public Color Color;
    public Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi - 0.05f, 1, 0.1f, 10000);
    public RenderTarget2D renderTarget;

    public Light(Transform transform, Color color)
    {
        Transform = transform;
        Color = color;
        renderTarget = null;

    }

    public Light(Vector3 position, Color color)
    {
        Transform = new Transform
        {
            Position = position
        };
        Color = color;
        renderTarget = null;
    }

    public RenderTarget2D GetDepthMap(GraphicsDevice device, Scene scene)
    {


        device.SetRenderTarget(renderTarget);
        device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

        Matrix downViewProjection = GetDownViewProjection();

        Shadow renderer = new(downViewProjection, new Vector3(1, 0, 0));

        foreach (var e in scene.GetGameObjects())
        {
            e.Draw(scene, renderer);
        }

        return renderTarget;

    }

    public Matrix GetDownViewProjection()
    {
        return Matrix.CreateLookAt(Transform.AbsolutePosition, Transform.AbsolutePosition - Vector3.Up, Vector3.UnitX) * Projection;
    }

}