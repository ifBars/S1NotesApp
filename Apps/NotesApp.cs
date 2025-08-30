using UnityEngine;
using UnityEngine.UI;
using S1API.PhoneApp;
using S1API.UI;
using S1NotesApp.Services;
using S1API.Internal.Utils;
using UnityEngine.EventSystems;
using S1API.Internal.Abstraction;
using S1API.Input;

namespace S1NotesApp.Apps
{
	public class NotesApp : PhoneApp
	{
		private RectTransform _listContent;
		private GameObject _editorPanel;
		private InputField _titleInput;
		private InputField _bodyInput;
		private Button _saveButton;
		private Button _cancelButton;
		private Button _starToggleButton;
		private Button _deleteButton;
		private Text _starToggleLabel;
		private string _editingNoteId = string.Empty;

		protected override string AppName => "NotesApp";
		protected override string AppTitle => "Notes";
		protected override string IconLabel => "Notes";
		protected override string IconFileName => Path.Combine("NotesApp", "notes.png");

		protected override void OnCreated()
		{
			// Subscribe to notes loaded event to refresh UI when save data is loaded
			NotesManager.OnNotesLoaded += OnNotesLoaded;
		}

		protected override void OnDestroyed()
		{
			// Unsubscribe from events
			NotesManager.OnNotesLoaded -= OnNotesLoaded;
		}

		private void OnNotesLoaded()
		{
			// Refresh the list when notes are loaded from save data
			RefreshList();
		}

		protected override void OnCreatedUI(GameObject container)
		{
			// Background and top bar
			var bg = UIFactory.Panel("MainBG", container.transform, Color.black, fullAnchor: true);
			var topBar = UIFactory.TopBar("TopBar", bg.transform, "Notes", 0.82f, 75, 75, 0, 35);
			// Add New button on top bar (right-hand side)
			var (newBtnGO, newBtn, newBtnLbl) = UIFactory.RoundedButtonWithLabel("NewNoteTopBtn", "New", topBar.transform, new Color(0.2f, 0.6f, 0.2f), 110, 40, 16, Color.white);
			ButtonUtils.AddListener(newBtn, CreateNewNote);

			// Left list panel
			var leftPanel = UIFactory.Panel("NotesListPanel", bg.transform, new Color(0.1f, 0.1f, 0.1f), new Vector2(0.02f, 0.05f), new Vector2(0.49f, 0.82f));
			_listContent = UIFactory.ScrollableVerticalList("NotesListScroll", leftPanel.transform, out _);
			UIFactory.FitContentHeight(_listContent);

			// Right detail/editor panel
			var rightPanel = UIFactory.Panel("DetailPanel", bg.transform, new Color(0.12f, 0.12f, 0.12f), new Vector2(0.49f, 0f), new Vector2(0.98f, 0.82f));
			UIFactory.VerticalLayoutOnGO(rightPanel, spacing: 14, padding: new RectOffset(24, 50, 15, 70));
			_editorPanel = CreateEditorPanel(rightPanel.transform);
			_editorPanel.SetActive(true);

			// container.SetActive(true);
            RefreshList();
        }

		protected override void OnPhoneClosed()
		{
			// Hard reset typing and clear any focused UI so gameplay input resumes
			Controls.IsTyping = false;
			if (EventSystem.current != null)
				EventSystem.current.SetSelectedGameObject(null);
			_titleInput?.DeactivateInputField();
			_bodyInput?.DeactivateInputField();
		}

		private GameObject CreateEditorPanel(Transform parent)
		{
			var panel = UIFactory.Panel("EditorPanel", parent, new Color(0.1f, 0.1f, 0.1f, 0.95f));
			var rt = panel.GetComponent<RectTransform>();
			rt.anchorMin = new Vector2(0f, 0f);
			rt.anchorMax = new Vector2(1f, 1f);
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;

			UIFactory.VerticalLayoutOnGO(panel, spacing: 12, padding: new RectOffset(16, 16, 16, 16));

			UIFactory.Text("TitleLabel", "Title", panel.transform, 16, TextAnchor.MiddleLeft, FontStyle.Bold);
			_titleInput = CreateInputField(panel.transform, multiline: false, height: 34);

			UIFactory.Text("BodyLabel", "Body", panel.transform, 16, TextAnchor.MiddleLeft, FontStyle.Bold);
			_bodyInput = CreateInputField(panel.transform, multiline: true, height: 220);

			var buttonsRow = UIFactory.ButtonRow("EditorButtons", panel.transform, spacing: 12f, alignment: TextAnchor.MiddleRight);
			var (saveGo, saveBtn, saveLbl) = UIFactory.RoundedButtonWithLabel("SaveBtn", "Save", buttonsRow.transform, new Color(0.2f, 0.5f, 0.9f), 110, 36, 16, Color.white);
			_saveButton = saveBtn;
			ButtonUtils.AddListener(_saveButton, SaveEditingNote);

			var (cancelGo, cancelBtn, cancelLbl) = UIFactory.RoundedButtonWithLabel("CancelBtn", "Cancel", buttonsRow.transform, new Color(0.35f, 0.35f, 0.35f), 110, 36, 16, Color.white);
			_cancelButton = cancelBtn;
			ButtonUtils.AddListener(_cancelButton, CancelEditing);

			var (starGo, starBtn, starLbl) = UIFactory.RoundedButtonWithLabel("StarToggleBtn", "Star", buttonsRow.transform, new Color(0.75f, 0.65f, 0.2f), 110, 36, 16, Color.black);
			_starToggleButton = starBtn;
			_starToggleLabel = starLbl;
			ButtonUtils.AddListener(_starToggleButton, ToggleStarFromEditor);

			var (delGo, delBtn, delLbl) = UIFactory.RoundedButtonWithLabel("DeleteBtn", "Delete", buttonsRow.transform, new Color(0.7f, 0.2f, 0.2f), 110, 36, 16, Color.white);
			_deleteButton = delBtn;
			ButtonUtils.AddListener(_deleteButton, DeleteFromEditor);

			return panel;
		}

		private InputField CreateInputField(Transform parent, bool multiline, float height)
		{
			var go = new GameObject("Input");
			go.transform.SetParent(parent, false);
			var rt = go.AddComponent<RectTransform>();
			rt.sizeDelta = new Vector2(0, height);
			var le = go.AddComponent<LayoutElement>();
			le.minHeight = height;
			le.preferredHeight = height;

			var img = go.AddComponent<Image>();
			img.color = new Color(0.2f, 0.2f, 0.2f, 1f);
			img.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");
			img.type = Image.Type.Sliced;

			var input = go.AddComponent<InputField>();
			// Text component with proper padding/fill
			var text = UIFactory.Text("Text", string.Empty, go.transform, 14, TextAnchor.UpperLeft);
			var textRT = text.GetComponent<RectTransform>();
			textRT.anchorMin = Vector2.zero;
			textRT.anchorMax = Vector2.one;
			textRT.offsetMin = new Vector2(10, 8);
			textRT.offsetMax = new Vector2(-10, -8);
			text.horizontalOverflow = HorizontalWrapMode.Wrap;
			text.verticalOverflow = VerticalWrapMode.Overflow;

			// Placeholder with same padding
			var placeholder = UIFactory.Text("Placeholder", multiline ? "Write your note here..." : "Enter title...", go.transform, 14, TextAnchor.UpperLeft, FontStyle.Italic);
			var phRT = placeholder.GetComponent<RectTransform>();
			phRT.anchorMin = Vector2.zero;
			phRT.anchorMax = Vector2.one;
			phRT.offsetMin = new Vector2(10, 8);
			phRT.offsetMax = new Vector2(-10, -8);
			placeholder.color = new Color(0.8f, 0.8f, 0.8f, 0.6f);

			input.textComponent = text;
			input.placeholder = placeholder;
			input.lineType = multiline ? InputField.LineType.MultiLineNewline : InputField.LineType.SingleLine;
			input.interactable = true;

			// Typing state management to disable game controls while editing
			var trigger = go.AddComponent<EventTrigger>();
            EventHelper.AddEventTrigger(trigger, EventTriggerType.Select, () => Controls.IsTyping = true);
            EventHelper.AddEventTrigger(trigger, EventTriggerType.Deselect, () => Controls.IsTyping = false);
			EventHelper.AddListener<string>(_ => Controls.IsTyping = false, input.onEndEdit);
			EventHelper.AddListener<string>(_ => { if (input.isFocused) Controls.IsTyping = true; }, input.onValueChanged);
			return input;
		}

		private void RefreshList()
		{
			if (_listContent == null)
				return;

			UIFactory.ClearChildren(_listContent);
			IReadOnlyList<NotesManager.NoteEntry> notes = NotesManager.Instance.GetAllNotes();
			for (int i = 0; i < notes.Count; i++)
			{
				var note = notes[i];
				var row = UIFactory.CreateQuestRow($"Note_{note.Id}", _listContent, out var iconPanel, out var textPanel);
				
				// Add note icon to the icon panel
				CreateNoteIcon(iconPanel.transform, note.Starred);
				
				string title = note.Starred ? $"★ {note.Title}" : note.Title;
				UIFactory.CreateTextBlock(textPanel.transform, title, note.Updated.ToString("g"), false);
				var rowBtn = row.GetComponent<Button>();
				ButtonUtils.ClearListeners(rowBtn);
				ButtonUtils.AddListener(rowBtn, () => BeginEdit(note.Id));
			}
		}

		private void CreateNoteIcon(Transform parent, bool starred)
		{
			// Create an image component for the note icon
			var iconGO = new GameObject("NoteIcon");
			iconGO.transform.SetParent(parent, false);
			
			var iconRT = iconGO.AddComponent<RectTransform>();
			iconRT.anchorMin = Vector2.zero;
			iconRT.anchorMax = Vector2.one;
			iconRT.offsetMin = Vector2.zero;
			iconRT.offsetMax = Vector2.zero;
			
			var iconImage = iconGO.AddComponent<Image>();
			
			// Load the notes.png image
			var iconPath = Path.Combine("NotesApp", "notes.png");
			var iconSprite = ImageUtils.LoadImage(iconPath);
			if (iconSprite != null)
			{
				iconImage.sprite = iconSprite;
			}
			else
			{
				// Fallback to text if image not found
				iconImage.enabled = false;
				var iconText = UIFactory.Text("NoteIconText", "📝", parent, 24, TextAnchor.MiddleCenter, FontStyle.Bold);
				iconText.color = starred ? new Color(1f, 0.8f, 0f, 1f) : new Color(0.8f, 0.8f, 0.8f, 1f);
				var textRT = iconText.GetComponent<RectTransform>();
				textRT.anchorMin = Vector2.zero;
				textRT.anchorMax = Vector2.one;
				textRT.offsetMin = Vector2.zero;
				textRT.offsetMax = Vector2.zero;
			}
			
			// Tint the icon based on starred status
			iconImage.color = starred ? new Color(1f, 0.8f, 0f, 1f) : Color.white;
		}

		private void CreateNewNote()
		{
			var id = NotesManager.Instance.CreateNote("Untitled", string.Empty);
			BeginEdit(id);
		}

		private void BeginEdit(string noteId)
		{
			var note = NotesManager.Instance.GetNote(noteId);
			if (note == null)
				return;
			_editingNoteId = noteId;
			_titleInput.text = note.Title;
			_bodyInput.text = note.Body;
			_starToggleLabel.text = note.Starred ? "Unstar" : "Star";
			_editorPanel.SetActive(true);
		}

		private void SaveEditingNote()
		{
			if (string.IsNullOrEmpty(_editingNoteId))
				return;
			NotesManager.Instance.UpdateNote(_editingNoteId, _titleInput.text ?? string.Empty, _bodyInput.text ?? string.Empty);
			_editorPanel.SetActive(false);
			Controls.IsTyping = false;
			RefreshList();
		}

		private void CancelEditing()
		{
			_editorPanel.SetActive(false);
			_editingNoteId = string.Empty;
			Controls.IsTyping = false;
		}

		private void ToggleStarFromEditor()
		{
			if (string.IsNullOrEmpty(_editingNoteId))
				return;
			NotesManager.Instance.ToggleStar(_editingNoteId);
			var note = NotesManager.Instance.GetNote(_editingNoteId);
			_starToggleLabel.text = note != null && note.Starred ? "Unstar" : "Star";
			RefreshList();
		}

		private void DeleteFromEditor()
		{
			if (string.IsNullOrEmpty(_editingNoteId))
				return;
			NotesManager.Instance.DeleteNote(_editingNoteId);
			_editingNoteId = string.Empty;
			_titleInput.text = string.Empty;
			_bodyInput.text = string.Empty;
			RefreshList();
		}
	}
}


