using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;

public class ObjectModel
{
    private Model _model;
    private Matrix[] _boneTransforms;
    private Dictionary<string, Transform> _parts;


    public ObjectModel(Model model)
    {
        _model = model;
        _parts = new();
        _boneTransforms = new Matrix[_model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(_boneTransforms);
    }

    public void SetTransformToPart(string partName, Transform transform)
    {
        _parts.Add(partName, transform);
    }

    public Matrix GetPartTransform(ModelMesh mesh, Transform defaultTransform)
    {
        bool hasPart = _parts.TryGetValue(mesh.Name, out var transform);
        if (hasPart)
        {
            return transform.LocalToWorldMatrix(_boneTransforms[mesh.ParentBone.Index]);
        }
        else
        {
            return defaultTransform.LocalToWorldMatrix(_boneTransforms[mesh.ParentBone.Index]);
        }
    }

    public ModelMeshCollection GetMeshes()
    {
        return _model.Meshes;
    }
}