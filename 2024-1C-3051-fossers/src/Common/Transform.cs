using System;
using System.Collections.Generic;
using BepuPhysics;
using Microsoft.Xna.Framework;

namespace WarSteel.Common
{
    public class Transform
    {
        private Vector3 _dimensions = Vector3.One;
        private Vector3 _position = Vector3.Zero;
        private Quaternion _orientation = Quaternion.Identity;
        private Transform _parent;
        private Matrix _worldMatrix;
        private bool _isDirty = true;

        protected List<Transform> _children = new();

        public Vector3 Dimensions
        {
            get => _dimensions;
            set
            {
                _dimensions = value;
                MarkDirty();
            }
        }

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                MarkDirty();
            }
        }

        public Quaternion Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                MarkDirty();
            }
        }

        public Transform Parent
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    MarkDirty();
                    _parent._children.Add(this);
                }
            }
        }

        public Matrix World
        {
            get
            {
                if (_isDirty)
                {
                    _worldMatrix = Matrix.CreateScale(Dimensions) *
                                   Matrix.CreateFromQuaternion(Orientation) *
                                   Matrix.CreateTranslation(Position) *
                                   (Parent == null ? Matrix.Identity : Parent.World);
                    _isDirty = false;
                }
                return _worldMatrix;
            }
        }

        public Matrix View => GetLookAt(AbsolutePosition + Forward * 10);

        public Vector3 Forward => World.Forward;

        public Vector3 Right => World.Right;

        public Vector3 Up => World.Up;

        public Vector3 Backward => World.Backward;

        public Vector3 Down => World.Down;

        public Vector3 AbsolutePosition => World.Translation;

        public Transform()
        {
            _dimensions = Vector3.One;
            _position = Vector3.Zero;
            _orientation = Quaternion.Identity;
            _parent = null;
        }

        public Transform(Transform parent, Vector3 pos)
        {
            _dimensions = Vector3.One;
            _position = pos;
            _orientation = Quaternion.Identity;
            Parent = parent;
        }

        public Vector3 LocalToWorldPosition(Vector3 point)
        {
            return Parent == null ? Vector3.Transform(point, World) : Parent.LocalToWorldPosition(Vector3.Transform(point, World));
        }

        public Vector3 WorldToLocalPosition(Vector3 point)
        {
            return Vector3.Transform(point, Matrix.Invert(World));
        }

        public Quaternion LocalToWorldOrientation(Quaternion orientation)
        {
            return Parent == null ? orientation * Orientation : Parent.LocalToWorldOrientation(orientation * Orientation);
        }

        public Quaternion WorldToLocalOrientation(Quaternion orientation)
        {
            return Parent == null ? Quaternion.Inverse(Orientation) * orientation : Parent.WorldToLocalOrientation(Quaternion.Inverse(Orientation) * orientation);
        }

        public Matrix LocalToWorldMatrix(Matrix matrix)
        {
            return matrix * World;
        }

        public void LookAt(Vector3 point)
        {
            var view = GetLookAt(point);
            view.Translation = Vector3.Zero;
            var orientation = Quaternion.CreateFromRotationMatrix(Matrix.Invert(view));
            Orientation = Parent == null ? orientation : Parent.WorldToLocalOrientation(orientation);
        }

        public Matrix GetLookAt(Vector3 point)
        {
            var direction = Vector3.Normalize(point - AbsolutePosition);
            var up = Vector3.UnitY;
            if (MathF.Abs(Vector3.Dot(direction, up)) > 0.9999f)
            {
                up = Math.Abs(direction.Y) < 0.9999f ? Vector3.UnitY : Vector3.UnitX;
            }
            return Matrix.CreateLookAt(AbsolutePosition, point, up);
        }

        public void RotateEuler(Vector3 eulerAngles)
        {
            Orientation = Quaternion.CreateFromYawPitchRoll(
                MathHelper.ToRadians(eulerAngles.Y),
                MathHelper.ToRadians(eulerAngles.X),
                MathHelper.ToRadians(eulerAngles.Z)
            ) * Orientation;
        }

        private void MarkDirty()
        {
            _isDirty = true;
            _children.ForEach(c => c.MarkDirty());
        }
    }
}
