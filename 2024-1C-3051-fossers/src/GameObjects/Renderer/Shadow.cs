using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;
using WarSteel.Scenes.SceneProcessors;


public class Shadow : GameObjectRenderer
{


    private Matrix _viewProjection;
    private Vector3 _mask;
    public Shadow(Matrix viewProjection,Vector3 mask) : base(ContentRepoManager.Instance().GetEffect("ShadowMap"))
    {
        _viewProjection = viewProjection;
        _mask = mask;
    }


    public override void Draw(GameObject gameObject, Scene scene)
    {

        foreach (var modelMesh in gameObject.Model.GetMeshes())
        {
            foreach (var part in modelMesh.MeshParts)
            {
                part.Effect = _effect;
            }
            _effect.Parameters["WorldViewProjection"].SetValue(gameObject.Model.GetPartTransform(modelMesh,gameObject.Transform) * _viewProjection);
            // _effect.Parameters["Mask"].SetValue(_mask);
            modelMesh.Draw();
        }
    }
}