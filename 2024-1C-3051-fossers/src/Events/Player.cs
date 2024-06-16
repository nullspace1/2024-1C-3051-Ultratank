using System;

public static class PlayerEvents
{

    public static void TriggerReload(int reloadingTime)
    {
        EventManager.Instance.TriggerEvent(Constants.Events.Player.RELOADING, reloadingTime);
    }

    public static void SubscribeToReload(Action<int> cb)
    {
        EventManager.Instance.StartListening(Constants.Events.Player.RELOADING, cb);
    }

    public static void TriggerHealthChanged(float newHealthValue)
    {
        EventManager.Instance.TriggerEvent(Constants.Events.Player.HEALTH_CHANGED, newHealthValue);
    }

    public static void SubscribeToHealthChanged(Action<float> cb)
    {
        EventManager.Instance.StartListening(Constants.Events.Player.HEALTH_CHANGED, cb);
    }
}