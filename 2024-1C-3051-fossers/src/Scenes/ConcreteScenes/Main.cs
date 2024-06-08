
using Microsoft.Xna.Framework;
using WarSteel.Entities;
using WarSteel.Common;
using WarSteel.Scenes.SceneProcessors;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace WarSteel.Scenes.Main;

public class MainScene : Scene
{
    public MainScene(GraphicsDeviceManager Graphics, SpriteBatch SpriteBatch) : base(Graphics, SpriteBatch)
    {
    }

    public override void Initialize()
    {
        MainSceneFactory factory = new MainSceneFactory(this);

        Camera = new(new Vector3(0, 800, -500), GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, GraphicsDeviceManager.GraphicsDevice, MathHelper.PiOver2, 0.1f, 300000f);
        SetCamera(Camera);

        AddSceneProcessor(new LightProcessor(Color.AliceBlue,new List<LightSource>(){new(Color.White, new Vector3(2000, 9000, 0))}));
        AddSceneProcessor(new PhysicsProcessor());
        AddSceneProcessor(new GizmosProcessor());

        GameObject player = factory.PlayerTank(new Vector3(0,500,0));

        AddGameObject(factory.Ground(Vector3.Zero));
        AddGameObject(player);
        
        
        Camera.Follow(player);

    }

}