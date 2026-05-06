# Jellyfin Custom Tabs With Effects — Refactoring Brief

## Overview
This plugin adds custom tabs to the Jellyfin web client navbar with visual effects (glow, heartbeat pulse, RGB cycling). It depends on the FileTransformation plugin (v2.2.1.0+) for JS/HTML injection.

## Architecture (After Refactoring)

```
Jellyfin.Plugin.CustomTabs/
├── Attributes/
│   └── JellyfinVersionAttribute.cs      # Assembly attribute storing target Jellyfin version
├── Configuration/
│   ├── PluginConfiguration.cs           # Config model: TabConfig[] with effect settings
│   └── config.html                      # Admin dashboard UI (embedded resource)
├── Controller/
│   └── CustomTabsController.cs          # GET /CustomTabs/Config → returns TabConfig[]
├── Helpers/
│   └── EmbeddedResourceReader.cs        # Utility to read embedded resources by relative path
├── Inject/
│   ├── addCustomTabs.js                 # Client-side script: creates tabs, applies effects
│   └── tabTemplate.html                 # HTML template: {{tab_id}}, {{tab_index}}, {{tab_content}}
├── Model/
│   ├── PatchRequestPayload.cs           # DTO from FileTransformation (single Contents string)
│   └── TransformationRegistration.cs    # DTO for transformation registration metadata
├── Properties/
│   └── AssemblyInfo.cs                  # Assembly metadata + [JellyfinVersion("10.11.2")]
├── Services/
│   ├── StartupService.cs                # IScheduledTask — orchestrates startup registration
│   └── TransformationRegistrar.cs       # Static utility — finds FileTransformation assembly, calls reflection
├── Transformers/
│   ├── IndexHtmlTransformer.cs          # Static: injects addCustomTabs.js before </body>
│   └── HomeHtmlChunkTransformer.cs      # Static: injects tab content divs after favorites tab
├── JellyfinVersionSpecific/
│   ├── 10.10.7/StartupServiceHelper.cs  # Uses TaskTriggerInfo.TriggerStartup (10.10 API)
│   └── 10.11/StartupServiceHelper.cs    # Uses TaskTriggerInfoType.StartupTrigger (10.11 API)
├── CustomTabsPlugin.cs                  # Main entry: BasePlugin<PluginConfiguration>, IHasWebPages, singleton
└── Jellyfin.Plugin.CustomTabs.csproj    # Targets net9.0 (10.11) or net8.0 (10.10.7)
```

## What Changed (Refactoring Summary)

### C# Backend
1. **`Helpers/TransformationPatches.cs` → `Transformers/` folder** — Static class with 3 methods replaced by 2 individual static classes (`IndexHtmlTransformer`, `HomeHtmlChunkTransformer`). Each owns its file pattern and `static Transform()` method. Dead `MainBundle` method removed. Note: FileTransformation invokes callbacks via `MethodInfo.Invoke(null, ...)` so transformer methods **must be static** — C# static classes cannot implement interfaces, so no interface is used.
2. **`Helpers/EmbeddedResourceReader.cs`** — New utility that centralizes embedded resource reading (used by both transformers).
3. **`Model/TransformationRegistration.cs`** — New DTO that encapsulates registration metadata (ID, pattern, assembly, class, method) and serializes to `JObject`.
4. **`Services/TransformationRegistrar.cs`** — New static utility that handles the reflection-based registration with FileTransformation. Extracted from `StartupService.ExecuteAsync` for testability.
5. **`Services/StartupService.cs`** — Simplified to just declare registrations and delegate to `TransformationRegistrar`. Removed unused imports (`ILibraryManager`, `IPlaylistManager`).
6. **`csproj`** — Removed reference to nonexistent `10.11.0/` directory. Simplified redundant `JellyfinNugetVersion` conditions.

### JavaScript Frontend
7. **`Inject/addCustomTabs.js`** — Rewritten from IIFE/object-literal pattern to ES6 classes:
   - `StyleManager` — single `@keyframes` style element management
   - `GlowEffect` — base class with static `boxShadow()` helper
   - `HeartbeatEffect extends GlowEffect` — heartbeat animation
   - `RgbEffect extends GlowEffect` — RGB cycling animation
   - `TabCreator` — DOM creation + effect application
   - `CustomTabsPlugin` — main controller class (replaces `window.customTabsPlugin` object)
   - All event listeners remain identical (behavior unchanged)

### Admin UI
8. **`Configuration/config.html`** — Consolidated 3 near-identical toggle methods (`toggleHeartbeatContent`, `toggleRgbContent`, `toggleGlowSettings`) into single `animateSection(el, show, useHeight)` method.

## Key Design Decisions
- **Static Transformers**: FileTransformation invokes callbacks via `MethodInfo.Invoke(null, ...)` so transformer methods must be static. C# static classes cannot implement interfaces, so transformers are plain static classes with `static FileNamePattern` and `static Transform()` — no interface indirection.
- **Single Responsibility**: Each transformer handles exactly one file pattern. `TransformationRegistrar` handles only the reflection plumbing. `EmbeddedResourceReader` centralizes embedded resource loading.
- **Open/Closed Principle**: Adding a new transformation requires creating a new static class with `FileNamePattern` and `Transform()`, then registering it in `StartupService` — no modification to existing transformers.
- **Inheritance in JS**: `HeartbeatEffect` and `RgbEffect` inherit blur calculations from `GlowEffect`, eliminating duplicate blur math.

## Unchanged Files
- `CustomTabsPlugin.cs` — singleton pattern, plugin ID, page serving (unchanged)
- `Configuration/PluginConfiguration.cs` — config model (unchanged)
- `Controller/CustomTabsController.cs` — API endpoint (unchanged)
- `Model/PatchRequestPayload.cs` — DTO (unchanged)
- `Attributes/JellyfinVersionAttribute.cs` — version attribute (unchanged)
- `JellyfinVersionSpecific/` — version-specific helpers (unchanged)
- `Properties/AssemblyInfo.cs` — assembly metadata (unchanged)
- `Inject/tabTemplate.html` — template string (unchanged)

## Build Notes
- Set `JellyfinVersion` to `10.11.2` for net9.0 / Jellyfin 10.11
- Set `JellyfinVersion` to `10.10.7` for net8.0 / Jellyfin 10.10
- Post-build xcopy targets are local to this machine (paths hardcoded)
