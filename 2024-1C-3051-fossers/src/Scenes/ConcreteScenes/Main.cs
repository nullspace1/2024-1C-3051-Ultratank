
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
        MapFactory factory = new(this);
        _pauseScreen = new(this);
        AudioManager.Instance.AddSoundEffect(Audios.AMBIENT, ContentRepoManager.Instance().GetSoundEffect("ambient"));
        AudioManager.Instance.SetVolume(Audios.AMBIENT, 0.05f);
        AudioManager.Instance.PlaySound(Audios.AMBIENT, true);

        Camera = new(new Vector3(0, 900, -200), GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, MathHelper.PiOver2, 0.1f, 30000f);
        SetCamera(Camera);

        SkyBox = new SkyBox(ContentRepoManager.Instance().GetTextureCube("skybox"));



        var processor = new LightProcessor(GraphicsDeviceManager.GraphicsDevice,Color.Red,new Vector3(0,0.5f,0.5f));
        AddSceneProcessor(processor);
        AddSceneProcessor(new PhysicsProcessor());


        Player player = factory.PlayerTank(new Vector3(0, 100, 0));
        PlayerScreen playerScreen = new(this);
        playerScreen.Initialize();

    

        AddGameObject(factory.Ground(Vector3.Zero));
        AddGameObject(player);
        Random rand = new();

        int numTress = 2;
        int numBushes = 5;
        int numRocks = 25;

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