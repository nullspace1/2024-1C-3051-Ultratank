using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WarSteel.Common;


public class Transform
{

    public Vector3 Dimensions;
    public Vector3 Position;
    public Quaternion Orientation;
    public Transform Parent;

    public Matrix World
    {
        get => Matrix.CreateScale(Dimensions) * Matrix.CreateFromQuaternion(Orientation) * Matrix.CreateTranslation(Position) * (Parent == null ? Matrix.Identity : Parent.World);
    }

    public Vector3 Forward
    {
        get => World.Forward;
    }

    public Vector3 Right
    {
        get => World.Right;
    }

    public Vector3 Up
    {
        get => World.Up;
    }

    public Vector3 Backward
    {
        get => World.Backward;
    }

    public Vector3 Down
    {
        get => World.Down;
    }

    public Vector3 AbsolutePosition
    {
        get => World.Translation;
    }

    public Transform()
    {
        Dimensions = Vector3.One;
        Position = Vector3.Zero;
        Orientation = Quaternion.Identity;
        Parent = null;
    }

    public Transform(Transform transform, Vector3 pos)
    {
        Dimensions = Vector3.One;
        Position = pos;
        Orientation = Quaternion.Identity;
        Parent = transform;
    }

    public void RotateEuler(Vector3 eulerAngles) => Orientation = Quaternion.CreateFromYawPitchRoll(
        MathHelper.ToRadians(eulerAngles.Y),
        MathHelper.ToRadians(eulerAngles.X),
        MathHelper.ToRadians(eulerAngles.Z)
    ) * Orientation;

    public void RotateQuaternion(Quaternion quaternion) => Orientation = quaternion * Orientation;

    public Vector3 TransformPoint(Vector3 point)
    {
        return Vector3.Transform(point, World);
    }

    public Quaternion TransformQuaternion(Quaternion quaternion)
    {
        return Parent == null ? quaternion * Orientation : Parent.TransformQuaternion(quaternion * Orientation);
    }

    public Matrix TransformMatrix(Matrix matrix)
    {
        return matrix * World;
    }

    public void LookAt(Vector3 point)
    {
        Matrix rotationMatrix = Matrix.CreateLookAt(Position, point, Vector3.Up);
        Orientation = Quaternion.CreateFromRotationMatrix(Matrix.Invert(rotationMatrix));
    }





}



