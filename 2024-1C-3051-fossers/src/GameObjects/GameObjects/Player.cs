using Microsoft.Xna.Framework;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;
using WarSteel.Scenes.Main;

public class Player : GameObject
{
    private float _health = 100;

    public bool touchingGround = true;

    public float Health
    {
        get => _health;
        set
        {
            _health = value;
            PlayerEvents.TriggerHealthChanged(value);
            if (_health <= 0) OnDie();
        }
    }
    private float _damage = 25;
    public float Damage
    {
        get => _damage;
        set
        {
            _damage = value;
            PlayerEvents.TriggerDamageChanged(value);
        }
    }
    Scene _scene;

    public Player(Scene scene, Vector3 pos) : base(new string[] { "player" }, new() { Position = pos }, ContentRepoManager.Instance().GetModel("Tanks/Simple"), new Renderer(Color.Blue))
    {
        _scene = scene;
        Renderer = new Renderer(Color.Blue);
        Transform turretTransform = new(Transform, Vector3.Zero);
        Transform cannonTransform = new(turretTransform, Vector3.Zero);
        Model.SetTransformToPart("Turret", turretTransform);
        Model.SetTransformToPart("Cannon", cannonTransform);

        AddComponent(new DynamicBody(new Collider(new ConvexShape(Model.GetModel()), (c) =>
        {
            if (c.Entity.HasTag("ground"))
            {
                touchingGround = true;
            }
        }), new Vector3(0, 0, 0), 5000, 0.9f, 2f));

        AddComponent(new TurretController(turretTransform, scene.GetCamera(), 3f));
        AddComponent(new CannonController(cannonTransform, scene.GetCamera()));
        AddComponent(new PlayerControls(cannonTransform));
        AddComponent(new WheelsController(Model, Transform));
        AddComponent(new Stabilizer());
    }

    public void OnDie()
    {
        WaveProcessor wave = _scene.GetSceneProcessor<WaveProcessor>();
        wave.StopEnemies();
        new LooseScreen(_scene).Initialize(wave.EnemiesLeft, wave.WaveNumber, wave.GetScore());
        RemoveComponent<PlayerControls>(_scene);
        _scene.Camera.StopFollowing();
    }
}