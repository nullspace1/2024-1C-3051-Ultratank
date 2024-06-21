using WarSteel.Scenes;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WarSteel.UIKit;
using WarSteel.Utils;

public class MenuScreen
{
    Scene _scene;

    public MenuScreen(Scene scene)
    {
        _scene = scene;
    }

    public void Initialize()
    {

        Vector2 screenCenter = Screen.GetScreenCenter(_scene.GraphicsDeviceManager);
        int screenWidth = Screen.GetScreenWidth(_scene.GraphicsDeviceManager);

        // UI background = new(new Vector2(screenCenter.X, screenCenter.Y), screenWidth, screenWidth, new Image("UI/menu-bg"));
        // _scene.AddUI(background);

        Vector2 GetBtnPos(int pos)
        {
            int margin = 90;
            return new Vector2(screenCenter.X, screenCenter.Y - 50 + margin * pos);
        }

        Vector2 GetControlPos(int pos, Vector2 screenCenter)
        {
            int margin = 50;
            return new Vector2(screenCenter.X, screenCenter.Y - 2 * margin + margin * pos);
        }

        List<UI> startMenu = new();
        List<UI> controlsMenu = new();
        List<UI> exitMenu = new();

        UI headerMain = new(new Vector2(screenCenter.X, screenCenter.Y - 160), new Header("WARSTEEL"));

        UI startButton = new(GetBtnPos(0), 300, 60, new PrimaryBtn("Start"), (Scene scene, UI ui) =>
        {
            AudioManager.Instance.StopSound(Audios.MENU_SONG);
            SceneManager.Instance().SetCurrentScene(ScenesNames.MAIN);
        });

        UI controlsButton = new(GetBtnPos(1), 300, 60, new SecondaryBtn("Controls"), (Scene scene, UI ui) =>
        {
            _scene.RemoveUI(startMenu);
            _scene.AddUI(controlsMenu);
        });

        UI exitButton = new(GetBtnPos(2), 300, 60, new SecondaryBtn("Exit"), (Scene scene, UI ui) =>
        {
            _scene.RemoveUI(startMenu);
            _scene.AddUI(exitMenu);
        });

        UI controlsBg = new(screenCenter, 500, 400, new Image("UI/primary-btn"));

        UI headerControls = new(new Vector2(screenCenter.X, 100), new Header("Tank Controls"));

        UI w = new(GetControlPos(0, screenCenter), new Paragraph("W - Move tank forward."));

        UI s = new(GetControlPos(1, screenCenter), new Paragraph("S - Move tank backwards."));

        UI a = new(GetControlPos(2, screenCenter), new Paragraph("A - Rotate tank to the left."));

        UI d = new(GetControlPos(3, screenCenter), new Paragraph("D - Rotate tank to the right."));

        UI lmb = new(GetControlPos(4, screenCenter), new Paragraph("LMB - Shoot projectile."));

        UI backBtn = new(new Vector2(screenCenter.X, Screen.GetScreenHeight(_scene.GraphicsDeviceManager) - 50), 300, 60, new SecondaryBtn("Back"),
            (_scene, ui) =>
            {
                _scene.RemoveUI(controlsMenu);
                _scene.AddUI(startMenu);
            }
        );

        UI confirmMessage = new(new Vector2(screenCenter.X, screenCenter.Y - 100), new Paragraph("Are you sure you want to exit?"));

        UI yesBtn = new(new Vector2(screenCenter.X - 100, screenCenter.Y), 100, 60, new SecondaryBtn("Yes"),
            (_scene, ui) =>
            {
                Environment.Exit(0);
            });

        UI noBtn = new(new Vector2(screenCenter.X + 100, screenCenter.Y), 100, 60, new PrimaryBtn("No"),
            (_scene, ui) =>
            {
                _scene.RemoveUI(exitMenu);
                _scene.AddUI(startMenu);

            }
        );



        startMenu.AddRange(new UI[]{
            headerMain,
            startButton,
            controlsButton,
            exitButton
        });

        controlsMenu.AddRange(new UI[]{
            controlsBg,
            headerControls,
            w,
            s,
            a,
            d,
            lmb,
            backBtn
        });

        exitMenu.AddRange(new UI[]{
            confirmMessage,
            yesBtn,
            noBtn
        });

        _scene.AddUI(startMenu);
    }
}