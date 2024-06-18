using Microsoft.Xna.Framework;
using WarSteel.Common;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;

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
    public float Damage { get => _damage; }
    private EnemyHealthBar _healthBarUI;
    private WaveProcessor _wave;

    public Enemy(Vector3 pos, float damage, WaveProcessor wave) : base(new string[] { "enemy" }, new() { Position = pos }, ContentRepoManager.Instance().GetModel("Tanks/Panzer/Panzer"))
    {
        Transform turretTransform = new(Transform, Vector3.Zero);
        Transform cannonTransform = new(turretTransform, Vector3.Zero);

        RigidBody rb = new DynamicBody(new Collider(new BoxShape(200, 325, 450), (c) => { }), new Vector3(0, 100, 0), 200, 0.9f, 2f);
        EnemyAI ai = new(turretTransform, cannonTransform);

        _defaultRenderer = new TankRenderable(turretTransform, cannonTransform);
        _damage = damage;
        _wave = wave;

        AddComponent(rb);
        AddComponent(ai);
    }

    public override void Update(Scene scene, GameTime time)
    {
        base.Update(scene, time);
        _healthBarUI.CalculateHealthPos();
    }

    public override void Initialize(Scene scene)
    {
        base.Initialize(scene);
        _healthBarUI = new(scene, this);
    }

    private void OnDie()
    {
        _wave.EnemyDie();
        isDead = true;
    }

    public override void OnDestroy(Scene scene)
    {
        _healthBarUI.Remove();
        base.OnDestroy(scene);
    }
}