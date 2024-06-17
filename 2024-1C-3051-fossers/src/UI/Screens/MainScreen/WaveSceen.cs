using System;
using Microsoft.Xna.Framework;
using WarSteel.Scenes;
using WarSteel.UIKit;
using WarSteel.Utils;

public class WaveInfoScreen
{
    private Scene _scene;
    private TextUI _waveText;
    private TextUI _enemiesLeftText;
    private TextUI _waveCounterText;
    private UI _waveCounterUI;
    private UI _waveStartsInUI;

    public WaveInfoScreen(Scene scene)
    {
        _scene = scene;
    }

    public void Initialize()
    {
        _waveText = new Paragraph(GetWaveText(0));
        UI waveUI = new UI(new(75, 50), _waveText);

        _enemiesLeftText = new Paragraph(GetEnemiesLeftText(0));
        UI enemiesLeftUI = new UI(new(125, 90), _enemiesLeftText);

        _scene.AddUI(waveUI);
        _scene.AddUI(enemiesLeftUI);

        WaveEvents.SubscribeToNewWave(OnNewWave);
        WaveEvents.SubscribeToEnemiesLeft(OnEnemiesLeft);
    }


    private string GetWaveText(int waveNumber)
    {
        return "Wave: " + waveNumber;
    }

    private string GetEnemiesLeftText(int enemiesLeft)
    {
        return "Enemies Left: " + enemiesLeft;
    }

    public void OnNewWave(int waveNumber)
    {
        _waveText.Text = GetWaveText(waveNumber);
    }

    private void RemoveCounterUI()
    {
        _scene.RemoveUI(_waveCounterUI);
        _scene.RemoveUI(_waveStartsInUI);
    }

    private void UpdateWaveCounter()
    {
        int count = Int32.Parse(_waveCounterText.Text) - 1;
        _waveCounterText.Text = count.ToString();
        if (count >= 0)
            Timer.Timeout(1000, UpdateWaveCounter);
        else
            Timer.Timeout(1000, RemoveCounterUI);
    }

    public void ShowNewWaveCounter()
    {
        GraphicsDeviceManager GraphicsDeviceManager = _scene.GraphicsDeviceManager;
        Vector2 screenCenter = Screen.GetScreenCenter(GraphicsDeviceManager);
        int screenWidth = Screen.GetScreenWidth(GraphicsDeviceManager);
        int screenHeight = Screen.GetScreenHeight(GraphicsDeviceManager);

        _waveCounterText = new Header("3");
        _waveStartsInUI = new UI(new(screenCenter.X, screenCenter.Y - 25), new Paragraph("Next wave starts in:"));
        _waveCounterUI = new UI(new(screenCenter.X, screenCenter.Y + 25), _waveCounterText);

        _scene.AddUI(_waveStartsInUI);
        _scene.AddUI(_waveCounterUI);

        Timer.Timeout(1000, UpdateWaveCounter);
    }

    public void OnEnemiesLeft(int enemiesLeft)
    {
        _enemiesLeftText.Text = GetEnemiesLeftText(enemiesLeft);
        if (enemiesLeft == 0)
        {
            ShowNewWaveCounter();
        }
    }
}