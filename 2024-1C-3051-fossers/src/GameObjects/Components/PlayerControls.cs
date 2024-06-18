using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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

    Transform _tankCannon;

    public PlayerControls(Transform tankCannon)
    {
        _tankCannon = tankCannon;
        AudioManager.Instance.AddSoundEffect(Audios.SHOOT, ContentRepoManager.Instance().GetSoundEffect("tank-shot"));
    }

    public void OnUpdate(GameObject self, GameTime gameTime, Scene scene)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.W))
        {
            // model is reversed
            rb.ApplyForce(self.Transform.Backward * 2 * 2000);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.S))
        {
            rb.ApplyForce(self.Transform.Forward * 2 * 2000);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            rb.ApplyTorque(self.Transform.Up * 15 * 32050f);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            rb.ApplyTorque(self.Transform.Down * 15 * 32050f);
        }
        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
        {
            Shoot(self, scene);
        }
    }

    public void Shoot(GameObject self, Scene scene)
    {
        if (IsReloading) return;

        GameObject bullet = CreateBullet((Player)self);
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
                ((Enemy)c.Entity).Health -= self.Damage;
                bullet.Destroy();
            }
        }), Vector3.Zero, 5, 0, 0));
        bullet.AddComponent(new LightComponent(Color.White));
        bullet.GetComponent<DynamicBody>().Velocity = self.GetComponent<DynamicBody>().Velocity;
        bullet.Transform.Position = _tankCannon.AbsolutePosition - _tankCannon.Forward * 500 + _tankCannon.Up * 200;
        return bullet;
    }

    public void OnStart(GameObject self, Scene scene)
    {
        rb = self.GetComponent<DynamicBody>();
    }

    public void Destroy(GameObject self, Scene scene) { }
}