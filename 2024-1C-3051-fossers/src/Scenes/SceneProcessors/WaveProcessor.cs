using System;
using Microsoft.Xna.Framework;
using WarSteel.Scenes;

public class WaveProcessor : ISceneProcessor
{
    private int _waveNumber { get; set; }
    private int _enemiesLeft { get; set; }

    public int WaveNumber
    {
        get => _waveNumber;
        // Trigger event to update UI
        set
        {
            _waveNumber = value;
            // Update UI here if necessary
        }
    }

    public int EnemiesLeft
    {
        get => _enemiesLeft;
        // Trigger event to update UI
        set
        {
            _enemiesLeft = value;
            // Update UI here if necessary
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

    public void Update(Scene scene, GameTime gameTime) { }

    public void Draw(Scene scene) { }

    private void StartNextWave()
    {
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
        return WaveNumber * 5;
    }
}