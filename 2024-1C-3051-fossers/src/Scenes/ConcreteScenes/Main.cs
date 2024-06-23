
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
using System.Collections.Generic;

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

        SkyBox = new SkyBox(ContentRepoManager.Instance().GetTextureCube("skybox"));

        Camera = new(new Vector3(0, 900, -200), GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, MathHelper.PiOver2, 0.1f, 40000f);
        SetCamera(Camera);

        var processor = new LightProcessor(GraphicsDeviceManager.GraphicsDevice, Color.Orange, new Vector3(0, 0.5f, 0.5f));
        AddSceneProcessor(processor);
        AddSceneProcessor(new PhysicsProcessor());

        Player player = factory.PlayerTank(new Vector3(0, 100, 0));
        PlayerScreen playerScreen = new(this);
        playerScreen.Initialize();

        GameObject leftWall = factory.Ground(new Vector3(0, -9200, 9600));
        leftWall.Transform.Orientation = Quaternion.CreateFromYawPitchRoll(0, MathF.PI / 2, 0);
        AddGameObject(leftWall);

        GameObject rightWall = factory.Ground(new Vector3(0, -9200, -9600));
        rightWall.Transform.Orientation = Quaternion.CreateFromYawPitchRoll(0, MathF.PI / 2, 0);
        AddGameObject(rightWall);

        GameObject topWall = factory.Ground(new Vector3(9600, -9200, 0));
        topWall.Transform.Orientation = Quaternion.CreateFromYawPitchRoll(0, MathF.PI/2, MathF.PI/2);
        AddGameObject(topWall);

        GameObject bottomWall = factory.Ground(new Vector3(-9600, -9200, 0));
        bottomWall.Transform.Orientation = Quaternion.CreateFromYawPitchRoll(0, MathF.PI/2, MathF.PI/2);
        AddGameObject(bottomWall);


        AddGameObject(factory.Ground(Vector3.Up * -100));

        AddGameObject(player);
        Random rand = new();

        int numTrees = 5;
        int numBushes = 4;
        int numRocks = 10;

        HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();

        void AddObjectSafely(Func<Vector3, GameObject> createObject, int count, Vector3 offset)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 position;
                do
                {
                    position = VectorUtils.GetRandomVec3Pos(offset, rand);
                    // Introduce some noise to make the terrain more natural
                    position.X += (float)(rand.NextDouble() - 0.5) * 50;
                    position.Z += (float)(rand.NextDouble() - 0.5) * 50;
                } while (occupiedPositions.Contains(position));

                occupiedPositions.Add(position);
                GameObject obj = createObject(position);
                obj.Transform.Orientation = Quaternion.CreateFromYawPitchRoll((float)rand.NextDouble() * 3,0,0);
                AddGameObject(createObject(position));
            }
        }

        AddObjectSafely(pos => factory.Bush(pos), numBushes, Vector3.Zero);
        AddObjectSafely(pos => factory.Tree(pos), numTrees, Vector3.Zero);
        AddObjectSafely(pos => factory.Rock(pos, RockSize.LARGE), numRocks, new Vector3(0, 50, 0));

        new WaveInfoScreen(this).Initialize();
        AddSceneProcessor(new WaveProcessor(player));
        AddSceneProcessor(new PostProcessing());
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