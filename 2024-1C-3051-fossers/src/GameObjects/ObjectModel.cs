using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;

public class ObjectModel {

    private Model _model;
    private Dictionary<string, Transform> _parts;

    public ObjectModel(Model model){
        _model = model;
        _parts = new();
    }

    public void SetTransformToPart(string partName, Transform transform){
        _parts.Add(partName, transform);
    }

    public Matrix GetPartTransform(ModelMesh mesh, Transform defaultTransform){
        bool hasPart = _parts.TryGetValue(mesh.Name, out var transform);
        if (hasPart){
            return transform.LocalToWorldMatrix(mesh.ParentBone.Transform);
        } else {
            return defaultTransform.LocalToWorldMatrix(mesh.ParentBone.Transform);
        }
    }

    public ModelMeshCollection GetMeshes(){
        return _model.Meshes;
    }

}