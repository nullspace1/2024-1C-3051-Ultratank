using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WarSteel.Managers;
using WarSteel.Scenes.Main;

namespace WarSteel;

public class Game : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager Graphics { get; }
    private SpriteBatch SpriteBatch;

    private SceneManager SceneManager;

    public Game()
    {
        Graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        Graphics.PreferredBackBufferWidth = screenWidth;
        Graphics.PreferredBackBufferHeight = screenHeight;
        if (!Constants.DEBUG_MODE)
            Graphics.IsFullScreen = true;
    }

    protected override void Initialize()
    {
        // init singleton classes
        ContentRepoManager.SetUpInstance(Content);
        SceneManager.SetUpInstance(ScenesNames.MENU);
        SceneManager = SceneManager.Instance();
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        SceneManager.AddScene(ScenesNames.MENU, new MenuScene(Graphics, SpriteBatch));
        SceneManager.AddScene(ScenesNames.MAIN, new MainScene(Graphics, SpriteBatch));

        SceneManager.CurrentScene().Initialize();

        base.Initialize();
    }

    protected override void Draw(GameTime gameTime)
    {
        Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
        SceneManager.CurrentScene().Draw();


        base.Draw(gameTime);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            SceneManager.SetCurrentScene(ScenesNames.MENU);
        }
        SceneManager.CurrentScene().Update(gameTime);


        base.Update(gameTime);
    }

    protected override void UnloadContent()
    {
        SceneManager.CurrentScene().Unload();
        Content.Unload();

        base.UnloadContent();
    }
}