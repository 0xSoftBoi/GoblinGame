# GOBLIN CHAIN: Crypto Chaos Tycoon
## Final Game Design Document — Production Build Spec
**Version:** 1.0 FINAL
**Date:** March 31, 2026
**Target Platform:** S&box (Source 2 Engine, C#)
**Launch Target:** April 28, 2026 (S&box Launch Day)
**Player Count:** 4–8 per lobby
**Session Length:** ~2 hours per server cycle
**Dev Scope:** Solo developer

---

## 1. Elevator Pitch

You're a literal goblin running a crypto startup out of a collapsing WeWork — code shitcoins, shill them on GoblinTwitter, dodge SEC raids, and backstab your co-founders in a physics-driven comedy tycoon where every market crash sends your office furniture flying. Think Content Warning meets Crypto Bro culture: a satirical multiplayer tycoon with social deduction, proximity chat, and an auto-clip recorder built for Twitch moments. The only crypto game on Steam that's actually funny, launching into an empty lane on S&box day one with zero tycoon competition.

---

## 2. Core Gameplay Loop

The session is a 2-hour server cycle divided into **Market Cycles** (~5–10 minutes each). Each cycle follows this beat:

### Minute-by-Minute Walkthrough (Single Cycle)

**0:00–1:30 — CODE phase**
Player sits at their desk (a folding table, cardboard box, or mahogany desk depending on era). A minigame appears: connect nodes on a circuit-board-style diagram to "write" the smart contract. Speed and accuracy determine **Token Quality** (0–100). Mistakes inject bugs that can cause crashes later. Other goblins can physically bump your chair, knock your monitor, or steal your keyboard (physics interactions). You can also copy-paste from another goblin's screen if you sneak behind them (proximity-based steal).

**1:30–3:00 — LAUNCH phase**
Token goes live on the in-game **GoblinExchange**. An announcement pops on every player's GoblinTwitter feed. Initial price is set by Token Quality + Name Hype Score (algorithm scores meme potential of the token name). The token appears on the shared Market Board mounted on the office wall (physical object, readable from across the room).

**3:00–5:00 — SHILL phase**
The core engagement window. Player opens GoblinTwitter on their in-game phone and posts shills (see Section 4). Other players see these posts and decide to buy, ignore, or counter-shill. Proximity chat lets you verbally pitch your coin to nearby goblins. Price moves in real-time based on aggregated shill effectiveness, buy volume, and market sentiment. This is where alliances form and break.

**5:00–7:00 — MARKET REACTION phase**
The market processes all inputs. Price either moons or tanks. Physics events trigger based on price movement: moon = confetti cannons, champagne bottles pop, desk levitates slightly; crash = ceiling tiles fall, monitors explode, chairs collapse (Domino Effect System, Section 8). Players holding the token either profit or lose GoblinCoin. A **Sentiment Meter** on the office wall shows the overall vibe.

**7:00–8:00 — DECISION phase**
Token creator faces the **Rug/Pivot Choice**:
- **RUG PULL:** Cash out instantly. All holders lose everything. Your reputation tanks. Office gets visibly shadier (neon signs, dark lighting). High short-term gain, long-term social consequences — other players remember.
- **PIVOT:** Rebrand the token (costs GoblinCoin). Keep holders, build reputation. Office improves slightly. Slower money, but sustainable.
- **HOLD:** Do nothing. Let the market decide. Risky but neutral reputation.

**8:00–10:00 — AFTERMATH**
Payouts distribute. Players spend GoblinCoin on office upgrades, cosmetics, or invest in other goblins' tokens. The Secret Rugger (Section 6) takes covert actions. Next cycle begins. The office physically reflects the collective financial state — furniture quality, lighting, wall decorations all shift dynamically.

### Between Cycles
- Office decoration/upgrade (30 seconds)
- Check GoblinTwitter trending tab
- Form/break alliances via proximity chat
- Trade tokens on the exchange floor

---

## 3. Token Creation System

### The Coding Minigame

**Interface:** Full-screen overlay showing a circuit board with nodes. Player drags connections between nodes to "write code." Each connection is a yes/no logic gate. Time limit: 90 seconds.

**Mechanics:**
- **Correct connections** increase Token Quality (max 100)
- **Wrong connections** inject bugs: price volatility multiplier, random crash chance, or "backdoor" that lets other players exploit it
- **Speed bonus:** Finishing under 60 seconds adds +10 Quality
- **Sabotage:** If another goblin bumps your chair during coding (physics collision), your cursor jumps — can misconnect a node
- **Copy-paste exploit:** Sneak behind another goblin's desk, hold interact for 3 seconds — you steal their template. Your token launches with their Quality score minus 20, but they get a "PLAGIARISM DETECTED" notification on GoblinTwitter (reputation hit for both)

**Quality Tiers:**
| Quality | Rating | Effect |
|---------|--------|--------|
| 0–20 | Scam | Price extremely volatile, 40% chance of auto-crash within 2 minutes |
| 21–50 | Shitcoin | Moderate volatility, basic functionality |
| 51–80 | Legit | Stable price movement, holder confidence bonus |
| 81–100 | Blue Chip | Low volatility, passive income generation, attracts NPC investors |

### Token Properties (Generated at Launch)
- **Name:** Player-chosen (see Section 11)
- **Ticker:** Auto-generated 3–5 letter abbreviation
- **Icon:** Chosen from template library or drawn in pixel editor
- **Supply:** Fixed at creation (player chooses: 1K, 10K, 100K, 1M)
- **Quality Score:** From minigame
- **Bug Count:** Hidden, from minigame mistakes
- **Creator Wallet:** Tracks who made it for rug pull attribution

---

## 4. GoblinTwitter Shilling Mechanic

GoblinTwitter is the primary economic driver. It is not a menu — it is a phone the goblin physically holds, visible to other players.

### Interface
- Accessed by pressing `T` — goblin pulls out phone, screen visible to nearby players (proximity read)
- Other goblins can peek over your shoulder to see what you're posting
- Feed updates in real-time for all players
- Three tabs: **Feed** (all posts), **Trending** (top tokens by mention volume), **DMs** (private shill/alliance)

### Posting a Shill
Player selects a token from their portfolio and composes a post. Posts are constructed from **Shill Components:**

**Template System (mix and match):**
- **Hype Opener** (pick 1): "THIS IS NOT FINANCIAL ADVICE BUT…" / "GOBLIN INSIDER INFO 🚨" / "Why is nobody talking about…" / "Just mortgaged my cave for…"
- **Claim** (pick 1): "going to 100x by tomorrow" / "backed by ancient goblin magic" / "the devs are literally wizards" / "this is the next GoblinCoin"
- **Social Proof** (pick 1): "my cousin's cousin works at GoblinSEC" / "3 whales just aped in" / "trending on GoblinReddit" / "Elon Goblin just tweeted about it"
- **CTA** (pick 1): "BUY NOW OR CRY LATER" / "last chance before moon" / "don't say I didn't warn you" / "NFA but also FA"
- **Custom text field:** 80-character free text for personal flair

**Each component has stats:**
- **Hype Power** (0–10): Raw price impact
- **Credibility** (0–10): How believable it is; affects how many NPCs buy
- **Cringe Factor** (0–10): High cringe = viral spread but lower credibility
- **Risk** (0–10): How likely to attract SEC attention

### Shill Effectiveness Formula
```
Effectiveness = (HypePower × 0.3) + (Credibility × 0.3) + (ViralSpread × 0.2) + (ReputationMultiplier × 0.2)
```

- **ViralSpread** = Cringe Factor × number of times other players interact with the post (like/repost/reply)
- **ReputationMultiplier** = Your current reputation score (0.5x to 2.0x). Rug pullers have low rep, honest goblins have high rep. But high-cringe posts from low-rep goblins can still go viral (the "dirtbag influencer" effect)

### Player Interactions with Posts
- **Like (🔥):** Small price bump (+0.5% per like)
- **Repost (🔄):** Moderate price bump (+1.5%), spreads post to more NPCs
- **FUD Reply (💀):** Counter-shill. Reduces effectiveness by 30%. Starts a public beef (content gold for clip recorder)
- **Report (🚨):** Flags post. If 3+ reports, SEC attention increases for that token

### NPC Reactions
The server runs 20–50 NPC "retail investor" bots per lobby. They:
- Browse GoblinTwitter autonomously
- Buy tokens based on shill Effectiveness minus their individual Skepticism stat (0–100)
- Panic sell on FUD or price drops
- Occasionally post their own takes ("just lost my life savings on $GRUG, thanks goblins" — affects market sentiment)
- Create organic-feeling market volume so the economy works even with 4 players

### Trending Algorithm
Posts are ranked by `(Likes + Reposts×2 + Replies×1.5) / TimeSincePost^0.5`. Top 3 trending tokens get a **Trending Bonus**: +15% buy pressure from NPCs for 60 seconds. Getting trending is a mini-win condition within each cycle.

---

## 5. Market Simulation & Manipulation

### Price Engine

Each token has a price that updates every 2 seconds based on:

```
NewPrice = CurrentPrice × (1 + NetPressure)

NetPressure = BuyPressure - SellPressure + SentimentDrift + RandomNoise

BuyPressure = (PlayerBuys × 0.01) + (NPCBuys × 0.005) + (ShillEffect × 0.008) + TrendingBonus
SellPressure = (PlayerSells × 0.01) + (NPCSells × 0.005) + (FUDEffect × 0.008) + PanicMultiplier
SentimentDrift = GlobalSentiment × 0.002 (range: -1 to +1)
RandomNoise = Random(-0.005, +0.005)
```

**PanicMultiplier** activates when price drops more than 20% in 30 seconds: all NPC sell pressure doubles for 15 seconds (cascade crash). This creates the "OH NO" moments.

### Market Manipulation Tools (Unlocked by Era)

| Tool | Era | Effect | Risk |
|------|-----|--------|------|
| Wash Trading | Startup | Fake volume (+10% buy appearance) | SEC attention +15 |
| Pump Group DM | Startup | Coordinate buy with 2+ players, 2x buy pressure for 30s | If caught, all participants get SEC heat |
| Insider Leak | Funded | Plant fake "partnership announcement" on GoblinTwitter | SEC attention +30, but massive short-term pump |
| Short Selling | Funded | Bet against a token, profit from crash | Other players see you shorted (social consequences) |
| Market Maker Bot | Exchange | Auto-stabilize your token's price within ±5% | Costs 500 GoblinCoin/cycle |
| Flash Crash Exploit | Exchange | Instantly crash any token by 40% (one-time use per session) | SEC raid guaranteed within 60 seconds |

### Market Events (Server-Wide, Random)
Every 2–3 cycles, a random event fires:
- **Bull Run:** All prices +20% for 2 minutes. Euphoria.
- **Bear Market:** All prices -15%. Panic.
- **Whale Alert:** NPC mega-buyer enters. Buys the top trending token for 50K GoblinCoin.
- **Exchange Hack:** Random token loses 50% of value. Holders get "REKT" notification.
- **Celebrity Endorsement:** Random NPC influencer shills the lowest-cap token. Price 3x.
- **Regulatory FUD:** SEC announcement. All prices freeze for 30 seconds, then drop 10%.

### The GoblinExchange (Physical Location)
A wall-mounted board in the office showing all active tokens with live prices, mini-charts, and volume bars. Players physically walk up to it to trade. This creates natural gathering points, proximity chat moments, and physical comedy (goblins shoving each other to reach the buy button during a pump).

---

## 6. Social Deduction — Rug Pull Mechanic

### Setup
At session start, one goblin is secretly assigned the **Rugger** role (like Werewolf/Among Us). The Rugger's win condition is different from everyone else.

**Normal Goblins:** Maximize personal GoblinCoin over the 2-hour session.
**The Rugger:** Execute a **Grand Rug** — accumulate 50%+ of the total GoblinCoin supply, then trigger a mass exit. If successful, the Rugger wins and everyone else's final score is halved.

### Rugger Abilities (Hidden)
- **Shadow Wallet:** Can hide up to 30% of their holdings from the public ledger
- **Fake Shill Boost:** Their GoblinTwitter posts get +20% hidden effectiveness
- **Insider Info:** Can see token Quality scores before launch (normally hidden)
- **Panic Button:** Once per session, can trigger a market-wide -25% flash crash (anonymous, but suspicious)

### Detection Mechanics
Other players must figure out who the Rugger is before the Grand Rug triggers.

**Clue System:**
- **Wallet Discrepancy:** If a player's visible wallet doesn't match their spending (they bought a lot but wallet shows low), that's suspicious. Players can check each other's public wallet at the Exchange board.
- **Shill Analysis:** Rugger's posts tend to perform unusually well. Observant players notice patterns.
- **Behavioral Tells:** Rugger might avoid buying other players' tokens, or might be overly generous (buying everything to accumulate through trading).
- **The Audit:** Any player can spend 1000 GoblinCoin to initiate an **Audit Vote**. All players vote on who they suspect. If majority votes correctly, the Rugger is exposed — their Shadow Wallet becomes visible, they lose the hidden shill boost, and they can't trigger the Grand Rug. If the vote is wrong, the falsely accused player loses 500 GoblinCoin (witch hunt penalty).

### Grand Rug Execution
If the Rugger reaches 50% supply and hasn't been caught:
- A dramatic 10-second countdown appears on all screens
- The office physically transforms — lights go red, alarms blare, all monitors show "RUG PULLED"
- All token prices crash to zero simultaneously
- Physics mayhem: desks flip, chairs fly, ceiling collapses
- The Rugger's goblin does a victory dance on a pile of GoblinCoin
- Session ends early. Rugger gets bonus XP. Everyone else gets a "RUGGED" badge

### Optional: No Rugger
30% chance a session has no Rugger (the "honest market" variant). This adds paranoia — players suspect each other even when no one is guilty. The paranoia itself affects market behavior.

---

## 7. SEC Encounters & Raids

### SEC Attention System

Each player has a hidden **SEC Heat** meter (0–100). Actions that increase heat:

| Action | Heat Increase |
|--------|--------------|
| Shill post with Risk > 7 | +5 per post |
| Wash trading | +15 |
| Insider leak | +30 |
| Rug pull | +40 |
| Getting reported on GoblinTwitter (3+ reports) | +10 |
| Pump group coordination | +10 per participant |
| Flash crash exploit | +50 |

Heat decays at -2 per minute. Heat above 70 triggers an **SEC Warning** (goblin gets a letter on their desk). Heat at 100 triggers a **Raid**.

### SEC Raid Sequence

**Warning Phase (30 seconds):**
A black SUV appears outside the office window. Dramatic music sting. Sirens. All players are alerted: "SEC INCOMING."

**The Raid (60 seconds):**
- 2–4 SEC Agent NPCs burst through the door (ragdoll physics on the door, debris flies)
- Agents pathfind toward the target goblin
- **Escape Options:**
  - **Shred Documents:** Interact with paper shredder within 10 seconds. Reduces evidence, fine is halved.
  - **Hide Under Desk:** Physics-based hiding. If agents don't pathfind to you within 30 seconds, you evade.
  - **Bribe:** Costs 2000 GoblinCoin. 70% success rate. Failure = double fine.
  - **Flee Through Window:** Jump out the window (physics ragdoll). You escape but lose your current office setup.
  - **Blame Another Goblin:** Point at another player. If that player has any SEC Heat, agents redirect (friendship-ending mechanic).

**Consequences of Getting Caught:**
- **Fine:** 50% of current GoblinCoin
- **Token Freeze:** Your active tokens are frozen for 1 full cycle (can't trade)
- **Public Shame:** "ARRESTED" stamp appears on your GoblinTwitter profile for 2 cycles
- **Office Downgrade:** Your desk loses one tier of upgrades

**Consequences of Escape:**
- Heat resets to 40 (not zero — they're still watching)
- Reputation boost ("slippery goblin" badge, some players respect this)
- Great clip moment

### SEC Miniboss: The Auditor
Once per session (around the 60-minute mark), a special **SEC Auditor** NPC arrives. This is a mini-event:
- The Auditor sits at an empty desk and reviews the Market Board
- All players must "look busy" — if caught idle (not at desk or trading) for 10+ seconds, instant +30 SEC Heat
- The Auditor leaves after 3 minutes
- If the Auditor sees a rug pull happen in real-time, instant raid on the perpetrator (no warning phase)

---

## 8. Physics Comedy / Domino Effect System

### Philosophy
Source 2's Rubikon physics engine is the comedy backbone. Every object in the office is a physics prop. The game is designed so that economic events have physical consequences, and physical interactions have economic consequences.

### Domino Effect System (DES)

The DES is a chain-reaction system where one physics event triggers another. The key design goal: every major financial event should create a **visible, chaotic, shareable physical sequence.**

**Trigger → Reaction Chains:**

**Token Moons (+50% in 30 seconds):**
1. Creator's monitor displays green rockets
2. Champagne bottle on desk uncorks (projectile physics)
3. Cork hits ceiling tile → tile falls
4. Falling tile hits neighboring goblin's monitor → monitor falls off desk
5. That goblin's chair rolls backward (reaction force)
6. Chair hits the water cooler → water spills → goblins slip on wet floor

**Token Crashes (-50% in 30 seconds):**
1. Creator's monitor cracks and sparks
2. Desk legs collapse on one side (hinge joint breaks)
3. Everything on desk slides off → cascading prop physics
4. Overhead fluorescent light flickers → falls → hits the Exchange Board
5. Exchange Board sparks → nearby paper stacks catch "fire" (particle effect, no actual fire simulation)
6. Fire alarm triggers → sprinklers → everyone gets wet → slippery floor for 30 seconds

**SEC Raid:**
1. Door bashes open (hinge destruction)
2. Door hits the coat rack → coat rack falls into a desk
3. Agents rush in → knock over the coffee table
4. Coffee mugs become projectiles
5. Panicking goblins flip tables (player-triggered or auto if AFK)
6. Paper shredder overheats → shoots confetti

**Grand Rug Pull:**
1. Floor literally cracks (decal + physics)
2. All furniture slides toward the center of the room
3. Ceiling tiles rain down
4. Lights explode one by one (timed sequence)
5. The office window shatters
6. Everything gets sucked toward the window (wind force)
7. Rugger's desk rises above the chaos (comedic levitation)

### Player-Initiated Physics
- **Flip Table:** Hold `F` at any desk. Sends all props flying. Used for rage or comedy. 5-second cooldown.
- **Throw Object:** Pick up any small prop (mug, keyboard, phone) with `E`, throw with left click. Physics projectile. Can knock another goblin off their chair.
- **Chair Race:** While seated, use movement keys to roll your chair. Collision physics with other chairs and furniture.
- **Whiteboard Eraser:** Throwable. Erases whatever it hits on a whiteboard (can erase someone's token plans).

### Physics Prop Registry (Minimum Viable Set)
Every prop must be a physics-enabled entity: desks, chairs (rolling), monitors, keyboards, mice, coffee mugs, water cooler, paper stacks, filing cabinet, whiteboard, markers, coat rack, ceiling tiles (breakable constraints), fluorescent lights (hinge joints), doors (hinge + breakable), windows (breakable), the Exchange Board, paper shredder, champagne bottles, potted plant, trash can, pizza boxes, energy drink cans, server rack (era 3+).

---

## 9. In-Game Clip Recorder System

### Design Philosophy
Modeled after Content Warning's recorder. Chaos is only valuable if it's capturable and shareable. The clip system must be zero-friction.

### Auto-Capture System
The game automatically flags **Clip-Worthy Moments** using an event scoring system:

| Event | Clip Score |
|-------|-----------|
| Token moons (+100%) | 80 |
| Token crashes to zero | 90 |
| SEC Raid begins | 85 |
| Grand Rug Pull | 100 |
| Player launched by physics (velocity > threshold) | 70 |
| 3+ objects colliding in 2 seconds | 60 |
| Player throws object and hits another player | 75 |
| Audit vote (any) | 65 |
| Two goblins arguing in proximity chat (volume detection) | 50 |

When a Clip Score > 50 event occurs:
1. A 15-second buffer is saved (10 seconds before, 5 seconds after the event)
2. A "📹 CLIP SAVED" notification appears briefly
3. Clip is stored in the session's clip reel

### Manual Recording
- Press `F9` to start/stop manual recording (max 30 seconds)
- Manual clips are added to the session clip reel
- A red "REC" indicator appears on screen during manual recording

### Session Clip Reel
At session end (or accessible mid-session via pause menu):
- All auto-captured and manual clips displayed as thumbnails
- Player can: **Preview**, **Delete**, **Export**
- Export options:
  - **GIF** (compressed, watermarked with game logo)
  - **MP4** (720p, watermarked)
  - **Save to disk**
- Clips include a simple overlay: player names, token prices at time of clip, and a "GOBLIN CHAIN" watermark

### Replay Camera
Post-session, players can re-watch key moments from any angle:
- Free camera orbit around the event
- Slow motion (0.25x, 0.5x)
- A "Director Mode" auto-selects the best angle per moment (pre-calculated)
- Export from replay camera as well

### Technical Implementation
- Use Source 2's demo recording system under the hood
- Store event timestamps with the demo file
- Render clips by replaying the demo at marked timestamps
- Keeps file sizes manageable: only store the full demo, render clips on demand
- GIF export: capture frames from playback, encode client-side via a lightweight GIF encoder (or export as webm and note that advanced export is post-launch)

### Sharing Pipeline (Post-Launch Priority)
- One-click share to Discord (webhook integration — post-launch)
- Copy GIF to clipboard
- Session highlight reel: auto-generated 60-second compilation of the session's top moments, ranked by Clip Score

---

## 10. Proximity Chat Design

### Non-Negotiable Spec
Proximity voice chat is not optional. It is the backbone of player interaction, deal-making, betrayal, and content creation.

### Technical Design

**Voice Transmission:**
- Uses Source 2's built-in voice system (Steamworks Voice API)
- Spatial audio: voice volume attenuates with distance
- **Full volume:** 0–3 meters (in-game units)
- **Falloff:** 3–8 meters (linear volume reduction)
- **Inaudible:** 8+ meters
- Directional bias: voices are 20% louder if facing the speaker (stereo panning)

**Voice Zones:**
- **Open Office:** Standard proximity rules
- **Meeting Room:** Any enclosed room acts as a private channel. Only goblins inside the room hear each other. Walls block sound. This is where secret deals happen.
- **The Exchange Board:** 1.5x volume radius (it's loud and chaotic at the trading floor)
- **Bathroom:** Private zone (one goblin at a time). Used for secret phone calls (GoblinTwitter DMs read aloud for comedy)

### Social Dynamics by Design
- **Whispering:** Hold `V` to speak at 50% volume and 50% radius. For secret conversations in public spaces. Other goblins see a "whispering" icon above your head, creating suspicion even though they can't hear.
- **Shouting:** Hold `B` to speak at 2x volume and 2x radius. For announcements, shilling, or pure chaos. "EVERYONE BUY $GRUG RIGHT NOW" heard across the office.
- **Mute:** Press `M` to mute. Your goblin puts on headphones (visual indicator). You can't hear others and they can't hear you. For when you need to focus on coding or GoblinTwitter.

### Content Creation Value
- Proximity chat + physics events = organic comedy
- The clip recorder captures voice audio with the clip
- "I TOLD YOU NOT TO BUY THAT" moments are the core shareable content
- Designed for Twitch/YouTube: viewers hear what the streamer hears spatially

### Fallback
- Text chat available via `Enter` key (global or proximity toggle)
- Text chat messages appear as speech bubbles above goblin heads
- Text chat is also posted to a chat log in the corner of the screen

---

## 11. Custom Token Creator (UGC)

### Philosophy
Infinite replayability through player-generated meme coins. The name and icon of a token directly affect its market performance through the **Meme Score** algorithm.

### Token Naming
- Free text input: 3–20 characters
- Auto-generates ticker: first letters of each word, capped at 5 characters
  - "Goblin Rocket Moon" → $GRM
  - "aaaaaaa" → $AAAAA
- Profanity filter: light touch. Block slurs, allow innuendo. The game is rated M.
- Name history: the game tracks all token names ever created per session. Duplicate names get a number suffix ($DOGE, $DOGE2, etc.)

### Meme Score Algorithm
The token's name affects market behavior through a **Meme Score** (0–100):

```
MemeScore = KeywordBonus + LengthBonus + AllCapsBonus + NumberBonus + RepetitionBonus
```

- **KeywordBonus** (0–40): Name contains known meme keywords. Maintained keyword list: "moon," "rocket," "doge," "pepe," "rug," "grug," "ape," "diamond," "hands," "hodl," "wagmi," "ngmi," "cope," "seethe," etc. Each keyword = +10, max +40.
- **LengthBonus** (0–10): Short names (3–5 chars) get +10. Long names (15+) get +5 (ironic appeal). Medium names get +0.
- **AllCapsBonus** (0–15): Fully capitalized name = +15.
- **NumberBonus** (0–10): Contains "69," "420," or "1000x" = +10.
- **RepetitionBonus** (0–10): Repeated characters ("AAAA") = +10.
- **Penalties:** Generic/boring names ("Token," "Coin," "Investment") get -20.

**Meme Score Effects:**
- Score 0–30: NPC investors skeptical. -10% NPC buy rate.
- Score 31–60: Normal NPC behavior.
- Score 61–80: NPCs more likely to FOMO buy. +15% NPC buy rate.
- Score 81–100: Viral potential. NPC influencer may auto-shill it. +30% NPC buy rate.

### Token Icon Creator
A simple pixel art editor (16x16 grid, 8-color palette):
- Player draws a tiny icon for their token
- Displayed on GoblinTwitter, the Exchange Board, and in trade UIs
- Pre-made template icons available for speed (rocket, skull, diamond, frog, etc.)
- Icons are visible to all players — bad art is part of the comedy

### Token Metadata (Player-Set)
- **Whitepaper:** A free-text field (200 char max). Displayed on the token's Exchange page. Pure flavor. "This token will revolutionize goblin-to-goblin payments" or "i made this in 30 seconds lol"
- **Website:** Optional. Player can type a fake URL that displays on the token page (cosmetic only, not a real link)

---

## 12. Progression System

### Overview
Five eras representing the arc from basement startup to crypto empire. **Launch scope: Eras 1–3.** Eras 4–5 are post-launch content.

### Era 1: Garage (0–15 min of session)

**Setting:** A literal garage/basement. Folding table, one shared computer, extension cord, pizza boxes, bare bulb lighting.

**Mechanics Available:**
- Basic token creation (coding minigame, simple version)
- GoblinTwitter (text posts only, no images)
- Manual trading on a whiteboard (walk up, write price, other goblins agree/disagree)
- Basic proximity chat
- One shared desk (goblins take turns or shove each other off)

**Upgrade Path:**
- Earn 500 GoblinCoin → unlock Era 2
- Each player progresses individually (one goblin can be in Garage while another is in Startup)

**Physics Props:** Folding table, folding chairs, pizza boxes, a single monitor, lamp, cardboard boxes.

**Vibe:** Scrappy, cramped, funny because you're literally goblins in a garage trying to make it.

### Era 2: Startup (15–40 min)

**Setting:** A proper WeWork-style open office. Individual desks, rolling chairs, a whiteboard, water cooler, slightly better lighting.

**New Mechanics Unlocked:**
- Individual desks with personal monitors
- GoblinTwitter images (attach a token icon to posts)
- The Exchange Board (wall-mounted, shared)
- Wash trading (market manipulation tier 1)
- Pump group DMs
- SEC Warning system activates (SEC agents can now appear)
- Meeting rooms unlock (private proximity chat zones)

**Upgrade Path:**
- Earn 3000 GoblinCoin total → unlock Era 3
- Or: get a token to trend 3 times on GoblinTwitter → unlock Era 3

**New Physics Props:** Rolling chairs, water cooler, coffee machine, whiteboard, filing cabinet, coat rack, potted plants, better desks with drawers.

**Vibe:** "We made it! Kind of." Actual office but clearly still chaotic.

### Era 3: Funded (40–80 min)

**Setting:** Corner office upgrade. Glass walls, standing desks, a couch, espresso machine, motivational posters ("HODL" in corporate font), a server rack in the corner.

**New Mechanics Unlocked:**
- Insider Leak (market manipulation tier 2)
- Short Selling
- Token analytics dashboard (charts, holder count, transaction history)
- NPC employees appear (walk around the office, react to events, sometimes leak info)
- The Auditor mini-event can now occur
- Office customization: choose from cosmetic items to decorate (posters, plants, desk toys)

**Upgrade Path:**
- Era 4 unlock (post-launch): 10,000 GoblinCoin + survive an SEC raid

**New Physics Props:** Standing desks (adjustable height — can be raised as a barricade), espresso machine (produces projectile cups), server rack (explodes during crashes), glass walls (breakable), couch (can be flipped), motivational posters (fall off walls during crashes).

**Vibe:** "We definitely have too much money and not enough oversight."

### Era 4: Exchange (Post-Launch)

**Setting:** You've built your own crypto exchange. Trading floor aesthetic — multiple screens, ticker tape, a bell to ring when tokens launch.

**New Mechanics:**
- Market Maker Bot
- Flash Crash Exploit
- Player-owned exchange: take a 1% fee on all trades through your exchange
- Hire NPC traders (they auto-trade for you with configurable risk levels)
- Rival exchanges (PvP between player-owned exchanges)

### Era 5: Empire (Post-Launch)

**Setting:** Penthouse office. Gold everything. A goblin throne. The office is on top of a skyscraper.

**New Mechanics:**
- Lobby-wide influence: your GoblinTwitter posts have 2x reach
- Launch your own blockchain (mega-project, requires 50,000 GoblinCoin)
- Political donations (NPC politicians, affects regulation intensity)
- Legacy system: your empire persists across sessions (light meta-progression)

### Progression Flow (Session Timeline)
```
0 min ──── Era 1: Garage ─── 15 min ──── Era 2: Startup ─── 40 min ──── Era 3: Funded ─── 80 min ────── Session End (120 min)
            Learn basics          Build & compete          High stakes & chaos          Wind-down / Grand Rug window
```

---

## 13. Multiplayer Design

### Lobby Structure
- **Lobby size:** 4–8 players (balanced for content density vs. performance)
- **Matchmaking:** Quick play (random lobby) or invite code (friends)
- **Session length:** 2 hours (fixed, with a countdown visible in the office)
- **Late join:** Allowed up to the 30-minute mark. Late joiners start in Era 1 with a 2x GoblinCoin earn rate for 10 minutes (catch-up mechanic). After 30 minutes, lobby is locked.
- **Disconnect handling:** Player's goblin goes AFK (sits at desk, does nothing). GoblinCoin holdings are frozen. If they reconnect within 5 minutes, they resume. After 5 minutes, their holdings are liquidated and distributed to the market.

### Shared Economy
- **Single currency:** GoblinCoin (earned and spent within a session, does NOT persist across sessions)
- **Market is shared:** All tokens are tradable by all players on the same Exchange Board
- **Zero-sum adjacent:** GoblinCoin is created through NPC market activity and cycle rewards, destroyed through SEC fines and rug pull losses. Net supply grows slowly over the session to prevent deflation death spirals.
- **Leaderboard:** Visible in the office (a whiteboard showing player rankings by GoblinCoin holdings). Updates every cycle. Creates social pressure and targets.

### Session Flow (Server-Side)
```
Session Start → Role Assignment (Rugger or not) → Era 1 for all → Individual Progression → Market Cycles (12-20 per session) → Grand Rug Window (last 20 min) → Session End → Scoring & Clips
```

### Scoring (End of Session)
Players are ranked by final GoblinCoin holdings. Bonus modifiers:
- **Survivor Bonus:** +500 if you never got raided by SEC
- **Shill King:** +300 for most trending posts
- **Diamond Hands:** +200 for holding a single token the entire session
- **Paper Hands:** +100 for most trades (ironic reward)
- **Rugger Victory:** +1000 if Rugger wins (Rugger only)
- **Detective:** +500 if you correctly identified the Rugger in an audit vote

XP from session scores feeds into a **persistent profile** (across sessions) that tracks:
- Total GoblinCoin earned (lifetime)
- Rug pulls executed / survived
- SEC raids escaped
- Tokens created
- Clips exported

### Anti-Grief
- **AFK kick:** 3 minutes of no input = kicked
- **Team kill prevention:** You can't directly reduce another player's GoblinCoin except through market mechanics (no stealing from wallets)
- **Vote kick:** Majority vote can kick a player (refunds their holdings to the market)
- **Report system:** Standard Steam report integration

---

## 14. Art Direction

### Constraints
Solo dev. Every art decision must be achievable by one person using S&box's tooling, asset store, and procedural generation.

### Visual Style: "Corporate Goblin"
- **Goblins:** Stylized, cartoony. Green skin, big ears, sharp teeth, beady eyes. Exaggerated proportions (big head, small body). Think World of Warcraft goblin meets Office Space.
- **Body type:** Single base mesh with morph targets for variation (fat/thin, tall/short). Customizable via cosmetics (hats, glasses, ties, hoodies).
- **Animation style:** Snappy, exaggerated. Idle animations include: picking nose, counting money, nervous sweating. Reaction animations for market events (fist pump on moon, head in hands on crash).

### Environment: The Office
- **Modular kit approach:** Build the office from modular pieces (wall segments, floor tiles, desk units, window frames). Rearrange for different eras.
- **Era 1 (Garage):** Concrete floor, bare drywall, fluorescent tubes, cardboard furniture, duct tape.
- **Era 2 (Startup):** Laminate flooring, drywall with paint, office furniture (IKEA aesthetic), motivational posters, a plant or two.
- **Era 3 (Funded):** Hardwood floor, glass partitions, standing desks, exposed brick accent wall, espresso machine, art on walls.
- **Color palette per era:**
  - Garage: Grays, browns, yellow (bare bulb warmth)
  - Startup: White, teal, orange (WeWork energy)
  - Funded: Navy, gold, white (corporate luxury)

### UI Style
- **GoblinTwitter:** Dark mode interface mimicking Twitter/X. Green accent color instead of blue. Goblin avatars next to posts. Slightly janky font (Comic Sans or a custom goblin font).
- **Exchange Board:** Green/red price tickers on a dark background. Retro LED aesthetic.
- **HUD:** Minimal. GoblinCoin count (top right), SEC Heat indicator (small thermometer, top left), current era (bottom left), cycle timer (bottom right).
- **Menus:** Clipboard/notepad aesthetic. Hand-drawn UI elements on paper textures.

### Asset Pipeline (Solo Dev Practical)
1. **Goblin model:** One base model, rigged, with blend shapes. Created in Blender, imported to S&box.
2. **Office props:** Source 2 has a built-in prop library. Use existing props where possible, retexture to match style. Custom-model only unique props (Exchange Board, GoblinPhone, specific desk items).
3. **VFX:** Source 2 particle system for: confetti, sparks, fire particles, money rain, champagne spray, screen cracks. All achievable with default particle tools.
4. **Sound:** Royalty-free SFX libraries + simple synth music. Key sounds: cash register (cha-ching on profit), crash sound (glass breaking on token crash), alarm (SEC raid), phone buzz (GoblinTwitter notification).
5. **Animations:** Mixamo for base locomotion. Custom animations only for goblin-specific actions (typing, phone use, table flip, reactions). Keep to under 20 custom animations.

### Scope Cut Art Priority
If behind schedule, cut in this order (last = cut first):
1. ~~Era 3 unique props~~ (reuse Era 2 with gold retexture)
2. ~~Custom goblin animations~~ (use generic humanoid anims)
3. ~~Pixel art token editor~~ (template icons only)
4. ~~Replay camera system~~ (keep auto-clip, cut replay)
5. **NEVER CUT:** Goblin base model, office modular kit, GoblinTwitter UI, Exchange Board, physics props

---

## 15. Technical Architecture

### Platform: S&box (Source 2, C#)

S&box provides: Source 2 rendering, Rubikon physics, networking (built-in multiplayer), Steam integration, C# scripting, asset pipeline, and a gamemode framework.

### Project Structure
```
goblin-chain/
├── code/
│   ├── Game.cs                    // Entry point, game state machine
│   ├── Player/
│   │   ├── GoblinPlayer.cs        // Player entity, input handling
│   │   ├── GoblinInventory.cs     // GoblinCoin wallet, token holdings
│   │   ├── GoblinProgression.cs   // Era tracking, unlock logic
│   │   └── GoblinCosmetics.cs     // Visual customization
│   ├── Economy/
│   │   ├── MarketEngine.cs        // Price simulation (server-authoritative)
│   │   ├── Token.cs               // Token data model
│   │   ├── TokenFactory.cs        // Token creation + quality scoring
│   │   ├── TradeManager.cs        // Buy/sell execution
│   │   ├── MemeScorer.cs          // Token name analysis
│   │   └── NPCInvestor.cs         // NPC buy/sell AI
│   ├── Social/
│   │   ├── GoblinTwitter.cs       // Feed, posting, trending algorithm
│   │   ├── ShillBuilder.cs        // Post composition UI
│   │   ├── ShillEffectCalc.cs     // Effectiveness formula
│   │   └── ReputationSystem.cs    // Player reputation tracking
│   ├── Deduction/
│   │   ├── RoleManager.cs         // Rugger assignment
│   │   ├── RuggerAbilities.cs     // Shadow wallet, fake boost, etc.
│   │   ├── AuditVote.cs           // Voting mechanic
│   │   └── GrandRug.cs            // End-game rug pull sequence
│   ├── SEC/
│   │   ├── SECHeatTracker.cs      // Per-player heat meter
│   │   ├── SECRaid.cs             // Raid event sequence
│   │   ├── SECAgent.cs            // NPC agent behavior
│   │   └── AuditorEvent.cs        // Mini-boss event
│   ├── Physics/
│   │   ├── DominoEffectSystem.cs  // Chain reaction manager
│   │   ├── PhysicsTrigger.cs      // Event → physics mapping
│   │   └── PropRegistry.cs        // Physics prop setup
│   ├── Clips/
│   │   ├── ClipRecorder.cs        // Auto-capture + manual recording
│   │   ├── ClipScorer.cs          // Event scoring for auto-capture
│   │   ├── ClipExporter.cs        // GIF/MP4 export
│   │   └── SessionReel.cs         // End-of-session compilation
│   ├── Office/
│   │   ├── OfficeManager.cs       // Era-based office state
│   │   ├── OfficeUpgrades.cs      // Furniture/decor progression
│   │   └── DeskStation.cs         // Individual desk interaction
│   ├── UI/
│   │   ├── HUD.cs                 // Minimal HUD overlay
│   │   ├── ExchangeBoard.cs       // In-world market display
│   │   ├── Leaderboard.cs         // In-world ranking board
│   │   ├── TokenCreatorUI.cs      // Name + icon + whitepaper entry
│   │   └── EndSessionScreen.cs    // Scoring, clips, stats
│   └── Networking/
│       ├── LobbyManager.cs        // Session creation, join codes
│       ├── SyncManager.cs         // State synchronization
│       └── VoiceManager.cs        // Proximity voice setup
├── assets/
│   ├── models/                    // Goblin, props, office modules
│   ├── materials/                 // Textures, shaders
│   ├── particles/                 // VFX
│   ├── sounds/                    // SFX, music
│   └── ui/                        // UI textures, fonts
└── config/
    ├── keywords.json              // Meme score keyword list
    ├── market_events.json         // Random event definitions
    ├── shill_templates.json       // Shill component library
    └── balance.json               // All tunable numbers
```

### Networking Model
- **Server-authoritative:** All economy logic (prices, trades, GoblinCoin transfers) runs on the server. Clients send requests, server validates and broadcasts results.
- **Client-predicted:** Player movement, physics interactions are client-predicted with server reconciliation.
- **State sync:** Market prices broadcast every 2 seconds. GoblinTwitter posts broadcast on creation. Physics events broadcast on trigger (server calculates chain, broadcasts result positions for non-critical props; critical props like the Exchange Board are server-authoritative).

### Key Technical Decisions

**Market Engine runs on a fixed 500ms tick.** Every tick:
1. Process pending trades
2. Recalculate all token prices
3. Evaluate NPC investor decisions
4. Check for market events
5. Broadcast new prices

**GoblinTwitter is a server-side data structure.** Posts are stored server-side, clients request feed updates. Trending is recalculated every 5 seconds. This prevents client-side manipulation.

**Domino Effect System uses a trigger → response table.** When an event fires (token moons, crash, raid), the DES looks up the chain in a config file and spawns the physics sequence. Physics is handled by Rubikon — we just apply forces/break constraints at the right time. Chain steps are time-delayed (0.3–0.5 seconds between each step) for comedic timing.

**Clip recorder hooks into Source 2's demo system.** The game records a continuous demo. On clip-worthy events, timestamp + 10-second-pre-buffer are saved. At export, the demo is replayed headlessly and frames are captured. This keeps runtime performance impact minimal (demo recording is already lightweight in Source 2).

### Performance Budget
- **Target:** 60 FPS on mid-range hardware (GTX 1060 / RX 580 equivalent)
- **Physics prop limit:** 200 active physics objects max. Props beyond view distance or not recently interacted with are set to sleep.
- **NPC limit:** 50 NPCs max (lightweight: simple pathfinding, no complex AI, no physics collision except during raids)
- **Network bandwidth:** Target < 64 KB/s per client. Market data is small. Voice is the bandwidth hog — use Opus codec at 24kbps per active speaker.

---

## 16. Monetization Strategy

### Revenue Streams (In Order of Priority)

**1. S&box Play Fund (Passive, Day 1)**
- S&box's built-in revenue share system. Players pay S&box, developers earn based on playtime in their gamemode.
- Zero implementation work. Just publish the gamemode.
- Revenue scales linearly with player count.
- This is the primary revenue stream at launch.

**2. Cosmetic Shop (Week 4 Post-Launch)**
- **Goblin Skins:** Full-body reskins. "Business Goblin" (suit), "Crypto Bro Goblin" (hoodie + backwards cap), "Hacker Goblin" (black hoodie, Guy Fawkes mask), "Diamond Goblin" (crystalline skin). $1.99–$4.99 each.
- **Office Decor:** Desk items, posters, desk toys. Visible to other players. $0.99–$1.99 each.
- **GoblinTwitter Flair:** Custom post borders, emoji packs, profile badges. $0.99 each.
- **Chair Skins:** Different rolling chairs (gaming chair, toilet, shopping cart, throne). $1.99 each.
- **Emotes:** Custom goblin emotes (dances, taunts, celebrations). $0.99 each.
- **NO pay-to-win.** No items that affect GoblinCoin earning, market performance, or gameplay mechanics.

**3. Season Pass ($9.99, Season 1 Launch ~Week 6)**
- 30-tier reward track (free track + premium track)
- Free track: basic cosmetics, GoblinCoin boosters (cosmetic only — a sparkle effect when earning, no actual multiplier)
- Premium track: exclusive skins, office themes, GoblinTwitter flair, unique chair skins
- Seasons last 8 weeks, themed around real-world crypto events (Season 1: "The Great Goblin Halving")

**4. UGC Marketplace (Post-Launch, Month 2+)**
- Players create and sell cosmetic items
- Revenue split: 70% creator / 30% developer
- Moderation: community voting + manual review for quality control
- Leverages S&box's workshop system

**5. Steam Standalone Export (Long-Term, 6+ Months)**
- If the S&box version proves the concept and builds an audience, export as a standalone Steam game
- Standalone pricing: $14.99
- Requires porting from S&box gamemode to standalone Source 2 project
- Only pursue if S&box player count plateaus and the game has proven demand

### Pricing Philosophy
- The game itself is free (within S&box, which costs money)
- All monetization is cosmetic
- Average Revenue Per User target: $3–5 over lifetime
- Whales who buy every skin: $50+ over lifetime
- Season pass is the workhorse revenue: recurring, predictable, ties players to the game

---

## 17. Community Building & Viral Strategy

### Pre-Launch (Now → April 28)

**Discord Server (Immediately):**
- Set up a Discord with channels: announcements, dev-logs, meme-submissions, token-ideas, feedback
- Post daily dev updates (screenshot + 2 sentences). Consistency > quality.
- "Name a Token" channel: players submit meme coin names, top voted ones become default templates in-game
- Role: "OG Goblin" for anyone who joins before launch (cosmetic badge in-game)

**Social Media (Twitter/X, TikTok):**
- Dev account posts short clips of physics comedy, rug pull moments, SEC raids
- Focus on the absurdity. "What if goblins ran crypto?" is the pitch.
- One 15-second TikTok per week showing a funny physics chain reaction
- Use #indiedev, #gamedev, #s&box tags

**S&box Community:**
- Engage in S&box Discord and forums
- Position as "the tycoon gamemode" — no competition in this lane
- Offer early access / beta testing to S&box community members

### Launch Day Strategy

**Discord Rich Presence (Day 1):**
- Shows "Playing GOBLIN CHAIN — Currently shilling $DOGRUG" in Discord status
- Includes lobby invite link (one-click join)
- This is free organic marketing: every player's Discord friends see it

**Streamer Seeding:**
- Identify 10–20 small S&box streamers (100–1000 viewers)
- Give them early access keys + "Streamer" in-game badge
- The game is designed for streaming: proximity chat + physics + betrayal = content
- Don't chase big streamers. Let the content speak. Small streamers who love it will make better content than big streamers doing a sponsored 1-hour stream.

**Launch Trailer:**
- 60-second trailer cut from actual gameplay
- Structure: Setup (goblin at desk) → Shill montage → Market moon → SEC raid → Grand Rug Pull → Physics chaos → Logo
- Post on YouTube, Twitter, TikTok, Reddit (r/gaming, r/indiegaming, r/cryptocurrency, r/sboxgame)

### Post-Launch Viral Mechanics

**The Clip Recorder IS the Marketing:**
- Every exported clip has a watermark with the game name
- Clips are designed to be out-of-context funny (a goblin flying through a window while money rains)
- Weekly "Best Clips" compilation on the game's YouTube channel (community-submitted)
- Monthly "Clip Contest" with cosmetic prizes

**Seasons Tied to Real Crypto Events:**
- When a real-world crypto event happens (Bitcoin halving, major hack, celebrity coin launch), release a themed mini-event within 48 hours
- "Elon Goblin tweeted about $DOGRUG" — timed with actual Elon tweets about crypto
- This creates relevance and news hooks. Game journalists write about the game when crypto is in the news.

**"Taboo Theme" Appeal:**
- Crypto culture is polarizing. People either love it or love to mock it. Both groups are the target audience.
- The game lets you live out the fantasy (getting rich on meme coins) AND mock it (everything crashes hilariously)
- Controversy is marketing. If crypto Twitter gets mad about the game, that's free press.
- Reference point: Schedule I reached 414K concurrent by leaning into a taboo theme. Goblin Chain leans into the crypto degen fantasy.

**Reddit Strategy:**
- r/cryptocurrency: "Someone made a game where you're a goblin running a crypto scam" (let the community discover it as news, don't self-promote)
- r/gaming: Clip posts with no context, just the game being funny
- r/indiegaming: Dev journey posts, transparent about being solo dev

**UGC Flywheel (Month 2+):**
- Players create tokens with absurd names → screenshots go viral → new players join to create their own tokens → more content → more virality
- Players create cosmetic items → marketplace activity → community investment in the game → retention

### Community Health
- Active Discord moderation from day 1
- Weekly "Goblin Town Hall" voice chat with dev (30 min, take feedback live)
- Public Trello/roadmap so players see what's coming
- Respond to every bug report within 24 hours

---

## 18. 10-Week Dev Roadmap

### Timeline: March 31 → April 28 (Launch) → June 9 (Post-Launch Phase 1)

**Development Philosophy:** Ship the minimum lovable product on April 28. Every feature in Weeks 1–4 must be in the launch build. Weeks 5–8 are polish and beta. Weeks 9–10 are post-launch.

---

### WEEK 1 (Mar 31 – Apr 6): Foundation

**Goal:** Player can walk around an office, sit at a desk, and interact with physics props.

| Task | Priority | Est. Hours |
|------|----------|-----------|
| S&box project setup, repo, CI | P0 | 4 |
| GoblinPlayer.cs — movement, input, basic 3rd person camera | P0 | 8 |
| Office modular kit — walls, floors, basic furniture (use S&box/Source 2 default props, retexture later) | P0 | 6 |
| Physics prop setup — desks, chairs, mugs, all interactable | P0 | 6 |
| Basic interactions: sit at desk, pick up/throw objects, flip table | P0 | 8 |
| Placeholder goblin model (capsule or basic humanoid from Mixamo) | P0 | 4 |
| Lobby creation — host/join with invite code (S&box built-in) | P0 | 4 |

**Deliverable:** 4 players in an office throwing mugs at each other. Physics works. Multiplayer works.

---

### WEEK 2 (Apr 7 – Apr 13): Economy Core

**Goal:** Tokens exist, prices move, players can trade.

| Task | Priority | Est. Hours |
|------|----------|-----------|
| Token.cs + TokenFactory.cs — data model, creation flow | P0 | 6 |
| Coding minigame — node-connection UI, quality scoring | P0 | 10 |
| MarketEngine.cs — price tick loop, buy/sell pressure calc | P0 | 10 |
| TradeManager.cs — buy/sell execution, wallet updates | P0 | 6 |
| Exchange Board — in-world UI showing prices (TextRenderer on a prop) | P0 | 6 |
| GoblinCoin wallet — HUD display, earn/spend tracking | P0 | 4 |
| MemeScorer.cs — name analysis, keyword list | P1 | 4 |

**Deliverable:** Players code tokens, launch them, and trade on the Exchange Board. Prices move based on supply/demand.

---

### WEEK 3 (Apr 14 – Apr 20): GoblinTwitter + Social

**Goal:** GoblinTwitter is functional. Players shill tokens. NPCs react. Proximity chat works.

| Task | Priority | Est. Hours |
|------|----------|-----------|
| GoblinTwitter UI — feed, posting, trending tab (in-game phone) | P0 | 12 |
| ShillBuilder.cs — template system, component selection | P0 | 8 |
| ShillEffectCalc.cs — effectiveness formula, market integration | P0 | 6 |
| NPC Investors — basic buy/sell AI reacting to shills and prices | P0 | 8 |
| Proximity voice chat — Steamworks Voice API, spatial attenuation | P0 | 6 |
| Voice zones — meeting rooms block sound | P1 | 3 |
| Like/Repost/FUD reply on GoblinTwitter posts | P0 | 4 |

**Deliverable:** Full core loop works: code → launch → shill → market reacts → profit/crash. Voice chat creates organic social moments.

---

### WEEK 4 (Apr 21 – Apr 27): Social Deduction + SEC + Polish

**Goal:** Rugger role, SEC raids, Domino Effect System, clip recorder basics. Launch-ready.

| Task | Priority | Est. Hours |
|------|----------|-----------|
| RoleManager.cs — Rugger assignment, hidden abilities | P0 | 6 |
| AuditVote.cs — voting UI, accusation logic | P0 | 4 |
| GrandRug.cs — rug pull sequence, physics mayhem, session end | P0 | 6 |
| SECHeatTracker.cs — per-player heat, decay, threshold triggers | P0 | 4 |
| SECRaid.cs — agent spawn, pathfinding to target, escape options | P0 | 8 |
| DominoEffectSystem.cs — chain reaction configs for moon/crash/raid/rug | P0 | 8 |
| ClipRecorder.cs — auto-capture on events, manual record, basic export | P1 | 8 |
| Era progression (Eras 1–3) — office state transitions, unlock logic | P0 | 6 |
| Bug fixes, balance pass, playtest | P0 | 10 |
| Discord Rich Presence integration | P1 | 2 |

**Deliverable:** Feature-complete launch build. All core mechanics work. Ready for beta testing.

---

### LAUNCH: April 28, 2026

**Day 1 Feature Set:**
- 4–8 player lobbies with invite codes
- Goblin character with basic customization
- 3 office eras (Garage → Startup → Funded)
- Token creation (coding minigame + naming + template icons)
- GoblinTwitter (post, like, repost, FUD, trending)
- Market simulation (price engine, NPC investors, market events)
- Social deduction (Rugger role, audit vote, grand rug)
- SEC encounters (heat system, raids, escape options)
- Physics comedy (Domino Effect System for all major events)
- Proximity voice chat (spatial, meeting room zones)
- Basic clip recorder (auto-capture, manual record, GIF/MP4 export)
- Discord Rich Presence
- End-of-session scoring and stats

**NOT in Day 1 (Explicitly Cut for Scope):**
- Custom pixel art token icons (template only)
- Replay camera system
- Cosmetic shop
- Season pass
- UGC marketplace
- Eras 4–5
- Market Maker Bot / Flash Crash Exploit
- Whisper/shout voice modes
- Session highlight reel auto-compilation
- Streamer integration
- The Auditor mini-boss event

---

### WEEK 5–6 (Apr 28 – May 11): Hotfix + First Content

**Goal:** Fix launch bugs, add most-requested features, start cosmetic shop.

| Task | Priority | Est. Hours |
|------|----------|-----------|
| Bug fixes from launch feedback | P0 | 20 |
| Balance tuning — market volatility, shill effectiveness, SEC heat rates | P0 | 8 |
| Goblin model — proper model replacing placeholder (if placeholder shipped) | P0 | 12 |
| Cosmetic shop v1 — 5 goblin skins, 10 office items | P1 | 12 |
| Whisper/shout voice modes | P1 | 4 |
| The Auditor mini-boss event | P1 | 6 |
| Pixel art token icon editor | P2 | 8 |
| Session highlight reel | P2 | 8 |

---

### WEEK 7–8 (May 12 – May 25): Season 1 Prep

| Task | Priority | Est. Hours |
|------|----------|-----------|
| Season Pass infrastructure — tier system, reward track, XP earn | P1 | 16 |
| Season 1 content — themed cosmetics, market event tied to real crypto news | P1 | 12 |
| Replay camera system | P2 | 12 |
| Market Maker Bot + Flash Crash Exploit (market manipulation tier 2) | P2 | 8 |
| Performance optimization pass | P1 | 8 |
| Community feedback features (top requests from Discord) | P1 | 8 |

---

### WEEK 9–10 (May 26 – Jun 9): Season 1 Launch + UGC Groundwork

| Task | Priority | Est. Hours |
|------|----------|-----------|
| Season 1 live launch | P1 | 4 |
| UGC Marketplace foundation — item submission, review, listing | P2 | 20 |
| Era 4 (Exchange) — initial implementation | P2 | 16 |
| Clip sharing pipeline — Discord webhook, copy to clipboard | P2 | 6 |
| Ongoing balance and bug fixes | P0 | 12 |

---

### Scope Cut Priority (If Behind Schedule)

Cut in this order. Bottom = cut first. Top = never cut.

```
NEVER CUT (Core Experience):
├── Multiplayer lobbies (4-8 players)
├── Goblin movement + physics props
├── Token creation (coding minigame)
├── GoblinTwitter (post + feed + trending)
├── Market simulation (price engine)
├── Proximity voice chat
└── Basic buy/sell trading

CUT RELUCTANTLY (High Value):
├── Social deduction (Rugger role) → Simplify to: any player CAN rug pull, no hidden role
├── SEC raids → Simplify to: fines only, no physical raid event
├── Domino Effect chains → Simplify to: single physics reactions, no chains
└── Era progression → Ship with single era (Startup), unlock others post-launch

CUT IF NEEDED (Nice to Have):
├── Clip recorder → Cut entirely, players use OBS/Medal
├── Discord Rich Presence → Post-launch
├── Meeting room voice zones → All proximity, no zones
├── Market events (random bull/bear) → Static market, player-driven only
└── NPC investors → Player-only economy (needs 6+ players to work)

CUT FIRST (Polish):
├── End-of-session replay camera
├── Session highlight reel
├── Pixel art token icon editor
├── Whisper/shout voice modes
├── Scoring bonuses and badges
└── The Auditor mini-boss
```

---

## Appendix A: Key Balance Numbers

All values in `balance.json`. Tunable without code changes.

| Parameter | Value | Notes |
|-----------|-------|-------|
| Starting GoblinCoin | 1000 | Per player, per session |
| Cycle length | 5–10 min | Varies by phase |
| Token coding time | 90 sec | Minigame duration |
| Max active tokens per player | 3 | Prevents spam |
| Max total tokens per session | 24 | 8 players × 3 |
| NPC investor count | 30 | Scale with player count: 30 base + 5 per player |
| NPC skepticism range | 20–80 | Uniform distribution |
| SEC heat decay | -2/min | Keeps pressure temporary |
| SEC raid threshold | 100 heat | Triggers raid event |
| Rug pull reputation penalty | -40 rep | Out of 100 |
| Audit vote cost | 1000 GoblinCoin | Prevents spam voting |
| Trending bonus duration | 60 sec | +15% NPC buy pressure |
| Grand Rug threshold | 50% supply | Rugger needs this to win |
| Session length | 120 min | Fixed |
| Late join cutoff | 30 min | Lobby locks after |
| AFK kick timer | 3 min | No input = kicked |
| Physics prop limit | 200 active | Sleep distant props |
| Voice full range | 3m | In-game units |
| Voice falloff range | 3–8m | Linear attenuation |
| Voice inaudible range | 8m+ | Silent |

---

## Appendix B: File/Config Schema

### keywords.json (Meme Score)
```json
{
  "tier1_10pts": ["moon", "rocket", "doge", "pepe", "rug", "ape", "diamond", "hodl"],
  "tier2_5pts": ["wagmi", "ngmi", "cope", "seethe", "fren", "grug", "hands", "bags"],
  "penalties_neg20": ["token", "coin", "investment", "blockchain", "protocol", "finance"],
  "number_bonus_10pts": ["69", "420", "1000x", "100x"]
}
```

### shill_templates.json (Sample)
```json
{
  "openers": [
    {"text": "THIS IS NOT FINANCIAL ADVICE BUT…", "hype": 6, "credibility": 3, "cringe": 7, "risk": 2},
    {"text": "GOBLIN INSIDER INFO", "hype": 8, "credibility": 5, "cringe": 5, "risk": 6},
    {"text": "Why is nobody talking about…", "hype": 5, "credibility": 7, "cringe": 3, "risk": 1}
  ],
  "claims": [
    {"text": "going to 100x by tomorrow", "hype": 9, "credibility": 1, "cringe": 8, "risk": 3},
    {"text": "backed by ancient goblin magic", "hype": 7, "credibility": 2, "cringe": 9, "risk": 1},
    {"text": "the devs are literally wizards", "hype": 6, "credibility": 4, "cringe": 6, "risk": 2}
  ]
}
```

---

*This document is the single source of truth. Build from it. Deviate only with documented reasoning. Ship April 28.*

*GOBLIN CHAIN: Crypto Chaos Tycoon — because every degen deserves a tycoon game.*
