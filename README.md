# Knuckleheads (Quest): Unity project

A Unity 6 project of **Knuckleheads** (`com.LoosetoothGames.Knuckleheads`, version 36), a
physics ragdoll brawler for Meta Quest built on Photon Fusion. It compiles, connects online
and builds for Quest.

This is a private learning and sideloading project. The game, its art and its code belong to
Loosetooth Games. Don't publish builds of it.

## Layout

| Path | What it is |
|---|---|
| (this folder) | The Unity project. Open it in Unity **6000.2.7f2**. |
| `Assets/Scripts/Assembly-CSharp/` | Game code (`Ragdoll.*`, `Island`). |
| `Assets/Shader/` | The game's custom URP shaders. |
| `Assets/Scenes/` | The 11 build scenes, from MainMenu and Tutorial to the maps. |
| `Assets/Photon/` | Photon Fusion 2.1.1 and Photon Voice 2.64 (Realtime 5). |
| `Packages/` | Package manifest, plus the embedded LIV Creator Kit 1.4.7. |
| `Docs/BACKEND_AUTH.md` | Notes on the online sign-in, which is currently off. |

## Packages

- **Unity registry:** URP 17.2, Input System, XR Interaction Toolkit 3.2, OpenXR 1.15,
  XR Hands, Burst, Collections
- **Meta:** Meta XR Core and Platform SDK 207, from Meta's scoped registry
- **Photon:** Fusion 2.1.1 and Voice 2.64, Realtime 5 edition
- **Other:** LIV Creator Kit 1.4.7, Cartoon FX Remaster (`Assets/JMO Assets`), TextMeshPro
- **Editor tooling:** MCP for Unity, which lets Claude Code drive the editor

## Game code notes

- Networked classes use the normal Fusion source style (`[Networked]` properties, `[Rpc]`
  methods). Fusion weaves `Assembly-CSharp` and `Meta.XR.MultiplayerBlocks.Fusion` on compile.
- **Additions that aren't part of the original game:**
  - `Ragdoll/DiscordLog.cs` posts player joins and leaves to a Discord webhook.
  - `Ragdoll/DeskControlsHud.cs` shows the on-screen PC controls panel.
  - `NetworkManager.SignIn()` signs in anonymously and gives players random `PLAYER####` names.

## Networking

- **Photon App IDs** for Fusion and Voice are in `Assets/Resources/PhotonAppSettings.asset`.
  The region comes from the in-game picker and defaults to `cae`.
- **Sign-in is off.** Clients join Photon anonymously. The original Meta and backend sign-in
  is still in `NetworkManager.cs`, commented out. `Docs/BACKEND_AUTH.md` explains how to turn it back on.
- **Discord webhook:** the URL is in `DiscordLog.cs` and ships inside the APK, so regenerate it if it leaks.
- **Joining** is done from the menu TV: Quick Join, room code or create room. You have to
  finish the tutorial before you can join.

## Building for Quest

The project is already set up like the original build:
- Android, IL2CPP ARM64, Vulkan only, ASTC, minSdk 32 / targetSdk 34, version 36.
- OpenXR on Android, Single Pass Instanced, with Meta Quest Support, Meta XR, foveation,
  performance settings, the game's 90 Hz refresh-rate feature, and the Touch / Touch Pro /
  Touch Plus controller profiles.
- Target devices: Quest 2, Pro, 3 and 3S.

To build:
1. Open this folder with Android Build Support installed.
2. **File → Build Profiles**, choose Android (already active), then **Build**.
3. Sideload the APK with `adb install -r Knuckleheads.apk` or Meta Quest Developer Hub.

## Playing in the editor (no headset)

Flat-screen controls switch on automatically when no headset is tracking. Click the Game view
first so the mouse locks. An on-screen panel lists the controls; **Insert** hides and shows it.

| Key | Action | Key | Action |
|---|---|---|---|
| Mouse | Look | WASD | Move |
| Left Shift | Sprint | Left Ctrl | Crouch |
| F (hold) | Charged jump | R | Recentre |
| Space | Pull arms back (wind-up) | Q | Raise hands |
| LMB / RMB | Grab left / right | E | Grab both |
| Esc | Unlock mouse (hold to leave) | Y / N / S | Answer prompts / settings |

## Known gaps

- **Not checked on a headset yet.** Online play is only verified in the editor.
- **Rotation order:** a few rotation orders in movement, getting up, aiming, Drawbridge and
  TrainWreck may need adjusting if something turns the wrong way.
- **Meta app ID:** `Assets/Resources/OculusPlatformSettings.asset` still has the original
  game's Meta app ID. It's unused while sign-in is off; replace it with your own before turning auth back on.
- **Editor-only code:** a few editor-only tools from the original aren't present, such as
  MeshBake's preview. They're marked `TODO(recon)`.
