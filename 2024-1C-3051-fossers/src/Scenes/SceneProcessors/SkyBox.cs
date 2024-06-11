using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Scenes;

public class SkyBoxProcessor : ISceneProcessor
{

    public GameObjectRenderer _skybox;
    private Transform _transform = new();

    public SkyBoxProcessor(GameObjectRenderer skybox){
        _skybox = skybox;
    }
    
    public void Draw(Scene scene)
    {
        GraphicsDevice gDevice = scene.GraphicsDeviceManager.GraphicsDevice;
        RasterizerState state_r = gDevice.RasterizerState;
        gDevice.RasterizerState = RasterizerState.CullNone;
        _skybox.Draw(_transform,scene);
        gDevice.RasterizerState = state_r;
    }

    public void Initialize(Scene scene){}

    public void Update(Scene scene, GameTime gameTime){}
}