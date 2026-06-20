# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Web会議参加者自動チェックツール — a Windows WPF desktop app that monitors participant lists in Zoom and Microsoft Teams meetings using Windows UI Automation (COM interop `UIAutomationClient`). It reads participant names from the meeting window's accessibility tree and checks them against a preset CSV list.

## Commands

### Build
```powershell
dotnet build WebMeetingParticipantChecker.sln
```

### Run tests
```powershell
dotnet test WebMeetingParticipantChecker.sln
```

### Run a single test class
```powershell
dotnet test WebMeetingParticipantCheckerTests --filter "FullyQualifiedName~MonitoringModelTests"
```

### Publish (self-contained Windows x64)
```powershell
dotnet publish WebMeetingParticipantChecker/WebMeetingParticipantChecker.csproj -c Release -r win-x64 --self-contained
```

## Architecture

### Projects
- `WebMeetingParticipantChecker/` — main WPF app (net8.0-windows10.0.17763.0)
- `WebMeetingParticipantCheckerTests/` — MSTest unit tests using Moq

### Layer structure (MVVM)

**Views** (`Views/`) — XAML + code-behind. `MainWindow` hosts two UserControls: `Preset` (left panel) and `Monitoring` (right panel). `SettingDialog` is a modal. Views use `WeakReferenceMessenger` to receive dialog requests from ViewModels.

**ViewModels** (`ViewModels/`) — use `CommunityToolkit.Mvvm` (`ObservableObject`, `RelayCommand`, `AsyncRelayCommand`). `MonitoringViewModel` is the central VM driving the monitoring workflow.

**Models** (`Models/`) — split into:
- `Config/` — `AppSettingsManager` (static, reads `usersettings.json` via `IConfigurationRoot`). Config file is `usersettings.json` (copied to output). Defaults are hardcoded in each property if the key is missing.
- `Monitoring/` — `MonitoringModel` runs the polling loop (configurable cycle via `MonitoringCycleMs`). Uses `SemaphoreSlim` for pause/resume. `UserState` tracks each participant's join state (auto vs. manual).
- `Preset/` — `PresetModel` reads CSV files from `Preset/` folder next to the exe. Registered as `AddSingleton` in DI. First CSV column = participant name.
- `UIAutomation/` — Two parallel hierarchies:
  - `TargetElementGetter/Auto/` — automatically finds the participant list panel in Zoom/Teams windows by traversing the UI Automation tree.
  - `TargetElementGetter/Manual/` — falls back to focus-change subscription if auto-detection fails.
  - `UserNameGetter/` — `UserNameElementGetter` (abstract base) + `UserNameElementGetterForZoom` / `UserNameElementGetterForTeams`. Zoom uses `UIA_ListItemControlTypeId` on direct children; Teams uses `UIA_TreeItemControlTypeId` on descendants. Zoom names are comma-split (e.g., "Name, Host" → ["Name", "Host"]); Teams names are not split.
- `FileWriter/` — exports monitoring results to CSV via `MonitoringResultExporter`.
- `Theme/` — reads OS dark/light preference from registry; overridable via `ThemeId` in config.

### DI wiring
`App.xaml.cs` builds the `IServiceProvider`. `MonitoringModel` and `PresetModel` are resolved on startup. `PresetModel` is singleton; everything else is transient. Config is loaded from `usersettings.json` before DI setup.

### Name matching logic
`MonitoringModel.RegisterMonitoringTargets` builds a secondary `_searchInfos` list with spaces removed and first/last name swapped (space as delimiter). Matching in `UpdateJoinState` checks both exact key lookup and `Contains` on the detected name dictionary keys. This handles "山田 太郎" vs "太郎 山田" and display name variations.

### Auto-scroll (Zoom only)
When enabled, `ElementScroller` sends arrow-key events to the Zoom participant list to scroll it. `UserNameElementGetter` accumulates detected names across scroll iterations (never clears `_nameInfos` between iterations to avoid missing names when scrolling back).

## Key configuration (`usersettings.json`)
| Key | Default | Notes |
|-----|---------|-------|
| `MonitoringCycleMs` | 2000 | Polling interval in ms |
| `KeydownMaxCount` | 500 | Failsafe max scroll key presses |

> ⚠️ Config-key typo: `AppSettingsManager` reads the key `KeydownMaxCount`, but the shipped `usersettings.json` (and `ConfigrationParameter`) spell it `KyedownMaxCount`. The JSON value is therefore ignored and the default (500) always applies. Fix both spellings if you intend this key to take effect.

| `ThemeId` | 2 (auto) | 0=Dark, 1=Light, 2=follow OS |
| `ZoomRootName` | "Zoom ミーティング" | Window title prefix |
| `ZoomParticipantListName` | "参加者リスト" | Japanese element name |
| `ZoomParticipantListNameEn` | "Participant list" | English fallback |
| `TeamsRootName` | "との会議 \| Microsoft Teams" | Window title suffix |
| `TeamsParticipantListName` | "出席者" | Teams panel name |
