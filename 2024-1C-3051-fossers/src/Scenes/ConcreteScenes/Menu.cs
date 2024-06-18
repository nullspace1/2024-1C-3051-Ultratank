using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Managers;


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
        // AudioManager.Instance.PlaySound(Audios.MENU_SONG, true);
        screen.Initialize();
    }
}