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

public class MenuScene : Scene
{
    public MenuScene(GraphicsDeviceManager Graphics, SpriteBatch SpriteBatch) : base(Graphics, SpriteBatch)
    {
    }

    public override void Initialize()
    {
        MenuScreen screen = new(this);
        AudioManager.Instance.AddSoundEffect(Audios.MENU_SONG, ContentRepoManager.Instance().GetSoundEffect("start-song"));
        AudioManager.Instance.SetVolume(Audios.MENU_SONG, 0.5f);
        AudioManager.Instance.PlaySound(Audios.MENU_SONG, true);
        screen.Initialize();

        Camera = new(new Vector3(0, 900, -200), GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio, MathHelper.PiOver2, 0.1f, 300000f);
        SetCamera(Camera);

        MapFactory factory = new(this);

        SkyBox = new SkyBox(ContentRepoManager.Instance().GetTextureCube("skybox"));

        var processor = new LightProcessor(GraphicsDeviceManager.GraphicsDevice, Color.Orange, new(0, 1, 2));

        AddSceneProcessor(processor);
        AddSceneProcessor(new PhysicsProcessor());

        AddGameObject(factory.Ground(Vector3.Zero));
        Random rand = new();

        int numTress = 2;
        int numBushes = 2;
        int numRocks = 25;

        for (int i = 0; i < numBushes; i++)
            AddGameObject(factory.Bush(VectorUtils.GetRandomVec3Pos(Vector3.Zero, rand)));
        for (int i = 0; i < numTress; i++)
            AddGameObject(factory.Tree(VectorUtils.GetRandomVec3Pos(Vector3.Zero, rand)));
        for (int i = 0; i < numRocks; i++)
            AddGameObject(factory.Rock(VectorUtils.GetRandomVec3Pos(new Vector3(0, 100, 0), rand), RockSize.LARGE));
    }
}