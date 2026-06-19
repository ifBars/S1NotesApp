# S1NotesApp

S1NotesApp is an example Schedule I phone app built with S1API. It adds a Notes app to the in-game phone, persists notes through the S1API save system, and can create quest entries for starred notes.

## Features

- Create, edit, delete, and star notes from the in-game phone.
- Persist notes per save slot through S1API saveables.
- Surface starred notes as quest entries.
- Use an embedded app icon and S1API UI helpers for phone integration.
- Build against the current forked S1API MelonLoader targets for Mono and IL2CPP.

## Requirements

- Schedule I
- MelonLoader
- S1API / S1API.Forked

The current S1API fork is maintained for MelonLoader. This example does not target BepInEx.

## Building

This repository expects to live next to the S1API repository:

```text
ScheduleOne/
  S1API/
  S1NotesApp/
```

Build S1API first so the local references exist, then build the Notes app:

```bash
dotnet build ..\S1API\S1API.sln -c Release
dotnet build S1NotesApp.csproj -c Mono
dotnet build S1NotesApp.csproj -c Il2cpp
```

The project references:

- `..\S1API\S1API\bin\MonoMelon\netstandard2.1\S1API.dll`
- `..\S1API\S1API\bin\Il2CppMelon\net6.0\S1API.dll`

Update the local `GamePath` values in `S1NotesApp.csproj` if your Schedule I installs are in different folders.

## Installation

1. Install MelonLoader for Schedule I.
2. Install S1API.
3. Build or download `S1NotesApp.dll`.
4. Place `S1NotesApp.dll` in the game's `Mods` folder.
5. Launch the game and open the in-game phone.

## Usage

1. Open the in-game phone.
2. Open the Notes app.
3. Create a note with a title and body.
4. Save changes to persist the note.
5. Star a note to surface it through the quest integration.

## Project Structure

```text
S1NotesApp/
  Apps/
    NotesApp.cs
    notes.png
  Quests/
    StarredNoteQuest.cs
  Services/
    NotesManager.cs
  Utils/
    Constants.cs
  Core.cs
  S1NotesApp.csproj
```

## Notes for Maintainers

- `NotesApp` owns the phone UI.
- `NotesManager` owns note data, saveable fields, and quest synchronization.
- `StarredNoteQuest` bridges starred notes into the S1API quest system.
- `Core` registers the phone app and save manager during scene initialization.

This repo is intended as a working S1API example as much as a standalone mod.
