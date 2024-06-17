using Microsoft.Xna.Framework;
using WarSteel.Common;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;
using WarSteel.Scenes.Main;

public class Player : GameObject
{
    private float _health = 100;
    public float Health
    {
        get => _health;
        set
        {
            _health = value;
            PlayerEvents.TriggerHealthChanged(value);
        }
    }
    private float _damage;
    public float Damage
    {
        get => _damage;
        set => _damage = value;
    }

    public Player(Scene scene, Vector3 pos) : base(new string[] { }, new() { Position = pos }, ContentRepoManager.Instance().GetModel("Tanks/Panzer/Panzer"))
    {
        Transform turretTransform = new(Transform, Vector3.Zero);
        Transform cannonTransform = new(turretTransform, Vector3.Zero);
        _defaultRenderer = new TankRenderable(turretTransform, cannonTransform);

        AddComponent(new DynamicBody(new Collider(new BoxShape(200, 325, 450), (c) => { }), new Vector3(0, 100, 0), 200, 0.9f, 2f));
        AddComponent(new PlayerControls(cannonTransform));
        AddComponent(new TurretController(turretTransform, scene.GetCamera(), 3f));
        AddComponent(new CannonController(cannonTransform, scene.GetCamera()));
    }
}