using System;
using Microsoft.Xna.Framework;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;
using WarSteel.Utils;

public class EnemyAI : IComponent
{
    private const float ChaseRange = 32000f;
    private const float AttackRange = 5000f;
    private const int DelayBeforeMove = 1000;
    private const int ReloadingTimeInMs = 3000;
    private const float BulletForce = 3600000 * 3;
    private const float BulletMass = 500 * 3;
    private const float MaxForce = 81000f;
    private const float MinVelocityToMove = 10;
    private const float TimeToDetectStuck = 1;
    private const int SetAsUnStuckDelay = 1000;
    private const float MinVelocityForForceApplication = 300;
    private const float RotationSpeed = 2.0f;
    private const float TorqueValue = 9000000;

    private bool _canMove = true;
    private bool _isReloading = false;
    private float _stuckTime = 0;
    private bool _stuckFlag = false;

    private GameObject _lastBullet;
    private Player _player;
    private DynamicBody _rb;
    private Transform _turretTransform;
    private Transform _cannonTransform;

    private enum State { Idle, Chase, Attack }
    private State _currentState;

    public EnemyAI(Transform turretTransform, Transform cannonTransform)
    {
        _turretTransform = turretTransform ?? throw new ArgumentNullException(nameof(turretTransform));
        _cannonTransform = cannonTransform ?? throw new ArgumentNullException(nameof(cannonTransform));
        _currentState = State.Idle;
    }

    public void OnStart(GameObject self, Scene scene)
    {
        _rb = self?.GetComponent<DynamicBody>() ?? throw new InvalidOperationException("Dynamic body not found in enemy.");
        _player = scene?.GetEntityByTag<Player>("player") ?? throw new InvalidOperationException("Player not found in the scene.");
    }

    public void HandleCollision()
    {
        _stuckFlag = true;
        Timer.Timeout(SetAsUnStuckDelay, () => _stuckFlag = false);
    }

    public void OnUpdate(GameObject self, GameTime gameTime, Scene scene)
    {
        if (self is not Enemy enemy || enemy.isDead) return;

        if (Vector3.Dot(enemy.Transform.Up, Vector3.Up) < 0.4f)
        {
            enemy.Health = 0;
            return;
        }

        var directionToPlayer = _player.Transform.AbsolutePosition - enemy.Transform.AbsolutePosition;
        var distanceToPlayer = directionToPlayer.Length();
        directionToPlayer.Normalize();

        switch (_currentState)
        {
            case State.Idle:
                IdleState(distanceToPlayer);
                break;
            case State.Chase:
                ChaseState(gameTime, enemy, distanceToPlayer);
                break;
            case State.Attack:
                AttackState(gameTime, scene, enemy, distanceToPlayer);
                break;
        }
    }

    private void IdleState(float distanceToPlayer)
    {
        if (distanceToPlayer <= ChaseRange)
        {
            _currentState = State.Chase;
        }
    }

    private void ChaseState(GameTime gameTime, Enemy self, float distanceToPlayer)
    {
        if (distanceToPlayer > AttackRange)
        {
            RotateTurret(self, gameTime);
            RotateBody(self);

            if (_stuckFlag)
            {
                _rb.ApplyForce(self.Transform.Forward * MaxForce);
                return;
            }
            else
            {
                _stuckTime = 0;
            }

            if (_rb.Velocity.Length() < MinVelocityToMove)
            {
                if (_stuckTime == 0)
                {
                    _stuckTime = gameTime.TotalGameTime.Seconds;
                }

                if (gameTime.TotalGameTime.Seconds - _stuckTime > TimeToDetectStuck)
                {
                    _stuckFlag = true;
                    Timer.Timeout(SetAsUnStuckDelay, () => _stuckFlag = false);
                }
            }


            if (_rb.Velocity.Length() < MinVelocityForForceApplication)
            {
                _rb.ApplyForce(-self.Transform.Forward * MaxForce);
            }
            else
            {
                _rb.ApplyForce(self.Transform.Forward * MaxForce);
            }
        }
        else
        {
            _currentState = State.Attack;
            _stuckTime = 0;
        }
    }

    private void AttackState(GameTime gameTime, Scene scene, Enemy self, float distanceToPlayer)
    {
        RotateTurret(self, gameTime);
        Shoot(scene, self);
        if (distanceToPlayer > AttackRange)
        {
            _currentState = State.Chase;
        }
    }

    public void Shoot(Scene scene, Enemy self)
    {
        if (_isReloading) return;

        _lastBullet?.Destroy();
        var bullet = CreateBullet(self.Damage, scene);
        _lastBullet = bullet;
        scene.AddGameObject(bullet);
        bullet.GetComponent<DynamicBody>().ApplyForce(-_cannonTransform.Forward * BulletForce);
        AudioManager.Instance.PlaySound(Audios.SHOOT);
        _isReloading = true;
        Timer.Timeout(ReloadingTimeInMs, () => _isReloading = false);
    }

    private GameObject CreateBullet(float damage, Scene scene)
    {
        var bullet = new GameObject(new[] { "bullet" }, new Transform(), ContentRepoManager.Instance().GetModel("Tanks/Bullet"), new Renderer(Color.Red))
        {
            Transform =
            {
                Position = _cannonTransform.AbsolutePosition - _cannonTransform.Forward * 500 + _cannonTransform.Up * 250
            }
        };

        bullet.AddComponent(new DynamicBody(new Collider(new SphereShape(10), c =>
        {
            if (c.Entity.HasTag("player") && !bullet.HasTag("HitGround"))
            {
                _player.Health -= damage;
                _player.Renderer.AddImpact(c.Entity.Transform.WorldToLocalPosition(bullet.Transform.AbsolutePosition));
                bullet.Destroy();
            }
            if (c.Entity.HasTag("ground"))
            {
                bullet.AddTag("HitGround");
            }
            bullet.RemoveComponent<LightComponent>(scene);
        }), Vector3.Zero, BulletMass, 0, 0));

        bullet.AddComponent(new LightComponent(Color.White));
        Timer.Timeout(3 * ReloadingTimeInMs, () => bullet.Destroy());

        return bullet;
    }

    private void RotateTurret(Enemy self, GameTime gameTime)
    {
        float angle = CalculateAngleToPlayer(self, _turretTransform);
        var targetOrientation = angle == 0 ? Quaternion.Identity : Quaternion.CreateFromAxisAngle(Vector3.Up, angle);
        _turretTransform.Orientation = Quaternion.Slerp(_turretTransform.Orientation, _turretTransform.Orientation * targetOrientation, RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    private void RotateBody(Enemy self)
    {
        float angle = CalculateAngleToPlayer(self, self.Transform);
        if (angle > 0)
        {
            _rb.ApplyTorque(Vector3.Up * TorqueValue);
        }
        else if (angle < 0)
        {
            _rb.ApplyTorque(-Vector3.Up * TorqueValue);
        }
    }

    private float CalculateAngleToPlayer(Enemy self, Transform transform)
    {
        var direction = _player.Transform.AbsolutePosition + _player.GetComponent<DynamicBody>().Velocity * 0.2f;
        direction.Y = self.Transform.AbsolutePosition.Y;
        direction = 2 * self.Transform.AbsolutePosition - direction;

        var forward = transform.Forward;
        forward.Y = 0;
        forward = Vector3.Normalize(forward);

        var targetDirection = Vector3.Normalize(direction - self.Transform.AbsolutePosition);
        targetDirection.Y = 0;
        targetDirection = Vector3.Normalize(targetDirection);

        var angle = (float)Math.Acos(Vector3.Dot(forward, targetDirection));
        if (float.IsNaN(angle)) return 0;

        var cross = Vector3.Cross(forward, targetDirection);
        if (cross.Y < 0)
        {
            angle = -angle;
        }

        return angle;
    }

    public void Destroy(GameObject self, Scene scene) { }
}
