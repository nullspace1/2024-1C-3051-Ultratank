using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Entities;
using WarSteel.Managers;

public class Enemy : GameObject
{
    private EnemyHealthBar _healthBar;
    private WaveProcessor _wave;
    private List<Vector3> _impacts;

    public bool isDead = false;
    private float _health = 100;
    public float Health
    {
        get => _health;
        set
        {
            _health = value;
            _healthBar.SetHealth(value);
            if (_health <= 0 && !isDead) OnDie();
        }
    }
    private float _damage;
    public float Damage { get => _damage; }

    public Enemy(Vector3 pos, float damage, WaveProcessor wave) : base(new string[] { "enemy" }, new() { Position = pos }, ContentRepoManager.Instance().GetModel("Tanks/Panzer/Panzer"), null)
    {
        _impacts = new List<Vector3>();
        _defaultRenderer = new Dent(Color.Red, _impacts);
        Transform turretTransform = new(Transform, Vector3.Zero);
        Transform cannonTransform = new(turretTransform, Vector3.Zero);

        RigidBody rb = new DynamicBody(new Collider(new BoxShape(200, 325, 450), (c) => { }), new Vector3(0, 100, 0), 5000, 0.9f, 2f);
        EnemyAI ai = new(turretTransform, cannonTransform);
        _healthBar = new();

        Model.SetTransformToPart("Turret", turretTransform);
        Model.SetTransformToPart("Cannon", cannonTransform);
        _damage = damage;
        _wave = wave;

        AudioManager.Instance.AddSoundEffect(Audios.ENEMY_DIED, ContentRepoManager.Instance().GetSoundEffect("enemy_died"));

        AddComponent(rb);
        AddComponent(ai);
        AddComponent(_healthBar);
    }

    public void AddImpact(Vector3 impact)
    {
        if (_impacts.Count <= 5)
        {
            _impacts.Add(impact);
        }
    }

    private void OnDie()
    {
        _wave.EnemyDie();
        AudioManager.Instance.PlaySound(Audios.ENEMY_DIED);
        isDead = true;
    }
}