
using Microsoft.Xna.Framework;
using WarSteel.Entities;
using WarSteel.Common;
using WarSteel.Scenes.SceneProcessors;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Managers;
using WarSteel.Common.Shaders;
using System;
using WarSteel.Entities.Map;

namespace WarSteel.Scenes.Main;

public class MainScene : Scene
{
    public MainScene(GraphicsDeviceManager Graphics, SpriteBatch SpriteBatch) : base(Graphics, SpriteBatch)
    {
    }

    public override void Initialize()
    {
        MainSceneFactory factory = new(this);


        Camera = new(new Vector3(0, 900, -200), GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, MathHelper.PiOver2, 0.1f, 300000f);
        SetCamera(Camera);

        Model model = ContentRepoManager.Instance().GetModel("SkyBox/cube");
        TextureCube skyboxTexture = ContentRepoManager.Instance().GetTextureCube("sun-in-space");
        GameObjectRenderer skyboxRenderer = new SkyBoxShader(skyboxTexture);
        GameObject skybox = new(Array.Empty<string>(), new Transform(), model, skyboxRenderer);

        AddSceneProcessor(new LightProcessor(Color.White, new Vector3(0, 10000, 0)));
        AddSceneProcessor(new PhysicsProcessor());
        AddSceneProcessor(new GizmosProcessor());
        AddSceneProcessor(new SkyBoxProcessor(skybox));

        GetSceneProcessor<LightProcessor>().AddLightSource(new LightSource(Color.White, new Vector3(0, 1000, 0)));

        Player player = factory.PlayerTank(new Vector3(0, 100, 0));
        PlayerScreen playerScreen = new(this);
        playerScreen.Initialize();

        AddGameObject(factory.Ground(Vector3.Zero));
        AddGameObject(player);
        Random rand = new();

        int numTress = 100;
        int numBushes = 25;
        int numRocks = 25;

        for (int i = 0; i < numBushes; i++)
            AddGameObject(factory.Bush(VectorUtils.GetRandomVec3Pos(Vector3.Zero, rand)));
        for (int i = 0; i < numTress; i++)
            AddGameObject(factory.Tree(VectorUtils.GetRandomVec3Pos(Vector3.Zero, rand)));
        for (int i = 0; i < numRocks; i++)
            AddGameObject(factory.Rock(VectorUtils.GetRandomVec3Pos(Vector3.Zero, rand), RockSize.SMALL));
        for (int i = 0; i < numRocks; i++)
            AddGameObject(factory.Rock(VectorUtils.GetRandomVec3Pos(Vector3.Zero, rand), RockSize.MEDIUM));
        for (int i = 0; i < numRocks; i++)
            AddGameObject(factory.Rock(VectorUtils.GetRandomVec3Pos(Vector3.Zero, rand), RockSize.LARGE));



        new WaveInfoScreen(this).Initialize();
        AddSceneProcessor(new WaveProcessor(player));
        Camera.Follow(player);
    }
}