# Lyrics system

[Polski](lyrics_pl-PL.md) · [Documentation index](../README.md#technical-documentation)

## Sources and priority

`LyricsCore` maintains the local lyrics library and selects a document for the current track. Local library assignments have priority over embedded or adjacent data. Within available content, more precise synchronization is preferred: word-synchronized, then line-synchronized, then plain text.

Supported sources include:

- Local VisualPlayer lyrics documents.
- Adjacent `.lrc` files and manually imported LRC or text files.
- Embedded audio metadata read through TagLibSharp.
- Online LRCLIB search.

Settings control whether embedded lyrics are used, automatically imported, and whether compatible online lyrics are downloaded automatically.

## Lyrics types

`LyricsType` describes three levels:

- `Plain`: lines without playback timestamps.
- `LineSynced`: each line has a start time.
- `WordSynced`: words or syllables have individual timestamps.

The LRC parser accepts line timestamps such as `[00:12.40]` and enhanced timestamps such as `<00:13.10>`. Enhanced segments are expanded into editor rows so each timestamp can be edited independently.

## Local storage

Lyrics documents are stored below the application data `Lyrics` directory. Each document records its name, source, type, optional track path, LRC metadata, and timed lines. The `Cache` child directory contains LRCLIB responses with a 24-hour lifetime.

## Embedded metadata

`EmbeddedLyricsReader` uses TagLibSharp. It supports common unsynchronized lyrics fields and ID3v2 `SYLT` frames with absolute millisecond timestamps. Synchronized ID3 entries are converted into line-synchronized documents. Common metadata is also passed through the LRC parser, allowing embedded LRC text to retain synchronization.

## Online provider

`LrcLibLyricsProvider` implements `ILyricsProvider`. It supports free-text queries and searches based on track title, artist, and album. Requests use a 15-second timeout, identify the application through the user-agent, cache successful responses, and report rate limiting.

## Editor

`InternalViewLyricsEditor` provides:

- Type selection and timed text rows.
- Precise position input.
- Configurable short, medium, and long seek intervals.
- Playback, stop, seek, playback-anchor, and speed controls.
- Time validation and visible validation errors.
- Undo and redo.
- Dirty-state detection and confirmation before closing.

Plain lyrics are exported as `.txt`; synchronized lyrics are exported as `.lrc`. Import uses the same parser to determine the document type.

## Playback overlay

`LyricsControl` respects `LyricsConfig.DisplayLyrics`. Timed lyrics are displayed in a horizontal bar above the player, with previous, current, and next content. Plain lyrics use the larger area between the information bar, side panel, player, and right edge. Text sizes and background gradient opacity are configurable.
