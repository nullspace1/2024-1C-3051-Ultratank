using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Entities;


namespace WarSteel.Scenes;

public abstract class Scene
{
    private Dictionary<string, GameObject> _gameObjects = new();
    private List<UI> _UIs = new();
    private Dictionary<Type, ISceneProcessor> _sceneProcessors = new();
    public GraphicsDeviceManager GraphicsDeviceManager;
    public SpriteBatch SpriteBatch;
    public Camera Camera;

    public Scene(GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
    {
        GraphicsDeviceManager = graphics;
        SpriteBatch = spriteBatch;
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

    public void AddUI(UI ui)
    {
        _UIs.Add(ui);
    }

    public void AddUI(List<UI> UIs){
        _UIs.AddRange(UIs);
    }

    public void RemoveUI(UI ui){
        _UIs.Remove(ui);
    }

    public void RemoveUI(List<UI> UIs){
        UIs.ForEach(ui => _UIs.Remove(ui));
    }

    public void AddGameObject(GameObject entity)
    {
        entity.Initialize(this);
        _gameObjects.Add(entity.Id, entity);
    }

    public T GetSceneProcessor<T>() where T : class, ISceneProcessor
    {
        return _sceneProcessors.TryGetValue(typeof(T), out var processor) ? processor as T : default;
    }

    public List<GameObject> GetEntities()
    {
        List<GameObject> list = new();
        foreach (var e in _gameObjects.Values)
        {
            list.Add(e);
        }
        return list;
    }

    public abstract void Initialize();

    public void Draw()
    {

        foreach (var ui in _UIs)
        {
            ui.Draw(SpriteBatch);
        }

        ResetGraphicsDevice();

        foreach (var entity in _gameObjects.Values)
        {
            entity.Draw(this);
        }

        foreach (var sceneProcessor in _sceneProcessors.Values)
        {
            sceneProcessor.Draw(this);
        }

    }

    public void Update(GameTime gameTime)
    {

        Camera.Update(this,gameTime);

        foreach (var entity in new List<GameObject>(_gameObjects.Values))
        {
            entity.Update(this, gameTime);
        }

        foreach (var ui in _UIs)
        {
            ui.Update(this, gameTime);
        }

        foreach (var processor in _sceneProcessors.Values)
        {
            processor.Update(this, gameTime);
        }


        CheckDeletedEntities();

    }
    private void CheckDeletedEntities()
    {
        var copyEntities = new Dictionary<string, GameObject>(_gameObjects);
        foreach (var entity in copyEntities.Values)
        {
            if (entity.IsDestroyed())
            {
                entity.OnDestroy(this);
                _gameObjects.Remove(entity.Id);
            }
        }

        var copyUI = new List<UI>(_UIs);
        foreach (var ui in copyUI)
        {
            if (ui.IsDestroyed())
            {
                _UIs.Remove(ui);
            }
        }
    }

    public void Unload()
    {
        _gameObjects.Clear();
        _UIs.Clear();
        _sceneProcessors.Clear();
        Camera = null;
    }

    public void ResetGraphicsDevice()
    {
        GraphicsDeviceManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDeviceManager.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        GraphicsDeviceManager.GraphicsDevice.BlendState = BlendState.Opaque;
        GraphicsDeviceManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
        GraphicsDeviceManager.GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
    }

}
