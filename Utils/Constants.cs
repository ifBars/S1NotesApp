﻿namespace S1NotesApp.Utils
{
    public static class Constants
    {
        /// <summary>
        /// Mod information
        /// </summary>
        public const string MOD_NAME = "S1NotesApp";
        public const string MOD_VERSION = "1.0.0";
        public const string MOD_AUTHOR = "Bars";
        public const string MOD_DESCRIPTION = "Mod description...";

        /// <summary>
        /// MelonPreferences configuration
        /// </summary>
        public const string PREFERENCES_CATEGORY = "S1NotesApp";

        /// <summary>
        /// Default preference values
        /// </summary>
        public static class Defaults
        {
            public const bool BOOLEAN_DEFAULT = false;
        }

        /// <summary>
        /// Preference value constraints
        /// </summary>
        public static class Constraints
        {
            public const float MIN_CONSTRAINT = 0f;
            public const float MAX_CONSTRAINT = 100f;
        }

        /// <summary>
        /// Game-related constants
        /// </summary>
        public static class Game
        {
            public const string GAME_STUDIO = "TVGS";
            public const string GAME_NAME = "Schedule I";
        }

    }
}
