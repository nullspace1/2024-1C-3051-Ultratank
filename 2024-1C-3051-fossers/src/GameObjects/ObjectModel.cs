using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Utils;

public class ObjectModel
{
    private readonly Model _model;
    private readonly Matrix[] _boneTransforms;
    private readonly Dictionary<string, Transform> _parts;
    private readonly Dictionary<ModelMesh, List<(Vector3, Vector3)>> _meshImpactPoints;
    private readonly float _maxSize;
    private readonly Transform _defaultTransform;

    public int ImpactCount => _meshImpactPoints.Values.Sum(list => list.Count);

    public ObjectModel(Model model, Transform defaultTransform)
    {
        _model = model;
        _parts = new();
        _boneTransforms = new Matrix[_model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(_boneTransforms);
        _maxSize = MathHelper.Max(ModelUtils.GetHeight(_model), ModelUtils.GetWidth(_model));
        _defaultTransform = defaultTransform;
        _meshImpactPoints = new();
    }

    public void SetTransformToPart(string partName, Transform transform)
    {
        _parts.Add(partName, transform);
    }

    public Matrix GetPartWorld(ModelMesh mesh)
    {
        return _parts.TryGetValue(mesh.Name, out var transform) ? transform.World : _defaultTransform.World;
    }

    public Matrix GetPartTransform(ModelMesh mesh)
    {
        return _boneTransforms[mesh.ParentBone.Index];
    }

    public ModelMeshCollection GetMeshes()
    {
        return _model.Meshes;
    }

    public float GetMinDistance()
    {
        return _maxSize;
    }

    public Model GetModel()
    {
        return _model;
    }

    public void AddImpact(Vector3 position, Vector3 velocity)
    {
        if (ImpactCount < 5)
        {
            foreach (var mesh in _model.Meshes)
            {
                if (_parts.ContainsKey(mesh.Name))
                {
                    var localPosition = Vector3.Transform(position, Matrix.Invert(GetPartWorld(mesh)));
                    var localVelocity = Vector3.Normalize(Vector3.Transform(velocity, Matrix.Invert(GetPartWorld(mesh))));

                    if (Vector3.Dot(localVelocity,localPosition - _defaultTransform.AbsolutePosition) > 0 ){
                        localVelocity *= -1;
                    }

                    if (!_meshImpactPoints.ContainsKey(mesh))
                    {
                        _meshImpactPoints[mesh] = new List<(Vector3, Vector3)>();
                    }
                    _meshImpactPoints[mesh].Add((localPosition, localVelocity));
                }
            }
        }
    }

    public Vector3[] GetImpactPositions(ModelMesh mesh)
    {
        return _meshImpactPoints.TryGetValue(mesh, out var impactPoints) ? impactPoints.Select(i => i.Item1).ToArray() : Array.Empty<Vector3>();
    }

    public Vector3[] GetImpactVelocities(ModelMesh mesh)
    {
        return _meshImpactPoints.TryGetValue(mesh, out var impactPoints) ? impactPoints.Select(i => i.Item2).ToArray() : Array.Empty<Vector3>();
    }

    public void ClearImpacts()
    {
        _meshImpactPoints.Clear();
    }
}
