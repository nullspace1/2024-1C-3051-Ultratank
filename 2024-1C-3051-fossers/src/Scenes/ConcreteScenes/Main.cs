
using Microsoft.Xna.Framework;
using WarSteel.Entities;
using WarSteel.Common;
using WarSteel.Scenes.SceneProcessors;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using WarSteel.Managers;
using WarSteel.Common.Shaders;


namespace WarSteel.Scenes.Main;

public class MainScene : Scene
{
    public MainScene(GraphicsDeviceManager Graphics, SpriteBatch SpriteBatch) : base(Graphics, SpriteBatch)
    {
    }

    public override void Initialize()
    {
        MainSceneFactory factory = new(this);

        Camera = new(new Vector3(0, 900, -200), GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, GraphicsDeviceManager.GraphicsDevice, MathHelper.PiOver2, 0.1f, 300000f);
        SetCamera(Camera);

        Model model = ContentRepoManager.Instance().GetModel("SkyBox/cube");
        TextureCube skyboxTexture = ContentRepoManager.Instance().GetTextureCube("sun-in-space");
        GameObjectRenderer renderer = new(model, new SkyBoxShader(skyboxTexture));

        AddSceneProcessor(new LightProcessor(Color.AliceBlue, new List<LightSource>() { new(Color.White, new Vector3(2000, 9000, 0)) }));
        AddSceneProcessor(new PhysicsProcessor());
        AddSceneProcessor(new GizmosProcessor());
        AddSceneProcessor(new SkyBoxProcessor(renderer));

        GetSceneProcessor<LightProcessor>().AddLightSource(new LightSource(Color.White,new Vector3(0,500,0)));

        GameObject player = factory.PlayerTank(new Vector3(0, 500, 0));

        AddGameObject(factory.Ground(Vector3.Zero));
        AddGameObject(player);

        Camera.Follow(player);

    }

}