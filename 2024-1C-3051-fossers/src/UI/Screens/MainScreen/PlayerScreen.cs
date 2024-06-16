using System;
using Microsoft.Xna.Framework;
using WarSteel.Scenes;
using WarSteel.UIKit;
using WarSteel.Utils;

public class PlayerScreen
{
    private UI _healthBar;
    private int _healthBarWidth = 600;

    private UI _reloadingTimeUI;
    private TextUI _reloadingTimeUIText;
    private float _reloadingTime;
    private float _currentReloadTime;
    Vector2 screenCenter;
    private Scene _scene;

    public PlayerScreen(Scene scene)
    {
        _scene = scene;
    }

    public void Initialize()
    {
        GraphicsDeviceManager GraphicsDeviceManager = _scene.GraphicsDeviceManager;
        screenCenter = Screen.GetScreenCenter(GraphicsDeviceManager);
        int screenWidth = Screen.GetScreenWidth(GraphicsDeviceManager);
        int screenHeight = Screen.GetScreenHeight(GraphicsDeviceManager);

        Vector2 healthBarPos = new(screenCenter.X, screenHeight - 60);
        UI healthBarBG = new(healthBarPos, _healthBarWidth, 30, new Image("UI/health-bar-bg"));
        _healthBar = new(healthBarPos, _healthBarWidth, 30, new Image("UI/health-bar-fill"));

        // add ui elements
        _scene.AddUI(healthBarBG);
        _scene.AddUI(_healthBar);

        // subscribe to player events and update the ui accordingly
        PlayerEvents.SubscribeToReload(OnPlayerStartedReloading);
        PlayerEvents.SubscribeToHealthChanged(OnPlayerHealthUpdated);
    }

    private string GetReloadingText()
    {
        return $"{_currentReloadTime:F2} s";
    }

    private void UpdateReloadingTimeText()
    {
        _currentReloadTime -= 10 / 1000f;
        if (_currentReloadTime <= 0)
        {
            _scene.RemoveUI(_reloadingTimeUI);
            return;
        }
        _reloadingTimeUIText.Text = GetReloadingText();
        Timer.Timeout(10, UpdateReloadingTimeText);
    }

    private void OnPlayerStartedReloading(int reloadingTimeInMs)
    {
        _scene.RemoveUI(_reloadingTimeUI);
        _reloadingTime = reloadingTimeInMs / 1000;
        _currentReloadTime = _reloadingTime;
        _reloadingTimeUIText = new Paragraph(GetReloadingText());
        _reloadingTimeUI = new UI(new(screenCenter.X, screenCenter.Y), _reloadingTimeUIText);
        _scene.AddUI(_reloadingTimeUI);

        Timer.Timeout(10, UpdateReloadingTimeText);
    }

    private void OnPlayerHealthUpdated(float health)
    {
        // Calculate the width of the health bar based on the player's health percentage
        float healthPercentage = health / 100f; // Calculate health percentage
        int newHealthBarWidth = (int)(_healthBarWidth * healthPercentage); // Calculate new width

        // Ensure the width does not go below 0
        newHealthBarWidth = Math.Max(0, newHealthBarWidth);

        // Update the health bar width
        _healthBar.Width = newHealthBarWidth;
    }
}