using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Utils;

namespace WarSteel.Scenes.Main
{
    public class PlayerControls : IComponent
    {
        private const int ReloadingTimeInMs = 1000;
        private const float ForwardForce = 100000f;
        private const float TorqueForce = 1000000f;


        private const float BulletForce = 3600000 * 3;
        private const float BulletMass = 500 * 3;
        private const float FlipTimeThreshold = 10f;
        private const float HealthReductionOnFlip = 1f;
        private const float CanMoveAngleThreshold = 0.5f;
        private const float FlipAngleThreshold = 0.6f;
        private const float BulletPositionOffsetForward = 1000f;
        private const float BulletPositionOffsetUp = 200f;

        private DynamicBody _rb;
        private bool _isReloading = false;
        private GameObject _lastBullet;
        private WheelsController _wheelsController;
        private float _startFlipTime = 0;
        private bool _canMove = false;
        private Transform _tankCannon;

        public PlayerControls(Transform tankCannon)
        {
            _tankCannon = tankCannon;
            AudioManager.Instance.AddSoundEffect(Audios.SHOOT, ContentRepoManager.Instance().GetSoundEffect("tank-shot"));
        }

        public void OnStart(GameObject self, Scene scene)
        {
            _rb = self.GetComponent<DynamicBody>();
            _wheelsController = self.GetComponent<WheelsController>();
        }

        public void OnUpdate(GameObject self, GameTime gameTime, Scene scene)
        {
            var wheel = _wheelsController.WheelTransform;
            bool isMoving = false;
            bool isRotating = false;

            if (Vector3.Dot(self.Transform.Up, Vector3.Up) < FlipAngleThreshold)
            {
                if (_startFlipTime == 0)
                {
                    _startFlipTime = gameTime.TotalGameTime.Seconds;
                }

                if (gameTime.TotalGameTime.Seconds - _startFlipTime > FlipTimeThreshold)
                {
                    ((Player)self).Health -= HealthReductionOnFlip;
                }
            }
            else
            {
                _startFlipTime = 0;
            }

            _canMove = Vector3.Dot(self.Transform.Up, Vector3.Up) >= CanMoveAngleThreshold && ((Player)self).touchingGround;

            if (Keyboard.GetState().IsKeyDown(Keys.W) && _canMove)
            {
                isMoving = true;
                _wheelsController.RotateForwards();
                _rb.ApplyForce(Vector3.Normalize(wheel.Backward) * ForwardForce);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S) && _canMove)
            {
                isMoving = true;
                _wheelsController.RotateBackwards();
                _rb.ApplyForce(Vector3.Normalize(wheel.Forward) * ForwardForce);
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
            {
                _rb.ApplyTorque(Vector3.Normalize(self.Transform.Up) * _wheelsController.Angle * TorqueForce);
            }
            if (!isRotating)
            {
                _wheelsController.ResetWheels();
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Shoot(self, scene);
            }

            if (self.GetComponent<DynamicBody>().Velocity.Y > 0) ((Player)self).touchingGround = false;
        }

        public void Shoot(GameObject self, Scene scene)
        {
            if (_isReloading) return;

            _lastBullet?.Destroy();
            var bullet = CreateBullet((Player)self, scene);
            _lastBullet = bullet;
            AudioManager.Instance.PlaySound(Audios.SHOOT);
            scene.AddGameObject(bullet);
            bullet.GetComponent<DynamicBody>().ApplyForce(-_tankCannon.Forward * BulletForce);
            PlayerEvents.TriggerReload(ReloadingTimeInMs);
            _isReloading = true;
            Timer.Timeout(ReloadingTimeInMs, () => _isReloading = false);
        }

        public GameObject CreateBullet(Player self, Scene scene)
        {
            var bullet = new GameObject(new[] { "bullet" }, new Transform(), ContentRepoManager.Instance().GetModel("Tanks/Bullet"), new Renderer(Color.Red));
            bullet.AddComponent(new DynamicBody(new Collider(new SphereShape(10), c =>
        {
            if (c.Entity.HasTag("enemy") && !bullet.HasTag("HitGround"))
            {
                var enemy = (Enemy)c.Entity;
                enemy.Health -= self.Damage;
                enemy.Renderer.AddImpact(c.Entity.Transform.WorldToLocalPosition(bullet.Transform.AbsolutePosition));
                bullet.Destroy();
            }
            if (c.Entity.HasTag("ground"))
            {
                bullet.AddTag("HitGround");
            }
            bullet.RemoveComponent<LightComponent>(scene);
        }), Vector3.Zero, BulletMass, 0, 0));
            bullet.AddComponent(new LightComponent(Color.Blue));
            bullet.GetComponent<DynamicBody>().Velocity = self.GetComponent<DynamicBody>().Velocity;
            bullet.Transform.Position = _tankCannon.AbsolutePosition - _tankCannon.Forward * BulletPositionOffsetForward + _tankCannon.Up * BulletPositionOffsetUp;
            return bullet;
        }

        public void Destroy(GameObject self, Scene scene) { }
    }
}
