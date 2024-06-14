using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Managers;
using WarSteel.Scenes;

namespace WarSteel.Entities;

public class TankRenderable : Default
{

    private static readonly string TurretBone = "Turret";
    private static readonly string Cannonbone = "Cannon";
    private Transform _turretTransform;
    private Transform _cannonTransform;

    public TankRenderable(Transform turretTransform, Transform cannonTransform) : base(0.5f,0.5f,Color.Green)
    {
        _turretTransform = turretTransform;
        _cannonTransform = cannonTransform;
    }

    public override void Draw(GameObject gameObject, Scene scene)
    {
        LoadEffectParams(scene);
        

        foreach (var m in gameObject.Model.Meshes)
        {
            foreach (var p in m.MeshParts)
            {
                Matrix world;

                if (m.Name is string t && t == TurretBone)
                {
                    world = _turretTransform.LocalToWorldMatrix(m.ParentBone.Transform);
                }
                else if (m.Name is string c && c == Cannonbone)
                {
                    world = _cannonTransform.LocalToWorldMatrix(m.ParentBone.Transform);
                }
                else
                {
                    world = gameObject.Transform.LocalToWorldMatrix(m.ParentBone.Transform);
                }

                _effect.Parameters["World"].SetValue(world);
                _effect.Parameters["InverseTransposeWorld"].SetValue(Matrix.Transpose(Matrix.Invert(world)));

                p.Effect = _effect;
                
            }
            m.Draw();
        }
    }


}
