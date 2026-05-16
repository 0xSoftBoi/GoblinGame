# GOBLIN CHAIN — S&box Editor Setup Guide

One-shot setup. Follow this top-to-bottom after pasting `goblin_chain/` into your S&box addons folder.

---

## Step 0: Install

1. Copy the entire `goblin_chain/` folder to `C:\Program Files (x86)\Steam\steamapps\common\sbox\addons\goblin_chain\`
2. Open S&box Editor
3. Select "goblin_chain" from project list (it reads the `.sbproj`)
4. Wait for compile — all `.cs` and `.razor` files auto-compile

---

## Step 1: Custom Input Actions

Go to **Project Settings → Input** and add these actions:

| Name | Type | Default Binding |
|------|------|----------------|
| `Trade` | Button | `T` |
| `OpenMarket` | Button | `M` |
| `Sabotage` | Button | `Q` |
| `PlaceRig` | Button | `E` |
| `CycleToken` | Button | `Tab` |

(Standard actions like Forward/Jump/Crouch/Use/Voice are already built-in.)

---

## Step 2: Create the Player Prefab

1. **File → New Prefab** → name it `Player.prefab`, save to `prefabs/`
2. Root GameObject "Player":
   - Add component: **CharacterController**
     - Height: 72, Radius: 16
   - Add component: **ModelRenderer**
     - Model: `models/citizen/citizen.vmdl` (built-in citizen model)
   - Add component: **CitizenAnimationHelper**
   - Add component: **GoblinPlayer** (your code)
   - Add component: **PlayerInput** (your code)
     - Drag `EMP.prefab` (from Step 3) into EMPGrenadePrefab slot
   - Add component: **CryptoWallet** (your code)
   - Add component: **SabotageInventory** (your code)
   - Add component: **ProximityVoice** (your code)
   - Add component: **PlayerNametag** (your code)
   - Add component: **Voice** (built-in S&box voice)
3. Create child GameObject "Head":
   - Position: (0, 0, 64) — eye height
   - No components needed, just a transform target
4. On the **GoblinPlayer** component:
   - Drag "Head" child → **Head** slot
   - Drag "Player" root → **Body** slot
5. Save the prefab

---

## Step 3: Create the EMP Grenade Prefab

1. **File → New Prefab** → name it `EMP.prefab`, save to `prefabs/`
2. Root GameObject "EMP":
   - Add component: **Rigidbody**
     - Mass: 1.0, Gravity: true
   - Add component: **SphereCollider**
     - Radius: 6
   - Add component: **ModelRenderer**
     - Use any small prop model (or a placeholder sphere)
   - Add component: **EMPGrenade** (your code)
3. Save the prefab

---

## Step 4: Create the Mining Rig Prefab

1. **File → New Prefab** → name it `MiningRig.prefab`, save to `prefabs/`
2. Root GameObject "MiningRig":
   - Add component: **ModelRenderer**
     - Use a server/computer model or placeholder box
   - Add component: **BoxCollider**
     - Size to match your model
   - Add component: **Rigidbody**
     - Mass: 50, or check "Static" if you want rigs immovable
   - Add component: **MiningRig** (your code)
   - Add component: **MiningRigScreen** (your code, in UI folder)
3. Save the prefab

---

## Step 5: Open the Main Scene

`scenes/main.scene` is pre-built and checked in. Open it in the S&box Editor:

1. **File → Open Scene** → select `scenes/main.scene`
2. On the **GameManagers** object, wire these asset references (they can't be stored in the JSON — drag them in):
   - **GoblinChainGame → PlayerPrefab**: drag `Player.prefab`
   - **NetworkedRigSpawner → RigPrefab**: drag `MiningRig.prefab`
   - **PlayerInput → EMPGrenadePrefab**: set on the Player prefab, not the scene
3. The `GameManagers` object contains all 12 system components:
   - GoblinChainGame, GameStateManager, CryptoMarket, TokenSystem, GoblinTwitter,
     TradingSystem, RandomEvents, SECSystem, SocialDeduction, NetworkedRigSpawner,
     NPCInvestors, MarketManipulation, OfficeSetup
4. The scene includes 8 SpawnPoints and a box-geometry office room (floor, 4 walls, ceiling).
5. For a more polished office, replace the dev-box geometry with actual props — OfficeSetup.cs will
   handle spawning desks/chairs/monitors when you wire up the prefab lists in the editor.

> **Note:** `DebugSkipMinPlayers` is ON by default in the scene — turn it off before workshop upload.

---

## Step 6: HUD Scene (Pre-Built)

`scenes/hud.scene` is pre-built and checked in. It is loaded **automatically** by `GoblinChainGame.OnStart()` — no manual wiring needed.

It contains a `ScreenPanel` root with these child panels:
- GoblinHud, CryptoTicker, TradePanel, PhaseAnnouncement, NotificationFeed, ResultsScreen
- GoblinPhone, TokenCreator, RugPullPrompt, SECRaidPanel, AuditVote

No editor work required for the HUD.

---

## Step 7: Create the Menu Scene

1. **File → New Scene** → save as `scenes/menu.scene`
2. Create "MenuRoot" GameObject:
   - Add component: **ScreenPanel**
3. Create child "MainMenu":
   - Add component: **MainMenu** (your .razor)
4. Save
5. Set this as the **default scene** in Project Settings so the game boots to the menu

---

## Step 8: Test

1. **Single player test:** Set `DebugSkipMinPlayers = true` on GoblinChainGame
2. Hit Play in the editor
3. You should:
   - Spawn at a SpawnPoint with a citizen model
   - See the HUD (wallet, timer, leaderboard)
   - See the crypto ticker scrolling at the bottom
   - Be able to walk around (WASD), jump (Space), sprint (Shift)
   - Press E near a surface to place a rig (during Mining phase)
   - See mining rigs with floating WorldPanel status screens
   - See the phase cycle through: Pregame → Mining → Trading → Chaos → Results
4. **Multiplayer test:** Launch two instances of S&box, one hosts, one joins

---

## Step 9: Sounds (Placeholder)

The code references these `.sound` files. Create them in `sounds/` or they'll silently fail (no crash):

| Sound File | When It Plays |
|---|---|
| `sounds/phase_transition.sound` | Phase changes |
| `sounds/market_crash.sound` | Market crashes |
| `sounds/market_moon.sound` | Market moons |
| `sounds/trade_complete.sound` | Trade executed |
| `sounds/rig_placed.sound` | Rig placed |
| `sounds/rig_upgrade.sound` | Rig upgraded |
| `sounds/emp_blast.sound` | EMP detonation |
| `sounds/emp_throw.sound` | EMP thrown |
| `sounds/winner.sound` | Winner announced |
| `sounds/event_positive.sound` | Positive random event |
| `sounds/event_negative.sound` | Negative random event |

For each: create a `.sound` file in the Asset Browser (right-click → New → Sound), reference a `.wav` or `.mp3` source file.

---

## Quick Debug Checklist

- [ ] Project compiles with no errors
- [ ] Player prefab has all components listed in Step 2
- [ ] EMP prefab has Rigidbody + SphereCollider + EMPGrenade
- [ ] MiningRig prefab has Collider + MiningRig + MiningRigScreen
- [ ] `scenes/main.scene` loads without errors in the editor
- [ ] `scenes/hud.scene` loads without errors in the editor
- [ ] **GoblinChainGame → PlayerPrefab** is wired (drag Player.prefab in editor)
- [ ] **NetworkedRigSpawner → RigPrefab** is wired (drag MiningRig.prefab in editor)
- [ ] **PlayerInput → EMPGrenadePrefab** is wired on the Player prefab
- [ ] Main scene has 8 SpawnPoints (pre-placed at startup — verify positions)
- [ ] HUD auto-loads (GoblinChainGame.OnStart calls LoadHud — check log for errors)
- [ ] Custom input actions are defined in Project Settings (see Step 1)
- [ ] `DebugSkipMinPlayers` is ON for solo testing, OFF before workshop upload

---

## Component Quick Reference

### Player Prefab Components
```
CharacterController     (built-in)
ModelRenderer           (built-in, citizen model)
CitizenAnimationHelper  (built-in)
Voice                   (built-in)
GoblinPlayer            (code/Player/)
PlayerInput             (code/Player/)
CryptoWallet            (code/Systems/)
SabotageInventory       (code/Player/)
ProximityVoice          (code/Player/)
PlayerNametag           (code/UI/)
```

### Main Scene Manager Components (all on "GameManagers" object)
```
GoblinChainGame         (code/)
GameStateManager        (code/Systems/)
CryptoMarket            (code/Systems/)
TokenSystem             (code/Systems/)   ← token creation + rug pull engine
GoblinTwitter           (code/Systems/)   ← shill feed + NPC posts
TradingSystem           (code/Systems/)
RandomEvents            (code/Systems/)
SECSystem               (code/Systems/)   ← heat meter + raid events
SocialDeduction         (code/Systems/)   ← secret Rugger role + Grand Rug
NetworkedRigSpawner     (code/Systems/)
NPCInvestors            (code/Systems/)   ← 50 NPC market bots
MarketManipulation      (code/Systems/)   ← wash trading / pump groups
OfficeSetup             (code/Environment/) ← procedural office layout
```

### HUD Scene Panel Components (auto-loaded by GoblinChainGame)
```
ScreenPanel             (built-in)
├── GoblinHud           (code/UI/)
├── CryptoTicker        (code/UI/)
├── TradePanel          (code/UI/)
├── PhaseAnnouncement   (code/UI/)
├── NotificationFeed    (code/UI/)
├── ResultsScreen       (code/UI/)
├── GoblinPhone         (code/UI/)        ← T key: shill feed + compose posts
├── TokenCreator        (code/UI/)        ← launch your meme token
├── RugPullPrompt       (code/UI/)        ← chaos-phase rug/pivot/hold decision
├── SECRaidPanel        (code/UI/)        ← full-screen raid overlay
└── AuditVote           (code/UI/)        ← vote on suspected Rugger
```

### MiningRig Prefab Components
```
ModelRenderer           (built-in)
BoxCollider             (built-in)
Rigidbody               (built-in, optional)
MiningRig               (code/Entities/)
MiningRigScreen         (code/UI/)
```

### EMP Prefab Components
```
Rigidbody               (built-in)
SphereCollider          (built-in)
ModelRenderer           (built-in)
EMPGrenade              (code/Entities/)
```
