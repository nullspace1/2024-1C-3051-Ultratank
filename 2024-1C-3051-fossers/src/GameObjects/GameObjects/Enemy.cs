using System;
using Microsoft.Xna.Framework;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;
using WarSteel.Utils;

public class Enemy : GameObject
{
    public bool isDead = false;
    private float _health = 100;
    public float Health
    {
        get => _health;
        set
        {
            _health = value;
            _healthBarUI._healthBar.SetHealth(value);
            if (_health <= 0 && !isDead) OnDie();
        }
    }
    private float _damage;
    private Player _player;
    private float _chaseRange = 8000f;
    private float _attackRange = 3000f;
    private float _speed = 10000f;
    private bool _canMove = true;
    private float _delayBeforeMove = 1000f;
    private float _currentDelay = 0;
    private DynamicBody _rb;
    private Transform _turretTransform;
    private Transform _cannonTransform;
    public bool _isReloading = false;
    public int _reloadingTimeInMs = 3000;
    public float _bulletForce = 36000;
    private Scene _scene;
    private EnemyHealthBar _healthBarUI;

    public Enemy(Vector3 pos, float damage, Player player) : base(new string[] { "enemy" }, new() { Position = pos }, ContentRepoManager.Instance().GetModel("Tanks/Panzer/Panzer"))
    {
        _turretTransform = new(Transform, Vector3.Zero);
        _cannonTransform = new(_turretTransform, Vector3.Zero);
        _defaultRenderer = new TankRenderable(_turretTransform, _cannonTransform);
        _damage = damage;
        _player = player;

        _rb = new DynamicBody(new Collider(new BoxShape(200, 325, 450), (c) => { }), new Vector3(0, 100, 0), 200, 0.9f, 2f);
        AddComponent(_rb);
    }

    public override void Initialize(Scene scene)
    {
        base.Initialize(scene);
        _healthBarUI = new(scene, this);
        _scene = scene;
    }

    public override void Update(Scene scene, GameTime gameTime)
    {
        base.Update(scene, gameTime);
        _healthBarUI.CalculateHealthPos();

        if (isDead) return;

        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector3 directionToPlayer = _player.Transform.AbsolutePosition - Transform.AbsolutePosition;
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
                Shoot(scene);
            }
        }
    }

    public void Shoot(Scene scene)
    {
        if (_isReloading) return;

        GameObject bullet = CreateBullet();
        scene.AddGameObject(bullet);

        // Calculate direction towards the player
        Vector3 directionToPlayer = _player.Transform.AbsolutePosition - Transform.AbsolutePosition;
        directionToPlayer.Normalize();

        // Apply force to the bullet in the direction towards the player
        bullet.GetComponent<DynamicBody>().ApplyForce(-_cannonTransform.Forward * _bulletForce);

        _isReloading = true;
        Timer.Timeout(_reloadingTimeInMs, () => _isReloading = false);
    }

    private GameObject CreateBullet()
    {
        GameObject bullet = new GameObject(new string[] { "bullet" }, new Transform(), ContentRepoManager.Instance().GetModel("Tanks/Bullet"), new Default(0.9f, 0.1f, Color.Red));
        bullet.AddComponent(new DynamicBody(new Collider(new SphereShape(10), c =>
        {
            if (c.Entity.HasTag("player")) _player.Health -= _damage;
        }), Vector3.Zero, 5, 0, 0));
        bullet.AddComponent(new LightComponent(Color.White, Vector3.Zero));
        bullet.Transform.Position = _cannonTransform.AbsolutePosition + _cannonTransform.Forward * 500 + _cannonTransform.Up * 200;
        return bullet;
    }

    private void RotateTurretAndCannon(Vector3 direction)
    {
        _turretTransform.Orientation = Quaternion.CreateFromRotationMatrix(Matrix.CreateWorld(Vector3.Zero, direction, Vector3.UnitY));
        _cannonTransform.Orientation = Quaternion.CreateFromRotationMatrix(Matrix.CreateWorld(Vector3.Zero, direction, Vector3.UnitY));
    }

    private void OnDie()
    {
        _scene.GetSceneProcessor<WaveProcessor>().EnemiesLeft -= 1;
        isDead = true;
    }

    public override void OnDestroy(Scene scene)
    {
        _healthBarUI.Remove();
        base.OnDestroy(scene);
    }
}