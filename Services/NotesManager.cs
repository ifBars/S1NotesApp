using S1API.Internal.Abstraction;
using S1API.Saveables;
using S1API.Quests;
using S1NotesApp.Quests;

namespace S1NotesApp.Services
{
	public class NotesManager : Saveable
	{
		public NotesManager()
		{
			Instance = this;
		}

		public class NoteEntry
		{
			public string Id;
			public string Title;
			public string Body;
			public bool Starred;
			public DateTime Created;
			public DateTime Updated;
		}

		[SaveableField("NotesData")]
		private List<NoteEntry> _notes = new List<NoteEntry>();

		public static NotesManager Instance { get; private set; } = new NotesManager();

		/// <summary>
		/// Event fired when notes are loaded from save data
		/// </summary>
		public static event Action? OnNotesLoaded;

		protected override void OnLoaded()
		{
			Instance = this;
			// Normalize IDs/timestamps on load
			foreach (var n in _notes)
			{
				if (string.IsNullOrEmpty(n.Id)) n.Id = Guid.NewGuid().ToString("N");
				if (n.Created == default) n.Created = DateTime.UtcNow;
				if (n.Updated == default) n.Updated = n.Created;
			}

			// Notify UI that notes have been loaded
			OnNotesLoaded?.Invoke();
		}

		protected override void OnCreated()
		{
			Instance = this;
		}

		public static void RegisterWithS1API()
		{
			ModSaveableRegistry.Register(Instance, folderName: "Notes");
		}

		public IReadOnlyList<NoteEntry> GetAllNotes()
		{
			return _notes.OrderBy(n => n.Starred ? 0 : 1).ThenByDescending(n => n.Updated).ToList();
		}

		public NoteEntry? GetNote(string id)
		{
			return _notes.FirstOrDefault(n => n.Id == id);
		}

		public void ClearNotes()
		{
			_notes.Clear();
		}

		public string CreateNote(string title, string body)
		{
			var note = new NoteEntry
			{
				Id = Guid.NewGuid().ToString("N"),
				Title = title,
				Body = body,
				Starred = false,
				Created = DateTime.UtcNow,
				Updated = DateTime.UtcNow
			};
			_notes.Add(note);
			return note.Id;
		}

		public void UpdateNote(string id, string title, string body)
		{
			var note = GetNote(id);
			if (note == null) return;
			note.Title = title;
			note.Body = body;
			note.Updated = DateTime.UtcNow;
		}

		public void DeleteNote(string id)
		{
			var note = GetNote(id);
			if (note == null) return;
			if (note.Starred)
			{
				// Unstar to remove quest
				SetStar(note, false);
			}
			_notes.Remove(note);
		}

		public void ToggleStar(string id)
		{
			var note = GetNote(id);
			if (note == null) return;
			SetStar(note, !note.Starred);
			note.Updated = DateTime.UtcNow;
		}

		private void SetStar(NoteEntry note, bool starred)
		{
			note.Starred = starred;
			if (starred)
			{
				var quest = QuestManager.CreateQuest<StarredNoteQuest>() as StarredNoteQuest;
				if (quest != null)
				{
					quest.BindNote(note.Id, note.Title, note.Body);
				}
			}
			else
			{
				var existing = StarredNoteQuest.FindByNoteId(note.Id);
				existing?.Complete();
			}
		}
	}
}


