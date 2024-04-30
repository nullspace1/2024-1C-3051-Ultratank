using System;
using Microsoft.Xna.Framework;
using WarSteel.Common;
using WarSteel.Scenes;

namespace WarSteel.Entities;

public class Entity
{
    public string Id { get; }
    public string Name { get; }
    public string[] Tags { get; }

    public Component[] Modifiers { get; }

    public Transform Transform { get; }
    protected Renderable Renderable { get; set; }

    public Entity(string name, string[] tags, Transform transform, Component[] modifiers)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Tags = tags;
        Transform = transform;
        Modifiers = modifiers;
        Renderable = null;
    }

    public Entity(string name, string[] tags, Transform transform, Renderable renderable)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Tags = tags;
        Transform = transform;
        Renderable = renderable;
    }

    public virtual void Initialize() { }
    public virtual void LoadContent() { }
    public virtual void Draw(Scene scene)
    {
        if (Renderable != null)
            Renderable.Draw(Transform.GetWorld(), scene);
    }
    public virtual void Update(GameTime gameTime, Scene scene)
    {
        foreach (var m in Modifiers)
        {
            m.UpdateEntity(this, gameTime, scene);
        }
    }
    public virtual void OnDestroy() { }
}