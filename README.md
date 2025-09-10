# S1NotesApp

A comprehensive notes application for Schedule I that integrates seamlessly with the in-game phone system.

## 📱 Overview

S1NotesApp adds a fully-featured notes application to your Schedule I phone, allowing you to create, edit, organize, and manage notes directly within the game. The app features a clean, intuitive interface with quest integration for starred notes.

## ✨ Features

### 📝 Note Management
- **Create Notes**: Add new notes with custom titles and content
- **Edit Notes**: Modify existing notes with a rich text editor
- **Delete Notes**: Remove notes you no longer need
- **Auto-Save**: Notes are automatically saved and persist between game sessions when you save the game normally

### ⭐ Starred Notes
- **Star Important Notes**: Mark important notes with a star for easy identification
- **Quest Integration**: Starred notes automatically create quest entries in your quest log
- **Visual Indicators**: Starred notes appear with a star icon and are sorted to the top of the list

### 🎨 User Interface
- **Split-Panel Design**: Notes list on the left, editor on the right
- **Responsive Layout**: Clean, modern interface that fits the game's aesthetic
- **Welcome Message**: Helpful guidance when no note is selected
- **Custom Icons**: Embedded notes icon for consistent visual experience

### 💾 Data Persistence
- **Save System Integration**: Uses S1API's save integration for per-save-slot data storage
- **Cross-Session Persistence**: Notes are saved and loaded with the game
- **Data Integrity**: Robust error handling and data validation

## 🚀 Installation

### Prerequisites
- **Schedule I** (Steam version)
- **MelonLoader** (latest version)
- **S1API** (required dependency)

### Installation Steps
1. Download the latest release from the releases page
2. Extract the mod files to your Schedule I installation directory
3. Place the mod DLL in your `Mods/` folder
4. Launch the game - the Notes app will appear on your phone

### File Structure
```
Schedule I/
├── Mods/
└── └── S1NotesApp.dll
```

## 📖 Usage

### Opening the App
1. Open your in-game phone
2. Look for the "Notes" app icon
3. Tap to open the Notes application

### Creating a Note
1. Click the **"New"** button in the top-right corner
2. Enter a title in the title field
3. Write your note content in the body field
4. Click **"Save"** to save the note

### Editing a Note
1. Click on any note in the left panel to select it
2. The note will open in the editor on the right
3. Make your changes
4. Click **"Save"** to save changes or **"Cancel"** to discard them

### Starring Notes
1. Open a note for editing
2. Click the **"Star"** button to mark it as important
3. Starred notes will:
   - Appear at the top of the notes list
   - Show a golden star icon
   - Create a quest entry in your quest log

### Deleting Notes
1. Open the note you want to delete
2. Click the **"Delete"** button
3. Confirm the deletion (note will be permanently removed)

## 🔧 Technical Details

### Architecture
- **Framework**: Built on S1API's PhoneApp system
- **UI Framework**: Uses S1API's UIFactory for consistent styling
- **Save System**: Integrates with S1API's ModSaveableRegistry
- **Quest Integration**: Uses S1API's Quest system for starred notes

### Dependencies
- **S1API**: Core API for Schedule I modding
- **MelonLoader**: Mod loader framework
- **Unity Engine**: Game engine (via S1API)

### File Structure
```
S1NotesApp/
├── Apps/
│   ├── NotesApp.cs          # Main phone app implementation
│   └── notes.png            # Embedded app icon
├── Services/
│   └── NotesManager.cs      # Note data management
├── Quests/
│   └── StarredNoteQuest.cs  # Quest integration for starred notes
├── Integrations/
│   └── HarmonyPatches.cs     # Harmony integration
├── Utils/
│   └── Constants.cs         # Mod constants and configuration
└── Core.cs                  # Main mod entry point
```

## 🐛 Troubleshooting

### Common Issues

**Notes app doesn't appear on phone:**
- Ensure S1API is properly installed
- Check that the mod DLL is in the correct Mods folder
- Verify MelonLoader is running correctly

**Notes not saving:**
- Check that S1API's save system is working
- Ensure you have write permissions to the game directory
- Try creating a new note to test the save functionality

**Starred notes not creating quests:**
- Verify that the quest system is functioning
- Check that S1API's QuestManager is properly initialized
- Try starring and unstarring a note to refresh the quest

## 📄 License

This mod is released under the MIT License. See the LICENSE file for details.

**Made with ❤️ for the Schedule I community**
