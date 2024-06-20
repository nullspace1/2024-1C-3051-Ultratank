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
    private float _nearPlaneDistance = 1;

    private float _farPlaneDistance = 1;


    public Shadow(Matrix viewProjection, float nearPlaneDistance, float farPlaneDistance) : base(ContentRepoManager.Instance().GetEffect("ShadowMap"))
    {
        _viewProjection = viewProjection;
        _nearPlaneDistance = nearPlaneDistance;
        _farPlaneDistance = farPlaneDistance;
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
            _effect.Parameters["FarPlaneDistance"].SetValue(_farPlaneDistance);
            // _effect.Parameters["NearPlaneDistance"].SetValue(_nearPlaneDistance);
            modelMesh.Draw();
        }

    }
}