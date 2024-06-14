using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Entities;
using WarSteel.Scenes;

public class SkyBoxProcessor : ISceneProcessor
{

    public GameObject _skybox;

    public SkyBoxProcessor(GameObject skybox){
        _skybox = skybox;
    }
    
    public void Draw(Scene scene)
    {
        GraphicsDevice gDevice = scene.GraphicsDeviceManager.GraphicsDevice;
        RasterizerState state_r = gDevice.RasterizerState;
        gDevice.RasterizerState = RasterizerState.CullNone;
        _skybox.Draw(scene);
        gDevice.RasterizerState = state_r;
    }

    public void Initialize(Scene scene){}

    public void Update(Scene scene, GameTime gameTime){}
}