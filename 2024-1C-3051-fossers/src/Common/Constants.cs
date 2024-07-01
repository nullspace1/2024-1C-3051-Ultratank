using System;

public static class Constants
{
    public const bool DEBUG_MODE = true;
    public const bool PLAY_SOUND = true;

    public static class Events
    {
        public static class Player
        {
            public const string RELOADING = "player_reloading";
            public const string HEALTH_CHANGED = "player_health_changed";
            public const string DMG_CHANGED = "player_damage_changed";
        }

        public static class Wave
        {
            public const string NEW_WAVE = "wave_new";
            public const string ENEMIES_LEFT = "wave_enemies_left";
        }
    }
}