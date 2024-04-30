using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WarSteel.Common;
using WarSteel.Entities;

namespace WarSteel.Scenes;

public class Scene
{
    private Dictionary<string, Entity> _entities = new Dictionary<string, Entity>();
    protected GraphicsDeviceManager Graphics;

    protected Camera Camera;

    public Scene(GraphicsDeviceManager Graphics)
    {
        this.Graphics = Graphics;
    }

    public void SetCamera(Camera camera)
    {
        _entities.Add(camera.Id, camera);
        this.Camera = camera;
    }

    public Camera GetCamera()
    {
        return Camera;
    }

    public void AddEntity(Entity entity)
    {
        _entities.Add(entity.Id, entity);
    }

    public Entity GetEntityByName(string name)
    {
        foreach (var entity in _entities.Values)
        {
            if (entity.Name == name)
                return entity;
        }
        return null;
    }

    public virtual void Initialize()
    {
        foreach (var entity in _entities.Values)
        {
            entity.Initialize();
        }
    }

    public virtual void LoadContent()
    {
        foreach (var entity in _entities.Values)
        {
            entity.LoadContent();
        }
    }

    public virtual void Draw()
    {
        foreach (var entity in _entities.Values)
        {
            entity.Draw(this);
        }
    }
    public virtual void Update(GameTime gameTime)
    {
        foreach (var entity in _entities.Values)
        {
            entity.Update(gameTime, this);
        }

    }

    public virtual void Unload()
    {

    }

}