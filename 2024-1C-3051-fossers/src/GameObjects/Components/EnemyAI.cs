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
    private float _chaseRange = 8000f;
    private float _attackRange = 3000f;
    private float _speed = 10000f;
    private bool _canMove = true;
    private int _delayBeforeMove = 1000;
    public bool _isReloading = false;
    public int _reloadingTimeInMs = 3000;
    public float _bulletForce = 36000;
    private float _maxForce = 5000;
    private GameObject _lastBullet;

    private Player _player;

    private DynamicBody _rb;
    private Transform _turretTransform;
    private Transform _cannonTransform;

    public EnemyAI(Transform turretTransform, Transform cannonTransform)
    {
        _turretTransform = turretTransform;
        _cannonTransform = cannonTransform;
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



        if (self.isDead) return;

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector3 directionToPlayer = _player.Transform.AbsolutePosition - self.Transform.AbsolutePosition;
        float distanceToPlayer = directionToPlayer.Length();
        directionToPlayer.Normalize();

        RotateTowardsPlayer(self, directionToPlayer);

        if (_canMove && distanceToPlayer > 0)
        {
            Timer.Timeout(_delayBeforeMove, () => _canMove = true);

            directionToPlayer.Normalize();




            if (distanceToPlayer <= _chaseRange && distanceToPlayer >= _attackRange && Vector3.Dot(self.Transform.Forward, _player.Transform.Forward) > 0.9)
            {
                RotateTowardsPlayer(self, directionToPlayer);
                RotateTurret(self, directionToPlayer);
                Vector3 desiredVelocity = self.Transform.Forward * _speed;
                Vector3 currentVelocity = _rb.Velocity;
                Vector3 force = desiredVelocity - currentVelocity;

                if (force.Length() > _maxForce)
                {
                    force.Normalize();
                    force *= -_maxForce;
                }

                _rb.ApplyForce(force);
            }
        }

        if (distanceToPlayer <= _attackRange)
        {
            RotateTurret(self, directionToPlayer);
            Shoot(scene, self);
        }
    }

    public void Shoot(Scene scene, Enemy self)
    {
        if (_isReloading) return;
        _lastBullet?.Destroy();
        GameObject bullet = CreateBullet(self, self.Damage);
        _lastBullet = bullet;
        scene.AddGameObject(bullet);

        // Apply force to the bullet in the direction towards the player
        bullet.GetComponent<DynamicBody>().ApplyForce(-_cannonTransform.Forward * _bulletForce);

        _isReloading = true;
        Timer.Timeout(_reloadingTimeInMs, () => _isReloading = false);
    }

    private GameObject CreateBullet(Enemy self, float damage)
    {
        GameObject bullet = new(new string[] { "bullet" }, new Transform(), ContentRepoManager.Instance().GetModel("Tanks/Bullet"), new Default(Color.Red));
        bullet.Transform.Position = _cannonTransform.AbsolutePosition - _cannonTransform.Forward * 500 + _cannonTransform.Up * 200;
        bullet.AddComponent(new DynamicBody(new Collider(new SphereShape(10), c =>
        {
            if (c.Entity.HasTag("player"))
            {
                _player.Health -= damage;
                bullet.Destroy();
            }
        }), Vector3.Zero, 5, 0, 0));
        bullet.AddComponent(new LightComponent(Color.White));
        return bullet;
    }

    private void RotateTurret(Enemy self, Vector3 direction)
    {
        Vector3 absPosition = _player.Transform.AbsolutePosition + _player.GetComponent<DynamicBody>().Velocity * 0.2f;
        absPosition.Y = self.Transform.AbsolutePosition.Y + 10;
        _turretTransform.LookAt(absPosition);
        _turretTransform.Orientation *= Quaternion.CreateFromAxisAngle(_turretTransform.Up, MathHelper.ToRadians(180));
    }

    private void RotateTowardsPlayer(Enemy self, Vector3 direction)
    {
        self.GetComponent<DynamicBody>().ApplyTorque(self.Transform.Up * 500000 * Vector3.Dot(direction - self.Transform.Forward, self.Transform.Right));
    }

    public void Destroy(GameObject self, Scene scene) { }
}