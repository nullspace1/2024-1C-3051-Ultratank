using System;
using Microsoft.Xna.Framework;
using WarSteel.Common;
using WarSteel.Entities;
using WarSteel.Scenes;


public class TurretController : IComponent
{
    private Transform _transform;
    private Camera _camera;
    private float _rotationSpeed;

    public TurretController(Transform transform, Camera camera, float rotationSpeed)
    {
        _transform = transform;
        _camera = camera;
        _rotationSpeed = rotationSpeed;
    }

    public void OnStart(GameObject self, Scene scene){}
    public void Destroy(GameObject self, Scene scene){}

    public void OnUpdate(GameObject self, GameTime gameTime, Scene scene)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Quaternion localOrientation = _transform.Parent.WorldToLocalOrientation(_camera.Transform.Orientation);

        Vector3 forward = Vector3.Transform(Vector3.Forward, localOrientation);
        float yaw = (float)Math.Atan2(forward.X, forward.Z);
        Quaternion yawRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw);

        _transform.Orientation = Quaternion.Slerp(_transform.Orientation, yawRotation, _rotationSpeed * dt);
    }
}

public class CannonController : IComponent
{
    private Transform _transform;
    private Camera _camera;

    public CannonController(Transform transform, Camera camera)
    {
        _transform = transform;
        _camera = camera;
    }

    public void OnStart(GameObject self, Scene scene){}

    public void Destroy(GameObject self, Scene scene){}

    public void OnUpdate(GameObject self, GameTime gameTime, Scene scene)
    {
        Quaternion localOrientation = _transform.Parent.WorldToLocalOrientation(_camera.Transform.Orientation);
        Vector3 forward = Vector3.Transform(Vector3.Forward, localOrientation);
        float pitch = MathHelper.Clamp(-(float)Math.Atan2(forward.Y, forward.Z) - 0.1f, -MathF.PI / 4, 0.1f);
        Quaternion pitchRotation = Quaternion.CreateFromAxisAngle(Vector3.Right, pitch);
        _transform.Orientation = pitchRotation;
    }

}