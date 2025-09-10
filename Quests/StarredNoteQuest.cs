using MelonLoader.Utils;
using S1API.Internal.Utils;
using S1API.Quests;
using S1API.Saveables;
using UnityEngine;
using System.Reflection;
using MelonLoader;

namespace S1NotesApp.Quests
{
	public class StarredNoteQuest : Quest
	{
		[SaveableField("QuestData")]
		private QuestData _data = new QuestData(typeof(StarredNoteQuest).FullName ?? "StarredNoteQuest");

		private string _noteId = string.Empty;
		private string _title = string.Empty;
		private string _body = string.Empty;

		protected override string Title => string.IsNullOrEmpty(_title) ? "Starred Note" : _title;
		protected override string Description => string.IsNullOrEmpty(_body) ? "" : _body;
		protected override bool AutoBegin => true;
        protected override Sprite? QuestIcon => LoadEmbeddedNotesIcon();

		/// <summary>
		/// Loads the embedded notes.png image from the assembly resources
		/// </summary>
		/// <returns>The loaded sprite or null if not found</returns>
		private Sprite? LoadEmbeddedNotesIcon()
		{
			try
			{
				// Get the current assembly
				var assembly = Assembly.GetExecutingAssembly();
				
				// Try different possible resource names
				string[] possibleNames = {
					"S1NotesApp.notes.png",
					"S1NotesApp.Apps.notes.png",
					"notes.png"
				};
				
				foreach (string resourceName in possibleNames)
				{
					using var stream = assembly.GetManifestResourceStream(resourceName);
					if (stream != null)
					{
						// Read the stream into a byte array
						byte[] data = new byte[stream.Length];
						stream.Read(data, 0, data.Length);
						
						// Use S1API's ImageUtils to load the image from byte array
						return ImageUtils.LoadImageRaw(data);
					}
				}
			}
			catch (Exception ex)
			{
				MelonLogger.Msg($"Failed to load embedded notes icon: {ex.Message}");
			}
			
			return null;
		}

        public void BindNote(string noteId, string title, string body)
		{
			_noteId = noteId;
			_title = title;
			_body = body;
			// Use body as the quest entry text when present so the overlay shows note content
			string entryText = string.IsNullOrWhiteSpace(body) ? title : body;
			AddEntry(entryText);
		}

		public static StarredNoteQuest? FindByNoteId(string id)
		{
			var all = QuestManager.GetQuestByName("Starred Note");
			// Fallback linear search across active quests
			for (int i = 0; i < QuestManagerQuests.Count; i++)
			{
				if (QuestManagerQuests[i] is StarredNoteQuest q && q._noteId == id)
					return q;
			}
			return null;
		}

		private static List<Quest> QuestManagerQuests => (List<Quest>)typeof(QuestManager)
			.GetField("Quests", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
			.GetValue(null);
	}
}


