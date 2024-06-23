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
    private float _chaseRange = 32000f;
    private float _attackRange = 5000f;
    private bool _canMove = true;
    private int _delayBeforeMove = 1000;
    public bool _isReloading = false;
    public int _reloadingTimeInMs = 3000;
    public float _bulletForce = 36000;
    private float _maxForce = 9000f * 9;
    private GameObject _lastBullet;
    private Player _player;
    private DynamicBody _rb;
    private Transform _turretTransform;
    private Transform _cannonTransform;

    private float _stuckTime = 0;
    private bool _flag = true;

    private enum State { Idle, Chase, Attack, Retreat }
    private State _currentState;

    public EnemyAI(Transform turretTransform, Transform cannonTransform)
    {
        _turretTransform = turretTransform;
        _cannonTransform = cannonTransform;
        _currentState = State.Idle;
    }

    public void OnStart(GameObject self, Scene scene)
    {
        _rb = self.GetComponent<DynamicBody>() ?? throw new Exception("Dynamic body not found in enemy.");
        Player player = scene.GetEntityByTag<Player>("player") ?? throw new Exception("Player not found in the scene.");
        _player = player;
    }

    public void OnUpdate(GameObject _self, GameTime gameTime, Scene scene)
    {
        Enemy self = (Enemy)_self;

        if (Vector3.Dot(self.Transform.Up, Vector3.Up) < 0.4f)
        {
            self.Health = 0;
            return;
        }

        if (self.isDead) return;

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector3 directionToPlayer = _player.Transform.AbsolutePosition - self.Transform.AbsolutePosition;
        float distanceToPlayer = directionToPlayer.Length();
        directionToPlayer.Normalize();

        switch (_currentState)
        {
            case State.Idle:
                if (distanceToPlayer <= _chaseRange)
                {
                    _currentState = State.Chase;
                }
                break;

            case State.Chase:
                if (_canMove && distanceToPlayer > _attackRange)
                {
                    Timer.Timeout(_delayBeforeMove, () => _canMove = true);
                    RotateTurret(self, gameTime);
                    RotateBody(self);

                    if (_rb.Velocity.Length() < 100 && _flag)
                    {
                        if (_stuckTime == 0)
                        {
                            _stuckTime = gameTime.TotalGameTime.Seconds;
                        }

                        if (gameTime.TotalGameTime.Seconds - _stuckTime > 1)
                        {
                            _rb.ApplyForce(self.Transform.Forward * _maxForce * 20);
                            _flag = false;
                            Timer.Timeout(300,() => _flag = true);
                        }
                    }
                    else {
                        _stuckTime = 0;
                    }

                    if (_rb.Velocity.Length() < 300)
                    {
                        _rb.ApplyForce(-self.Transform.Forward * _maxForce);
                    }
                    else
                    {
                        _rb.ApplyForce(self.Transform.Forward * _maxForce);
                    }

                }
                if (distanceToPlayer <= _attackRange)
                {
                    _currentState = State.Attack;
                }
                break;

            case State.Attack:
                RotateTurret(self, gameTime);
                Shoot(scene, self);
                if (distanceToPlayer > _attackRange)
                {
                    _currentState = State.Chase;
                }
                break;

            case State.Retreat:
                // Implement retreat logic if necessary
                break;
        }
    }

    public void Shoot(Scene scene, Enemy self)
    {
        if (_isReloading) return;
        _lastBullet?.Destroy();
        GameObject bullet = CreateBullet(self.Damage);
        _lastBullet = bullet;
        scene.AddGameObject(bullet);

        // Apply force to the bullet in the direction towards the player
        bullet.GetComponent<DynamicBody>().ApplyForce(-_cannonTransform.Forward * _bulletForce);

        _isReloading = true;
        Timer.Timeout(_reloadingTimeInMs, () => _isReloading = false);
    }

    private GameObject CreateBullet(float damage)
    {
        GameObject bullet = new(new string[] { "bullet" }, new Transform(), ContentRepoManager.Instance().GetModel("Tanks/Bullet"), new Renderer(Color.Red));
        bullet.Transform.Position = _cannonTransform.AbsolutePosition - _cannonTransform.Forward * 500 + _cannonTransform.Up * 200;
        bullet.AddComponent(new DynamicBody(new Collider(new SphereShape(10), c =>
        {
            if (c.Entity.HasTag("player"))
            {
                _player.Health -= damage;
                bullet.Destroy();
                _player.Renderer.AddImpact(c.Entity.Transform.WorldToLocalPosition(bullet.Transform.AbsolutePosition));
            }
        }), Vector3.Zero, 5, 0, 0));
        bullet.AddComponent(new LightComponent(Color.White));
        Timer.Timeout(3 * _reloadingTimeInMs, () => bullet.Destroy());
        return bullet;
    }

    private void RotateTurret(Enemy self, GameTime gameTime)
    {
        float angle = CalculateAngle(self, _turretTransform);
        float rotationSpeed = 2.0f;
        Quaternion targetOrientation = angle == 0 ? Quaternion.Identity : Quaternion.CreateFromAxisAngle(Vector3.Up, angle);
        _turretTransform.Orientation = Quaternion.Slerp(_turretTransform.Orientation, _turretTransform.Orientation * targetOrientation, rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
    }

    private void RotateBody(Enemy self)
    {
        float angle = CalculateAngle(self, self.Transform);
        if (angle > 0)
        {
            _rb.ApplyTorque(Vector3.Up * 9000000);
        }
        else if (angle < 0)
        {
            _rb.ApplyTorque(-Vector3.Up * 9000000);
        }

    }

    private float CalculateAngle(Enemy self, Transform transform)
    {
        Vector3 direction = _player.Transform.AbsolutePosition + _player.GetComponent<DynamicBody>().Velocity * 0.2f;
        direction.Y = self.Transform.AbsolutePosition.Y;
        direction = 2 * self.Transform.AbsolutePosition - direction;

        Vector3 forward = transform.Forward;
        forward.Y = 0;
        forward = Vector3.Normalize(forward);

        Vector3 targetDirection = Vector3.Normalize(direction - self.Transform.AbsolutePosition);
        targetDirection.Y = 0;
        targetDirection = Vector3.Normalize(targetDirection);

        float angle = (float)Math.Acos(Vector3.Dot(forward, targetDirection));

        if (float.IsNaN(angle)) return 0;

        Vector3 cross = Vector3.Cross(forward, targetDirection);
        if (cross.Y < 0)
        {
            angle = -angle;
        }

        return angle;
    }

    public void Destroy(GameObject self, Scene scene) { }
}
