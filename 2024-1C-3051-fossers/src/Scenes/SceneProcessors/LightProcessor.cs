using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Entities;


namespace WarSteel.Scenes.SceneProcessors;

class LightProcessor : ISceneProcessor
{

    private Color _ambientLight;

    private Vector3 _position;

    public Transform LightPosition;

    public Matrix Projection;

    private List<LightSource> _sources;
    private List<LightSource> _staticSources;

    public RenderTarget2D Render;

    private GraphicsDevice _device;

    public int ShadowMapSize = 2048;

    private Shadow _shadowShader;

    public LightProcessor(Color ambientLight, Vector3 ambientLightPosition)
    {
        _ambientLight = ambientLight;
        _position = ambientLightPosition;
        _sources = new List<LightSource>();
        _staticSources = new List<LightSource>();
    }

    public void Initialize(Scene scene)
    {
        Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 0.1f, 100000);
        LightPosition = new Transform()
        {
            Position = new Vector3(0, 10000, 0)
        };
        Render = new RenderTarget2D(scene.GraphicsDeviceManager.GraphicsDevice, ShadowMapSize, ShadowMapSize, false,
                SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);

        _shadowShader = new Shadow(ShadowMapSize);
        _device = scene.GraphicsDeviceManager.GraphicsDevice;
        LightPosition.LookAt(Vector3.Zero);
    }

    public void Draw(Scene scene)
    {
        _device.DepthStencilState = DepthStencilState.Default;
        _device.SetRenderTarget(Render);
        _device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

        foreach (var r in scene.GetEntities())
        {
            r.Draw(scene, _shadowShader);
        }

        _device.SetRenderTarget(null);
        scene.GraphicsDeviceManager.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
    

    }

    public void Update(Scene scene, GameTime time)
    {
        List<GameObject> entities = scene.GetEntities().FindAll(e => e.GetComponent<LightComponent>() != default);
        _sources = entities.ConvertAll(e => e.GetComponent<LightComponent>().GetLightSource());
    }

    public List<LightSource> GetLightSources()
    {
        List<LightSource> allSources = new();
        allSources.AddRange(_sources);
        allSources.AddRange(_staticSources);
        return allSources;
    }

    public void AddLightSource(LightSource s)
    {
        _staticSources.Add(s);
    }

    public Color GetAmbientColor()
    {
        return _ambientLight;
    }

    public Vector3 GetAmbientLightDirection()
    {
        return Vector3.Normalize(LightPosition.Position);
    }

    public RenderTarget2D GetRenderTarget()
    {
        return Render;
    }

    public Matrix GetLightViewProjection()
    {
        return LightPosition.View * Projection;
    }

    public Vector2 GetShadowMapSize()
    {
        return Vector2.One * ShadowMapSize;
    }

}