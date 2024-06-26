using System;

public static class WaveEvents
{

    public static void TriggerNewWave(int waveNumber)
    {
        EventManager.Instance.TriggerEvent(Constants.Events.Wave.NEW_WAVE, waveNumber);
    }

    public static void SubscribeToNewWave(Action<int> cb)
    {
        EventManager.Instance.StartListening(Constants.Events.Wave.NEW_WAVE, cb);
    }

    public static void TriggerEnemiesLeft(int enemiesLeft)
    {
        EventManager.Instance.TriggerEvent(Constants.Events.Wave.ENEMIES_LEFT, enemiesLeft);
    }

    public static void SubscribeToEnemiesLeft(Action<int> cb)
    {
        EventManager.Instance.StartListening(Constants.Events.Wave.ENEMIES_LEFT, cb);
    }
}