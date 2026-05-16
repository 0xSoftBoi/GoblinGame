# GOBLIN CHAIN — Implementation Plan
## 26 Days to S&box Launch (April 28, 2026)

**Rule:** Every task outputs a playable build. No dead-end work.

---

## PHASE 1: GoblinTwitter MVP (April 2–8)
> **Goal:** The game goes from "mine & trade" to "shill & scheme" in one week.

### 1A. Data Layer — Token + Post models (Day 1)

**New file: `code/Systems/TokenSystem.cs`**
- `TokenData` struct (net-serializable): `Guid Id`, `string Name`, `string Ticker`, `Guid CreatorId`, `string CreatorName`, `float Quality`, `float Price`, `float Supply`, `float CreatedAt`, `bool IsRugged`
- `NetDictionary<Guid, TokenData> ActiveTokens` synced from host
- `[Rpc.Host] RequestCreateToken(string name, string ticker)` — validates uniqueness, deducts creation fee (50 GBC), sets initial price from name hype + randomness
- `[Rpc.Host] RequestRugPull(Guid tokenId)` — creator-only, cashes out holdings, crashes price to 0, marks `IsRugged`, broadcasts shame notification
- Price update hook: each token's price ticks every 2s based on shill pressure + market noise (reuse `CryptoMarket` pattern)

**Modify: `code/Systems/CryptoMarket.cs`**
- Change from single GBC price to multi-token price engine
- `GetTokenPrice(Guid tokenId)` / `GetAllTokenPrices()`
- Each token has independent `BuyPressure` / `SellPressure` fed by shills and trades
- Keep GBC as the base currency (you spend GBC to buy tokens)

**Modify: `code/Systems/CryptoWallet.cs`**
- Add `NetDictionary<Guid, float> TokenHoldings` — tracks how many of each token the player owns
- `BuyToken(Guid tokenId, float gbcAmount)` / `SellToken(Guid tokenId, float tokenAmount)`

### 1B. GoblinTwitter Core — Posts + Feed (Days 2–3)

**New file: `code/Systems/GoblinTwitter.cs`**
- Server-side post manager
- `PostData` struct: `Guid PostId`, `Guid AuthorId`, `string AuthorName`, `Guid TokenId`, `string TokenTicker`, `string Content`, `int Likes`, `int Reposts`, `int FudReplies`, `int Reports`, `float ShillPower`, `float CreatedAt`
- `NetList<PostData> Feed` — last 50 posts, synced to all clients
- `[Rpc.Host] RequestPost(Guid tokenId, int hybeOpenerIdx, int claimIdx, int socialProofIdx, int ctaIdx, string customText)`
  - Server assembles post text from template indices
  - Calculates `ShillPower` from component stats + player reputation
  - Adds to Feed, triggers price impact on the token
- `[Rpc.Host] RequestLike(Guid postId)` / `RequestRepost(Guid postId)` / `RequestFud(Guid postId)` / `RequestReport(Guid postId)`
  - Like: +0.5% token price, +1 like count
  - Repost: +1.5% token price, +1 repost, spreads to NPCs
  - FUD: -30% shill effectiveness on that post
  - Report: +1 report count, 3+ reports = SEC heat on author
- Trending calculation: `(Likes + Reposts*2 + FudReplies*1.5) / sqrt(TimeSincePost)` — top 3 get +15% NPC buy pressure

**New file: `code/Systems/ShillTemplates.cs`**
- Static data — all the shill components from the GDD:
- `HypeOpeners[]` with stats: `string Text, int HypePower, int Credibility, int CringeFactor, int Risk`
  - "THIS IS NOT FINANCIAL ADVICE BUT…" (Hype:7, Cred:3, Cringe:4, Risk:2)
  - "GOBLIN INSIDER INFO 🚨" (Hype:9, Cred:2, Cringe:6, Risk:7)
  - "Why is nobody talking about…" (Hype:5, Cred:6, Cringe:2, Risk:1)
  - "Just mortgaged my cave for…" (Hype:8, Cred:4, Cringe:8, Risk:3)
  - + 4 more
- `Claims[]`, `SocialProofs[]`, `CTAs[]` — same pattern, 6-8 each
- Effectiveness formula: `(HypePower * 0.3) + (Credibility * 0.3) + (CringeFactor * 0.2) + (ReputationMult * 0.2)`

### 1C. GoblinTwitter UI — Phone Screen (Days 4–5)

**New file: `code/UI/GoblinPhone.razor`**
- Full-screen overlay toggled by `T` key (or partial — bottom-right phone popup)
- Three tabs: **Feed**, **Trending**, **Compose**
- **Feed tab**: scrollable list of posts with Like/Repost/FUD/Report buttons, author name, token ticker, post text, engagement counts
- **Trending tab**: top 3 tokens by mention volume, mini price charts
- **Compose tab**: 4 dropdown selectors (opener/claim/proof/CTA) + custom text field + token picker + POST button. Shows preview of shill power before posting.
- Style: dark mode, green-on-black terminal aesthetic (goblin hacker vibe)

**Modify: `code/Player/PlayerInput.cs`**
- `T` key → toggle `GoblinPhone.razor` (currently mapped to trade — remap trade to `Y`)
- When phone is open, suppress movement input (player stands still looking at phone — visible to others via proximity)

### 1D. Token Creation UI (Day 6)

**New file: `code/UI/TokenCreator.razor`**
- Opens during Mining phase (repurpose mining phase as "create + mine")
- Fields: Token Name (text input), Ticker (auto-generated or manual, 3-5 chars), Icon (pick from 12 preset pixel icons)
- "LAUNCH" button → calls `TokenSystem.RequestCreateToken()`
- Shows cost (50 GBC) and estimated initial price
- One token per player per round

**Modify: `code/Systems/GameStateManager.cs`**
- Rename phases conceptually: Mining → **CREATE** (create tokens + place rigs), Trading → **SHILL** (post on GoblinTwitter + trade tokens), Chaos → **CHAOS** (stays same + rug pull window)
- Add rug pull window: first 15s of Chaos phase, token creators get a "RUG PULL?" prompt

### 1E. Rug Pull Mechanic (Day 7)

**New file: `code/UI/RugPullPrompt.razor`**
- Appears at start of Chaos phase for anyone who created a token this round
- Three buttons: **RUG** (cash out all holders' value, your token → 0), **PIVOT** (rebrand, costs 100 GBC, keeps holders), **HOLD** (do nothing)
- RUG: dramatic screen flash, all holders get "RUGGED" notification, creator gets the pooled GBC value, reputation tanks
- 10-second decision timer

**Modify: `code/Systems/TokenSystem.cs`**
- `[Rpc.Host] RequestRugPull(Guid tokenId)` — validate creator, execute rug
- `[Rpc.Host] RequestPivot(Guid tokenId, string newName)` — rebrand token
- Reputation tracking: `[Sync] float Reputation` on player (starts 1.0, rug = -0.3, honest round = +0.1)

**End of Phase 1 deliverable:** Players create tokens, name them, shill them on GoblinTwitter, trade them, and decide to rug or hold. The core loop is complete.

---

## PHASE 2: Social Deduction + SEC (April 9–15)

### 2A. Rugger Role System (Days 8–9)

**New file: `code/Systems/SocialDeduction.cs`**
- At session start, host randomly assigns one player as **The Rugger** (30% chance of no Rugger)
- `[Sync] bool HasRugger` (public — "there MAY be a rugger")
- Rugger's client gets private notification + ability unlocks
- **Shadow Wallet**: Rugger can hide up to 30% of holdings from public leaderboard
  - Modify `CryptoWallet.cs`: add `[Sync] float PublicBalance` (what others see) vs actual `GoblinCoin` (truth)
- **Shill Boost**: Rugger's posts get hidden +20% effectiveness in `GoblinTwitter.cs`
- **Grand Rug condition**: accumulate 50%+ of total GBC supply → trigger Grand Rug
- **Grand Rug sequence**: 10s countdown on all screens, all prices crash, office goes red, Rugger wins

### 2B. Audit Vote (Day 10)

**New file: `code/UI/AuditVote.razor`**
- Any player can spend 1000 GBC to call an audit (button on GoblinPhone)
- All players get a vote popup: pick who you think is the Rugger
- Majority correct → Rugger exposed (Shadow Wallet visible, shill boost removed, can't Grand Rug)
- Majority wrong → falsely accused player loses 500 GBC
- Max 2 audits per session

**Modify: `code/Systems/SocialDeduction.cs`**
- `[Rpc.Host] RequestAudit()` — deduct 1000 GBC, broadcast vote
- `[Rpc.Host] CastVote(Guid suspectId)` — collect votes
- `ResolveAudit()` — tally, broadcast result

### 2C. SEC Heat System (Days 11–12)

**New file: `code/Systems/SECSystem.cs`**
- Per-player `[Sync] float SECHeat` (0–100), visible only to that player
- Heat sources (from GDD):
  - High-risk shill post: +5
  - Rug pull: +40
  - 3+ reports on your post: +10
  - Flash crash exploit: +50
- Heat decay: -2/minute
- At heat 70: **SEC Warning** — letter appears on screen, warning sound
- At heat 100: **SEC Raid** — triggers raid sequence

### 2D. SEC Raid Event (Days 13–14)

**Modify: `code/Systems/SECSystem.cs`**
- Raid sequence (simplified for launch):
  1. Warning phase (10s): siren sound, "SEC INCOMING" banner on all screens
  2. Decision phase (15s): target player gets 4 buttons:
     - **Shred Documents** — halves the fine
     - **Bribe** (2000 GBC, 70% success)
     - **Blame Another Goblin** — pick a player, if they have any heat, redirect raid to them
     - **Accept Fate** — take the full hit
  3. Resolution: Fine (50% of GBC), token freeze for 1 round, "ARRESTED" badge on profile

**New file: `code/UI/SECRaidPanel.razor`**
- Dramatic full-screen overlay for the raid target
- Timer, action buttons, outcome display
- Other players see "SEC RAID IN PROGRESS" banner with target's name

**End of Phase 2 deliverable:** Secret Rugger adds paranoia and social dynamics. SEC heat punishes reckless play and creates hilarious "blame your friend" moments.

---

## PHASE 3: Environment + NPC Investors (April 16–22)

### 3A. Office Level (Days 15–17)

This is the map/level work — hardest to do from code alone, but we can scaffold it:

**New file: `code/Environment/OfficeSetup.cs`**
- Procedural office layout component: spawns desks, chairs, market board, GoblinExchange kiosk
- Uses S&box's built-in prop models (citizen furniture)
- Each player gets a desk area with their rig placement zone nearby
- Central area: Market Board (shows all token prices), Exchange Terminal (interact to trade)
- Wall-mounted GoblinTV showing the headline ticker

**Modify: existing scene setup**
- Create `main.scene` with a basic room (box room or prefab office)
- Place `OfficeSetup`, `GoblinChainGame`, `GameStateManager`, `CryptoMarket`, `TokenSystem`, `GoblinTwitter`, `TradingSystem`, `RandomEvents`, `SECSystem`, `SocialDeduction`, `NetworkedRigSpawner` components

### 3B. NPC Retail Investors (Days 18–19)

**New file: `code/Systems/NPCInvestors.cs`**
- 20 NPC bots per lobby (not physical — just market actors)
- Each NPC has: `string Name`, `float Skepticism` (0–100), `float GBC`, `Dictionary<Guid, float> Holdings`
- Every shill post tick, NPCs evaluate: `BuyChance = ShillPower - (Skepticism * 0.5) + TrendingBonus`
- NPCs buy/sell tokens, creating organic volume
- Occasionally post their own reactions to GoblinTwitter ("just lost my cave savings on $GRUG, thanks goblins")
- NPCs panic sell when price drops >20% in 30s (cascade crash mechanic from GDD)

**New file: `code/Systems/NPCNames.cs`**
- 50 procedural NPC names: "RetailRon", "BagHolderBob", "DiamondHandsDave", "PaperHandsPat", etc.

### 3C. Market Manipulation Tools (Day 20)

**Modify: `code/Systems/TokenSystem.cs` or new `MarketManipulation.cs`**
- **Wash Trading** (available always): fake +10% volume on your token, costs 100 GBC, adds +15 SEC heat
- **Pump Group DM**: invite 1-2 players to coordinated buy (2x buy pressure for 30s), +10 heat per participant
- **Short Selling** (visible to others): bet against a token, profit if it crashes
- Each tool is a simple RPC: validate → execute → broadcast → add SEC heat

### 3D. Polish Pass (Days 21–22)

- Sound effects placeholders (can use S&box built-in sounds)
- Balance tuning: token prices, shill effectiveness, SEC heat rates, rig costs
- Loading screen with random tips from `GameLore.cs`
- Fix any networking edge cases (player disconnect mid-trade, mid-vote, etc.)

**End of Phase 3 deliverable:** The game feels like a real place. NPCs fill out the economy. Market manipulation adds depth.

---

## PHASE 4: Launch Prep (April 23–28)

### 4A. Final Integration + Bugfix (Days 23–25)

- Playtest with 4 players (recruit from S&box Discord)
- Fix critical bugs
- Verify all RPCs work with real latency
- Tune timings: phase durations, cooldowns, prices
- Make sure results screen shows meaningful stats

### 4B. Steam/S&box Listing (Day 26)

- Workshop thumbnail (can screenshot in-engine)
- Workshop description (use elevator pitch from GDD)
- Tags: Tycoon, Comedy, Multiplayer, Crypto, Satire
- `.sbproj` metadata finalized

### 4C. Launch Day (April 28)

- Upload to S&box Workshop
- Post in S&box Discord #showcase
- Record 60s gameplay clip for Twitter/social

---

## File Summary — What Gets Created

| # | File | Type | Phase |
|---|------|------|-------|
| 1 | `code/Systems/TokenSystem.cs` | New | 1A |
| 2 | `code/Systems/GoblinTwitter.cs` | New | 1B |
| 3 | `code/Systems/ShillTemplates.cs` | New | 1B |
| 4 | `code/UI/GoblinPhone.razor` | New | 1C |
| 5 | `code/UI/TokenCreator.razor` | New | 1D |
| 6 | `code/UI/RugPullPrompt.razor` | New | 1E |
| 7 | `code/Systems/SocialDeduction.cs` | New | 2A |
| 8 | `code/UI/AuditVote.razor` | New | 2B |
| 9 | `code/Systems/SECSystem.cs` | New | 2C |
| 10 | `code/UI/SECRaidPanel.razor` | New | 2D |
| 11 | `code/Environment/OfficeSetup.cs` | New | 3A |
| 12 | `code/Systems/NPCInvestors.cs` | New | 3B |
| 13 | `code/Systems/NPCNames.cs` | New | 3B |
| 14 | `code/Systems/MarketManipulation.cs` | New | 3C |

| # | File | Type | Phase |
|---|------|------|-------|
| 1 | `code/Systems/CryptoMarket.cs` | Modify | 1A |
| 2 | `code/Systems/CryptoWallet.cs` | Modify | 1A |
| 3 | `code/Systems/GameStateManager.cs` | Modify | 1D |
| 4 | `code/Player/PlayerInput.cs` | Modify | 1C |
| 5 | `code/Utility/GameLore.cs` | Modify | 3D |

**Total: 14 new files + 5 modified files = 19 file operations to ship the real game.**

---

## What's Explicitly Cut for Launch

- Story Mode (14-chapter campaign) → post-launch content update
- Office Eras (startup → penthouse visual progression) → ship with one static office, add eras later
- Auto-clip recorder → post-launch
- Full SEC escape minigame (shred/hide/flee physics) → simplified to button choices
- Token coding minigame (circuit board) → simplified to name/ticker/icon picker
- Copy-paste exploit (steal another player's token template) → post-launch
