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

namespace WarSteel.Scenes.Main
{
    public class MainScene : Scene
    {
        private PauseScreen _pauseScreen;
        private bool _checkEsc = true;

        private const int EscDelay = 150;
        private const float AmbientVolume = 0.05f;
        private const float CameraFieldOfView = MathHelper.PiOver2;
        private const float CameraNearPlane = 0.1f;
        private const float CameraFarPlane = 50000f;
        private const int MapWidth = 9000;
        private const int MapHeight = 9000;

        private const int WallWidth = 9600;
        private const int WallHeight = 9200;

        private static readonly Vector3 InitialCameraPosition = new(0, 900, -200);
        private static readonly Vector3 PlayerInitialPosition = new(0, 100, 0);
        private static readonly Quaternion WallOrientation1 = Quaternion.CreateFromYawPitchRoll(0, MathF.PI / 2, 0);
        private static readonly Quaternion WallOrientation2 = Quaternion.CreateFromYawPitchRoll(0, MathF.PI / 2, 0);
        private static readonly Quaternion WallOrientation3 = Quaternion.CreateFromYawPitchRoll(0, MathF.PI / 2, MathF.PI / 2);
        private static readonly Quaternion WallOrientation4 = Quaternion.CreateFromYawPitchRoll(0, MathF.PI / 2, MathF.PI / 2);
        private static readonly Vector3 GroundPosition = Vector3.Up * -100;
        private static readonly Vector3 LightDirection = new(0, 0.5f, 0.5f);
        private static readonly Color LightColor = Color.Orange;
        private static readonly int CellSize = 200;
        private static readonly MapGrid grid = new MapGrid(CellSize, CellSize, MapWidth / CellSize, MapHeight / CellSize);

        public MainScene(GraphicsDeviceManager graphics, SpriteBatch spriteBatch) : base(graphics, spriteBatch)
        {
        }

        public override void Initialize()
        {

            _pauseScreen = new(this);

            InitializeAudio();
            InitializeSkyBox();
            InitializeCamera();
            InitializeSceneProcessors();

            var factory = new MapFactory(this);
            var player = InitializePlayer(factory);
            InitializeWalls(factory);
            InitializeGround(factory);
            InitializeGroundObjects(factory);
            InitializeUI(player);
        }

        private void InitializeAudio()
        {
            var audioManager = AudioManager.Instance;
            audioManager.AddSoundEffect(Audios.AMBIENT, ContentRepoManager.Instance().GetSoundEffect("ambient"));
            audioManager.SetVolume(Audios.AMBIENT, AmbientVolume);
            audioManager.PlaySound(Audios.AMBIENT, true);
        }

        private void InitializeSkyBox()
        {
            SkyBox = new SkyBox(ContentRepoManager.Instance().GetTextureCube("skybox"));
        }

        private void InitializeCamera()
        {
            Camera = new Camera(
                InitialCameraPosition,
                GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio,
                CameraFieldOfView, CameraNearPlane, CameraFarPlane
            );
            SetCamera(Camera);
        }

        private void InitializeSceneProcessors()
        {
            AddSceneProcessor(new LightProcessor(GraphicsDeviceManager.GraphicsDevice, LightColor, LightDirection));
            AddSceneProcessor(new PhysicsProcessor());
            AddSceneProcessor(new PostProcessing());
        }

        private Player InitializePlayer(MapFactory factory)
        {
            var player = factory.PlayerTank(PlayerInitialPosition);
            player.Transform.Position = grid.GetRandomUnusedGridPosition(100);
            AddGameObject(player);
            Camera.Follow(player);

            AddSceneProcessor(new WaveProcessor(player,grid));

            return player;
        }

        private void InitializeWalls(MapFactory factory)
        {
            AddWall(factory, new Vector3(0, -WallHeight, WallWidth), WallOrientation1);
            AddWall(factory, new Vector3(0, -WallHeight, -WallWidth), WallOrientation2);
            AddWall(factory, new Vector3(WallWidth, -WallHeight, 0), WallOrientation3);
            AddWall(factory, new Vector3(-WallWidth, -WallHeight, 0), WallOrientation4);
        }

        private void AddWall(MapFactory factory, Vector3 position, Quaternion orientation)
        {
            var wall = factory.Ground(position);
            wall.Transform.Orientation = orientation;
            AddGameObject(wall);
        }

        private void InitializeGround(MapFactory factory)
        {
            AddGameObject(factory.Ground(GroundPosition));
        }

        private void InitializeGroundObjects(MapFactory factory)
        {

            for (int i = 0; i < 10; i++)
            {
                Vector3 pos = grid.GetRandomUnusedGridPosition(10);
                GameObject rock = factory.Rock(pos, RockSize.SMALL);
                AddGameObject(rock);
            }
            for (int i = 0; i < 10; i++)
            {
                Vector3 pos = grid.GetRandomUnusedGridPosition(10);
                GameObject rock = factory.Rock(pos, RockSize.MEDIUM);
                AddGameObject(rock);
            }
            for (int i = 0; i < 10; i++)
            {
                Vector3 pos = grid.GetRandomUnusedGridPosition(10);
                GameObject rock = factory.Rock(pos, RockSize.LARGE);
                AddGameObject(rock);
            }
        }

        private void InitializeUI(Player player)
        {
            var playerScreen = new PlayerScreen(this);
            playerScreen.Initialize();

            new WaveInfoScreen(this).Initialize();
        }

        public override void Update(GameTime time)
        {
            HandlePause();
            base.Update(time);
        }

        private void HandlePause()
        {
            if (_checkEsc && Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                TogglePause();
                _checkEsc = false;
                Timer.Timeout(EscDelay, () => _checkEsc = true);
            }
        }

        new private void TogglePause()
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
        }
    }
}
