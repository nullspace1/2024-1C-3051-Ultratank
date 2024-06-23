using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Scenes;
using WarSteel.Scenes.SceneProcessors;

namespace WarSteel.Entities;

public class GameObject
{
    private Dictionary<Type, IComponent> _components = new();
    public Transform Transform { get; }
    public ObjectModel Model { get; set; }
    public Renderer Renderer;

    private string[] _tags { get; }
    private bool _toDestroy = false;
    public string Id { get; }
    public bool AlwaysRender;

    public GameObject(string[] tags, Transform transform, Model model, Renderer renderer, bool alwaysRender = false) : base()
    {
        Id = Guid.NewGuid().ToString();
        _tags = tags;
        Transform = transform;
        Model = new ObjectModel(model);
        Renderer = renderer;
        AlwaysRender = alwaysRender;
    }

    public void AddComponent(IComponent c)
    {
        _components.Add(c.GetType(), c);
    }

    public void RemoveComponent<T>() where T : class, IComponent
    {
        _components.Remove(typeof(T));
    }

    public T GetComponent<T>() where T : class, IComponent
    {
        return _components.TryGetValue(typeof(T), out var processor) ? processor as T : default;
    }

    public bool HasComponent<T>() where T : class, IComponent
    {
        return _components.TryGetValue(typeof(T), out var pr);
    }

    public void Initialize(Scene scene)
    {
        foreach (var c in _components.Values)
        {
            c?.OnStart(this, scene);
        }
    }

    public void Update(Scene scene, GameTime gameTime)
    {
        foreach (var m in _components.Values)
        {
            m.OnUpdate(this, gameTime, scene);
        }
    }

    public void OnDestroy(Scene scene)
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

    public BoundingSphere GetBoundingSphere(){
        return new BoundingSphere(Transform.AbsolutePosition, Model.GetMinDistance());
    }



}