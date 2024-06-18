
using Microsoft.Xna.Framework;
using WarSteel.Entities;
using WarSteel.Common;
using WarSteel.Scenes.SceneProcessors;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Managers;
using WarSteel.Common.Shaders;
using System;
using WarSteel.Entities.Map;
using Microsoft.Xna.Framework.Input;
using WarSteel.Utils;

namespace WarSteel.Scenes.Main;

public class MainScene : Scene
{
    private PauseScreen _pauseScreen;
    private bool _checkEsc = true;
    private int _escDelay = 150;

    public MainScene(GraphicsDeviceManager Graphics, SpriteBatch SpriteBatch) : base(Graphics, SpriteBatch)
    {

    }

    public override void Initialize()
    {
        MainSceneFactory factory = new(this);
        _pauseScreen = new(this);
        AudioManager.Instance.AddSong(Audios.AMBIENT, ContentRepoManager.Instance().GetSong("ambient"));
        AudioManager.Instance.PlaySong(Audios.AMBIENT);

        Camera = new(new Vector3(0, 900, -200), GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, MathHelper.PiOver2, 0.1f, 300000f);
        SetCamera(Camera);

        Model model = ContentRepoManager.Instance().GetModel("SkyBox/cube");
        TextureCube skyboxTexture = ContentRepoManager.Instance().GetTextureCube("sun-in-space");
        GameObjectRenderer skyboxRenderer = new SkyBoxShader(skyboxTexture);
        GameObject skybox = new(new string[]{"skybox"}, new Transform(), model, skyboxRenderer);

        AddSceneProcessor(new LightProcessor(GraphicsDeviceManager.GraphicsDevice));
        AddSceneProcessor(new PhysicsProcessor());
        AddSceneProcessor(new GizmosProcessor());
        AddSceneProcessor(new SkyBoxProcessor(skybox));

        GetSceneProcessor<LightProcessor>().AddLight(new Light(new Vector3(0, 1000, 0),Color.White));

        Player player = factory.PlayerTank(new Vector3(0, 100, 0));
        PlayerScreen playerScreen = new(this);
        playerScreen.Initialize();

        AddGameObject(factory.Ground(Vector3.Zero));
        AddGameObject(player);
        Random rand = new();

        int numTress = 6;
        int numBushes = 6;
        int numRocks = 6;

        for (int i = 0; i < numBushes; i++)
            AddGameObject(factory.Bush(VectorUtils.GetRandomVec3Pos(Vector3.Zero, rand)));
        for (int i = 0; i < numTress; i++)
            AddGameObject(factory.Tree(VectorUtils.GetRandomVec3Pos(Vector3.Zero, rand)));
        for (int i = 0; i < numRocks; i++)
            AddGameObject(factory.Rock(VectorUtils.GetRandomVec3Pos(Vector3.Zero, rand), RockSize.LARGE));

        new WaveInfoScreen(this).Initialize();
        AddSceneProcessor(new WaveProcessor(player));
        Camera.Follow(player);


    }

    public override void Update(GameTime time)
    {
        if (_checkEsc && Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            if (!IsPaused)
            {
                Pause();
                _pauseScreen.Initialize();
            }
            else
            {
                Resume();
                _pauseScreen.Destroy();
            }

            _checkEsc = false;
            Timer.Timeout(_escDelay, () => _checkEsc = true);
        }

        base.Update(time);
    }
}