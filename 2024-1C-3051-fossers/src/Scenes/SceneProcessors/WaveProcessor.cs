using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using WarSteel.Entities;
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

    public int EnemiesKilled = 0;

    private Scene _scene;
    private Player _player;

    private MapGrid _grid;

    public WaveProcessor(Player player, MapGrid grid)
    {
        _player = player;
        _grid = grid;
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
        _player.Health = 100;
        _player.Renderer.ClearImpacts();
        WaveNumber++;
        EnemiesLeft = GetEnemiesToSpawn();

        for (int i = 0; i < EnemiesLeft; i++)
        {
            Vector3 spawnPosition = _grid.GetRandomUnusedGridPosition(100);
            Enemy enemy = new(spawnPosition, WaveNumber * 5, this);
            _scene.AddGameObject(enemy);
        }

    }

    private int GetEnemiesToSpawn()
    {
        return WaveNumber;
    }

    public void EnemyDie()
    {
        EnemiesLeft--;
        EnemiesKilled++;
    }

    public float GetScore()
    {
        return EnemiesKilled + WaveNumber;
    }

    public void StopEnemies()
    {
        List<GameObject> enemies = _scene.GetEntitiesByTag("enemy");
        enemies.ForEach(enemy => enemy.RemoveComponent<EnemyAI>(_scene));
    }
}