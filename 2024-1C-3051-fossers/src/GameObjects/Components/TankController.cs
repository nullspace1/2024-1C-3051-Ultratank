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

    public void OnStart(GameObject self, Scene scene)
    {
    }

    public void Destroy(GameObject self, Scene scene) { }

    public void OnUpdate(GameObject self, GameTime gameTime, Scene scene)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Quaternion cameraOrientation = _camera.Transform.Orientation;
        Quaternion desiredWorldOrientation = cameraOrientation;

        Quaternion localOrientation = _transform.Parent.WorldToLocalOrientation(desiredWorldOrientation);

        Vector3 forward = Vector3.Transform(Vector3.Forward, localOrientation);
        float yaw = (float)Math.Atan2(forward.X, forward.Z);
        Quaternion yawRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, yaw);

        _transform.Orientation = Quaternion.Slerp(_transform.Orientation, yawRotation, _rotationSpeed * dt);
    }

    public void LoadContent(GameObject self) { }

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

    public void OnStart(GameObject self, Scene scene)
    {
    }

    public void Destroy(GameObject self, Scene scene) { }

    public void OnUpdate(GameObject self, GameTime gameTime, Scene scene)
    {
        Quaternion cameraOrientation = _camera.Transform.Orientation;
        Quaternion desiredWorldOrientation = cameraOrientation;
        Quaternion localOrientation = _transform.Parent.WorldToLocalOrientation(desiredWorldOrientation);
        Vector3 forward = Vector3.Transform(Vector3.Forward, localOrientation);
        float pitch = MathHelper.Clamp(-(float)Math.Atan2(forward.Y, forward.Z) - 0.1f, -MathF.PI / 4, -0.01f);

        Quaternion pitchRotation = Quaternion.CreateFromAxisAngle(Vector3.Right, pitch);
        _transform.Orientation = pitchRotation;
    }
}

public class WheelsController : IComponent
{
    private Transform[] _transforms;
    private float _rotationAngle;
    private float _rotationSpeedY = 0.5f;
    private const float _maxRotationAngleY = 10f;

    public Transform WheelTransform { get => _transforms[0]; }

    public bool IsMovingForwards = false;
    public bool IsMovingBackwards = false;
    public bool IsRotatingLeft = false;
    public bool IsRotatingRight = false;

    public float Angle { get => _rotationAngle; }

    public WheelsController(ObjectModel tankModel, Transform tankTransform)
    {
        int numWheels = 20;
        _transforms = new Transform[numWheels];
        _rotationAngle = 0;

        for (int i = 0; i < numWheels; i++)
        {
            _transforms[i] = new Transform(tankTransform, Vector3.Zero);
            tankModel.SetTransformToPart("Wheel" + (i + 1), _transforms[i]);
        }
    }

    public void OnStart(GameObject self, Scene scene)
    {
    }

    public void RotateRight()
    {
        CalculateRotationY(true);
    }

    public void RotateLeft()
    {
        CalculateRotationY(false);
    }

    public void RotateForwards()
    {
        CalculateRotationX(true);
    }

    public void RotateBackwards()
    {
        CalculateRotationX(false);
    }

    public void ResetWheels()
    {
        if (_rotationAngle == 0) return;

        if (_rotationAngle < 0)
            CalculateRotationY(false, 0);
        else
            CalculateRotationY(true, 0);
    }

    private void CalculateRotationY(bool isRight, float maxRotationAngle = _maxRotationAngleY)
    {
        float angle = isRight ? -_rotationSpeedY : _rotationSpeedY;
        float currentRotationAngle = _rotationAngle + angle;

        if (!isRight && currentRotationAngle > maxRotationAngle)
        {
            currentRotationAngle = maxRotationAngle;
        }
        else if (isRight && currentRotationAngle < -maxRotationAngle)
        {
            currentRotationAngle = -maxRotationAngle;
        }

        float toRotate = currentRotationAngle - _rotationAngle;
        _rotationAngle = currentRotationAngle;

        foreach (Transform wheelTransform in _transforms)
        {
            wheelTransform.RotateEuler(wheelTransform.Up * toRotate);
        }
    }

    private void CalculateRotationX(bool isForwards)
    {
        foreach (Transform wheelTransform in _transforms)
        {
            // wheelTransform.RotateEuler(wheelTransform.Right * rotationDelta);
        }
    }

    public void OnUpdate(GameObject self, GameTime gameTime, Scene scene)
    {
    }

    public void Destroy(GameObject self, Scene scene) { }
}