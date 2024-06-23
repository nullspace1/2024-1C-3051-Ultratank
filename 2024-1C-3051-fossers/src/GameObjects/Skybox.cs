using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;

namespace WarSteel.Common.Shaders;

public class SkyBox
{

    private TextureCube _texture;
    private Effect _effect;

    private Model _model;
    

    public SkyBox(TextureCube texture)
    {
        _texture = texture;
        _effect = ContentRepoManager.Instance().GetEffect("SkyBox");
        _model = ContentRepoManager.Instance().GetModel("SkyBox/cube");
    }

    public void DrawSkyBox(Scene scene)
    {
        
        GraphicsDevice gDevice = scene.GraphicsDeviceManager.GraphicsDevice;
        RasterizerState state_r = gDevice.RasterizerState;
        gDevice.RasterizerState = RasterizerState.CullNone;

        Transform cameraTransform = scene.GetCamera().Transform;
        _effect.Parameters["World"].SetValue(Matrix.CreateScale(30000) * Matrix.CreateTranslation(cameraTransform.AbsolutePosition));
        _effect.Parameters["SkyBoxTexture"].SetValue(_texture);
        _effect.Parameters["CameraPosition"].SetValue(cameraTransform.AbsolutePosition);
        _effect.Parameters["View"].SetValue(scene.GetCamera().View);
        _effect.Parameters["Projection"].SetValue(scene.GetCamera().Projection);

        foreach (var modelMesh in _model.Meshes)
        {
            foreach (var part in modelMesh.MeshParts)
                part.Effect = _effect;

            modelMesh.Draw();
        }

        gDevice.RasterizerState = state_r;

    }
}