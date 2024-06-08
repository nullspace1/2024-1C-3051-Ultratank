using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Common.Shaders;

namespace WarSteel.Entities;

public class TankRenderable : GameObjectRenderer
{

    private static readonly string TurretBone = "Turret";
    private static readonly string Cannonbone = "Cannon";
    private Transform _turretTransform;
    private Transform _cannonTransform;

    public TankRenderable(Model model, Shader shader, Transform turretTransform, Transform cannonTransform) : base(model, shader)
    {
        _turretTransform = turretTransform;
        _cannonTransform = cannonTransform;
    }

    public override Matrix GetMatrix(ModelMesh mesh, Transform transform)
    {

        if (mesh.Name is string t && t == TurretBone)
        {
            return _turretTransform.LocalToWorldMatrix(mesh.ParentBone.Transform);
        }

        if (mesh.Name is string c && c == Cannonbone)
        {
            return _cannonTransform.LocalToWorldMatrix(mesh.ParentBone.Transform);
        }

        return base.GetMatrix(mesh, transform);
    }

}
