using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.Samples.Geometries;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Entities;
using WarSteel.Managers;


namespace WarSteel.Scenes;

public abstract class Scene
{
    private List<GameObject> _gameObjects = new();
    private List<UI> _UIs = new();
    private Dictionary<Type, ISceneProcessor> _sceneProcessors = new();
    public GraphicsDeviceManager GraphicsDeviceManager;
    public SpriteBatch SpriteBatch;
    public Camera Camera;
    private bool _isPaused = false;
    public bool IsPaused { get => _isPaused; }
    public SkyBox SkyBox;


    public void Pause()
    {
        _isPaused = true;
    }

    public void Resume()
    {
        _isPaused = false;
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
    }

    public Scene(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        GraphicsDeviceManager = graphics;
        SpriteBatch = spriteBatch;
        Camera = new Camera(Vector3.Zero, GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, MathHelper.PiOver2, 0.1f, 300000f);
    }

    public void SetCamera(Camera camera)
    {
        Camera = camera;
    }

    public Camera GetCamera()
    {
        return Camera;
    }

    public void AddSceneProcessor(ISceneProcessor p)
    {
        p.Initialize(this);
        _sceneProcessors.Add(p.GetType(), p);
    }

    public void ClearUI()
    {
        _UIs.Clear();
    }

    public void AddUI(UI ui)
    {
        _UIs.Add(ui);
    }

    public void AddUI(List<UI> UIs)
    {
        _UIs.AddRange(UIs);
    }

    public bool HasUI(UI ui)
    {
        return _UIs.Contains(ui);
    }

    public void RemoveUI(UI ui)
    {
        if (_UIs.Contains(ui))
            _UIs.Remove(ui);
    }

    public void RemoveUI(List<UI> UIs)
    {
        UIs.ForEach(ui => _UIs.Remove(ui));
    }

    public void AddGameObject(GameObject entity)
    {
        entity.Initialize(this);
        _gameObjects.Add(entity);
    }

    public void RemoveGameObjectsByTag(string tag)
    {
        foreach (var e in _gameObjects)
        {
            if (e.HasTag(tag))
                e.Destroy();
        }
    }

    public T GetSceneProcessor<T>() where T : class, ISceneProcessor
    {
        return _sceneProcessors.TryGetValue(typeof(T), out var processor) ? processor as T : default;
    }

    public List<GameObject> GetGameObjects()
    {
        // creating this tmp list to prevent errors while modifying the collection in the loop.
        List<GameObject> gameObjects = new(_gameObjects);
        List<GameObject> list = new(gameObjects);
        foreach (var e in gameObjects)
        {
            list.Add(e);
        }
        return list;
    }

    public List<GameObject> GetEntitiesByTag(string tag)
    {
        List<GameObject> list = new();
        foreach (var e in _gameObjects)
        {
            if (e.HasTag(tag))
                list.Add(e);
        }
        return list;
    }

    public T GetEntityByTag<T>(string tag) where T : GameObject
    {
        foreach (var e in _gameObjects)
        {
            if (e.HasTag(tag))
                return (T)e;
        }

        return null;
    }

    public abstract void Initialize();

    public void Draw()
    {

        ResetGraphicsDevice();

        SkyBox.DrawSkyBox(this);

        List<GameObject> gb = GetVisibleObjects(Camera.View * Camera.Projection).OrderBy(g => g.Id).ToList();
        foreach (var entity in gb)
        {
            entity.Renderer.DrawDefault(entity, this);
        }

        foreach (var sceneProcessor in _sceneProcessors.Values)
        {
            sceneProcessor.Draw(this);
        }


        SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, null, null, null);
        List<UI> uis = new(_UIs);

        foreach (var ui in uis)
        {
            ui?.Draw(SpriteBatch);
        }

        SpriteBatch.End();

        ResetGraphicsDevice();

    }

    public List<GameObject> GetVisibleObjects(Matrix viewProjection)
    {
        List<GameObject> visibleObjects = new();
        foreach (var go in _gameObjects)
        {
            BoundingFrustum boundingFrustum = new(viewProjection);
            BoundingSphere boundingSphere = go.GetBoundingSphere();
            if (boundingFrustum.Contains(boundingSphere) != ContainmentType.Disjoint || go.AlwaysRender)
            {
                visibleObjects.Add(go);
            }
        }
        return visibleObjects;
    }

    public virtual void Update(GameTime gameTime)
    {
        foreach (var ui in new List<UI>(_UIs))
        {
            ui?.Update(this, gameTime);
        }

        if (_isPaused)
            return;

        Camera?.Update(this, gameTime);

        foreach (var entity in new List<GameObject>(_gameObjects))
        {
            entity.Update(this, gameTime);
        }

        foreach (var processor in _sceneProcessors.Values)
        {
            processor.Update(this, gameTime);
        }


        CheckDeletedEntities();

    }
    private void CheckDeletedEntities()
    {

        foreach (var entity in new List<GameObject>(_gameObjects))
        {
            if (entity.IsDestroyed())
            {
                entity.OnDestroy(this);
                _gameObjects.Remove(entity);
            }
        }

        foreach (var ui in new List<UI>(_UIs))
        {
            if (ui == null || ui.IsDestroyed())
            {
                _UIs.Remove(ui);
            }
        }
    }

    public virtual void Unload()
    {
        _gameObjects.Clear();
        _UIs.Clear();
        _sceneProcessors.Clear();
        Camera = null;
        _isPaused = false;
        AudioManager.Instance.StopAllSounds();
    }

    public void ResetGraphicsDevice()
    {
        GraphicsDeviceManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDeviceManager.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        GraphicsDeviceManager.GraphicsDevice.BlendState = BlendState.Opaque;
        GraphicsDeviceManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
    }

}
