using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WarSteel.Scenes;
using WarSteel.UIKit;
using WarSteel.Utils;

public class LooseScreen
{
    Scene _scene;

    public LooseScreen(Scene scene)
    {
        _scene = scene;
    }

    public void Initialize(int enemiesKilled, int wavesSurvived, float finalScore)
    {
        GraphicsDeviceManager GraphicsDeviceManager = _scene.GraphicsDeviceManager;
        Vector2 screenCenter = Screen.GetScreenCenter(GraphicsDeviceManager);
        int screenWidth = Screen.GetScreenWidth(GraphicsDeviceManager);
        int screenHeight = Screen.GetScreenHeight(GraphicsDeviceManager);

        List<UI> uis = new();

        Vector2 GetControlPos(float pos, Vector2 screenCenter)
        {
            float margin = 50;
            return new Vector2(screenCenter.X, screenCenter.Y - 2 * margin + margin * pos);
        }

        UI bg = new UI(screenCenter, 500, 350, new Image("UI/primary-btn"));

        UI header = new(new Vector2(screenCenter.X, 100), new Header("You died"));

        UI enemiesKilledUi = new(GetControlPos(0, screenCenter), new Paragraph("Enemies Killed:"));
        UI enemiesKilledNumber = new(GetControlPos(0.8f, screenCenter), new Paragraph(enemiesKilled.ToString()));

        UI wavesSurviveUi = new(GetControlPos(2, screenCenter), new Paragraph("Waves survived:"));
        UI wavesSurvivedNumber = new(GetControlPos(2.8f, screenCenter), new Paragraph(wavesSurvived.ToString()));

        UI score = new(GetControlPos(4, screenCenter), new Paragraph("Final score:"));
        UI scoreNumber = new(GetControlPos(4.8f, screenCenter), new Paragraph(finalScore.ToString()));

        UI restartBtn = new(new Vector2(screenCenter.X + 100, screenHeight - 100), 300, 60, new SecondaryBtn("Restart"),
            (_scene, ui) =>
            {
                SceneManager.Instance().RestartScene();
            }
        );

        UI startMenuBtn = new(new Vector2(screenCenter.X - 175, screenHeight - 100), 150, 60, new TertiaryBtn("Menu"),
                    (_scene, ui) =>
                    {
                        SceneManager.Instance().SetCurrentScene(ScenesNames.MENU);
                    }
                );


        uis.AddRange(new UI[]{
            bg,
            header,
            enemiesKilledUi,
            enemiesKilledNumber,
            wavesSurviveUi,
            wavesSurvivedNumber,
            score,
            scoreNumber,
            restartBtn,
            startMenuBtn
        });

        _scene.AddUI(uis);
    }
}