using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;

namespace WarSteel.Common.Shaders;

class SkyBoxShader : GameObjectRenderer
{

    private TextureCube _texture;

    public SkyBoxShader(TextureCube texture) : base(ContentRepoManager.Instance().GetEffect("SkyBox"))
    {
        _texture = texture;
    }

    public override void Draw(GameObject gameObject, Scene scene)
    {
        Transform cameraTransform = scene.GetCamera().Transform;
        _effect.Parameters["World"].SetValue(Matrix.CreateScale(100000) * Matrix.CreateTranslation(cameraTransform.AbsolutePosition));
        _effect.Parameters["SkyBoxTexture"].SetValue(_texture);
        _effect.Parameters["CameraPosition"].SetValue(cameraTransform.AbsolutePosition);
        _effect.Parameters["View"].SetValue(scene.GetCamera().View);
        _effect.Parameters["Projection"].SetValue(scene.GetCamera().Projection);

        var modelMeshesBaseTransforms = new Matrix[gameObject.Model.Bones.Count];
        gameObject.Model.CopyAbsoluteBoneTransformsTo(modelMeshesBaseTransforms);
        
        foreach (var modelMesh in gameObject.Model.Meshes)
        {
            foreach (var part in modelMesh.MeshParts)
                part.Effect = _effect;

            modelMesh.Draw();
        }
        
    }
}