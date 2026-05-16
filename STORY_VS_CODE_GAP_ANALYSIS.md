# GOBLIN CHAIN — Story vs Code Gap Analysis

**Date:** April 2, 2026 | **Days to S&box launch:** 26

---

## What Was Designed (The Vision)

The design docs describe TWO games layered together:

### 1. Multiplayer Tycoon (GDD v1 & v3)
The core multiplayer mode — 4-8 goblins in a shared WeWork office:
- **Token Creation Minigame** — circuit-board node-connection to "code" smart contracts, with quality tiers (Scam → Blue Chip)
- **GoblinTwitter** — in-game phone for shilling with template system (Hype Opener + Claim + Social Proof + CTA), likes/reposts/FUD replies, trending algorithm, NPC retail investors
- **Market Simulation** — price engine with buy/sell pressure, manipulation tools (wash trading, pump groups, insider leaks, short selling, flash crash exploits)
- **Social Deduction** — secret "Rugger" role with Shadow Wallet, hidden shill boost, Grand Rug mechanic, Audit Votes
- **SEC Raids** — heat meter, raid sequences with 5 escape options (shred, hide, bribe, flee, blame), Auditor miniboss
- **Office Eras** — startup → funded → exchange → penthouse, with progressive office upgrades reflecting wealth
- **Auto-Clip Recorder** — built for Twitch/YouTube moments
- **5-10 min cycles** within a 2-hour session

### 2. Story Mode ("The Goblin's Bargain" — Narrative Design Doc)
A massive 14-chapter + epilogue single-player campaign:
- **Protagonist: Grix** — small green goblin, DACA kid from Appalachia
- **Anti-power-fantasy** arc: seduction → success → emptiness → loss
- **Cast of 20+ named characters** with goblin parody names (Vitazzle the Pale = Vitalik, Bogan Tall = Logan Paul, etc.)
- **Relationship system (The Circle)** — ring of portraits that glow warm or crack dark based on your choices
- **Grind Gauge** — burnout mechanic (screen desaturates, dialogue gets cynical)
- **The Hoard** — money counter that deliberately becomes less satisfying over time
- **Goblin Reputation** — vanity score that feels important but means nothing
- Spans 2008-2024+ crypto history as a goblin parable

### 3. Lore Bible
Deep worldbuilding: The Underhive (underground goblin city), Sector 7G, The Exchange, The Furnace, The Penthouse, Blockchain Council, 5 fake cryptocurrencies with full backstories, factions, slang glossary.

---

## What Was Actually Coded (28 files)

A **simplified multiplayer mining tycoon** with 3 phases:

### What EXISTS in code:

| System | Files | Status |
|--------|-------|--------|
| Game loop (Mining → Trading → Chaos → Results) | `GoblinChainGame.cs`, `GameStateManager.cs` | ✅ Working scaffold |
| Player movement (FPS, mouse look, interaction) | `GoblinPlayer.cs`, `PlayerInput.cs` | ✅ Solid |
| Crypto market (price sim, crashes, moons) | `CryptoMarket.cs` | ✅ Working |
| Mining rigs (place, earn hash rate) | `MiningRig.cs`, `NetworkedRigSpawner.cs`, `RigUpgradeStation.cs` | ✅ Working |
| Wallet (balance, mining income) | `CryptoWallet.cs` | ✅ Working |
| Trading (propose/accept/reject between players) | `TradingSystem.cs` | ✅ Working |
| EMP sabotage (throw grenades to disable rigs) | `EMPGrenade.cs`, `SabotageInventory.cs` | ✅ Working |
| Random events (10 events across phases) | `RandomEvents.cs` | ✅ Working |
| Proximity voice chat | `ProximityVoice.cs` | ✅ Working |
| HUD (wallet, timer, leaderboard, ticker) | `GoblinHud.razor` | ✅ Working |
| Market ticker UI | `CryptoTicker.razor` | ✅ Working |
| Trade panel UI | `TradePanel.razor` | ✅ Working |
| Main menu | `MainMenu.razor` | ✅ Working |
| Notifications, phase announcements, results | Various `.razor` files | ✅ Working |
| Lore/flavor text (headlines, tips, quips) | `GameLore.cs` | ✅ Rich content |
| Spawn system | `SpawnPoint.cs` | ✅ Working |

### What's MISSING from code:

| Designed Feature | Priority | Effort | Notes |
|------------------|----------|--------|-------|
| **GoblinTwitter (shilling system)** | 🔴 CRITICAL | Large | THE core differentiator. No phone UI, no posting, no NPC reactions, no trending. Without this, it's just "mine + trade" like every other tycoon. |
| **Token Creation minigame** | 🔴 CRITICAL | Medium | The circuit-board coding game that determines token quality. Currently players just mine — no token authoring. |
| **Social Deduction (Rugger role)** | 🟡 HIGH | Medium | Secret role assignment, Shadow Wallet, Audit Vote, Grand Rug. This is the Among Us hook. |
| **SEC Raids** | 🟡 HIGH | Medium | Heat meter, raid animation, escape minigames. There's a "Regulator Raid" random event but it's just a balance drain — not the physical raid sequence. |
| **Office Eras / Upgrades** | 🟡 HIGH | Large | The visual progression from dump WeWork to penthouse. Currently no office environment at all in code. |
| **Rug Pull / Pivot mechanic** | 🟡 HIGH | Small | The core decision each cycle — rug, pivot, or hold on your token. Can't exist without token creation. |
| **NPC retail investors** | 🟢 MEDIUM | Medium | 20-50 bots that browse GoblinTwitter and create organic volume. Needed to make economy work with <8 players. |
| **Market manipulation tools** | 🟢 MEDIUM | Small | Wash trading, pump groups, insider leaks — unlocked by era. |
| **Story Mode** | ⚪ CUT | Massive | 14-chapter campaign. Not feasible for solo dev by April 28. Defer to post-launch. |
| **Auto-clip recorder** | ⚪ CUT | Medium | Nice-to-have for virality. Post-launch. |

---

## The Core Problem

The code built a **mining tycoon** (place rigs → earn coins → trade → sabotage). But the design describes a **shilling tycoon** (create tokens → shill on social media → manipulate markets → rug or pivot). These are fundamentally different games.

The mining loop is generic. The shilling loop is what makes Goblin Chain unique. GoblinTwitter is the game's soul — it's where the comedy, strategy, social dynamics, and Twitch moments all come from.

---

## What GameLore.cs Got Right

Despite context compacting killing the story implementation, `GameLore.cs` is actually excellent. It has:
- 5 coin lore entries with taglines and descriptions
- 15 crash headlines, 12 moon headlines, 13 normal headlines (all on-brand and funny)
- 25 loading screen tips (great tone — "If you can't spot the whale in the room, you're the liquidity")
- Phase-specific quips for all 5 phases
- 10 detailed event lore entries with both lore text and gameplay text
- Winner titles ("WOLF OF GOBLIN STREET", "HASH KING", etc.)

This lore layer IS the story showing up in the multiplayer mode. It's doing its job.

---

## Recommended Priority for Next 26 Days

**Ship a playable, funny game — not the full vision.**

### Week 1 (April 2-8): GoblinTwitter MVP
- Phone UI (open with T)
- Template-based post composer (pick components, post)
- Feed visible to all players
- Posts affect market price (simple multiplier)
- This alone transforms the game from "mine and trade" to "shill and scheme"

### Week 2 (April 9-15): Token Creation + Rug Pull
- Simple token creation (name + ticker + icon picker)
- Token appears on market board
- Rug Pull button (cash out, crash token, reputation hit)
- This completes the core loop: create → shill → rug or hold

### Week 3 (April 16-22): Social Deduction + SEC
- Rugger role assignment at round start
- Basic SEC heat meter + raid event (simplified — just a timer + fine, not the full escape sequence)
- Audit Vote mechanic

### Week 4 (April 23-28): Polish + Launch
- Bug fixes, balance tuning
- Office environment (even if it's just one static level)
- Steam page, screenshots, trailer
- S&box workshop upload

### Post-Launch
- Office eras/upgrades
- NPC investors
- Market manipulation tools
- Auto-clip recorder
- Story mode (long-term)
