using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;
using WarSteel.Utils;
using Quaternion = Microsoft.Xna.Framework.Quaternion;
using Vector3 = Microsoft.Xna.Framework.Vector3;

public class EnemyAI : IComponent
{
    private static readonly float ChaseDistance = 32000f;
    private static readonly float AttackDistance = 5000;
    private static readonly int ReloadDelayMs = 3000;
    private static readonly float BulletForce = 10800000f / 3;
    private static readonly float BulletMass = 500f;
    private static readonly float MaximumForce = 900f;
    private static readonly float MinimumVelocityToMove = 70f;
    private static readonly float StuckDetectionTime = 1f;
    private static readonly int UnstuckDelayMs = 1000;
    private static readonly float MinimumVelocityForForce = 750f;
    private static readonly float TurretRotationSpeed = 2.0f;
    private static readonly float TurretTorque = 9000000f;
    private static readonly int CollisionResolutionTimeMs = 2000;
    private static readonly float NearbyObjectDetectionAngle = 0.1f;  // Radians
    private static readonly float NearbyObjectDetectionDistance = 30f;

    private static readonly float MaxFrontVelocity = 100;

    private static readonly float MaxBackVelocity = 20;

    private bool _collided = false;
    private bool _isReloading = false;
    private float _stuckTime = 0;
    private bool _stuckFlag = false;
    private GameObject _lastBullet;
    private Player _player;
    private DynamicBody _rb;
    private Transform _turretTransform;
    private Transform _cannonTransform;
    private List<GameObject> _nearbyObjects;

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
        _collided = true;
        Timer.Timeout(CollisionResolutionTimeMs, () => _collided = false);
    }

    public void OnUpdate(GameObject self, GameTime gameTime, Scene scene)
    {
        if (self is not Enemy enemy || enemy.isDead) return;

        if (_nearbyObjects == null)
        {
            _nearbyObjects = scene.GetEntitiesByTag("ground");
        }

        if (Vector3.Dot(enemy.Transform.Up, Vector3.Up) < 0.4f)
        {
            enemy.Health -= 0.3f;
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
        if (distanceToPlayer <= ChaseDistance)
        {
            _currentState = State.Chase;
        }
    }

    private void ChaseState(GameTime gameTime, Enemy self, float distanceToPlayer)
    {

        foreach (var obj in _nearbyObjects)
        {
            var toObject = obj.Transform.AbsolutePosition - self.Transform.AbsolutePosition;
            float angle = Vector3.Dot(toObject, self.Transform.Forward) / (toObject.Length() * self.Transform.Forward.Length());
            angle = (float)Math.Acos(angle);
            float distance = toObject.Length();

            if (angle < NearbyObjectDetectionAngle && distance < NearbyObjectDetectionDistance)
            {
                _rb.ApplyForce(self.Transform.Forward * MaximumForce);
                return;
            }
        }


        if (_collided)
        {
            _rb.ApplyForce(self.Transform.Forward * MaximumForce * 70);
            return;
        }

        if (distanceToPlayer > AttackDistance)
        {
            RotateTurret(self, gameTime);
            RotateBody(self);

            if (_stuckFlag)
            {
                _rb.ApplyForce(self.Transform.Forward * MaximumForce * 70);
                return;
            }
            else
            {
                _stuckTime = 0;
            }

            if (_rb.Velocity.Length() < MinimumVelocityToMove)
            {
                if (_stuckTime == 0)
                {
                    _stuckTime = gameTime.TotalGameTime.Seconds;
                }

                if (gameTime.TotalGameTime.Seconds - _stuckTime > StuckDetectionTime)
                {
                    _stuckFlag = true;
                    Timer.Timeout(UnstuckDelayMs, () => _stuckFlag = false);
                }
            }

            _rb.ApplyForce(MaximumForce * MathHelper.Clamp((Vector3.Dot(_rb.Velocity,self.Transform.Forward) - MaxFrontVelocity),MaxFrontVelocity,-MaxBackVelocity) *  -self.Transform.Forward);


        }
        else
        {
            _currentState = State.Attack;
        }
    }

    private void AttackState(GameTime gameTime, Scene scene, Enemy self, float distanceToPlayer)
    {
        RotateTurret(self, gameTime);
        Shoot(scene, self);
        if (distanceToPlayer > AttackDistance)
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
        Timer.Timeout(ReloadDelayMs, () => _isReloading = false);
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
                _player.Model.AddImpact(bullet.Transform.AbsolutePosition, bullet.GetComponent<DynamicBody>().Velocity);
                bullet.Destroy();
            }
            if (c.Entity.HasTag("ground"))
            {
                bullet.AddTag("HitGround");
            }
            bullet.RemoveComponent<LightComponent>(scene);
        }), Vector3.Zero, BulletMass, 0, 0));

        bullet.AddComponent(new LightComponent(Color.White));
        Timer.Timeout(3 * ReloadDelayMs, () => bullet.Destroy());

        return bullet;
    }

    private void RotateTurret(Enemy self, GameTime gameTime)
    {
        float angle = CalculateAngleToPlayer(self, _turretTransform);
        var targetOrientation = angle == 0 ? Quaternion.Identity : Quaternion.CreateFromAxisAngle(Vector3.Up, angle);
        _turretTransform.Orientation = Quaternion.Slerp(_turretTransform.Orientation, _turretTransform.Orientation * targetOrientation, TurretRotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    private void RotateBody(Enemy self)
    {
        float angle = CalculateAngleToPlayer(self, self.Transform);
        if (angle > 0)
        {
            _rb.ApplyTorque(Vector3.Up * TurretTorque);
        }
        else if (angle < 0)
        {
            _rb.ApplyTorque(-Vector3.Up * TurretTorque);
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
