# GOBLIN CHAIN: Crypto Chaos Tycoon
## 27-Day Implementation Battle Plan
### Ship Date: April 28, 2026 | Solo Dev + Claude AI | Zero Budget

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Day-by-Day Development Schedule](#2-day-by-day-development-schedule)
3. [Core Systems Architecture](#3-core-systems-architecture)
4. [MVP vs Nice-to-Have (Scope Cut Decision Tree)](#4-mvp-vs-nice-to-have)
5. [Asset Pipeline](#5-asset-pipeline)
6. [Testing Plan](#6-testing-plan)
7. [Launch Day Checklist](#7-launch-day-checklist)
8. [Found Footage Collection Plan](#8-found-footage-collection-plan)
9. [Pre-Launch Community Building Strategy](#9-pre-launch-community-building-strategy)
10. [Risk Mitigation](#10-risk-mitigation)
11. [Week-by-Week Milestones with Go/No-Go Gates](#11-week-by-week-milestones)

---

## 1. Executive Summary

GOBLIN CHAIN is a satirical crypto developer simulator built in S&box (Source 2 engine, C#). Players are goblins running shady crypto startups — creating tokens, manipulating markets, posting on GoblinTwitter, and trying to rug pull each other. One goblin is secretly planning the ultimate rug pull. The game is designed to go viral through its built-in clip recorder (Content Warning model), proximity voice chat chaos, and inherently shareable social deduction moments.

**Tech Stack:** S&box (Source 2 / .NET 9.0 / C# 11), Box3D physics, S&box networking (host-authoritative with `[Sync]` and RPCs), S&box cloud asset library.

**Monetization:** S&box Play Fund (earn based on playtime — no microtransactions, no ads).

**The One Sentence Pitch:** "Among Us meets Crypto Bros meets Garry's Mod, and every round generates a YouTube video."

---

## 2. Day-by-Day Development Schedule

### PHASE 1: FOUNDATION (Days 1–5) — "Build the Office"

**Day 1 — April 1 (Tuesday): Project Skeleton + Networking**
- Create S&box project, configure `.sbproj`
- Set up Git repository with `.gitignore` for S&box
- Implement `GoblinGameManager` component (extends `Component`) — game state machine (Lobby → Working → Trading → Voting → Results)
- Implement basic networking: host creation, client join via `[Sync]` properties
- Test: two editor instances connecting via S&box multiplayer testing (Launch → Multiplayer → Create Lobby + Join)
- **Deliverable:** Two goblins in an empty room, networked

**Day 2 — April 2 (Wednesday): Player Controller + Goblin Character**
- `GoblinPlayerController` component: WASD movement, mouse look, jump
- Network player position/rotation with `[Sync]` attributes
- Import goblin placeholder model from S&box cloud asset library (search for any humanoid character — reskin later)
- Basic third-person camera rig
- **Deliverable:** Goblins walking around, visible to each other

**Day 3 — April 3 (Thursday): The Office Map (Greybox)**
- Greybox the core map in S&box Hammer editor:
  - Main office floor (cubicles, server room, break room)
  - Trading floor (screens, desks)
  - CEO's office (the rug pull room)
  - Rooftop (for dramatic moments)
- Add spawn points, nav mesh basics
- Collision volumes, basic lighting
- **Deliverable:** Walkable greyboxed office

**Day 4 — April 4 (Friday): Interaction System + UI Framework**
- `InteractionSystem` component: raycast from player, detect `IInteractable` interface
- Implement `IInteractable` on doors, chairs, computers
- Basic HUD: player name, money display, token portfolio
- S&box Panel UI system for menus (Razor-based UI)
- `GameHUD.razor` — main HUD overlay
- **Deliverable:** Players can interact with objects, see basic HUD

**Day 5 — April 5 (Saturday): Proximity Voice Chat**
- Implement `VoiceProximity` component using S&box's built-in voice API
- Distance-based volume falloff (loud < 5m, whisper < 15m, silent > 15m)
- Voice indicator UI (who's talking, volume level)
- Special zones: CEO's office is soundproof, break room has echo
- Test with two instances
- **Deliverable:** Proximity voice chat working — THE viral mechanic is live

---

### PHASE 2: CORE MECHANICS (Days 6–12) — "Build the Hustle"

**Day 6 — April 6 (Sunday): Token Creation System**
- `TokenFactory` component: create new tokens with name, ticker, supply, logo
- `Token` data class: `Name`, `Ticker`, `TotalSupply`, `CirculatingSupply`, `Price`, `CreatorId`
- Token creation UI panel (name generator with goblin-themed defaults: $GOBCOIN, $RUGME, $MOONRAT)
- Network token state via `[Sync]` on the `TokenManager` component
- Store tokens in a networked list on the host
- **Deliverable:** Players can create tokens that all players can see

**Day 7 — April 7 (Monday): Market Simulation Engine**
- `MarketSimulator` component (runs on host):
  - Price = f(buy_pressure, sell_pressure, hype, manipulation, random_noise)
  - Buy/sell order book (simplified — instant execution, no limit orders for MVP)
  - Price history stored as `List<PricePoint>` for charts
- `TradingTerminal` interactable: opens trading UI
- Simple line chart UI for price history (Razor panel with canvas drawing)
- Market events: random pumps, dumps, "whale alert" notifications
- **Deliverable:** Tokens have fluctuating prices, players can buy/sell

**Day 8 — April 8 (Tuesday): GoblinTwitter — The Core Mechanic**
- `GoblinTwitter` component: the in-game social media feed
- `Tweet` data class: `Author`, `Content`, `Timestamp`, `Likes`, `Retweets`, `Effect`
- Tweet composition UI: text input + optional token mention ($TICKER auto-links)
- Feed UI: scrollable timeline, real-time updates via RPC broadcast
- Tweet effects on market: mentioning a $TICKER creates buy pressure proportional to author's influence
- Influence system: `GoblinInfluence` score grows with followers, successful calls
- **Deliverable:** Players can tweet, see each other's tweets, tweets move markets

**Day 9 — April 9 (Wednesday): GoblinTwitter Advanced + Bot System**
- NPC bot accounts that auto-tweet (procedurally generated hype/FUD)
- Reply system (reply to tweets, quote tweet)
- Trending topics sidebar based on most-mentioned tickers
- "Verified" badge system (costs in-game money — satire of Twitter Blue)
- Tweet templates for quick posting: "🚀 $TICKER TO THE MOON", "Just rugged lmao", "Not financial advice but..."
- **Deliverable:** GoblinTwitter feels alive with bots and social dynamics

**Day 10 — April 10 (Thursday): Social Deduction — The Rug Puller**
- `RugPullRole` component: at round start, one player is secretly the Rug Puller
- Rug Puller gets secret objectives (accumulate X tokens, get Y influence, crash Z token)
- Rug Puller special ability: can "pull the rug" once — crashes their token to 0, steals the liquidity pool
- Innocent goblins win by: identifying the rug puller via vote, OR everyone surviving with profit
- Rug Puller wins by: executing the rug pull undetected, OR being the richest after chaos
- Voting system: `VotingPanel` UI, majority vote kicks a player
- Emergency meeting: any player can call one per round (like Among Us)
- **Deliverable:** Complete social deduction loop — someone IS the rug puller

**Day 11 — April 11 (Friday): Physics Comedy + Desk Flipping**
- `PhysicsInteraction` component: grab and throw objects using Box3D physics
- Desk flip mechanic: interact with desk → physics impulse → everything flies
- Domino effect system: objects near a flipped desk get impulse too
- Physics props: monitors, keyboards, coffee cups, server racks, money printers
- Ragdoll on player "bankruptcy" (net worth hits 0)
- **Deliverable:** Source 2 physics chaos — clips waiting to happen

**Day 12 — April 12 (Saturday): Round Structure + Game Loop**
- Complete round flow:
  1. **Lobby** (30s): Players join, see goblin avatars, ready up
  2. **Morning Meeting** (60s): Market opens, roles revealed privately, CEO announces theme
  3. **Work Phase** (5min): Create tokens, trade, tweet, investigate, scheme
  4. **Emergency Meeting** (optional): Called by player, discuss suspicions
  5. **Market Close** (60s): Final trades, portfolio tallied
  6. **Voting Phase** (90s): Vote on who's the rug puller
  7. **Reveal** (30s): Was the vote right? Rug puller revealed, profits/losses shown
  8. **Results** (30s): Leaderboard, MVP awards, clip highlights
- `RoundManager` component orchestrating phase transitions via RPC
- Timer UI, phase transition animations
- **Deliverable:** Full playable round from lobby to results

---

### PHASE 3: CONTENT CREATION ENGINE (Days 13–16) — "Make It Viral"

**Day 13 — April 13 (Sunday): In-Game Clip Recorder**
- `ClipRecorder` component: captures last N seconds of gameplay
- Uses S&box's rendering pipeline to capture frames
- Approach: Record game state (positions, events, chat) → replay system renders the "clip"
  - Fallback: simple screen capture to video file if replay too complex
- Clip triggers: manual button, auto-clip on big moments (rug pull, desk flip, bankruptcy)
- Save clips locally as video files
- **Deliverable:** Players can capture clips of their gameplay

**Day 14 — April 14 (Monday): Clip Viewer + SpöökTube Parody**
- `GoblinTube` in-game clip viewer (parody of SpöökTube from Content Warning)
- End-of-round clip reel: auto-compiled highlights
- Clip rating system: other players vote on clips (drives "views")
- "Views" as secondary currency / score
- Clip export: save to local file for sharing on real social media
- **Deliverable:** End-of-round clip viewing experience

**Day 15 — April 15 (Tuesday): Found Footage System**
- `FoundFootageManager` component: manages in-game documentary artifacts
- In-game TV screens that play "documentary footage" (text crawls + images, not actual video for file size)
- SEC filing props: physical documents scattered in the office, readable via interaction
- "Breaking News" ticker on office TVs — procedurally generated from game events
- News anchors (text + portrait) reporting on player actions ("GOBLIN CEO ACCUSED OF INSIDER TRADING")
- **Deliverable:** The office feels alive with found footage artifacts

**Day 16 — April 16 (Wednesday): Audio + Music + SFX**
- Sound design pass using freesound.org assets:
  - UI sounds (tweet sent, market ding, buy/sell confirm)
  - Ambient office sounds (keyboard typing, printer, coffee machine)
  - Dramatic stings (rug pull reveal, bankruptcy, emergency meeting)
  - Physics sounds (desk flip crash, glass break, server rack topple)
- Background music: lo-fi goblin hip-hop (procedurally arrange loops from free samples)
- Voice proximity SFX: radio crackle at edge of range
- **Deliverable:** Game sounds professional and polished

---

### PHASE 4: POLISH + MULTIPLAYER HARDENING (Days 17–22) — "Make It Work"

**Day 17 — April 17 (Thursday): Lobby System + Matchmaking**
- `LobbyManager` component: create/join/browse lobbies
- S&box lobby system integration (built-in matchmaking)
- Lobby UI: player list, ready status, game settings (round length, player count)
- Support 4–8 players per lobby
- Host migration (if host leaves, next player becomes host)
- Quick Play: auto-join available lobby
- **Deliverable:** Players can find and join games

**Day 18 — April 18 (Friday): Discord Rich Presence**
- Integrate Discord Game SDK (C# bindings via NuGet or direct DLL)
- Show current state: "In Lobby (3/8)", "Trading Phase — Round 2", "Watching Clips"
- "Ask to Join" / "Spectate" buttons in Discord
- Invite links that deep-link into S&box lobby
- **Deliverable:** Discord integration for organic discovery

**Day 19 — April 19 (Saturday): Visual Polish Pass**
- Replace greybox with themed assets from S&box cloud library:
  - Office furniture (desks, chairs, monitors, whiteboards)
  - Server room equipment
  - Goblin decorations (gold coins, moon posters, "HODL" signs)
- Lighting pass: moody office lighting, green tint for goblin aesthetic
- Particle effects: money explosion on rug pull, confetti on win
- Screen shake on desk flips
- Post-processing: slight film grain for found footage feel
- **Deliverable:** Game looks like a real game, not a prototype

**Day 20 — April 20 (Sunday): UI Polish Pass**
- Final UI art pass on all panels:
  - GoblinTwitter (dark theme, goblin-green accents)
  - Trading terminal (Bloomberg terminal parody — green/black)
  - Voting screen (dramatic red lighting)
  - Results screen (with meme-worthy stats)
- Animations: tweet slide-in, price ticker scroll, vote count reveal
- Tutorial tooltips for first-time players
- Settings menu (audio, graphics, keybinds)
- **Deliverable:** UI is intuitive and memeable

**Day 21 — April 21 (Monday): Multiplayer Stress Test + Bug Bash**
- Test with 8 players (recruit from S&box Discord, friends, alt accounts)
- Stress test scenarios:
  - All players tweeting simultaneously
  - Mass market manipulation (everyone buys same token)
  - Rug pull during emergency meeting
  - Physics chaos (everyone flipping desks at once)
- Network desync hunting: verify all `[Sync]` properties stay consistent
- Performance profiling: target 60fps with 8 players
- **Deliverable:** List of critical bugs, performance baseline

**Day 22 — April 22 (Tuesday): Critical Bug Fix Day**
- Fix all P0 bugs from Day 21 testing
- Network edge cases: late joiners, disconnections, host migration
- Physics stability: prevent objects clipping through walls
- Market simulation edge cases: prevent negative prices, overflow
- Memory leaks: ensure clips don't eat all RAM
- **Deliverable:** Stable build ready for external testing

---

### PHASE 5: LAUNCH PREP (Days 23–27) — "Ship It"

**Day 23 — April 23 (Wednesday): Content + Balance Pass**
- Tune market simulation values (volatility, manipulation power, bot frequency)
- Balance social deduction (rug puller shouldn't be too obvious OR too invisible)
- Add more tweet templates, bot personalities, token name generators
- Final found footage content: 10+ SEC filing props, 5+ news story templates, 20+ breaking news tickers
- Achievement-like milestones: "First Rug Pull", "Diamond Hands", "Wolf of Goblin Street"
- **Deliverable:** Game feels balanced, content-rich, replayable

**Day 24 — April 24 (Thursday): Workshop Publishing Prep**
- Create S&box project thumbnail (1280x720 — goblin + crypto aesthetic)
- Write project description with screenshots and feature list
- Record 60-second gameplay trailer (in-engine, use clip system)
- Create project tags: multiplayer, comedy, social-deduction, tycoon
- Test publish to S&box as "Private" — verify it uploads and downloads correctly
- **Deliverable:** Workshop page ready, just needs the "Public" toggle

**Day 25 — April 25 (Friday): External Playtest**
- Recruit 8–16 players from S&box Discord community for a 2-hour playtest
- Record full session for bug reports and clip material
- Collect feedback via Google Form: fun rating, confusion points, bugs, feature requests
- Stream the playtest on Twitch/YouTube for early visibility
- **Deliverable:** Real player feedback, launch trailer footage

**Day 26 — April 26 (Saturday): Final Fixes + Launch Build**
- Fix critical issues from Day 25 playtest (budget 8 hours max)
- Final optimization pass
- Lock the build — no new features, only bug fixes
- Prepare launch announcement posts (Discord, Twitter/X, Reddit r/sbox, r/indiegaming)
- Write patch notes / changelog for v1.0
- Upload final build to S&box Workshop (still Private)
- **Deliverable:** Gold master build uploaded, launch posts drafted

**Day 27 — April 27 (Sunday): Pre-Launch Day**
- Final smoke test: play through 3 full rounds
- Verify Discord Rich Presence works in production
- Schedule social media posts for launch morning
- Set up Discord server with channels (#general, #bug-reports, #clips, #suggestions)
- Prepare Day 1 hotfix workflow (know how to push Workshop updates fast)
- Mental prep: sleep, eat, hydrate
- **Deliverable:** Everything ready. Launch is tomorrow.

**LAUNCH — April 28 (Monday):**
- Set Workshop project to "Public" at 10:00 AM PST
- Post announcements everywhere
- Monitor Discord and Steam discussions
- Hotfix anything critical within 2 hours
- Engage with early clips/content on social media

---

## 3. Core Systems Architecture

### High-Level Component Diagram

```
Scene Root
├── GameManager (Component)
│   ├── RoundManager          — Phase state machine, timers
│   ├── TokenManager          — All tokens, networked list
│   ├── MarketSimulator       — Price engine, order execution
│   ├── GoblinTwitterManager  — Tweet storage, feed, bot system
│   ├── FoundFootageManager   — News generation, artifact placement
│   ├── VotingManager         — Vote collection, results
│   ├── ClipRecorder          — Game state recording, export
│   └── LobbyManager          — Player slots, ready states
│
├── Player Prefab (NetworkObject)
│   ├── GoblinPlayerController — Movement, camera, input
│   ├── PlayerIdentity         — Name, avatar, role [Sync]
│   ├── Portfolio              — Token holdings, cash [Sync]
│   ├── GoblinInfluence        — Social media stats [Sync]
│   ├── VoiceProximity         — Spatial voice processing
│   ├── InteractionSystem      — Raycast + IInteractable
│   └── PhysicsInteraction     — Grab, throw, desk flip
│
├── Map
│   ├── Office (static geometry)
│   ├── InteractableObjects[]  — Desks, computers, TVs
│   ├── TradingTerminals[]     — Market access points
│   ├── TwitterKiosks[]        — GoblinTwitter access points
│   └── FoundFootageProps[]    — SEC filings, news screens
│
└── UI (Razor Panels)
    ├── GameHUD.razor          — Money, phase, timer
    ├── GoblinTwitterPanel.razor — Tweet feed + compose
    ├── TradingPanel.razor     — Charts, buy/sell
    ├── VotingPanel.razor      — Player portraits, vote buttons
    ├── ClipViewer.razor       — End-round clip reel
    ├── LobbyPanel.razor       — Join/create/browse
    └── ResultsPanel.razor     — Leaderboard, stats, awards
```

### Key C# Classes and Their Relationships

```csharp
// === CORE GAME LOOP ===

public sealed class RoundManager : Component
{
    [Sync] public GamePhase CurrentPhase { get; set; }
    [Sync] public float PhaseTimeRemaining { get; set; }
    [Sync] public int RoundNumber { get; set; }

    // Phase enum: Lobby, MorningMeeting, WorkPhase,
    // EmergencyMeeting, MarketClose, Voting, Reveal, Results

    [Rpc.Broadcast]
    public void OnPhaseChanged(GamePhase phase) { }
}

// === NETWORKING PATTERN ===
// Host runs simulation, clients get [Sync] updates
// Player actions sent via [Rpc.Host], results broadcast via [Rpc.Broadcast]

public sealed class TokenManager : Component
{
    [Sync] public NetList<Token> Tokens { get; set; }

    [Rpc.Host]
    public void RequestCreateToken(string name, string ticker, int supply) { }

    [Rpc.Broadcast]
    public void OnTokenCreated(Token token) { }
}

public sealed class MarketSimulator : Component
{
    // Runs on host only — updates prices every tick
    // Price = BasePrice * (1 + BuyPressure - SellPressure + HypeFactor + Noise)

    [Rpc.Host]
    public void RequestBuy(string ticker, int amount) { }

    [Rpc.Host]
    public void RequestSell(string ticker, int amount) { }

    [Rpc.Broadcast]
    public void OnPriceUpdate(string ticker, float newPrice) { }
}

public sealed class GoblinTwitterManager : Component
{
    [Sync] public NetList<Tweet> Feed { get; set; }

    [Rpc.Host]
    public void PostTweet(string content) { }

    [Rpc.Broadcast]
    public void OnNewTweet(Tweet tweet) { }

    // Bot system runs on host, generates tweets periodically
    private void GenerateBotTweet() { }
}

// === SOCIAL DEDUCTION ===

public sealed class RugPullRole : Component
{
    [Sync(SyncFlags.OwnerOnly)]
    public bool IsRugPuller { get; set; }  // Only visible to the player

    [Rpc.Host]
    public void AttemptRugPull(string ticker) { }

    [Rpc.Broadcast]
    public void OnRugPullExecuted(string puller, string ticker) { }
}

// === INTERFACES ===

public interface IInteractable
{
    string InteractionPrompt { get; }
    void OnInteract(GoblinPlayerController player);
}

public class TradingTerminal : Component, IInteractable
{
    public string InteractionPrompt => "Open Trading Terminal";
    public void OnInteract(GoblinPlayerController player)
    {
        // Open TradingPanel for this player
    }
}
```

### Network Architecture

```
Host (Player 1)
├── Runs: MarketSimulator, BotSystem, RoundManager, VotingManager
├── Authoritative: Token prices, portfolios, game state
├── Receives: [Rpc.Host] calls from clients
└── Sends: [Rpc.Broadcast] for state changes, [Sync] for continuous state

Clients (Players 2–8)
├── Send: Buy/Sell/Tweet/Vote requests via [Rpc.Host]
├── Receive: State updates via [Sync] properties
├── Local: Camera, input, UI, voice processing, physics prediction
└── Display: Interpolated positions, synced market data
```

---

## 4. MVP vs Nice-to-Have

### MUST SHIP (MVP) — Cut everything else before these

| Feature | Why It's MVP |
|---|---|
| Multiplayer lobbies (4–8 players) | No multiplayer = no game |
| Basic goblin movement + interaction | Players need to DO things |
| Token creation + simple market | Core tycoon mechanic |
| GoblinTwitter (post + view feed) | THE differentiating mechanic |
| Tweets affect market prices | Social media ↔ market loop |
| Rug Puller role assignment | Social deduction hook |
| Voting to identify Rug Puller | Resolution mechanic |
| Proximity voice chat | Viral moments need voice |
| Round structure (start → end) | Game needs a loop |
| Basic office map | Players need a space |
| Results screen with winner | Closure + "one more round" |

### SHOULD SHIP (High Value, Cut if Behind)

| Feature | Cut Condition |
|---|---|
| In-game clip recorder | If Day 14 isn't done by Day 16, cut to "manual screenshot" only |
| Physics desk flipping | If physics causes network instability, make it client-side-only cosmetic |
| Found footage props | If behind, ship with 3 static news tickers instead of full system |
| Discord Rich Presence | If Day 18 takes > 1 day, cut entirely — add in Week 2 patch |
| NPC bot tweets | If behind, ship with 5 hardcoded bot messages on a timer |
| GoblinTube clip viewer | If clip recorder is cut, this is cut too |

### NICE-TO-HAVE (Post-Launch Patch Content)

| Feature | Patch Target |
|---|---|
| Multiple maps (Yacht, Garage startup, WeWork parody) | Week 2 |
| Custom goblin cosmetics | Week 3 |
| Token logo designer (in-game pixel art tool) | Week 3 |
| Spectator mode | Week 2 |
| Replay system (full round replay, not just clips) | Week 4 |
| Tutorial/onboarding flow | Week 2 (critical for retention) |
| Leaderboards (global, weekly) | Week 3 |
| "Whale" special role (second secret role) | Week 4 |
| In-game meme generator | Week 3 |
| Twitch integration (chat votes on events) | Month 2 |

### Scope Cut Decision Tree

```
Q: Is it Day 16 and Phase 3 isn't done?
├── YES → Cut clip recorder to "screenshot only", cut GoblinTube entirely
│         Reallocate Days 17-22 to polish Phase 2 features
├── NO  → Continue as planned

Q: Is it Day 20 and multiplayer is still buggy?
├── YES → Cancel Discord Rich Presence, cancel found footage system
│         Spend Days 20-26 on multiplayer stability only
├── NO  → Continue as planned

Q: Is it Day 24 and the game crashes in 8-player lobbies?
├── YES → Reduce max players to 4, skip external playtest
│         Fix crashes, ship reduced-scope MVP
├── NO  → Full launch as planned

GOLDEN RULE: If you have to choose, always prioritize:
Multiplayer stability > Core gameplay loop > Content > Polish
```

---

## 5. Asset Pipeline

### 3D Models — All Free Sources

| Asset | Source | Notes |
|---|---|---|
| Goblin character model | S&box Cloud Library (search "humanoid", "creature") | Retexture green. If nothing fits, use S&box citizen model with green shader |
| Office furniture | S&box Cloud Library (search "office", "desk", "chair") | Drag directly into scene from cloud browser |
| Computer monitors/screens | S&box Cloud Library (search "monitor", "screen", "computer") | UI renders on screen surfaces |
| Physics props (cups, keyboards) | S&box Cloud Library (search "props", "office") | Need Rigidbody + Collider |
| Server racks | S&box Cloud Library or Blender (simple box model) | Greybox is fine for MVP |
| Money/coin particles | Blender → simple icosphere, gold material | 10 minutes in Blender |
| Goblin portrait icons | AI-generated via free tools or hand-drawn pixel art | For GoblinTwitter avatars |

### Audio — All from freesound.org

| Sound | Search Terms | License |
|---|---|---|
| UI click/confirm | "ui click", "button press" | CC0 |
| Cash register / buy sound | "cash register", "coin drop" | CC0 |
| Market crash alarm | "alarm", "siren", "warning" | CC0 |
| Desk flip impact | "desk slam", "wood crash", "furniture break" | CC0 |
| Office ambience | "office ambience", "keyboard typing" | CC0 |
| Emergency meeting alarm | "emergency alarm", "alert buzzer" | CC0 |
| Rug pull dramatic sting | "dramatic sting", "horror reveal" | CC0 |
| Victory fanfare | "victory", "fanfare", "celebration" | CC0 |
| Goblin grunts/laughs | "goblin", "creature voice", "gremlin" | CC0 |
| Lo-fi background music | "lo-fi beat", "chill loop" | CC0 |

### Textures & Materials

| Material | Source |
|---|---|
| Office walls/floors | S&box built-in materials library |
| Goblin skin (green) | Custom: modify any skin material, shift hue to green |
| Screen UIs | Rendered via Razor panels onto WorldPanel components |
| "HODL" posters, crypto memes | Create in any free image editor (GIMP, Photopea) |
| Found footage newspaper props | Create as texture with text overlay in GIMP |

### Font & UI Assets

| Asset | Source |
|---|---|
| Monospace font (terminal) | S&box default or Google Fonts (Fira Mono, JetBrains Mono) |
| Meme font (Impact) | System font, universally available |
| GoblinTwitter bird logo | Simple SVG drawn in 5 minutes — goblin-ified bird |
| UI icons (buy/sell/tweet) | Lucide icons (MIT license) or hand-drawn pixel art |

---

## 6. Testing Plan

### Solo Multiplayer Testing (S&box Built-In)

S&box has first-class support for testing multiplayer locally:

1. **Editor Multi-Instance:** Launch your game → Multiplayer → "Create Lobby" in one instance, then launch a second instance of S&box → Join the lobby. S&box supports running multiple editor instances on the same machine.
2. **Bot Players:** Create a `TestBot` component that simulates player actions (random movement, random trades, random tweets) for stress testing with fewer real instances.
3. **Dedicated Server:** S&box supports local dedicated servers — run headless server + connect multiple clients.

### Testing Matrix

| Test | Method | When | Pass Criteria |
|---|---|---|---|
| 2-player networking | Two S&box editor instances | Daily during Phase 1-2 | No desync, smooth movement |
| Market simulation accuracy | Unit test class with mock data | Day 7-8 | Prices stay positive, no overflow |
| 8-player stress test | Recruit players from S&box Discord | Day 21, Day 25 | 60fps, no crashes, no desync |
| Voice chat range | Two instances, walk apart | Day 5 | Volume fades correctly with distance |
| Rug pull game flow | Full round with 4 players | Day 12+ | Role assignment works, voting works, winner declared |
| Physics networking | Desk flip observed on all clients | Day 11+ | All clients see desk flip, no ghost objects |
| Clip recording | Record 30-second clip, verify playback | Day 13+ | Clip saves, plays back, reasonable file size |
| Late joiner | Join mid-round | Day 17+ | Game state syncs, player can spectate or wait |
| Host disconnect | Kill host process mid-game | Day 22 | Host migration OR graceful error message |
| Load test (tokens) | Create 50+ tokens in one round | Day 23 | No performance drop, UI stays responsive |

### Automated Testing Strategy

```csharp
// TestBot.cs — Automated player for stress testing
public sealed class TestBot : Component
{
    private float _nextActionTime;

    protected override void OnUpdate()
    {
        if (Time.Now < _nextActionTime) return;
        _nextActionTime = Time.Now + Random.Shared.Float(1f, 5f);

        switch (Random.Shared.Int(0, 4))
        {
            case 0: MoveRandomly(); break;
            case 1: CreateRandomToken(); break;
            case 2: MakeRandomTrade(); break;
            case 3: PostRandomTweet(); break;
            case 4: FlipNearestDesk(); break;
        }
    }
}
```

---

## 7. Launch Day Checklist

### Pre-Launch (Day 27 Evening — April 27)

- [ ] Final build uploaded to S&box Workshop (Private)
- [ ] Play through 3 complete rounds with zero crashes
- [ ] Verify all `[Sync]` properties work in 4-player test
- [ ] Discord Rich Presence shows correct status
- [ ] Clip recorder saves files that actually play
- [ ] All placeholder text replaced (no "TODO" or "Lorem ipsum")
- [ ] Settings menu works (audio, graphics)
- [ ] Workshop page complete: title, description, screenshots (5+), tags

### Launch Morning (April 28 — 10:00 AM PST)

- [ ] Set Workshop project visibility to **Public**
- [ ] Verify project appears in S&box browse/search
- [ ] Download from Workshop on a separate account — confirm it works
- [ ] Test joining a public lobby from fresh install

### Social Media Blitz (April 28 — 10:15 AM PST)

- [ ] Post on Twitter/X with 30-second gameplay clip + link
- [ ] Post on r/sbox with gameplay GIFs
- [ ] Post on r/indiegaming, r/gamedev, r/cryptocurrency (if relevant subs allow)
- [ ] Post in S&box Discord #showcase channel
- [ ] Update Discord server with @everyone announcement
- [ ] Send DMs to any S&box content creators / streamers who showed interest

### Day 1 Monitoring (April 28 — All Day)

- [ ] Monitor S&box Workshop for download counts
- [ ] Watch Discord #bug-reports channel — fix P0 bugs immediately
- [ ] Respond to every piece of feedback within 2 hours
- [ ] Push hotfix if crash is found (have Workshop update process ready)
- [ ] Save/retweet any player-generated clips

### Workshop Publishing Details (S&box-Specific)

1. Open S&box Editor → Project Settings
2. Set project thumbnail (1280x720 PNG)
3. Add title: "GOBLIN CHAIN: Crypto Chaos Tycoon"
4. Write description (markdown supported)
5. Set tags: `multiplayer`, `comedy`, `social-deduction`, `tycoon`, `voice-chat`
6. Click "Upload Project"
7. Go to sbox.game → Your Profile → Project → Set visibility to Public

---

## 8. Found Footage Collection Plan

### Real Crypto Artifacts to Parody (In-Game Props)

These are satirical in-game versions inspired by real events. All presented as goblin-world parody — no direct reproduction of copyrighted material.

#### SEC Filings (Physical Document Props in CEO's Office)

| In-Game Prop | Real-World Inspiration | In-Game Text |
|---|---|---|
| "SEC vs GoblinBase" filing | SEC vs Coinbase | "The Securities & Exchange Coven hereby charges GoblinBase with offering unregistered cave securities..." |
| "Operation Choke Swamp" memo | Operation Choke Point 2.0 | "All goblin banks are hereby directed to de-cave any institution associated with digital gold..." |
| "GoblinStar Bankruptcy Filing" | Celsius/BlockFi bankruptcy | "GoblinStar Lending declares bankruptcy after discovering the vault was actually a painted wall..." |
| "The GobDAO Hack Report" | The DAO hack (2016) | "Approximately 3.6 million gold pieces were drained through a recursive cave exploration exploit..." |
| "Goblin-Fried-Bankman Indictment" | SBF indictment | "Count 1: Wire fraud. Count 2: Securities fraud. Count 3: Unreasonably large hair..." |

#### Breaking News Templates (Office TV Tickers)

```
"BREAKING: $TICKER up 400% after CEO tweets 'gm' — analysts baffled"
"SEC RAIDS: Goblin regulators raid [COMPANY] office, find only a whiteboard with 'blockchain' written 47 times"
"WHALE ALERT: Unknown wallet moves 10M $TICKER — Twitter detectives blame 'the illuminati'"
"CRASH: $TICKER drops 99% after developer tweets 'oops lol'"
"REGULATION: Goblinia passes law requiring all crypto devs to wear clown shoes"
"ADOPTION: Major retailer now accepts $TICKER — retailer is a hot dog stand"
"HACK: DeFi protocol hacked for $50M — hacker sends apology tweet"
"NFT: Goblin sells screenshot of a screenshot for 500 ETH"
```

#### Documentary-Style Text Crawls (Found Footage Screens)

- "The Rise and Fall of GoblinCoin" — mockumentary text about a failed meme token
- "Inside the Cave: A DeFi Documentary" — parody of crypto documentaries
- "The Goblin Who Knew Too Much" — whistleblower narrative
- "Ponzinomics 101" — educational parody explaining Ponzi schemes with goblin examples

#### Real Crypto Event Timeline (Easter Eggs on Whiteboards)

Write key dates from crypto history on in-game whiteboards in goblin parody form:
- "2009: Mysterious goblin publishes 'Bitcoin Caverock' paper"
- "2013: Mt. Box exchange loses 850,000 gold coins, claims they 'fell in a hole'"
- "2017: Everyone's grandma buys crypto. This will end well"
- "2021: Goblin JPEGs sell for millions. Civilization peaks"
- "2022: Everything crashes. 'This is good for Bitcoin' — some goblin"
- "2024: SEC sues everyone. Everything is a security except actual securities"

### Content Format (Technical Implementation)

All found footage is **text + static images**, not video, to keep file size minimal:
- SEC filings: `TextPanel` rendered on paper prop mesh
- News tickers: scrolling `Label` component on TV screen WorldPanel
- Documentaries: sequence of `TextPanel` pages with "Next" button
- Whiteboards: `TextPanel` rendered on whiteboard mesh

---

## 9. Pre-Launch Community Building Strategy

### Timeline (Starts Day 1 — April 1)

**Week 1 (Days 1–7): Stealth Build + Tease**
- Day 1: Post "what if crypto devs were goblins" poll on Twitter/X
- Day 3: Share first screenshot of greybox office with goblin placeholder
- Day 5: Record 15-second clip of proximity voice chat working — post as "proof of concept"
- Day 7: Post GoblinTwitter UI mockup — "we built Twitter inside a game"

**Week 2 (Days 8–14): Build Hype**
- Day 8: Create dedicated Twitter/X account @GoblinChainGame
- Day 10: Post "social deduction reveal" — announce the rug puller mechanic
- Day 12: Share 30-second gameplay clip of a complete round
- Day 14: Create Discord server, share invite link

**Week 3 (Days 15–21): Community Engagement**
- Day 15: Launch Discord with channels: #general, #dev-logs, #memes, #bug-reports, #suggestions
- Day 17: Post daily dev logs in Discord (screenshots, progress, decisions)
- Day 19: Announce external playtest, recruit from Discord + S&box community
- Day 21: Stream the 8-player stress test live

**Week 4 (Days 22–27): Launch Momentum**
- Day 22: Release gameplay trailer (60 seconds, captured in-engine)
- Day 24: Share workshop page preview, ask community for description feedback
- Day 25: Stream external playtest, encourage participants to clip and share
- Day 26: "24 hours until launch" countdown post
- Day 27: "Tomorrow. 10 AM PST." teaser with best clip from playtests

### Key Platforms

| Platform | Strategy | Posting Frequency |
|---|---|---|
| Twitter/X | Short clips, memes, polls, dev screenshots | Daily from Day 8 |
| S&box Discord | #showcase posts, engage with S&box community | Every 2-3 days |
| Reddit (r/sbox, r/indiegaming) | Major milestones only, no spam | Weekly |
| Own Discord | Dev logs, community building, playtest recruitment | Daily from Day 15 |
| TikTok | Vertical clips of funniest moments from playtests | Day 22+ |
| YouTube | Gameplay trailer, playtest VODs | Day 22+ |

### Content Strategy (What to Post)

The game sells itself through clips. Every post should be a clip or lead to wanting to see a clip:
1. **"This happened" clips** — desk flips, rug pulls, voice chat chaos
2. **Dev log screenshots** — before/after, UI evolution, funny bugs
3. **GoblinTwitter screenshots** — the in-game tweets ARE the marketing
4. **"Join the playtest" CTAs** — urgency + exclusivity
5. **Memes** — crypto memes with goblin twist, crosspost to crypto humor communities

---

## 10. Risk Mitigation

### Risk Matrix

| Risk | Probability | Impact | Mitigation |
|---|---|---|---|
| S&box networking breaks / API changes | Medium | Critical | Pin S&box version, check GitHub issues daily, have fallback to simple host-client model |
| Market simulation too complex, eats dev time | High | High | Start with simplest possible price formula (Day 7). Add complexity ONLY after core loop works |
| Clip recorder too technically complex | High | Medium | Fallback: encourage external tools (OBS, Medal.tv). In-game "screenshot" button as minimum |
| Physics causes network desync | Medium | Medium | Make physics client-side-only cosmetic. Only sync "desk was flipped" event, not every physics body |
| Can't recruit playtesters | Medium | High | Test with 2-3 alt accounts if needed. Reach out to S&box Discord early (Day 10) |
| GoblinTwitter UI takes too long | Medium | High | Start with plaintext chat log. Upgrade to Twitter-like UI ONLY when it works as chat first |
| Voice chat quality issues | Low | Critical | S&box has built-in voice — if it works, it works. If not, fall back to "Discord required" and add text chat |
| Solo dev burnout (27 days is brutal) | High | Critical | See below |
| S&box Play Fund revenue too low | Medium | Low | This is a passion/portfolio project first. Revenue is bonus. Build for virality, not monetization |
| Legal issues with crypto parody | Low | Medium | Everything is clearly satirical. Use goblin-ified parody names, never reproduce real logos/documents |

### Burnout Prevention Plan

This is a 27-day sprint. Burnout is the #1 risk.

- **Work blocks:** 4 hours focused AM, 4 hours focused PM, 2 hours for testing/admin. No more than 10 hours/day.
- **Mandatory breaks:** 1 hour lunch, 30 min afternoon break, stop by 10 PM
- **One day lighter:** Day 16 (audio) and Day 20 (UI polish) are intentionally less intense
- **Cut scope before cutting sleep:** If behind schedule, use the Scope Cut Decision Tree. Never pull an all-nighter — the bugs you write at 3 AM will cost more to fix than the features are worth.
- **Claude handles the grind:** Use Claude for boilerplate code, UI layout, data entry (tweet templates, token names, news tickers). Save your brain for design decisions and playtesting.

### "What If X Takes Longer" Contingency Table

| System | Expected | If It Takes 2x | Emergency Cut |
|---|---|---|---|
| Networking setup | 1 day | Spend Day 2 on it, push movement to Day 3 | Use simplest possible host-client, no host migration |
| Market simulation | 1 day | Simplify to random walk + player buy/sell pressure only | Price = starting price ± random. Players can still trade |
| GoblinTwitter | 2 days | Cut replies, cut quote tweets, cut trending | Text chat with "$TICKER" detection that nudges prices |
| Social deduction | 1 day | Cut secret objectives, just "one player is bad" | Skip rug puller role entirely — pure tycoon mode |
| Clip recorder | 2 days | Cut to screenshot-only on Day 15 | Remove entirely, tell players to use OBS |
| Physics comedy | 1 day | Make all physics client-side only | Remove desk flip, keep ragdoll on bankruptcy only |
| Voice chat | 1 day | If S&box voice broken, skip proximity — use always-on voice | Text chat only, add "VOIP required" in description |
| Lobby system | 1 day | Use S&box built-in lobby exactly as-is, no customization | Single hardcoded lobby, players join via code |

---

## 11. Week-by-Week Milestones with Go/No-Go Gates

### WEEK 1: Foundation (Days 1–7) — Milestone: "Goblins in an Office"

**Deliverables:**
- Networked multiplayer: 2+ players moving in shared space
- Greyboxed office map with interaction system
- Proximity voice chat functional
- Basic HUD and UI framework
- Token creation + market simulation prototype

**Go/No-Go Gate (End of Day 7):**
| Criteria | Go | No-Go Action |
|---|---|---|
| Can 2 players connect and see each other? | ✅ Continue | 🛑 Stop everything, fix networking. Nothing works without this |
| Can players create and trade tokens? | ✅ Continue | ⚠️ Acceptable if buy/sell works even without price charts |
| Is voice chat working? | ✅ Continue | ⚠️ Flag as P0, must fix by Day 10 or cut to text chat |
| Is the office map walkable? | ✅ Continue | ⚠️ Use a flat plane. Map polish can happen in Week 3 |

---

### WEEK 2: Core Mechanics (Days 8–14) — Milestone: "A Playable Round"

**Deliverables:**
- GoblinTwitter fully functional (post, read, market effects)
- Rug Puller role + voting system
- Physics desk flipping
- Complete round structure (lobby → results)
- Clip recorder prototype

**Go/No-Go Gate (End of Day 14):**
| Criteria | Go | No-Go Action |
|---|---|---|
| Can you play a complete round start to finish? | ✅ Continue | 🛑 Delay Phase 3, fix game loop. This is THE milestone |
| Does GoblinTwitter work? | ✅ Continue | ⚠️ Simplify to chat-only. Add Twitter UI post-launch |
| Does social deduction work? | ✅ Continue | ⚠️ Cut rug puller, ship as pure tycoon. Add role later |
| Does clip recorder work? | ✅ Continue | ✂️ Cut it. Tell players to use OBS. Not worth delaying |

---

### WEEK 3: Polish + Content (Days 15–21) — Milestone: "Looks Like a Real Game"

**Deliverables:**
- Found footage system with 10+ props
- Audio + SFX complete
- Lobby system + matchmaking
- Discord Rich Presence
- Visual polish (real assets, lighting, particles)
- UI polish (animations, themed panels)
- 8-player stress test completed

**Go/No-Go Gate (End of Day 21):**
| Criteria | Go | No-Go Action |
|---|---|---|
| Is 8-player stable (no crashes in 30 min)? | ✅ Continue | ⚠️ Reduce to 4-player max. Stability > player count |
| Does the game look presentable? | ✅ Continue | ⚠️ Acceptable. Gameplay > graphics for S&box audience |
| Is Discord Rich Presence working? | ✅ Continue | ✂️ Cut it. Nice-to-have, not launch-critical |
| Have you gotten external feedback? | ✅ Continue | ⚠️ Recruit testers NOW for Day 25 playtest |

---

### WEEK 4: Ship It (Days 22–27) — Milestone: "LAUNCH"

**Deliverables:**
- All critical bugs fixed
- Content + balance pass complete
- Workshop page published
- External playtest completed
- Launch build uploaded
- Community + social media ready

**Go/No-Go Gate (End of Day 26):**
| Criteria | Go: Launch April 28 | No-Go: Delay |
|---|---|---|
| Can 4 players play a round without crashes? | ✅ SHIP IT | 🛑 Delay 3 days, fix stability, launch May 1 |
| Is the core loop fun? (subjective) | ✅ SHIP IT | ⚠️ Ship anyway. Get feedback. Iterate live |
| Workshop upload works? | ✅ SHIP IT | 🛑 Debug publishing. Can't launch without this |
| Do you have at least basic marketing ready? | ✅ SHIP IT | ⚠️ Ship anyway. Marketing can follow by 24 hours |

**Final No-Go Override:** If the game crashes in 4-player lobbies on Day 26, delay to May 1. Three days of stability fixes is worth more than launching broken. A broken launch is worse than a late launch.

---

## Appendix A: Daily Time Budget Template

```
06:00 - 07:00  Wake up, review plan, respond to Discord/community
07:00 - 11:00  DEEP WORK BLOCK 1 (core coding, no distractions)
11:00 - 12:00  Lunch + walk (mandatory — you code worse without this)
12:00 - 16:00  DEEP WORK BLOCK 2 (continued coding or testing)
16:00 - 16:30  Break
16:30 - 18:30  TESTING + POLISH (playtest today's work, fix bugs)
18:30 - 19:30  Dinner
19:30 - 21:00  COMMUNITY + CONTENT (dev logs, social media, playtest recruitment)
21:00 - 22:00  PLANNING (review tomorrow's tasks, update this plan)
22:00          STOP. Sleep.
```

## Appendix B: Claude AI Pair Programming Strategy

Maximize Claude's output by giving it the right tasks:

| Give to Claude | Keep for Yourself |
|---|---|
| Boilerplate component scaffolding | Architecture decisions |
| UI layout (Razor panels) | Game feel tuning |
| Data entry (tweet templates, token names, news text) | Playtesting |
| Market simulation math | Balance tuning |
| Bug investigation (paste error, get analysis) | Creative direction |
| Unit test generation | Community management |
| Repetitive refactoring | Fun evaluation ("is this fun?") |
| Documentation, comments | Marketing voice/tone |

**Prompt pattern for Claude:**
```
"I'm building [SYSTEM] for GOBLIN CHAIN in S&box (C#).
Here's the current code: [paste]
Here's what I need: [specific requirement]
S&box uses: Components on GameObjects, [Sync] for networking, [Rpc.Broadcast/Host/Owner] for RPCs.
Write the implementation."
```

---

## Appendix C: Post-Launch Roadmap (Weeks 2–8)

| Week | Focus | Key Features |
|---|---|---|
| Week 2 | Stability + QoL | Bug fixes, tutorial, spectator mode, balance patch |
| Week 3 | Content Update 1 | New map (Yacht), custom goblin hats, token logo designer |
| Week 4 | Social Features | Global leaderboards, weekly challenges, clip sharing integration |
| Week 5 | Content Update 2 | New map (Garage Startup), "Whale" second secret role |
| Week 6 | Community Event | Tournament, community-voted features, meme contest |
| Week 7 | Content Update 3 | New map (WeWork parody), Twitch integration, in-game meme generator |
| Week 8 | Retrospective | Analyze Play Fund revenue, plan long-term roadmap, evaluate full-time viability |

---

*This plan was generated on April 1, 2026. Every day counts. Ship the game. Let the goblins loose.*

*"The only thing worse than a rug pull is not shipping." — Ancient Goblin Proverb*
