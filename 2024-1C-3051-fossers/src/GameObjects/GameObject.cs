using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Scenes;

namespace WarSteel.Entities;

public class GameObject
{
    private Dictionary<Type, IComponent> _components = new();
    public Transform Transform { get; }
    public Model Model { get; set; }
    protected GameObjectRenderer _defaultRenderer;

    private string[] _tags { get; }
    private bool _toDestroy = false;
    public string Id { get; }

    public GameObject(string[] tags, Transform transform, Model model, GameObjectRenderer renderer) : base()
    {
        Id = Guid.NewGuid().ToString();
        _tags = tags;
        Transform = transform;
        Model = model;
        _defaultRenderer = renderer;
    }

    public GameObject(string[] tags, Transform transform, Model model) : base()
    {
        Id = Guid.NewGuid().ToString();
        _tags = tags;
        Transform = transform;
        Model = model;
    }

    public GameObject(string[] tags, Model model) : base()
    {
        Id = Guid.NewGuid().ToString();
        _tags = tags;
        Transform = new();
        Model = model;
    }

    public void AddComponent(IComponent c)
    {
        _components.Add(c.GetType(), c);
    }

    public T GetComponent<T>() where T : class, IComponent
    {
        return _components.TryGetValue(typeof(T), out var processor) ? processor as T : default;
    }

    public bool HasComponent<T>() where T : class, IComponent
    {
        return _components.TryGetValue(typeof(T), out var pr);
    }

    public void RemoveComponent<T>() where T : class, IComponent
    {
        _components.Remove(typeof(T));
    }

    public virtual void Initialize(Scene scene)
    {
        foreach (var c in _components.Values)
        {
            c?.OnStart(this, scene);
        }
    }

    public virtual void Draw(Scene scene, GameObjectRenderer renderer = null)
    {
        GameObjectRenderer activeRenderer = renderer ?? _defaultRenderer;
        activeRenderer.Draw(this, scene);
    }

    public virtual void Update(Scene scene, GameTime gameTime)
    {
        foreach (var m in _components.Values)
        {
            m.OnUpdate(this, gameTime, scene);
        }
    }

    public virtual void OnDestroy(Scene scene)
    {
        foreach (var m in _components.Values)
        {
            m.Destroy(this, scene);
        }
    }

    public bool HasTag(string tag)
    {
        foreach (var t in _tags)
        {
            if (t == tag) return true;
        }
        return false;
    }

    public void Destroy()
    {
        _toDestroy = true;
    }

    public bool IsDestroyed()
    {
        return _toDestroy;
    }

}