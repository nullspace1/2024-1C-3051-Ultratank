using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Utils;

namespace WarSteel.Scenes.Main;

public class PlayerControls : IComponent
{
    DynamicBody rb;
    float BulletForce = 36000;
    bool IsReloading = false;
    int ReloadingTimeInMs = 1000;
    private GameObject _lastBullet;
    private WheelsController _wheelsController;
    private float _forwardForce = 100000f;
    private float _torqueForce = 1000000f;

    Transform _tankCannon;

    public PlayerControls(Transform tankCannon)
    {
        _tankCannon = tankCannon;
        AudioManager.Instance.AddSoundEffect(Audios.SHOOT, ContentRepoManager.Instance().GetSoundEffect("tank-shot"));
    }


    public void OnStart(GameObject self, Scene scene)
    {
        rb = self.GetComponent<DynamicBody>();
        _wheelsController = self.GetComponent<WheelsController>();
    }

    public void OnUpdate(GameObject self, GameTime gameTime, Scene scene)
    {
        Transform wheel = _wheelsController.WheelTransform;
        bool isMoving = false;
        bool isRotating = false;

        if (Keyboard.GetState().IsKeyDown(Keys.W))
        {
            isMoving = true;
            _wheelsController.RotateForwards();
            rb.ApplyForce(Vector3.Normalize(wheel.Backward) * _forwardForce);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.S))
        {
            isMoving = true;
            _wheelsController.RotateBackwards();
            rb.ApplyForce(Vector3.Normalize(wheel.Forward) * _forwardForce);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            isRotating = true;
            _wheelsController.RotateLeft();
        }
        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            isRotating = true;
            _wheelsController.RotateRight();
        }

        if (isMoving)
            rb.ApplyTorque(Vector3.Normalize(self.Transform.Up) * _wheelsController.Angle * _torqueForce);
        if (!isRotating)
            _wheelsController.ResetWheels();

        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
        {
            Shoot(self, scene);
        }
    }

    public void Shoot(GameObject self, Scene scene)
    {
        if (IsReloading) return;
        _lastBullet?.Destroy();
        GameObject bullet = CreateBullet((Player)self);
        _lastBullet = bullet;
        AudioManager.Instance.PlaySound(Audios.SHOOT);
        scene.AddGameObject(bullet);
        bullet.GetComponent<DynamicBody>().ApplyForce(-_tankCannon.Forward * BulletForce);
        PlayerEvents.TriggerReload(ReloadingTimeInMs);
        IsReloading = true;
        Timer.Timeout(ReloadingTimeInMs, () => IsReloading = false);
    }

    public GameObject CreateBullet(Player self)
    {
        GameObject bullet = new(new string[] { "bullet" }, new Transform(), ContentRepoManager.Instance().GetModel("Tanks/Bullet"), new Default(Color.Red));
        bullet.AddComponent(new DynamicBody(new Collider(new SphereShape(10), c =>
        {
            if (c.Entity.HasTag("enemy"))
            {
                Enemy enemy = (Enemy)c.Entity;
                enemy.Health -= self.Damage;
                enemy.AddImpact(c.ImpactPoint);
                bullet.Destroy();
            }
        }), Vector3.Zero, 5, 0, 0));
        bullet.AddComponent(new LightComponent(Color.White));
        bullet.GetComponent<DynamicBody>().Velocity = self.GetComponent<DynamicBody>().Velocity;
        bullet.Transform.Position = _tankCannon.AbsolutePosition - _tankCannon.Forward * 1000 + _tankCannon.Up * 200;
        return bullet;
    }

    public void Destroy(GameObject self, Scene scene) { }
}