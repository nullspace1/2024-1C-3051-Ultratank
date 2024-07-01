using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;
using WarSteel.Utils;
using Timer = WarSteel.Utils.Timer;

public class Enemy : GameObject
{
    private EnemyHealthBar _healthBar;
    private WaveProcessor _wave;

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

    public Enemy(Vector3 pos, float damage, WaveProcessor wave, Scene scene) : base(new string[] { "enemy" }, new() { Position = pos }, ContentRepoManager.Instance().GetModel("Tanks/Panzer/Panzer"), null)
    {
        Renderer = new Renderer(Color.Red);
        Transform turretTransform = new(Transform, Vector3.Zero);
        Transform cannonTransform = new(turretTransform, Vector3.Zero);

        EnemyAI ai = new(turretTransform, cannonTransform);
        _healthBar = new();

        Model.SetTransformToPart("Turret", turretTransform);
        Model.SetTransformToPart("Cannon", cannonTransform);
        _damage = damage;
        _wave = wave;

        AudioManager.Instance.AddSoundEffect(Audios.ENEMY_DIED, ContentRepoManager.Instance().GetSoundEffect("enemy_died"));

        AddComponent(new DynamicBody(new Collider(new ConvexShape(Model.GetModel()), (c) =>
        {
            if (!c.Entity.HasTag("SURFACE"))
            {
                GetComponent<EnemyAI>()?.HandleCollision();
            }
        }), new Vector3(0, 100, 0), 5000, 0.9f, 0.2f));
        AddComponent(ai);
        AddComponent(_healthBar);
        AddComponent(new Stabilizer());
        AddComponent(new LightComponent(Color.Red));

        Timer.Timeout(500, () =>
        {
            RemoveComponent<LightComponent>(scene);
        });
    }


    private void OnDie()
    {
        _wave.EnemyDie();
        AudioManager.Instance.PlaySound(Audios.ENEMY_DIED);
        isDead = true;
    }
}