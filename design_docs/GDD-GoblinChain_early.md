# GOBLIN CHAIN: Crypto Chaos Tycoon

## Game Design Document v1.0 — Production Draft

**Platform:** S&box (Source 2 / Rubikon Physics)
**Genre:** Physics Comedy Tycoon / Multiplayer Crypto Simulator
**Target:** Solo dev MVP in 8–12 weeks (ship by April 28, 2026)
**Audience:** Twitch/YouTube streamers, crypto-ironic community, tycoon fans

---

## ELEVATOR PITCH

You're a goblin running a crypto startup out of a collapsing WeWork. Code shitcoins, shill on GoblinTwitter, manipulate markets, dodge the SEC — and watch your office physically crumble or ascend based on your choices. Every action has physics consequences. Every lie compounds. Every crash is hilarious.

**One sentence:** *Crypto Tycoon meets Goat Simulator, but you're a goblin and the blockchain is made of cardboard.*

---

## TABLE OF CONTENTS

1. Core Fantasy & Tone
2. Core Gameplay Loop
3. Minute-by-Minute Gameplay
4. Session Structure
5. Token Creation System
6. The Shill Engine (GoblinTwitter / GoblinDiscord)
7. Market Simulation
8. The Rug/Pivot System
9. Physics Comedy Layer
10. Progression System (5 Eras)
11. Multiplayer Design
12. Economy Design
13. SEC & Threat Systems
14. Art Direction & Scope
15. Technical Architecture (S&box / C#)
16. Development Roadmap (8–12 Weeks)
17. Day 1 Ship vs. Post-Launch
18. Streamer & Viral Strategy

---

## 1. CORE FANTASY & TONE

### You Are a Goblin

The player is a literal goblin — green skin, pointy ears, too many teeth. This is non-negotiable framing. The goblin conceit does three things:

1. **Removes moral friction.** Players don't feel bad scamming because they're playing a cartoon creature. This lets the satire cut deeper.
2. **Explains the jank.** Physics bugs become features. Clipping through walls is "goblin engineering." Broken UI is "goblin coding."
3. **Creates visual comedy.** A goblin rage-flipping a desk, getting launched through the ceiling, or wearing a tiny suit to an SEC hearing is inherently funnier than a human doing the same.

### Tone Reference Points

- **Untitled Goose Game** — mischief as core mechanic
- **Surgeon Simulator** — physics fumbling creates comedy
- **Game Dev Tycoon** — management with consequences
- **Crypto Twitter** — the real thing is already a parody

### Comedy Rules

- Never punch down at retail investors who lost money. Punch at the goblins (the player).
- The game should feel like a crypto insider made it. Real terminology, real patterns, absurd execution.
- Physics comedy is the punchline delivery system. The tycoon systems write the jokes.

---

## 2. CORE GAMEPLAY LOOP

```
CODE → LAUNCH → SHILL → PUMP → (CHOICE: RUG or PIVOT) → CONSEQUENCES → REPEAT
```

### Loop Breakdown

| Phase | Duration | What Happens | Player Verbs |
|-------|----------|-------------|--------------|
| **Code** | 30–90 sec | Minigame: drag code blocks onto a whiteboard. Quality = how many bugs ship. | Drag, stack, skip QA |
| **Launch** | 10 sec | Token goes live. Physics: your goblin slams a comically large "DEPLOY" button. Office shakes. | Press button |
| **Shill** | 2–5 min | GoblinTwitter/GoblinDiscord: post, reply, manipulate sentiment. This is the CORE mechanic. | Write posts, reply to NPCs, coordinate with players |
| **Pump** | 1–3 min | Watch the chart. Market reacts to your shilling + other players + random events. | Monitor, counter-FUD, bribe influencers |
| **Choice** | 30 sec | RUG (cash out, tank reputation, office crumbles) or PIVOT (rebrand, keep building, slower profit). | Binary choice with timer pressure |
| **Consequences** | 30–60 sec | Physics sequence plays out. SEC raid chance. Reputation shifts. Office transforms. | Dodge, hide, celebrate |

**One full cycle: 5–10 minutes.** A session is 3–6 cycles. A full server wipe is ~2 hours.

---

## 3. MINUTE-BY-MINUTE GAMEPLAY

### First 60 Seconds of a New Game

0:00 — Goblin spawns in a bare WeWork cubicle. Folding table. Stolen laptop. Energy drink.
0:05 — Tutorial popup (a sticky note on the monitor): "Step 1: Make a coin. Step 2: Get rich. Step 3: Don't get caught."
0:10 — Player opens the CODE BOARD (whiteboard on the wall). Drags code blocks.
0:30 — Code compiled. Quality rating appears (e.g., "67% — will probably not explode").
0:40 — Player names their token via a keyboard on the desk. Types "GOBCOIN" or whatever.
0:50 — Slams the DEPLOY button. Office shakes. A ceiling tile falls. The chart appears on a monitor.
1:00 — GoblinTwitter opens on a second monitor. Tutorial: "Time to shill."

### Minutes 1–5: The Shill Phase

Player is now on GoblinTwitter (an in-game social media platform). This is a simplified but functional social feed:

- **Compose posts** with pre-built phrases + custom text
- **Reply to NPC posts** (influencers, skeptics, retail goblins)
- **DM other players** to coordinate pump timing
- **Spend GoblinGold** to boost posts, buy followers, bribe influencers

The chart on the office monitor moves in real-time based on:
- Your shill quality and frequency
- Other players' actions
- NPC sentiment
- Random market events (Elon tweet equivalent, exchange listing, hack rumor)

### Minutes 5–8: The Pump

If shilling worked, price is climbing. Player watches the chart, counters any FUD that appears, and decides when to pull the trigger. Tension builds. Other players may be counter-shilling or trying to short your coin.

### Minutes 8–10: The Choice

A timer appears. Two buttons materialize on the desk:

**🔴 RUG** — Cash out everything. Instant GoblinGold. Your office physically deteriorates (cracks in walls, furniture collapses). Reputation tanks. SEC suspicion rises.

**🟢 PIVOT** — Rebrand the token. Keep the community. Slower money but your office upgrades and reputation grows. Unlocks better mechanics in later eras.

If the player does nothing, the market decides for them (usually badly).

---

## 4. SESSION STRUCTURE

### Short Session (30 min — "Lunch Break")

- 3 token cycles
- Goal: make enough GoblinGold to upgrade one thing
- Good for: solo play, learning mechanics

### Standard Session (1–2 hours — "The Grind")

- Full era progression possible
- 5–8 token cycles
- Multiplayer economy peaks
- Good for: streaming, group play

### Marathon Session (2+ hours — "The Bull Run")

- Multi-era progression
- Server-wide events trigger
- SEC raids become frequent
- Office fully transforms
- Good for: content creation, competitive play

---

## 5. TOKEN CREATION SYSTEM

### The Code Board

A physical whiteboard in your office. You drag code blocks onto it to "build" your token.

**Block Types:**

| Block | Effect | Visual |
|-------|--------|--------|
| **Smart Contract** (required) | Base functionality. More = more features. | Blue sticky note |
| **Security Audit** | Reduces hack chance. Costs time. | Green sticky note |
| **Backdoor** | Lets you rug faster. Increases SEC detection. | Red sticky note (hidden behind others) |
| **Tokenomics** | Affects supply/demand curves. | Yellow sticky note |
| **Buzzword** | +shill effectiveness. "AI-powered," "quantum-resistant." | Gold sticky note |
| **Copy-Paste** | Copies another player's code. Fast but buggy. | Crumpled paper |

**Quality Score = (Smart Contracts × Security) − (Backdoors × Bugs) + Buzzwords**

Low quality: token is more likely to crash, get hacked, or attract SEC.
High quality: slower to build but more sustainable.

### Naming Your Token

Physical keyboard on the desk. Player types a name. The game generates a ticker symbol and auto-creates a logo (procedural goblin-art). Names affect NPC sentiment:

- Animal names: +10% retail interest
- "AI" in the name: +20% hype, +15% SEC attention
- Profanity: +30% Twitter engagement, −20% exchange listing chance

### Deploy

Giant red button on the desk. Physics-enabled. Slamming it harder makes the office shake more. Ceiling tiles fall. Coffee spills. A party horn goes off (or a sad trombone if quality is low).

---

## 6. THE SHILL ENGINE (GOBLINTWITTER / GOBLINDISCORD)

This is the core mechanic. Not a minigame — THE game.

### GoblinTwitter

An in-game social media feed displayed on your office monitor. Functionally similar to Twitter but populated by NPCs and other players.

**Post Types:**

| Type | Cost | Effect | Risk |
|------|------|--------|------|
| **Hype Post** | Free | +small sentiment boost | Low |
| **Fake Partnership** | 50 GG | +large boost, decays fast | Medium (can be called out) |
| **Technical Thread** | Free, takes time | +sustained boost for good code | None |
| **Influencer Bribe** | 200 GG | Influencer NPC shills for you | High (influencer might flip) |
| **FUD Attack** | 100 GG | Tanks another player's token | High (retaliation) |
| **Meme** | Free | Random effect, high engagement | Unpredictable |

**NPC Types on GoblinTwitter:**

- **Retail Goblins** — Easily swayed. Follow influencers. Panic sell on any FUD.
- **Influencer Goblins** — Bribeable. High follower counts. Can make or break a token.
- **Skeptic Goblins** — Post FUD. Can be countered with technical threads.
- **Whale Goblins** — Quietly accumulate. When they sell, price tanks hard.
- **SEC Agent Goblins** (disguised) — Ask suspiciously specific questions. Replying wrong increases raid chance.

**Engagement Mechanics:**

- Posts have likes, reposts, and replies (NPC-generated)
- Your engagement rate affects the Market Sentiment Index
- Viral posts trigger a "TRENDING" event — massive but temporary price spike
- Getting "ratio'd" by an NPC tanks sentiment

### GoblinDiscord

A second tab on the monitor. Your token's "community." Simpler than Twitter — primarily for:

- **Announcements** — affect holder confidence
- **Moderating FUD** — kick/ban NPCs (but they come back as alts)
- **Coordinating with players** — DM system for pump coordination
- **Community Events** — AMAs (you answer procedural questions), airdrops, governance votes

**Discord Health** is a secondary metric. If it drops too low, holders dump.

### Why This Is The Core Mechanic

The shill engine is where strategy lives. Token creation is a setup phase. The market is a reaction system. But shilling is where player skill, creativity, and social manipulation determine outcomes. It's the equivalent of combat in an action game — it's where you spend most of your active time, and it's where skill expression happens.

---

## 7. MARKET SIMULATION

### Price Model

Each token has a price driven by:

```
Price = BaseValue × SentimentMultiplier × SupplyDemandRatio × VolatilityNoise
```

**BaseValue** — Set by code quality and tokenomics choices.
**SentimentMultiplier** — Driven by GoblinTwitter/Discord activity. Ranges 0.1x to 10x.
**SupplyDemandRatio** — How many NPCs/players are buying vs. selling.
**VolatilityNoise** — Random factor. Higher in later eras. Simulates real market chaos.

### Market Events (Random)

Events fire every 2–5 minutes. Displayed as "breaking news" on an in-game TV in the WeWork.

| Event | Effect | Duration |
|-------|--------|----------|
| **Celebrity Goblin Tweet** | Random token gets +50% sentiment | 60 sec |
| **Exchange Listing** | Token with highest rep gets listed, +100% volume | Permanent |
| **Hack Rumor** | Random token loses 30% value (real hack if code quality low) | 120 sec |
| **Regulation Scare** | All tokens −20%. SEC activity increases. | 180 sec |
| **Bull Run** | All tokens +30%. Euphoria. | 120 sec |
| **Market Crash** | All tokens −50%. Panic selling. | 300 sec |
| **Airdrop Season** | New retail NPCs flood in. Sentiment volatile. | 180 sec |
| **Rug Pull News** | If any player recently rugged, all tokens suffer −15% trust | 120 sec |

### The Chart

Displayed on the office monitor as a real-time candlestick chart (simplified). The chart is also physically rendered — a scrolling paper printout that piles up on the floor as the session progresses. During crashes, the paper catches fire (physics object, can spread to furniture).

---

## 8. THE RUG/PIVOT SYSTEM

### The Choice

After each pump cycle, a timer counts down. Two physical buttons rise from the desk:

**RUG (Red Button)**
- Immediately converts token value to GoblinGold at current price
- Token price drops to near-zero
- All holder NPCs lose money (their sad faces appear on GoblinTwitter)
- Reputation drops sharply
- SEC Suspicion increases by 25%
- Your office physically deteriorates: cracks appear, furniture breaks, lights flicker
- Physics event: your goblin does a victory dance, then a ceiling tile bonks them

**PIVOT (Green Button)**
- Token rebrands (new name, same holders)
- You keep 20% of current value as GoblinGold
- Reputation increases
- Holders stay (some leave if you've pivoted too many times)
- New coding phase begins immediately with improved base stats
- Office improves slightly: new furniture appears, walls repair
- Physics event: a construction goblin NPC briefly appears and hammers something

**Neither (Timer Expires)**
- Market decides. Usually a slow bleed.
- Reputation slightly decreases (community sees inaction as weakness)
- Physics event: your goblin shrugs. A fly buzzes around the office.

### Reputation System

Reputation is a persistent score that affects:

- NPC willingness to invest in your tokens
- Influencer bribe costs (lower rep = higher cost)
- SEC raid frequency (lower rep = more frequent)
- Multiplayer trade costs (other players charge more to work with low-rep goblins)
- Era progression speed (need minimum rep thresholds for Eras 3–5)

Reputation resets partially on server wipe but carries a "legacy" modifier into new sessions.

---

## 9. PHYSICS COMEDY LAYER

### Design Philosophy

Physics is the punchline delivery system. The tycoon systems create dramatic situations. Physics makes them funny.

### The Domino Effect System

Every physics object in the office can trigger chain reactions. The system works on a simple cause-and-effect graph:

**Trigger → Reaction → Escalation**

Examples:

| Trigger | Reaction | Escalation |
|---------|----------|-----------|
| Slam DEPLOY button too hard | Desk shakes | Coffee spills → laptop sparks → small fire → sprinkler goes off → paperwork ruined (code quality drops) |
| Token crashes >50% | Goblin rage-flips desk | Desk hits whiteboard → code blocks scatter → nearby player's setup knocked over → their token bugs out |
| Get rugged by another player | Goblin launched upward | Through ceiling → lands on roof → slides off → ragdolls into dumpster behind WeWork |
| SEC raid triggered | Door kicked in | Door hits filing cabinet → papers fly everywhere → evidence scattered (can collect to reduce sentence) |
| Market crash event | TV falls off wall | Hits energy drink stack → cans roll everywhere (physics obstacles) → goblin slips and ragdolls |
| Hit 10x on token | Office physically upgrades | New furniture falls from ceiling → assembly animation → goblin gets bonked by falling desk |

### Physics Rules

1. **Everything on the desk is a physics object.** Laptop, coffee, energy drinks, sticky notes, phone.
2. **Chain reactions must be emergent, not scripted.** Place objects with Rubikon rigidbodies and let physics do the comedy.
3. **Player ragdoll on any strong force.** Getting hit by a door, falling furniture, or another player's thrown object triggers ragdoll.
4. **Recovery is fast.** Ragdoll lasts 2–3 seconds. Comedy comes from the moment, not from being stuck.
5. **Nothing is permanently destroyed.** Objects respawn after 30 seconds. The show must go on.

### Office as Physical Comedy Stage

The WeWork office is a single room per player (visible to others in multiplayer). The room contains:

- **Desk** — main interaction point. Monitors, keyboard, DEPLOY button, phone.
- **Whiteboard** — code board. Sticky notes are physics objects.
- **Filing Cabinet** — stores evidence. Falls over during raids.
- **Energy Drink Pyramid** — stack of cans. Fragile. Iconic.
- **Motivational Posters** — fall off walls during crashes. Text changes based on era.
- **Window** — view of other players' offices (multiplayer). Can throw objects through it.
- **Door** — SEC enters here. Can be barricaded (delays raids by 10 sec).
- **Ceiling** — tiles fall during earthquakes/crashes. Player can be launched through it.
- **Floor** — can crack and partially collapse in extreme scenarios.

---

## 10. PROGRESSION SYSTEM (5 ERAS)

### Era 1: THE GARAGE (Tokens 1–3)

**Setting:** Bare cubicle in a WeWork basement. Folding table. Stolen Wi-Fi.
**Mechanics Available:** Basic token creation, basic GoblinTwitter, simple rug/pivot.
**Goal:** Make 1,000 GoblinGold.
**Unlocks:** GoblinDiscord, influencer bribes, better code blocks.
**Physics State:** Minimal objects. Small chain reactions.
**Vibe:** "We're so early."

### Era 2: THE STARTUP (Tokens 4–8)

**Setting:** Proper cubicle. Standing desk. Multiple monitors. Free snacks corner.
**New Mechanics:** GoblinDiscord, fake partnerships, copy-paste coding, NPC team members (1–2 goblin devs you can hire/fire).
**Goal:** Make 10,000 GoblinGold. Maintain 50+ reputation.
**Unlocks:** Exchange listings, whale manipulation, marketing budget.
**Physics State:** More objects = bigger chain reactions. Team goblins are also physics objects.
**Vibe:** "We're disrupting finance."

### Era 3: THE FUNDED (Tokens 9–15)

**Setting:** Corner office. Glass walls (visible to all players). Espresso machine. Beanbag chairs.
**New Mechanics:** VC funding (take investment for GoblinGold but give up control), exchange listings, market manipulation tools (wash trading, spoofing as code blocks), hire up to 5 team members.
**Goal:** 100,000 GoblinGold. 75+ reputation. OR pull off a rug worth 50,000+ (but tank rep).
**Unlocks:** Conference attendance, regulatory lobbying, multi-token portfolio.
**Physics State:** Glass walls shatter during raids. Espresso machine explodes. Beanbags pop.
**Vibe:** "We're building the future."

### Era 4: THE EXCHANGE (Tokens 16–25)

**Setting:** Full floor of the WeWork. Reception desk. Conference room. Server room.
**New Mechanics:** Run your own exchange (list other players' tokens, take fees), governance voting, DAO mechanics, lobbying system to reduce SEC activity.
**Goal:** 1,000,000 GoblinGold. Control 3+ active tokens. OR become the most shorted goblin and survive.
**Unlocks:** Political connections, offshore accounts, final era access.
**Physics State:** Massive chain reactions. Server room can overheat and explode. Conference room table is launchable.
**Vibe:** "We're too big to fail."

### Era 5: THE EMPIRE (Tokens 26+)

**Setting:** Penthouse above the WeWork. Gold everything. Helicopter pad (decorative). Volcano lair aesthetic.
**New Mechanics:** Regulatory capture (you ARE the SEC now), market-wide manipulation, legacy system (your actions persist across server wipes as "lore").
**Goal:** There is no goal. You've won. Now survive everyone trying to take you down.
**Physics State:** Everything is gold-plated. Everything is explosive. Chain reactions are server-wide events.
**Vibe:** "I am the blockchain."

### Era Transitions

Transitions are physical events. When you hit the threshold:

1. Screen goes black for 2 seconds.
2. Construction sounds play.
3. Camera pulls back to show your office physically expanding/transforming.
4. New furniture falls from the ceiling and assembles (physics — sometimes comically wrong).
5. A "CONGRATULATIONS" banner unfurls (and sometimes catches fire).

---

## 11. MULTIPLAYER DESIGN

### Server Structure

- **8–16 players per server** (optimal: 12)
- **Persistent for 2-hour sessions**, then server wipe with legacy carryover
- **Shared economy** — all tokens exist on the same "GoblinChain"
- **Physical proximity** — offices are adjacent in the WeWork. You can see and interact with neighbors.

### Multiplayer Modes

#### Mode 1: FREE MARKET (Default)

All players compete in a shared economy. No teams. Alliances are informal and breakable.

**Interactions:**
- Trade GoblinGold directly
- Invest in each other's tokens
- Short each other's tokens
- Coordinate pumps via GoblinDiscord DMs
- Steal team members (hire away from another player at 2x salary)
- Throw objects through shared windows
- Sabotage (sneak into another player's office and mess with their code board)

#### Mode 2: SEC vs. GOBLINS (Social Deduction)

12 players. 2 are secretly SEC agents. Agent goblins look identical to regular goblins.

**SEC Agents Can:**
- Investigate other players' code boards (takes 10 sec, can be caught)
- Plant evidence in offices
- Call in raids on specific players
- File charges (if enough evidence, target player is "arrested" — 60 sec timeout)

**Goblins Can:**
- Bribe suspected agents
- Destroy evidence (burn the filing cabinet — physics event)
- Vote to "fire" a suspected agent from the WeWork (requires majority)

**Win Conditions:**
- SEC wins if they arrest 3+ goblins
- Goblins win if they survive the 2-hour session or identify and fire both agents

#### Mode 3: CO-OP CHAOS (Party Mode)

4 players run one mega-startup together. Shared office. Shared token. Shared responsibility.

**Roles:**
- **The Coder** — builds on the whiteboard
- **The Shill** — runs GoblinTwitter/Discord
- **The Trader** — manages the chart, handles buy/sell timing
- **The Fixer** — handles SEC, manages reputation, barricades doors

Roles can swap mid-game. Comedy comes from miscommunication and overlapping physics interactions in a shared space.

### Multiplayer Economy Interactions

| Action | Cost | Effect |
|--------|------|--------|
| **Buy another player's token** | Market price | Increases their price. You profit if it goes up. |
| **Short another player's token** | 10% collateral | You profit if their price drops. They see the short on their chart. |
| **Coordinate pump** | Free (DM) | Multiple players shill the same token. Massive but risky. |
| **Steal team member** | 2x their salary | Their team shrinks, yours grows. They get a notification. |
| **Sabotage** | Must be physically in their office | Rearrange code blocks, spill their coffee, knock over energy drinks. |
| **Bribe their influencer** | 300 GG | The influencer that was shilling their token starts FUDing it. |
| **Report to SEC** | Free | Anonymous tip. Increases their raid chance. Costs you reputation if discovered. |

---

## 12. ECONOMY DESIGN

### Currencies

**GoblinGold (GG)** — Primary currency. Earned from token profits. Spent on everything.

**Reputation (REP)** — Not a currency but a modifier. Affects costs, unlock thresholds, NPC behavior.

**Clout** — Earned from GoblinTwitter engagement. Spent on social mechanics (boosts, influencer access).

### GoblinGold Sinks (Preventing Inflation)

| Sink | Cost Range | Purpose |
|------|-----------|---------|
| Influencer bribes | 100–500 GG | Social mechanic |
| Office upgrades | 200–2000 GG | Progression cosmetics |
| Team salaries | 50–500 GG/cycle | Ongoing drain |
| Exchange listing fees | 1000 GG | Era 3+ mechanic |
| SEC fines | 500–5000 GG | Punishment |
| Sabotage tools | 100–300 GG | PvP mechanic |
| Lobbying | 2000–10000 GG | Era 4+ mechanic |

### GoblinGold Sources

| Source | Amount Range | Frequency |
|--------|-------------|-----------|
| Token profit (rug) | 500–50000 GG | Per rug |
| Token profit (pivot) | 100–10000 GG | Per pivot |
| Exchange fees | 10–100 GG | Passive (Era 4+) |
| Shorting profits | Variable | Per short |
| Stealing (sabotage) | 50–500 GG | Per sabotage |

### Economy Balance Targets

- **Early game:** Players should earn 200–500 GG per cycle. Barely enough to cover costs.
- **Mid game:** 1000–5000 GG per cycle. Comfortable but upgrades are expensive.
- **Late game:** 10000–50000 GG per cycle. Rich but everything is trying to take it from you.
- **Server-wide GG** should roughly double every 20 minutes, creating inflationary pressure that mirrors real crypto markets.

---

## 13. SEC & THREAT SYSTEMS

### SEC Suspicion Meter

Hidden value per player. Ranges 0–100.

**Increases from:**
- Rugging tokens (+25)
- Fake partnership posts (+10)
- Wash trading (+15)
- Low code quality tokens (+5 per launch)
- Player reports (+20)
- Getting caught sabotaging (+10)

**Decreases from:**
- Pivoting instead of rugging (−10)
- Security audits on code (−5)
- Lobbying (Era 4+) (−20)
- Time without suspicious activity (−2/min)

### SEC Raids

When suspicion hits thresholds, raids trigger:

**50 Suspicion — Warning Letter**
- A letter slides under your door (physics object).
- Reading it: "We're watching you." −0 GG but +anxiety.

**70 Suspicion — Office Visit**
- Two SEC goblins in suits knock on your door.
- You have 10 seconds to hide evidence (shove filing cabinet contents under desk, close code board).
- If evidence found: 500 GG fine. Suspicion resets to 40.
- If evidence hidden: "Sorry to bother you." Suspicion drops to 50.

**90 Suspicion — Full Raid**
- Door kicked in (physics — hits anything behind it).
- 4 SEC goblins storm the office.
- 30-second "evidence collection" phase — they grab everything not hidden.
- Fine: 2000–5000 GG. Forced to shut down current token.
- Your goblin is handcuffed for 30 seconds (ragdoll, dragged around).
- Suspicion resets to 20.

**100 Suspicion — Arrest**
- Helicopter lands on roof (physics chaos — wind blows everything).
- Goblin is escorted out.
- 60-second timeout. Lose 50% of GG.
- Suspicion resets to 0. Reputation hits floor.
- When you respawn, your office is bare (Era 1 state). You keep your era progress but lose all furnishings.

### Other Threats

**Hackers** — If code quality is low, NPC hackers can drain your token's liquidity. Displayed as a hooded goblin appearing on your monitor. Counter by having security audit blocks.

**Rug Pull Victims** — After you rug, angry NPC goblins show up outside your office window holding signs. They can throw objects through the window (physics). Lasts 60 seconds.

**Competitor FUD** — Other players can spend Clout to spread FUD about your token. Countered by your own shilling.

---

## 14. ART DIRECTION & SCOPE

### Style: Stylized Cartoon

- **Characters:** Low-poly goblins. Big heads, small bodies. Exaggerated animations. 4–5 body types, heavy reliance on accessories/hats for variety.
- **Environment:** Stylized office. Flat colors with slight texture. Think Overcooked meets corporate hell.
- **UI:** In-world wherever possible. Monitors display real UI. Whiteboard is the code editor. Phone rings for notifications.
- **VFX:** Particle-based. Confetti for launches, sparks for crashes, dollar signs floating up during pumps.

### What a Solo Dev Can Actually Build

**DO build:**
- Modular office room (one base room, props swap per era)
- 1 goblin model with swappable accessories (hat, glasses, tie)
- 10–15 physics prop models (desk, chair, monitors, energy drinks, filing cabinet, whiteboard, phone, coffee mug, deploy button, door, ceiling tiles, paper stacks, poster frames)
- 2D UI for GoblinTwitter/Discord (rendered on in-world monitor)
- Simple candlestick chart renderer
- Basic NPC goblins (recolored player model) for SEC, influencers, crowds

**DON'T build (use S&box defaults or skip):**
- Complex character customization (hats + colors is enough)
- Outdoor environments (the WeWork IS the game)
- Vehicles (except the SEC helicopter, which can be a simple box with rotors)
- Complex animations (ragdoll + 5–6 custom anims is plenty)
- Voice acting (text + goblin mumble SFX)

### Asset Priority List

| Priority | Asset | Complexity | Notes |
|----------|-------|-----------|-------|
| P0 | Office room (modular) | Medium | One room, swap props per era |
| P0 | Goblin player model | Medium | One model, accessory slots |
| P0 | Desk + monitors | Low | Core interaction point |
| P0 | Whiteboard | Low | Code board mechanic |
| P0 | Deploy button | Low | Iconic. Must feel good to slam. |
| P1 | Physics props (10 items) | Low each | Energy drinks, papers, etc. |
| P1 | GoblinTwitter UI | Medium | 2D overlay on monitor |
| P1 | Chart renderer | Medium | Simple candlestick |
| P2 | SEC goblin variant | Low | Recolor + suit accessory |
| P2 | Era-specific props | Low each | Gold desk, beanbags, etc. |
| P3 | Helicopter | Low | Box with rotors. Comedy asset. |
| P3 | Outdoor WeWork exterior | Low | Only visible through window |

---

## 15. TECHNICAL ARCHITECTURE (S&box / C#)

### S&box Architecture Overview

S&box uses C# with Source 2. Key systems:

- **GameManager** — Server authority. Manages game state, economy, market sim.
- **Pawn** — Player's goblin. Handles input, physics, interactions.
- **UI (Razor)** — S&box uses Razor components for UI. GoblinTwitter and charts render here.
- **Networking** — S&box handles networking. Use `[Net]` attributes for synced properties.

### Core Systems (C# Classes)

```
GoblinChain/
├── Game/
│   ├── GoblinChainGame.cs          — GameManager. Server setup, player join/leave.
│   ├── MarketSimulation.cs          — Server-side market sim. Runs every tick.
│   ├── EconomyManager.cs            — GoblinGold tracking, transactions.
│   └── EventSystem.cs               — Random market events, timers.
├── Player/
│   ├── GoblinPawn.cs                — Player pawn. Movement, interaction, ragdoll.
│   ├── GoblinInventory.cs           — GoblinGold, Clout, items.
│   ├── ReputationTracker.cs         — Rep score, history, era thresholds.
│   └── SECSuspicionTracker.cs       — Suspicion score, raid triggers.
├── Token/
│   ├── TokenData.cs                 — Token name, quality, price, holders.
│   ├── TokenFactory.cs              — Creation from code board input.
│   └── TokenLifecycle.cs            — Launch, pump, rug, pivot state machine.
├── Social/
│   ├── GoblinTwitter.cs             — Feed state, posts, NPCs, engagement calc.
│   ├── GoblinDiscord.cs             — Channel state, moderation, announcements.
│   ├── NPCSocialAgent.cs            — NPC behavior on social platforms.
│   └── SentimentEngine.cs           — Aggregates social activity into sentiment score.
├── Office/
│   ├── OfficeManager.cs             — Era-based prop spawning, layout.
│   ├── PhysicsProp.cs               — Base class for all interactive props.
│   ├── DominoEffectSystem.cs        — Chain reaction detection and escalation.
│   ├── CodeBoard.cs                 — Whiteboard interaction, block placement.
│   └── DeployButton.cs              — The big red button. Physics impulse on press.
├── SEC/
│   ├── SECRaidSystem.cs             — Raid spawning, evidence search, fines.
│   ├── SECAgentNPC.cs               — NPC behavior during raids.
│   └── EvidenceSystem.cs            — Evidence objects, hiding, discovery.
├── Multiplayer/
│   ├── SharedEconomy.cs             — Cross-player trading, shorting.
│   ├── SabotageSystem.cs            — Office invasion, code tampering.
│   └── SocialDeductionMode.cs       — SEC vs. Goblins mode logic.
└── UI/ (Razor)
    ├── HUD.razor                    — Minimal HUD (GG, Rep, Clout)
    ├── GoblinTwitterUI.razor        — Social feed interface
    ├── GoblinDiscordUI.razor        — Discord interface
    ├── ChartRenderer.razor          — Candlestick chart
    ├── CodeBoardUI.razor            — Overlay for code block placement
    ├── TokenCreateUI.razor          — Naming, stats display
    └── RugPivotUI.razor             — Choice interface with timer
```

### Networking Strategy

| Data | Sync Method | Authority |
|------|-------------|-----------|
| Token prices | `[Net]` property, server-authoritative | Server |
| GoblinGold balances | `[Net]`, server-authoritative | Server |
| Player position/physics | S&box default pawn networking | Client-predicted, server-corrected |
| GoblinTwitter feed | RPC from server on post, client renders | Server |
| Physics props | S&box networked physics (Rubikon) | Server |
| SEC suspicion | Server-only (hidden from client) | Server |
| Chat/DMs | RPC | Server relay |

### Performance Considerations

- **Market sim ticks** every 0.5 seconds (not every frame). Price updates interpolate on client.
- **GoblinTwitter** — limit visible feed to 20 posts. Older posts archive. NPC posts generated server-side on timer.
- **Physics props** — limit to 50 active physics objects per office. Despawn/sleep distant props.
- **Chain reactions** — cap at 3 levels deep to prevent physics explosion. Dampen forces after each step.

---

## 16. DEVELOPMENT ROADMAP (8–12 WEEKS)

### Pre-Production (Before Week 1)

- [ ] Set up S&box project, Git repo, CI
- [ ] Block out office room in Hammer/S&box editor
- [ ] Prototype goblin pawn (movement, basic interaction)
- [ ] Confirm Rubikon physics works for desk-scale objects

### Week 1–2: CORE LOOP SKELETON

**Goal:** Player can code a token, deploy it, and see a price chart move.

- [ ] Office room with desk, whiteboard, deploy button
- [ ] CodeBoard mechanic (drag blocks onto whiteboard)
- [ ] TokenFactory — create token from code board state
- [ ] Basic MarketSimulation — sine wave + noise
- [ ] ChartRenderer — display price on monitor
- [ ] Deploy button with physics impulse (desk shake, ceiling tile drop)
- [ ] GoblinPawn — walk around office, interact with objects

**Milestone:** Solo player can build and launch a token. Chart moves. Button feels good.

### Week 3–4: SHILL ENGINE

**Goal:** GoblinTwitter is playable. Shilling affects price.

- [ ] GoblinTwitterUI — compose posts, view feed
- [ ] NPCSocialAgent — 3 NPC types (retail, influencer, skeptic)
- [ ] SentimentEngine — posts affect sentiment, sentiment affects price
- [ ] Post types: hype, FUD, fake partnership, meme
- [ ] Engagement mechanics (likes, reposts affect sentiment weight)
- [ ] Basic Clout system

**Milestone:** Shilling on GoblinTwitter visibly moves the price chart. NPCs react.

### Week 5–6: RUG/PIVOT + CONSEQUENCES

**Goal:** Full cycle playable. Choices have physical consequences.

- [ ] Rug/Pivot choice UI with timer
- [ ] Reputation system
- [ ] Office transformation on rug (cracks, broken furniture)
- [ ] Office upgrade on pivot (new props spawn)
- [ ] Goblin ragdoll on rug (launched through ceiling)
- [ ] DominoEffectSystem — basic chain reactions (3–5 scripted chains + emergent)
- [ ] SEC suspicion tracking
- [ ] First SEC raid (warning letter + office visit)

**Milestone:** Full core loop: code → launch → shill → pump → rug/pivot → consequences. Playable solo.

### Week 7–8: MULTIPLAYER

**Goal:** 2+ players on a server with shared economy.

- [ ] Networked GoblinPawn
- [ ] SharedEconomy — players can see/buy/short each other's tokens
- [ ] Office adjacency — see neighbors through windows
- [ ] Basic sabotage — enter another player's office, mess with props
- [ ] GoblinTwitter shows all players' posts
- [ ] Cross-player price impact (buying someone's token raises their price)
- [ ] GoblinGold transfers between players

**Milestone:** Two players can compete in a shared economy. Core multiplayer works.

### Week 9–10: POLISH + PROGRESSION

**Goal:** Era system, full SEC raids, game feels complete.

- [ ] 5 Era system with thresholds and office transitions
- [ ] Full SEC raid sequence (warning → visit → raid → arrest)
- [ ] GoblinDiscord (basic version — announcements + moderation)
- [ ] Market events (random events on TV)
- [ ] More physics props and chain reactions
- [ ] Sound design pass (goblin mumbles, crash sounds, deploy SFX, shill notification dings)
- [ ] Tutorial (sticky notes on objects)
- [ ] Session timer + server wipe + legacy system

**Milestone:** Full game playable from Era 1 to Era 5. All core systems functional.

### Week 11–12: CONTENT + STREAMER PREP

**Goal:** Enough content for launch. Viral moments built in.

- [ ] SEC vs. Goblins mode (social deduction)
- [ ] Co-op Chaos mode (4-player shared office)
- [ ] 20+ NPC post templates for GoblinTwitter variety
- [ ] 10+ market event types
- [ ] Cosmetic goblin accessories (5 hats, 3 glasses, 3 ties)
- [ ] Spectator camera for streamers
- [ ] Clip-worthy moment detection (highlight reel system — optional)
- [ ] Balance pass (economy numbers, progression speed)
- [ ] Bug fixing, networking stress test
- [ ] Launch trailer capture

**Milestone:** Ship-ready MVP.

---

## 17. DAY 1 SHIP vs. POST-LAUNCH

### MUST SHIP (Day 1 MVP)

- [ ] Solo and multiplayer Free Market mode (8–16 players)
- [ ] Full core loop (code → shill → pump → rug/pivot)
- [ ] GoblinTwitter with NPC agents
- [ ] Market simulation with 5+ event types
- [ ] SEC suspicion + raids (at least warning + office visit + full raid)
- [ ] 3 Eras (Garage, Startup, Funded) — enough for 2-hour sessions
- [ ] Physics comedy (ragdoll, chain reactions, desk flip)
- [ ] Basic office customization per era
- [ ] Multiplayer economy (buy/short/trade)

### POST-LAUNCH (Weeks 1–4 After Ship)

- [ ] Eras 4 and 5 (Exchange, Empire)
- [ ] SEC vs. Goblins mode
- [ ] Co-op Chaos mode
- [ ] GoblinDiscord full implementation
- [ ] Exchange mechanic (run your own exchange)
- [ ] Lobbying system
- [ ] More cosmetics
- [ ] Community-suggested GoblinTwitter post templates
- [ ] Leaderboards (most GG earned, biggest rug, longest without SEC raid)

### FUTURE (Month 2+)

- [ ] Custom token logos (player-drawn)
- [ ] NFT parody system (goblins mint actual in-game NFTs that do nothing)
- [ ] WeWork building exterior (see all offices from outside)
- [ ] Seasonal events (crypto winter, bull market week)
- [ ] Workshop support (community maps, modes)
- [ ] Mobile companion app (check GoblinTwitter from phone — stretch goal)

---

## 18. STREAMER & VIRAL STRATEGY

### Why This Game Goes Viral

1. **Physics = clips.** Every ragdoll, chain reaction, and desk flip is a potential clip. S&box's physics engine generates unique moments every session.
2. **Social deception = content.** SEC vs. Goblins mode is Among Us energy. Accusations, betrayals, dramatic reveals.
3. **Crypto satire = engagement.** Crypto Twitter will share this ironically. Anti-crypto people will share it earnestly. Both audiences drive installs.
4. **Multiplayer chaos = streams.** 4+ streamers in one server creates content passively. They're competing, sabotaging, and screaming.

### Streamer-Specific Features

- **Spectator mode** — free camera for content creators not playing
- **Highlight detection** — game marks moments with high physics activity or big price swings for easy clip finding
- **Streamer names on tokens** — when a streamer creates a token, it becomes content. Chat can see the chart and root for/against it.
- **"Raid" mechanic** — when a streamer raids another streamer's Twitch channel, trigger an in-game event (cosmetic, fun, encourages cross-promotion)

### Launch Strategy

**Week −2:** Post dev clips on Twitter/TikTok. Physics comedy clips. "I'm making a game where you're a goblin running a crypto scam."

**Week −1:** Send keys to 10–20 mid-tier streamers (1k–50k followers) who play tycoon/comedy games. Prioritize those who've played Lethal Company, Content Warning, Goat Simulator, or crypto-related content.

**Launch Day:** Coordinate 4+ streamers to play together on one server. The multiplayer chaos creates organic content.

**Week +1:** Community highlight reel. Share the best clips. Let the community drive the conversation.

**Ongoing:** Weekly "patch notes" framed as in-universe GoblinChain updates. "GoblinChain v1.2: SEC now has tasers. Energy drink pyramid structural integrity reduced by 40%."

### Social Media Framing

The game's social media presence should be IN CHARACTER. The GoblinChain Twitter account posts like a real crypto project:

- "GM goblins. Big announcement coming. 🔥"
- "GoblinChain is NOT a scam. Our smart contracts have been audited by GoblinAudit (a subsidiary of GoblinChain)."
- "To the moon. 🚀" (with a screenshot of a goblin being launched through the ceiling)

This creates a meta-layer where the marketing IS the satire.

---

## APPENDIX A: KEY METRICS TO TRACK

| Metric | Target | Why |
|--------|--------|-----|
| Average session length | 45–90 min | Long enough for full cycles, short enough for weeknight play |
| Tokens created per session | 4–8 | Enough variety, not overwhelming |
| Rug vs. Pivot ratio | 60/40 | Rugging should be tempting but pivoting should be viable |
| GoblinTwitter posts per player per session | 20–40 | Social engine is active |
| SEC raids per session (server-wide) | 3–6 | Threatening but not constant |
| Physics ragdolls per session per player | 5–10 | Frequent enough to be funny, rare enough to be notable |
| Average multiplayer server fill | 8+ | Healthy economy requires players |

---

## APPENDIX B: RISK REGISTER

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|-----------|
| Physics networking too expensive | Medium | High | Cap networked physics objects at 30. Sleep distant props. |
| GoblinTwitter NPC posts feel repetitive | High | Medium | Template system with 50+ templates. Community submissions post-launch. |
| Economy breaks (hyperinflation/deflation) | High | High | Server wipes every 2 hours reset economy. Tune sinks aggressively. |
| Solo dev burnout | High | Critical | Strict scope. Ship 3 eras, not 5. Cut social deduction mode if needed. |
| S&box API changes before launch | Medium | High | Pin to stable S&box version. Avoid bleeding-edge APIs. |
| Players find the "optimal" strategy and stop experimenting | Medium | Medium | Random events ensure no strategy is always correct. Balance patches. |
| Crypto community takes offense | Low | Low | The goblin framing makes it clearly satirical. Lean into it. |

---

## APPENDIX C: SOLO DEV SCOPE CUTS (IF BEHIND SCHEDULE)

**Cut in this order if running behind:**

1. ~~SEC vs. Goblins mode~~ → Post-launch
2. ~~Co-op Chaos mode~~ → Post-launch
3. ~~Eras 4 and 5~~ → Post-launch (ship with 3 eras)
4. ~~GoblinDiscord~~ → Post-launch (GoblinTwitter is enough)
5. ~~Market events~~ → Reduce to 3 types instead of 10
6. ~~Sabotage system~~ → Post-launch (keep economy interaction only)
7. ~~Tutorial~~ → Sticky notes on objects + community wiki

**NEVER cut:**
- Core loop (code → shill → pump → rug/pivot)
- GoblinTwitter
- Physics comedy
- Multiplayer shared economy
- SEC raids (at least basic version)

These are the game. Everything else is content.

---

*Document version: 1.0*
*Last updated: March 31, 2026*
*Author: GDD generated for solo dev sprint targeting S&box launch April 28, 2026*
