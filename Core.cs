using MelonLoader;
using S1NotesApp.Integrations;
using S1NotesApp.Services;
using S1NotesApp.Utils;

[assembly: MelonInfo(typeof(S1NotesApp.Core), Constants.MOD_NAME, Constants.MOD_VERSION, Constants.MOD_AUTHOR)]
[assembly: MelonGame(Constants.Game.GAME_STUDIO, Constants.Game.GAME_NAME)]

namespace S1NotesApp
{
    public class Core : MelonMod
    {
        public static Core? Instance { get; private set; }

        public override void OnInitializeMelon()
        {
            Instance = this;
            HarmonyPatches.SetModInstance(this);
            NotesManager.RegisterWithS1API();
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (sceneName != "Menu") return;
            NotesManager.Instance.ClearNotes();
        }

        public override void OnApplicationQuit()
        {
            Instance = null;
        }
    }
}