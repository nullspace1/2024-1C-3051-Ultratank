using System;
using Microsoft.Xna.Framework;
using WarSteel.Scenes;
using WarSteel.UIKit;

public class HealthBar
{
    private UI _healthBar;
    private UI _healthBarBg;
    private int _healthBarWidth;
    Scene _scene;

    public UI UI
    {
        get => _healthBar;
    }

    public HealthBar(Scene scene, Vector2 pos, int width, int height)
    {
        _scene = scene;
        _healthBarWidth = width;
        _healthBarBg = new(pos, width, height, new Image("UI/health-bar-bg"));
        _healthBar = new(pos, width, height, new Image("UI/health-bar-fill"));

        scene.AddUI(_healthBarBg);
        scene.AddUI(_healthBar);
    }

    public void SetHealth(float health)
    {
        float healthPercentage = health / 100f;
        int newHealthBarWidth = (int)(_healthBarWidth * healthPercentage);
        newHealthBarWidth = Math.Max(0, newHealthBarWidth);
        _healthBar.Width = newHealthBarWidth;
    }

    public void SetPosition(Vector2 pos)
    {
        _healthBarBg.Position = pos;
        _healthBar.Position = pos;
    }

    public void SetVisibility(bool visible)
    {
        _healthBarBg.Visible = visible;
        _healthBar.Visible = visible;
    }

    public void Destroy()
    {
        _healthBar.Destroy();
        _healthBarBg.Destroy();
        _scene.RemoveUI(_healthBarBg);
        _scene.RemoveUI(_healthBar);
    }
}