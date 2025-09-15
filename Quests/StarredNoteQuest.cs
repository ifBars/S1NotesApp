using MelonLoader.Utils;
using S1API.Internal.Utils;
using S1API.Quests;
using S1API.Saveables;
using UnityEngine;
using System.Reflection;
using MelonLoader;
using System;

namespace S1NotesApp.Quests
{
	public class StarredNoteQuest : Quest
	{
		[Serializable]
		private class PersistedData
		{
			public string NoteId = string.Empty;
			public string Title = string.Empty;
			public string Body = string.Empty;
		}

		[SaveableField("QuestData")]
		private PersistedData _data = new PersistedData();

		protected override string Title => string.IsNullOrEmpty(_data.Title) ? "Starred Note" : _data.Title;
		protected override string Description => string.IsNullOrEmpty(_data.Body) ? "" : _data.Body;
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
			_data.NoteId = noteId;
			_data.Title = title;
			_data.Body = body;
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
				if (QuestManagerQuests[i] is StarredNoteQuest q && q._data != null && q._data.NoteId == id)
					return q;
			}
			return null;
		}

		protected override void OnLoaded()
		{
			// Rebuild entry text on load if needed so the overlay/UI shows the saved content
			if (QuestEntries != null && QuestEntries.Count == 0)
			{
				string entryText = string.IsNullOrWhiteSpace(_data.Body) ? _data.Title : _data.Body;
				if (!string.IsNullOrWhiteSpace(entryText))
				{
					AddEntry(entryText);
				}
			}
		}

		private static List<Quest> QuestManagerQuests => (List<Quest>)typeof(QuestManager)
			.GetField("Quests", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
			.GetValue(null);
	}
}


