using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGC.MonoGame.Samples.Geometries;
using WarSteel.Managers;
using WarSteel.Scenes.Main;

namespace WarSteel;

public class Game : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager Graphics { get; }
    private SpriteBatch SpriteBatch;

    private SceneManager SceneManager;

    private Effect _quadEffect;

    private FullScreenQuad _fullScreenQuad;

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
        Graphics.PreparingDeviceSettings += (object s, PreparingDeviceSettingsEventArgs args) =>
{
    args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
};
        if (!Constants.DEBUG_MODE)
            Graphics.IsFullScreen = true;


    }

    protected override void Initialize()
    {
        // init singleton classes
        ContentRepoManager.SetUpInstance(Content, Graphics.GraphicsDevice);
        SceneManager.SetUpInstance(ScenesNames.MENU);
        SceneManager = SceneManager.Instance();
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        GraphicsDevice.BlendState = BlendState.AlphaBlend;

        SceneManager.AddScene(ScenesNames.MENU, new MenuScene(Graphics, SpriteBatch));
        SceneManager.AddScene(ScenesNames.MAIN, new MainScene(Graphics, SpriteBatch));

        SceneManager.CurrentScene().Initialize();

        _quadEffect = ContentRepoManager.Instance().GetEffect("DefaultQuad");
        _fullScreenQuad = new(Graphics.GraphicsDevice);

        base.Initialize();
    }

    protected override void Draw(GameTime gameTime)
    {
        Graphics.GraphicsDevice.SetRenderTarget(ContentRepoManager.Instance().GlobalRenderTarget);
        Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);
        SceneManager.CurrentScene().Draw();

        _quadEffect.Parameters["Texture"].SetValue(ContentRepoManager.Instance().GlobalRenderTarget);

        Graphics.GraphicsDevice.SetRenderTarget(null);
        Graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1f, 0);

        _fullScreenQuad.Draw(_quadEffect);


        base.Draw(gameTime);
    }

    protected override void Update(GameTime gameTime)
    {
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