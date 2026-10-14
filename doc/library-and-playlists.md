# Library and playlists

[Polski](library-and-playlists_pl-PL.md) · [Documentation index](../README.md#documentation)

## Supported audio files

The shared audio file filter accepts:

`.aac`, `.aif`, `.aiff`, `.asf`, `.flac`, `.m4a`, `.mp3`, `.mp4`, `.ogg`, `.opus`, `.wav`, and `.wma`.

Actual decoding also depends on the MiniAudioEx backend available on the current platform.

## Music library

Library folders are configured in General settings. Every folder can include only its direct files or all subfolders. Adding, removing, refreshing, or changing recursion can trigger a rescan.

`LibraryCore` scans folders on a worker task, ignores inaccessible paths, removes duplicate file paths, reads tags and artwork, and publishes tracks sorted by title. Artwork is stored in `CoverCache` and reused by playlist and library views.

The library manager provides four entry points:

- Tracks.
- Albums.
- Artists.
- Folders.

Artist navigation may open albums first or go directly to tracks. Folder navigation combines child directories and audio files. Layout is remembered independently for tracks, albums, artists, and directories, with details, column list, and multiple icon sizes.

## Now Playing

`PlayListCore` owns the live queue. It supports adding files, removing and reordering tracks, sorting, selection mode, shuffle navigation, and current-track state. The queue can optionally be restored from `now_playing.vpl` when the application starts.

The side-panel playlist can open a `.vpl` file or export the current queue through system file dialogs.

## Saved playlists

`PlayListLibraryCore` stores named `.vpl` files in the application data `PlayLists` directory. The playlist manager supports:

- Creating and duplicating playlists.
- Explicit saving with dirty-state confirmation.
- Replacing the current queue or appending to it.
- Importing audio files.
- Search and grouped sorting.
- Drag and drop ordering.
- Multi-selection, including Shift range selection.
- Copying selected tracks to another playlist.
- Exporting and deleting playlists.

The special “Now Playing” entry is memory-backed and is not treated as a saved library playlist.

## Metadata

`TrackMetadataUtils` reads title, album, artist, duration, and embedded artwork through TagLibSharp. When metadata is missing, display properties fall back to values derived from the file path.
