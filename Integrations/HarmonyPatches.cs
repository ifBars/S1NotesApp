using HarmonyLib;

namespace S1NotesApp.Integrations
{
    [HarmonyPatch]
    public static class HarmonyPatches
    {
        private static Core? _modInstance;

        /// <summary>
        /// Set the mod instance for patch callbacks
        /// </summary>
        public static void SetModInstance(Core modInstance)
        {
            _modInstance = modInstance;
        }

        // Save/load handled through S1API Saveable system (ModSaveableRegistry). No app-specific patches needed.
    }
}
