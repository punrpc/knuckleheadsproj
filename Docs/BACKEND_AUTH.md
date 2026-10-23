# Backend sign-in: currently disabled

**Status:** players connect to Photon **anonymously**. `NetworkManager.SignIn()`
(`Unity/Knuckleheads/Assets/Scripts/Assembly-CSharp/Ragdoll/NetworkManager.cs`)
returns `new AuthenticationValues { UserId = GameManager.MyId }`. The original call,
`return await GetAuthentication();`, is commented out right above it, and the whole
original flow (`GetAuthentication`, `AuthenticateWithServer`, `AuthenticateWithPhoton`)
is still in the file, unchanged.

Anonymous auth works because our Photon apps (Fusion `fada2381-…`, Voice `d816a616-…`,
set in `Assets/Resources/PhotonAppSettings.asset`) have no custom authentication
provider configured, so Photon accepts any client that knows the App ID.

## What the shipped game does

`SignIn()` → `GetAuthentication()` runs once and caches a ticket until it nearly expires:

1. **Meta Platform SDK**
   - `Core.AsyncInitialize()`, using the Meta app ID in `Assets/Resources/OculusPlatformSettings.asset`.
     This is still the original game's ID `1279889038548702` and must become ours.
   - `Entitlements.IsUserEntitledToApplication()`: the user must own the app.
   - `Users.GetLoggedInUser()`: gives the user ID, plus the display name stored as
     `metaName`. That name is used for the player nick and the "NAME'S ROOM" title.
   - `Users.GetOrgScopedID(user.ID)`: gives the org-scoped ID.
   - `Users.GetUserProof()`: gives a one-time nonce.
   - `DeviceApplicationIntegrity.GetIntegrityToken(nonce)`: gives an attestation token.
2. **Game backend.** `POST {AuthBaseUrl}/knuckle/auth/bootstrap`, JSON body:
   ```json
   { "UserId": "<org-scoped id>", "OculusId": "<meta user id>", "Nonce": "<user proof>", "IntegToken": "<integrity token>" }
   ```
   - The response must be `{ "Success": true, "UserId": "<same org-scoped id>", "Ticket": "<string>", "ExpiresAt": <unix seconds> }`.
   - The client rejects the response if `Success` is false, `UserId` doesn't match, `Ticket` is empty, or `ExpiresAt <= 0`.
   - It refreshes the ticket 60 s before `ExpiresAt`, measured against the server's `Date` header.
   - `AuthBaseUrl` is a constant in `NetworkManager`. It currently points at the **original
     developer's server**; never ship a build that talks to it.
3. **Photon custom auth.** `AuthType = Custom`, `UserId = <org-scoped id>`, and POST data
   `{ "UserId": ..., "Ticket": ... }`. Photon forwards that to whatever URL the Photon
   dashboard has configured for the app. The client doesn't contain that URL, so we don't
   know the original verify endpoint's path.

The same auth values are used for Fusion sessions, the session browser
(`JoinSessionLobby`) and Photon Voice (`Revoice()` sets `voice.Client.AuthValues`).

## To turn it back on with our own backend

1. **Meta app:** create our own app on the Meta developer dashboard and put its App ID in
   `OculusPlatformSettings.asset`. Entitlement checks only pass for builds uploaded to that app.
2. **Backend:**
   - `POST /knuckle/auth/bootstrap` (or any path; update the URL in `AuthenticateWithServer`):
     - Verify the nonce with Meta's `user_nonce_validate` Graph API (needs our app's
       access token, server-side only).
     - Verify the integrity token with Meta's attestation API.
     - Issue a signed, expiring ticket bound to the org-scoped ID, and return the JSON above.
   - Add a Photon custom-auth endpoint that receives `UserId` + `Ticket` and replies
     `{ "ResultCode": 1, "UserId": "<id>" }` when the ticket is valid, or `ResultCode: 2` when it isn't.
3. **Photon dashboard:** on both the Fusion and the Voice app, add a Custom Authentication
   provider pointing at that endpoint, and turn off "allow anonymous clients".
4. **Code:**
   - Set `AuthBaseUrl` to our server.
   - In `SignIn()`, restore `return await GetAuthentication();` and delete the two anonymous lines.
   - Optionally key `GameManager.MyId` (currently a random 8-char ID per launch) to the
     authenticated user, so mutes and rejoin survive restarts.

## Names while auth is off

`SignIn()` calls `AssignRandomName()`, which sets `metaName` to a random `PLAYER####`
(for example `PLAYER1919`) once per launch. That name is used for the player nickname
(`NetworkPlayer.Nick`) and the "NAME'S ROOM" title. Nothing talks to Meta. Once there is
a real Meta app ID and backend, the shipped `GetAuthentication()` fills `metaName` from
the Meta display name / Oculus ID again.

## Side effects while auth is off

- No entitlement check: anyone with the build and our App IDs can join.
- Player identity (`GameManager.MyId`) is random per launch. That was already true in the
  shipped game; only the Photon `UserId` changes.
