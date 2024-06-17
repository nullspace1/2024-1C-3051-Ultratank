using Microsoft.Xna.Framework;
using WarSteel.Scenes;
using WarSteel.Utils;

public class EnemyHealthBar
{
    private Enemy _enemy;
    public HealthBar _healthBar;
    private Scene _scene;

    public EnemyHealthBar(Scene scene, Enemy enemy)
    {
        _enemy = enemy;
        _scene = scene;
        _healthBar = new HealthBar(scene, new Vector2(enemy.Transform.AbsolutePosition.X, enemy.Transform.AbsolutePosition.Y + 100), 30, 10);
    }

    public void CalculateHealthPos()
    {
        if (IsEnemyInView())
        {
            Vector2 offset = new Vector2(0, -20);
            Vector3 enemyPosition = new(_enemy.Transform.AbsolutePosition.X, _enemy.Transform.AbsolutePosition.Y, _enemy.Transform.AbsolutePosition.Z);
            Vector3 screenPosition = _scene.GraphicsDeviceManager.GraphicsDevice.Viewport.Project(enemyPosition, _scene.Camera.Projection, _scene.Camera.View, Matrix.Identity);
            Vector2 newPosition = new Vector2(screenPosition.X, screenPosition.Y) + offset;
            _healthBar.SetPosition(newPosition);
            _healthBar.SetVisibility(true);
        }
        else
        {
            _healthBar.SetVisibility(false);
        }
    }

    private bool IsEnemyInView()
    {
        Vector3 enemyPosition = _enemy.Transform.AbsolutePosition;
        Vector3 screenPosition = _scene.GraphicsDeviceManager.GraphicsDevice.Viewport.Project(enemyPosition, _scene.Camera.Projection, _scene.Camera.View, Matrix.Identity);
        int screenWidth = Screen.GetScreenWidth(_scene.GraphicsDeviceManager);
        int screenHeight = Screen.GetScreenHeight(_scene.GraphicsDeviceManager);
        bool isVisible = screenPosition.Z > 0
                         && screenPosition.X >= 0 && screenPosition.X <= screenWidth
                         && screenPosition.Y >= 0 && screenPosition.Y <= screenHeight;

        Vector3 cameraForward = _scene.Camera.Transform.Forward;
        Vector3 vectorToEnemy = enemyPosition - _scene.Camera.Transform.AbsolutePosition;
        float dotProduct = Vector3.Dot(cameraForward, vectorToEnemy);
        isVisible = isVisible && dotProduct > 0;

        return isVisible;
    }

    public void Remove()
    {
        _healthBar.Destroy();
    }
}