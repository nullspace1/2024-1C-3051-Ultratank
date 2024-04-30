using System;
using Microsoft.Xna.Framework;
using WarSteel.Entities;
using WarSteel.Scenes;

namespace WarSteel.Common;

public class Camera : Entity
{
    public Matrix Projection { get; private set; }
    public Matrix View { get; private set; }
    private Entity _followedEntity;

    private const float DefaultNearPlaneDistance = 0.1f;
    private const float DefaultFarPlaneDistance = 1000f;
    private const float DefaultFOV = MathHelper.PiOver2;

    public Camera(Vector3 initialPosition, float aspectRatio, float fov = DefaultFOV, float nearPlaneDistance = DefaultNearPlaneDistance, float farPlaneDistance = DefaultFarPlaneDistance) : base("camera", Array.Empty<string>(), new Transform(), Array.Empty<Component>())
    {
        Transform.Pos = initialPosition;
        Projection = Matrix.CreatePerspectiveFieldOfView(fov, aspectRatio, nearPlaneDistance, farPlaneDistance);
    }

    public void Follow(Entity entity)
    {
        _followedEntity = entity;
    }

    public override void Update(GameTime time, Scene scene)
    {
        Transform.Pos = Vector3.Transform(Transform.Pos, _followedEntity.Transform.GetWorld());
        base.Update(time, scene);
        View = Matrix.CreateLookAt(Transform.Pos, _followedEntity.Transform.Pos, Transform.GetWorld().Up);
    }
}
