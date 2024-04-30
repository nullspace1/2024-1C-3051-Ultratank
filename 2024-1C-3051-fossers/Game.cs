using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using WarSteel.Managers;
using WarSteel.Scenes.Main;

namespace WarSteel;

public class Game : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics { get; }

    private SceneManager _sceneManager;

    public Game()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        ContentRepoManager.SetUpInstance(Content);
        _sceneManager = new SceneManager();

        _sceneManager.AddScene(ScenesNames.MAIN, new MainScene(_graphics));
        _sceneManager.SetCurrentScene(ScenesNames.MAIN);


        _sceneManager.CurrentScene().Initialize();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _sceneManager.CurrentScene().LoadContent();
        base.LoadContent();
    }

    protected override void Draw(GameTime gameTime)
    {
        _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        _sceneManager.CurrentScene().Draw();
        base.Draw(gameTime);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }
        _sceneManager.CurrentScene().Update(gameTime);

        base.Update(gameTime);
    }

    protected override void UnloadContent()
    {
        _sceneManager.CurrentScene().Unload();
        Content.Unload();
        base.UnloadContent();
    }
}