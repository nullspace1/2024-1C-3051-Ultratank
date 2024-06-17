using System;
using Microsoft.Xna.Framework;
using WarSteel.Scenes;
using WarSteel.Utils;

public class WaveProcessor : ISceneProcessor
{
    private int _waveNumber { get; set; }
    private int _enemiesLeft { get; set; }

    public int WaveNumber
    {
        get => _waveNumber;
        set
        {
            _waveNumber = value;
            WaveEvents.TriggerNewWave(value);
        }
    }

    public int EnemiesLeft
    {
        get => _enemiesLeft;
        set
        {
            _enemiesLeft = value;
            WaveEvents.TriggerEnemiesLeft(value);
            if (_enemiesLeft <= 0)
                Timer.Timeout(4000, StartNextWave);
        }
    }

    private Scene _scene;
    private Player _player;

    public WaveProcessor(Player player)
    {
        _player = player;
    }

    public void Initialize(Scene scene)
    {
        _scene = scene;
        WaveNumber = 0;
        StartNextWave();
    }

    public void Draw(Scene scene) { }

    public void Update(Scene scene, GameTime gameTime) { }

    private void StartNextWave()
    {
        _scene.RemoveGameObjectsByTag("enemy");
        _player.Damage += WaveNumber * 2;
        WaveNumber++;
        EnemiesLeft = GetEnemiesToSpawn();
        Random rand = new();

        // Spawn new enemies
        for (int i = 0; i < EnemiesLeft; i++)
        {
            Vector3 spawnPosition = VectorUtils.GetRandomVec3Pos(new(0, 100, 0), rand);
            Enemy enemy = new Enemy(spawnPosition, WaveNumber * 5, _player);
            _scene.AddGameObject(enemy);
        }
    }

    private int GetEnemiesToSpawn()
    {
        return WaveNumber;
    }
}