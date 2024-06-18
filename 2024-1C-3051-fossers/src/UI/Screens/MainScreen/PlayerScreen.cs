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

    private TextUI _dmgText;

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

        _dmgText = new Paragraph(GetDmgText(0));
        UI dmg = new(new(screenCenter.X, screenHeight - 120), _dmgText);

        // add ui elements
        _scene.AddUI(healthBarBG);
        _scene.AddUI(_healthBar);
        _scene.AddUI(dmg);

        PlayerEvents.SubscribeToReload(OnPlayerStartedReloading);
        PlayerEvents.SubscribeToHealthChanged(OnPlayerHealthUpdated);
        PlayerEvents.SubscribeToDamageChanged(OnPlayerDmgUpdated);
    }

    private string GetReloadingText()
    {
        return $"{_currentReloadTime:F2} s";
    }

    private void UpdateReloadingTimeText()
    {
        _currentReloadTime -= 10 / 1000f;
        if (_currentReloadTime <= 0 && _scene.HasUI(_reloadingTimeUI))
        {
            _reloadingTimeUI?.Destroy();
            return;
        }
        _reloadingTimeUIText.Text = GetReloadingText();
        Timer.Timeout(10, UpdateReloadingTimeText);
    }

    private void OnPlayerStartedReloading(int reloadingTimeInMs)
    {
        int screenHeight = Screen.GetScreenHeight(_scene.GraphicsDeviceManager);
        _reloadingTimeUI?.Destroy();
        _reloadingTime = reloadingTimeInMs / 1000;
        _currentReloadTime = _reloadingTime;
        _reloadingTimeUIText = new Paragraph(GetReloadingText());
        _reloadingTimeUI = new UI(new(screenCenter.X, screenHeight - 240), _reloadingTimeUIText);
        _scene.AddUI(_reloadingTimeUI);

        Timer.Timeout(10, UpdateReloadingTimeText);
    }

    private void OnPlayerHealthUpdated(float health)
    {
        float healthPercentage = health / 100f;
        int newHealthBarWidth = (int)(_healthBarWidth * healthPercentage);
        newHealthBarWidth = Math.Max(0, newHealthBarWidth);
        _healthBar.Width = newHealthBarWidth;
    }

    private string GetDmgText(float dmg)
    {
        return "Dmg: " + dmg;
    }

    private void OnPlayerDmgUpdated(float dmg)
    {
        _dmgText.Text = GetDmgText(dmg);
    }
}