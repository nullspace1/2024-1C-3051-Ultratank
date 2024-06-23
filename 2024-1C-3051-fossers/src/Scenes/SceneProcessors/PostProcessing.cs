
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGC.MonoGame.Samples.Geometries;
using WarSteel.Managers;
using WarSteel.Scenes;

public class PostProcessing : ISceneProcessor
{

    private Effect _postProcessingEffect;
    private FullScreenQuad _quad;

    public void Draw(Scene scene)
    {
        _postProcessingEffect.Parameters["Screen"].SetValue(ContentRepoManager.Instance().GlobalRenderTarget);;
        _quad.Draw(_postProcessingEffect);
    }

    public void Initialize(Scene scene)
    {
        _quad = new(scene.GraphicsDeviceManager.GraphicsDevice);
        _postProcessingEffect = ContentRepoManager.Instance().GetEffect("PostProcessing");
    }

    public void Update(Scene scene, GameTime gameTime)
    {
    }
}