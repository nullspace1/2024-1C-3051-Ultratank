
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
    private float _delayBeforeMove = 1000f;
    private float _currentDelay = 0;
    public bool _isReloading = false;
    public int _reloadingTimeInMs = 3000;
    public float _bulletForce = 36000;

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

        if (distanceToPlayer > 0)
        {
            if (!_canMove)
            {
                _currentDelay += deltaTime;
                if (_currentDelay >= _delayBeforeMove)
                {
                    _canMove = true;
                }
            }

            directionToPlayer.Normalize();

            if (distanceToPlayer <= _chaseRange && distanceToPlayer >= _attackRange)
            {
                _rb.ApplyForce(directionToPlayer * _speed);
            }

            if (distanceToPlayer <= _attackRange)
            {
                RotateTurretAndCannon(directionToPlayer);
                Shoot(scene, self);
            }
        }
    }

    public void Shoot(Scene scene, Enemy self)
    {
        if (_isReloading) return;

        GameObject bullet = CreateBullet(self.Damage);
        scene.AddGameObject(bullet);

        // Calculate direction towards the player
        Vector3 directionToPlayer = _player.Transform.AbsolutePosition - self.Transform.AbsolutePosition;
        directionToPlayer.Normalize();

        // Apply force to the bullet in the direction towards the player
        bullet.GetComponent<DynamicBody>().ApplyForce(-_cannonTransform.Forward * _bulletForce);

        _isReloading = true;
        Timer.Timeout(_reloadingTimeInMs, () => _isReloading = false);
    }

    private GameObject CreateBullet(float damage)
    {
        GameObject bullet = new GameObject(new string[] { "bullet" }, new Transform(), ContentRepoManager.Instance().GetModel("Tanks/Bullet"), new Default(Color.Red));
        bullet.AddComponent(new DynamicBody(new Collider(new SphereShape(10), c =>
        {
            if (c.Entity.HasTag("player")) _player.Health -= damage;
            bullet.Destroy();
        }), Vector3.Zero, 5, 0, 0));
        bullet.AddComponent(new LightComponent(Color.White));
        bullet.Transform.Position = _cannonTransform.AbsolutePosition + _cannonTransform.Forward * 500 + _cannonTransform.Up * 200;
        return bullet;
    }

    private void RotateTurretAndCannon(Vector3 direction)
    {
        _turretTransform.Orientation = Quaternion.CreateFromRotationMatrix(Matrix.CreateWorld(Vector3.Zero, direction, Vector3.UnitY));
        _cannonTransform.Orientation = Quaternion.CreateFromRotationMatrix(Matrix.CreateWorld(Vector3.Zero, direction, Vector3.UnitY));
    }

    public void Destroy(GameObject self, Scene scene) { }
}