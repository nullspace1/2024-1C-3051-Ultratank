using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WarSteel.Scenes;
using WarSteel.UIKit;
using WarSteel.Utils;

public class PauseScreen
{
    private Scene _scene;
    private List<UI> _uis;

    public PauseScreen(Scene scene)
    {
        _scene = scene;
        _uis = new();
    }

    public void Initialize()
    {
        ShowMainPause();
    }


    private void ShowMainPause()
    {
        Destroy();
        GraphicsDeviceManager GraphicsDeviceManager = _scene.GraphicsDeviceManager;
        Vector2 screenCenter = Screen.GetScreenCenter(GraphicsDeviceManager);

        UI header = new(new Vector2(screenCenter.X, screenCenter.Y - 150), new Header("Pause"));
        UI bg = new(new Vector2(screenCenter.X, screenCenter.Y), new Image("UI/transparent-bg"));

        UI resumeBtn = new(new Vector2(screenCenter.X, screenCenter.Y - 50), 300, 60, new PrimaryBtn("Resume"),
            (_, ui) =>
            {
                _scene.Resume();
                Destroy();
            }
        );
        UI startMenuBtn = new(new Vector2(screenCenter.X, screenCenter.Y + 50), 300, 60, new SecondaryBtn("Main Menu"),
                    (_scene, ui) =>
                    {
                        ShowConfirmMessage();
                    }
                );
        _uis.AddRange(new UI[] { header, bg, resumeBtn, startMenuBtn });
        _scene.AddUI(_uis);

    }

    private void ShowConfirmMessage()
    {

        Destroy();
        GraphicsDeviceManager GraphicsDeviceManager = _scene.GraphicsDeviceManager;
        Vector2 screenCenter = Screen.GetScreenCenter(GraphicsDeviceManager);


        UI bg = new(new Vector2(screenCenter.X, screenCenter.Y), new Image("UI/transparent-bg"));
        UI confirmMessage = new(new Vector2(screenCenter.X, screenCenter.Y - 100), new Paragraph("Are you sure you want to leave?"));

        UI yesBtn = new(new Vector2(screenCenter.X - 100, screenCenter.Y), 100, 60, new SecondaryBtn("Yes"),
            (_scene, ui) =>
            {
                SceneManager.Instance().SetCurrentScene(ScenesNames.MENU);
            });

        UI noBtn = new(new Vector2(screenCenter.X + 100, screenCenter.Y), 100, 60, new PrimaryBtn("No"),
            (_scene, ui) =>
            {
                ShowMainPause();
            }
        );
        _uis.AddRange(new UI[] { confirmMessage, bg, yesBtn, noBtn });
        _scene.AddUI(_uis);
    }

    public void Destroy()
    {
        _scene.RemoveUI(_uis);
        _uis.Clear();
    }
}