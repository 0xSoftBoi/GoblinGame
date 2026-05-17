# GOBLIN CHAIN: Crypto Chaos Tycoon
## Production-Ready Game Design Document — v3.0 FINAL
**Target Platform:** S&box (Source 2 / C#)
**Target Launch:** April 28, 2026
**Team Size:** Solo developer
**Lobby Size:** 4–8 players
**Session Length:** ~2 hours per server session
**Core Loop:** 5–10 minutes

---

## 1. ELEVATOR PITCH

You're a literal goblin running a crypto startup out of a collapsing WeWork. Scam investors with made-up meme coins, shill your token on GoblinTwitter, dodge SEC raids, sabotage rival goblins — and decide whether to rug pull or pivot legit before it all comes crashing down.

**One sentence:** *Among Us meets Game Dev Tycoon, set in the dumpster fire of crypto culture, with Source 2 physics comedy.*

**Why it wins:**
Every crypto game on Steam takes itself seriously and averages ~1 concurrent player. The satirical lane is completely empty. Goblin framing removes moral discomfort and lets the satire hit harder — you're not scamming real people, you're a goblin doing goblin things. S&box launches with zero tycoon gamemodes, making this a first-mover play. Schedule I proved taboo-theme + co-op chaos can hit 414K concurrent. Human Fall Flat went from 10K to 55M copies after adding multiplayer. This game is designed from the ground up to be both played AND watched.

---

## 2. CORE GAMEPLAY LOOP — MINUTE BY MINUTE

### Phase 1: The Bullpen (Minutes 0–2)
All 4–8 goblins spawn in a shared, dilapidated WeWork floor. Proximity chat is live. Each goblin claims a desk (physically — shove other goblins off chairs using Source 2 physics). A whiteboard in the center shows the current Market Mood (bull/bear/crab). Players open their laptops (in-game) and enter the Token Creator.

### Phase 2: Token Launch (Minutes 2–5)
Each goblin creates a custom meme coin using the Token Creator (name it, pick an icon, write a tagline). The token goes live on the in-game exchange. Initial price is near zero. The race begins: shill your coin on GoblinTwitter to pump the price before anyone else. This is frantic typing, meme selection, and hashtag strategy.

### Phase 3: The Shill War (Minutes 5–15)
The core mechanic loop. Players cycle between:

- **GoblinTwitter posting** — compose shills, reply to NPCs, ratio rival goblins, build follower count. Each post has a Virality Score based on timing, meme quality, and trend-riding.
- **Market watching** — monitor your token price, spot pump patterns, decide when to buy/sell rival tokens.
- **Office sabotage** — physically interact with the WeWork. Unplug a rival's router (their GoblinTwitter goes offline for 30 seconds). Steal their whiteboard markers. Stack furniture to block their desk. All physics-driven.
- **Investor meetings** — NPC investors walk into the WeWork. Pitch them in proximity chat. If they buy in, your token pumps. If you fumble, they invest in a rival.

### Phase 4: The Chaos Escalator (Minutes 15–25)
Events fire every 3–5 minutes, escalating intensity:

- **SEC Raid:** Agents storm the WeWork. Hide your laptop, destroy evidence, or bribe them. Getting caught freezes your GoblinTwitter for 60 seconds.
- **Market Crash:** A random external event tanks all tokens 30–50%. Opportunity to buy the dip or panic sell.
- **Whale Alert:** An NPC mega-investor enters. All goblins compete to pitch them. Whoever wins gets a massive price pump.
- **Infrastructure Decay:** The WeWork itself degrades. Ceiling tiles fall (physics!), lights flicker, the coffee machine explodes. The building is a ticking clock.

### Phase 5: The Rug or Pivot (Minutes 25–30)
Each goblin faces THE choice:

- **RUG PULL:** Cash out everything, drain investor funds, and escape through the fire exit. You get a massive one-time payout. Your reputation is destroyed. The building literally shakes. Other players see your token crater in real-time.
- **PIVOT:** Announce a "legitimate business pivot." Your token stabilizes. You earn slower but sustainable income. You can keep playing future rounds with reputation intact.

This is not binary — it's a spectrum. You can partial-rug (sell 60%, claim you're "rebalancing"). You can fake-pivot (announce legitimacy, then rug later). The social dynamics are the game.

### Phase 6: Aftermath & Scoring (Minutes 30–32)
Leaderboard shows: total goblin net worth, biggest rug pull, most GoblinTwitter followers, "Most Legitimate Goblin" award, best clip (auto-captured). The clip recorder highlights reel plays automatically. Players vote on "Goblin of the Round." New round begins with escalated difficulty.

### Full Session Arc (2 hours)
A server session runs 3–4 rounds. Each round, the WeWork degrades further (Era progression — see Section 7). By the final round, you're operating out of a literal dumpster behind the building. Tokens from previous rounds carry reputation effects into new ones.

---

## 3. MECHANICS — COMPLETE BREAKDOWN

### 3A. Token Creator (UGC System)

The Token Creator is the game's infinite content engine. Every player-created token is unique and spreadable.

**Creation Flow:**
1. **Name your coin** — free text input, profanity filter with goblin-themed bypass (e.g., "POOP" blocked, "$POOPGOBLIN" allowed). 32-character limit.
2. **Choose an icon** — pick from ~50 base icons (rocket, moon, diamond hands, dumpster fire, goblin face variants) OR upload a simple pixel art design using a 16x16 grid editor.
3. **Write a tagline** — 80 characters. This appears on GoblinTwitter and the exchange. "To the moon" energy.
4. **Set initial supply** — slider from 1M to 1T tokens. Higher supply = lower per-token price = feels more "memecoin."
5. **Tokenomics preset** — choose from satirical templates: "Ponzi Classic," "Deflationary Delusion," "Liquidity Mirage," "Honest Rug." Each has different price curve behavior.

**Why this matters for virality:** Players screenshot their coins. They share "$GOBLINBALLS is up 4000%!" on Discord. Every token is a micro-meme that lives outside the game. This is the Content Warning model — the game creates content that markets itself.

**Technical implementation (C#):**
```
public class TokenData {
    public string Name;           // Player-chosen
    public string Tagline;        // Player-chosen
    public Texture2D Icon;        // From preset or pixel editor
    public long TotalSupply;      // Player-chosen
    public TokenomicsType Curve;  // Enum: Ponzi, Deflationary, Mirage, Rug
    public float CurrentPrice;    // Driven by MarketSimulator
    public ulong CreatorSteamId;  // Attribution
    public List<Transaction> History;
}
```

### 3B. GoblinTwitter — The Core Shilling Mechanic

GoblinTwitter is the primary interaction system. It's a fake social media platform that runs inside the game, displayed on your goblin's laptop screen.

**Post Types:**
- **Shill Post** — promote your token. Costs Energy (regenerates over time). Virality depends on: timing (posting during a pump = bonus), meme template quality, hashtag relevance to current Market Mood.
- **FUD Post** — attack a rival's token. "I heard $GOONCOIN's dev is a lizard person." Reduces target token's Trust Score.
- **Reply/Ratio** — respond to NPC or player posts. Successful ratios boost your follower count. Failed ones make you look desperate.
- **Meme Post** — select from meme templates, fill in text. Higher effort = higher virality. The "Drake meme but with goblins" type content.

**Virality Score Calculation:**
```
ViralityScore = (MemeQuality * TrendMultiplier * FollowerBase * TimingBonus) - FatigueCounter
```
- **MemeQuality:** 1–10 based on template tier and text relevance
- **TrendMultiplier:** 1.0–3.0 based on matching current Market Mood hashtags
- **FollowerBase:** sqrt(Followers) — diminishing returns prevent runaway
- **TimingBonus:** 1.5x if posted within 30 seconds of a market event
- **FatigueCounter:** increases with each post per minute, penalizes spam

**Follower System:**
NPC followers accumulate based on post performance. Milestones unlock new post types and meme templates. Follower count persists across rounds within a session, creating compounding advantage for consistent shillers.

**Follower Milestones:**
| Followers | Unlock |
|-----------|--------|
| 100 | Reply/Ratio ability |
| 500 | Meme Post templates (Tier 2) |
| 1,000 | Promoted Posts (2x reach, costs in-game cash) |
| 5,000 | Blue Checkmark (GoblinVerified™ — 1.5x Trust on all posts) |
| 10,000 | Bot Army (auto-generates low-quality shill posts in background) |
| 50,000 | Influencer Status (NPC investors seek YOU out) |

### 3C. Market Manipulation & Economy Simulation

**The Exchange:**
A simplified order-book system. Each token has a price driven by buy/sell pressure plus external events. Players can:
- **Buy** any token (including rivals')
- **Sell** their holdings
- **Short** a rival's token (bet on it going down — high risk, high reward)
- **Wash Trade** their own token (buy and sell to yourself to fake volume — illegal IRL, hilarious in-game, risks SEC detection)

**Price Formula (simplified for gameplay):**
```
NewPrice = CurrentPrice * (1 + (BuyPressure - SellPressure) / TotalLiquidity) * MarketMoodMultiplier * EventMultiplier
```

**Market Moods (rotate every 5 minutes):**
- **Bull:** All prices drift upward 2%/min. Shilling is 1.5x effective. Everyone's happy.
- **Bear:** All prices drift downward 3%/min. FUD posts are 2x effective. Panic selling cascades.
- **Crab:** Prices barely move. The boring market. Players get restless and start sabotaging each other.
- **FOMO Frenzy:** One random token pumps 10x. Everyone scrambles to buy in. Classic bubble.
- **Dead Cat Bounce:** After a crash, a fake recovery. Traps greedy goblins.

### 3D. Social Deduction — The Secret Rug Puller

At the start of each round, one goblin is secretly assigned the **Rug Puller** role. This is the Among Us layer.

**How it works:**
- At round start, each player gets a card: "Legitimate Goblin" or "Rug Puller." The Rug Puller knows who they are. Nobody else does.
- The Rug Puller has a hidden objective: pump their token to a target price AND execute the rug pull before being identified. They get bonus tools: fake transaction records, a "dead wallet" to hide funds, and a 1-time GoblinTwitter bot swarm.
- Other goblins can call a **Goblin Council** vote (costs reputation to call, prevents spam). If the group correctly identifies and votes out the Rug Puller before the rug, they split a bounty. If they vote out an innocent goblin, the accused loses 50% of their holdings.
- The Rug Puller can also bluff: call a vote on someone else, claim someone else's suspicious trades are rug indicators, etc.

**Evidence system:**
Players can see public transaction logs on the exchange. Suspicious patterns (rapid accumulation, creation of multiple wallets, unusual sell orders) appear as "Blockchain Anomalies" that attentive players can spot. This creates emergent detective gameplay.

**Why this works:** It gives every round a paranoia layer. Proximity chat conversations become loaded. "Why did you just sell 40% of your $GOONCOIN, Steve?" "I was rebalancing!" "That's what a rug puller would say." This is pure streaming gold.

### 3E. SEC Encounters

SEC agents are NPC antagonists that create chaos and comedy.

**SEC Raid Event:**
- Warning: 15-second alert ("BREAKING: SEC vans spotted outside"). Players scramble.
- Agents enter the WeWork. They walk to random desks and "inspect" laptops.
- If caught with wash trades, bot armies, or other violations on your screen, you get "Investigated" — GoblinTwitter locked for 60 seconds, forced to attend an "SEC Hearing" minigame.

**SEC Hearing Minigame:**
A rapid-fire Q&A. The SEC asks questions ("Is $GOONCOIN a security?"), and you pick from goblin-brained answers ("It's not a security, it's a community," "I don't recall," "My lawyer is also a goblin"). Funny answers get you off with a warning. Bad answers mean a fine (lose 20% of cash).

**Bribery system:** You can physically hand cash to SEC agents (drag cash item onto agent). Small bribes make them look the other way. Large bribes make them investigate a RIVAL instead. Getting caught bribing is a bigger fine.

### 3F. Physics Comedy / Domino Effect System

Source 2's Rubikon physics engine is the comedy backbone. Every object in the WeWork is physically simulated.

**Interactive Objects:**
- Chairs (throwable, stackable, breakable)
- Desks (flippable — sends laptop flying)
- Whiteboards (writable with marker tool, physics-enabled)
- Coffee machine (explodes if overused, sprays physics-enabled coffee)
- Ceiling tiles (fall on timers, can be pulled down deliberately)
- Filing cabinets (blockade material)
- Server rack (the "internet" — unplug it to kill everyone's GoblinTwitter for 10 seconds)
- Printer (prints fake documents — can be used to forge investor reports)

**Domino Effect System:**
Chain reactions are tracked and scored. If you throw a chair that hits a desk that flips a laptop that smashes a coffee machine that sprays an SEC agent, each link in the chain adds to a "Chaos Multiplier." High Chaos Multiplier = bonus XP, clip recorder auto-triggers, and a "DOMINO EFFECT" banner appears on screen.

**Goblin Physics:**
Goblins are ragdoll-capable. They trip over debris, get knocked by falling objects, and can be physically shoved by other goblins. This is the "Human Fall Flat" comedy layer — the physical world is as much an opponent as the other players.

---

## 4. IN-GAME CLIP RECORDER

This is a critical virality system, modeled on Content Warning (which hit 200K concurrent in 24 hours because the game generates its own marketing content).

### Design Philosophy
Every rug pull, physics disaster, SEC raid, and social deduction accusation is a potential clip. The game should capture these moments automatically and make sharing frictionless.

### System Architecture

**Continuous Buffer:**
The game maintains a rolling 60-second video buffer at all times. When a "Clip-Worthy Event" triggers, it saves the buffer.

**Clip-Worthy Event Triggers:**
- Rug pull executed (any player)
- Token price change > 500% in 60 seconds
- Domino Effect chain of 3+ objects
- Goblin Council vote called
- SEC raid starts
- Player gets "Investigated"
- Any goblin ragdolls for > 3 seconds
- Custom token crosses a price milestone ($1, $100, $1000)
- Manual trigger (player presses F9)

**Auto-Edit System:**
When a clip triggers, the system:
1. Saves 15 seconds before + 10 seconds after the trigger event
2. Adds a dramatic zoom on the key moment
3. Overlays the token price chart (if market-related)
4. Adds a comedic subtitle based on event type ("RUG PULLED," "SEC'D," "GOBLIN'D")
5. Exports as MP4 at 720p (small file, easy to share)

**Sharing Flow:**
End-of-round screen shows all clips from that round as a highlight reel. Players vote on "Best Clip." Winner gets bonus cosmetic currency. One-click export to:
- Local file (for YouTube/TikTok upload)
- Steam screenshot overlay
- Discord (via Rich Presence integration)

**Technical Implementation (C#):**
```csharp
public class ClipRecorder : Component
{
    private RingBuffer<FrameData> frameBuffer; // 60-second rolling buffer
    private const int BUFFER_SECONDS = 60;
    private const int PRE_CLIP = 15;
    private const int POST_CLIP = 10;

    [Event("clip.trigger")]
    public void OnClipTrigger(ClipEvent evt)
    {
        var clip = frameBuffer.Extract(PRE_CLIP, POST_CLIP);
        clip.AddOverlay(evt.OverlayType);
        clip.AddSubtitle(evt.Caption);
        ClipLibrary.Save(clip, evt.Category);
    }
}
```

**Streamer Mode:**
A toggle that adds a persistent token ticker overlay to the game view (like a stock ticker on CNBC), making streams look like financial news coverage of goblin chaos. This is free production value for streamers.

---

## 5. PROXIMITY CHAT

Proximity chat is non-negotiable. Every breakout indie multiplayer hit (Content Warning, Lethal Company, Among Us with proximity mods) used spatial audio to create emergent comedy.

### Implementation

**Using Vivox (S&box built-in):**
S&box provides Vivox integration for voice chat. We use it in spatial/proximity mode.

**Range Settings:**
- **Full clarity:** 0–5 meters (desk-to-desk in the WeWork)
- **Fade zone:** 5–15 meters (can hear muffled, like overhearing a conversation)
- **Inaudible:** 15+ meters

**Gameplay Integration:**
- **Investor pitches** require you to be physically near the NPC investor AND talking into your mic. Proximity chat IS the pitch mechanic.
- **Goblin Council votes** create a "huddle" — goblins physically gather in the conference room. Proximity chat means side conversations happen. Whispered alliances. Accusations. This is the social deduction magic.
- **Eavesdropping** is a real mechanic. Sneak up on rival goblins to hear their strategy. Are they planning a rug? Are they colluding on a pump?
- **The Server Room** is a special zone — proximity chat is disabled inside it (it's "too loud"). This is the safe space for secret phone calls and hidden transactions.

**Voice Effects (post-launch):**
- Goblin voice filter (optional, pitch-shifted)
- "Phone call" effect when in the Server Room
- Echo in the bathroom
- Muffled through walls

---

## 6. CUSTOM TOKEN CREATOR — DEEP DIVE

### UGC Pipeline

The custom token system is designed to generate content that escapes the game and markets it organically.

**Token Card System:**
Every token gets auto-generated into a "trading card" format:
- Token name and icon
- Creator's goblin name
- Current price and 24h chart
- A satirical "whitepaper" blurb (auto-generated from templates + player tagline)

These cards are exportable as PNG. Players share them on Discord, Twitter, Reddit. Each card has the game's logo watermarked subtly. Every shared card is a free ad.

**Token Hall of Fame:**
A persistent leaderboard across all servers:
- Highest market cap token ever
- Fastest rug pull
- Most traded token
- Longest-surviving token
- Most creative name (community voted weekly)

**Token Templates (starter coins for new players):**
New players who don't want to create from scratch can pick from satirical presets:
- $GOBLINCOIN — the vanilla option
- $RUGRATS — "definitely not a rug, trust us"
- $MOONDUST — "we're going to the moon (the dust part is the rug)"
- $SAFEGOBLIN — "it's safe, we promise"
- $ELONSGOBLIN — the celebrity endorsement play

---

## 7. PROGRESSION SYSTEM — ERA BREAKDOWN

Progression is environmental AND mechanical. As rounds advance, the physical space degrades while the financial stakes escalate.

### Era 1: Stealth Startup (Rounds 1–2)
**Setting:** Pristine WeWork floor. Free kombucha. Beanbag chairs. Motivational posters ("Hustle Harder, Goblin").
**Mechanics available:** Basic GoblinTwitter (text posts only), simple buy/sell, no SEC presence.
**Token behavior:** Low volatility, small NPC investor pool.
**Tone:** Optimistic. Everything seems legitimate. The comedy comes from goblin incompetence.

### Era 2: Growth Hacking (Rounds 3–4)
**Setting:** WeWork is getting messy. Energy drink cans everywhere. One ceiling tile missing. A whiteboard covered in conspiracy-theory-style market analysis.
**New mechanics unlocked:** Meme posts on GoblinTwitter, short selling, first SEC patrol (passive — they walk through but don't raid yet).
**Token behavior:** Medium volatility, larger investor pool, first "Whale Alert" events.
**Tone:** The hustle intensifies. Goblins start side-eyeing each other.

### Era 3: Bubble Territory (Rounds 5–6)
**Setting:** WeWork is falling apart. Half the lights are out. The coffee machine is on fire (it stays on fire). Furniture is broken from previous physics chaos. Rain leaks through the ceiling.
**New mechanics unlocked:** Wash trading, bot armies, full SEC raids, Goblin Council votes, the Rug Puller role is now active.
**Token behavior:** High volatility, FOMO Frenzy events, Dead Cat Bounces. Prices can 100x or go to zero.
**Tone:** Paranoia. Everyone suspects everyone. The building might collapse.

### Era 4: The Dumpster (Final Round)
**Setting:** The WeWork has been condemned. Goblins operate from a dumpster in the alley behind the building, using a single shared laptop balanced on trash bags. A rat is an NPC investor.
**All mechanics active.** Extreme volatility. SEC raids every 2 minutes. The dumpster can tip over (physics event).
**Tone:** Peak absurdity. The game's final statement: this is where crypto culture ends up.

---

## 8. MULTIPLAYER DESIGN

### Lobby Structure

**4–8 Players per lobby.** This is non-negotiable for launch. Do not attempt 16+ players. The social deduction mechanics require intimacy — you need to know who everyone is and what they're doing.

**Matchmaking:**
- Quick Play: random lobby, fills to 4 minimum before starting (2-minute max wait, then starts with available players + bots)
- Friends Only: private lobby with invite code
- S&box Server Browser: community-hosted persistent servers

**Bot Backfill:**
If a lobby has fewer than 4 humans, goblin bots fill the remaining slots. Bots have simple behavior: create a random token, post basic shills, occasionally sabotage. They're not smart — they're there to fill the economy and be funny. Bot names are procedurally generated goblin names ("Skragz," "Bliznort," "Fungsworth III").

### Network Architecture

**Authority Model:** Server-authoritative for all economy actions (token prices, trades, cash balances). Client-side prediction for physics and movement (Source 2 handles this natively).

**Sync Priority:**
1. Trade/economy events (reliable, ordered)
2. GoblinTwitter posts (reliable, unordered)
3. Physics state (unreliable, interpolated)
4. Cosmetic/visual (lowest priority)

**Bandwidth target:** < 50 KB/s per player. Token state is small (price + volume per tick). GoblinTwitter is text. Physics uses S&box's built-in network replication.

---

## 9. ECONOMY DESIGN

### Currency Layers

**GoblinBucks ($GB):**
The primary in-game currency. Earned by selling tokens, winning investor pitches, collecting rent from token holders, and end-of-round bonuses. Spent on: GoblinTwitter promoted posts, SEC bribes, office items, wash trading fees.

**Reputation Points (RP):**
Earned by: successful shills (high virality), winning Goblin Council votes, completing rounds without rugging. Spent on: unlocking GoblinTwitter tiers, accessing premium meme templates, entering high-stakes lobbies. Reputation persists across sessions.

**Chaos Tokens (CT):**
Earned by: physics chain reactions, sabotage, and being generally chaotic. Spent on: cosmetic shop items only. This is the "play for fun" currency — you earn it by making the game entertaining, not by being financially successful.

### Economy Balance Targets

| Metric | Target |
|--------|--------|
| Starting cash per round | $10,000 GB |
| Average round earnings (no rug) | $15,000–$25,000 GB |
| Successful rug pull payout | $50,000–$100,000 GB |
| Failed rug pull penalty | -$30,000 GB + reputation loss |
| SEC fine (caught) | -20% current cash |
| Bribery cost | $2,000–$5,000 GB |

### Anti-Exploitation

- Token prices hard-cap at 10,000x initial price (prevents infinite exploits)
- Wash trading has a 5% fee per transaction (makes it costly to spam)
- GoblinTwitter post cooldown prevents bot-like behavior from humans
- Economy resets between server sessions (no cross-session wealth hoarding)

---

## 10. PHYSICS COMEDY — DOMINO EFFECT DEEP DIVE

### Chain Reaction Scoring

Every physics interaction is tracked. When objects collide with enough force to trigger secondary effects, a chain begins.

**Chain Scoring:**
```
ChainScore = BasePoints * (ChainLength ^ 1.5) * UniquenessBonus
```

- **BasePoints:** 10 per object involved
- **ChainLength:** number of sequential physics interactions
- **UniquenessBonus:** 1.5x if chain involves 3+ different object types

**Chain Examples:**
- Throw coffee mug → hits keyboard → laptop falls → lands on SEC agent's foot → agent ragdolls = 5-chain, ~170 points
- Flip desk → desk hits filing cabinet → cabinet topples → papers fly everywhere → paper lands on candle → fire starts = 6-chain + fire bonus

**Fire System (simple):**
Certain chain reactions can start small fires. Fire spreads to nearby paper/furniture over 30 seconds. A fire extinguisher exists (one per floor). Letting fire spread triggers the fire alarm, which causes ALL goblins to be forced outside for 20 seconds (disrupting trades and shills). This creates emergent strategy — start a fire when rivals are mid-rug-pull to interrupt them.

### Physics Sandbox Objects (Launch Set)

| Object | Interaction | Comedy Potential |
|--------|-------------|-----------------|
| Office chair (wheeled) | Sit, push, throw, ride down hallway | High — chair jousting |
| Standing desk | Raise/lower, flip | Medium — catapult items off it |
| Whiteboard | Write on, push over, use as shield | High — write accusations |
| Coffee machine | Use, overuse (explodes), throw | Very high — explosion chains |
| Water cooler | Drink, tip over (flood physics) | Medium — slippery floors |
| Server rack | Unplug cables, push over | High — kills everyone's internet |
| Printer | Print documents, jam, throw paper | Medium — paper storm |
| Fire extinguisher | Spray (knockback physics), throw | High — emergency weapon |
| Ceiling tiles | Pull down, fall on goblins | High — environmental hazard |
| Beanbag chair | Sit, throw (low damage, funny) | Medium — comfort weapon |

---

## 11. ART DIRECTION — SCOPED FOR SOLO DEV

### Visual Style: "Corporate Goblin"

**The Look:** Low-poly stylized with high-contrast, readable silhouettes. Think Overcooked meets Dungeon Keeper. Not realistic — goblins are 3-head-tall cartoony creatures in ill-fitting business casual.

### Color Palette

- **Primary:** Sickly green (goblin skin), corporate grey (WeWork walls), neon orange (crypto/hype elements)
- **Accent:** Gold (money/success), red (danger/SEC), blue (GoblinTwitter)
- **Environmental shift:** As eras progress, the palette shifts from clean corporate (whites, grays, accent colors) to grimy decay (browns, dark greens, flickering neon)

### Character Design

**Goblin Base Model:**
One base goblin mesh with modular customization. Small (roughly 1m tall), hunched posture, oversized head and hands (better readability at game camera distance), pointy ears that poke through headwear.

**Customization (launch):**
- 5 skin tones (various greens/browns)
- 8 hairstyles (including "bald with combover" and "crypto bro slicked back")
- Business casual clothing: wrinkled shirts, loose ties, khaki shorts, crocs with socks
- Accessories: oversized sunglasses, bluetooth earpiece, "HODL" wristband

**Cosmetics (post-launch, earnable via Chaos Tokens):**
- Diamond hands (literal diamond hand models)
- Laser eyes (red glow effect)
- Top hat and monocle ("Old Money Goblin")
- Hoodie with own token logo
- Goblin-sized Lambo toy car (carries it around)

### Environment Design

**The WeWork floor (one level, iterated across eras):**
A single open-plan office floor. Modular design — the same base layout is dressed differently per era.

- **Era 1:** Clean. IKEA-style furniture. Plants. Startup posters.
- **Era 2:** Messy. Food wrappers. Crooked posters. One broken window.
- **Era 3:** Destroyed. Half the furniture is broken/flipped. Water damage. Graffiti.
- **Era 4:** Exterior dumpster scene. Trash bags, cardboard boxes, a stolen laptop.

**Asset Budget (solo dev reality):**
- 1 goblin base model + modular parts (biggest time investment)
- ~30 unique props (desks, chairs, machines — see physics objects table)
- 1 environment base with 4 era-specific texture/prop swaps
- UI assets (GoblinTwitter, Exchange, Token Creator interfaces)
- No unique character animations beyond: idle, walk, run, sit, throw, ragdoll, type. Source 2 provides ragdoll and procedural animation systems.

### UI Design

**GoblinTwitter Interface:**
Styled like early 2010s Twitter but with goblin branding. Green accent color. Posts appear as scrollable feed on the in-game laptop. Notifications pop up as floating text above the goblin's head.

**Exchange Interface:**
Minimalist trading view. Candlestick charts (simplified), big BUY/SELL buttons, token list with prices. Looks like a parody of Robinhood.

**HUD:**
Minimal. Top-left: cash balance + reputation. Top-right: round timer + current era. Bottom-center: quick action bar (GoblinTwitter hotkey, Exchange hotkey, Emote wheel). No minimap — the WeWork is small enough to navigate by sight.

---

## 12. TECHNICAL ARCHITECTURE — S&BOX (C#)

### System Overview

```
GOBLIN_CHAIN/
├── Code/
│   ├── Core/
│   │   ├── GameManager.cs          // Round state, era progression, event scheduling
│   │   ├── LobbyManager.cs         // Matchmaking, player slots, bot backfill
│   │   └── SessionManager.cs       // 2-hour session tracking, persistence
│   ├── Economy/
│   │   ├── MarketSimulator.cs      // Price engine, order book, market moods
│   │   ├── TokenFactory.cs         // Token creation, UGC validation
│   │   ├── TradeExecutor.cs        // Buy/sell/short/wash trade logic
│   │   └── WalletManager.cs        // Per-player currency tracking
│   ├── Social/
│   │   ├── GoblinTwitter.cs        // Post creation, virality calc, feed rendering
│   │   ├── FollowerSystem.cs       // Follower counts, milestones, unlocks
│   │   ├── SocialDeduction.cs      // Rug Puller role assignment, evidence, voting
│   │   └── ProximityChat.cs        // Vivox spatial audio wrapper
│   ├── World/
│   │   ├── WeWorkManager.cs        // Environment state, era transitions, decay
│   │   ├── PhysicsTracker.cs       // Chain reaction detection, Domino scoring
│   │   ├── SECManager.cs           // Raid events, hearing minigame, bribery
│   │   ├── NPCInvestor.cs          // Investor spawning, pitch evaluation
│   │   └── EventScheduler.cs       // Chaos Escalator event queue
│   ├── Player/
│   │   ├── GoblinController.cs     // Movement, interaction, ragdoll
│   │   ├── GoblinInventory.cs      // Item pickup, carry, throw
│   │   └── GoblinCustomization.cs  // Cosmetics, skin, accessories
│   ├── Recording/
│   │   ├── ClipRecorder.cs         // Frame buffer, trigger detection, export
│   │   ├── ClipEditor.cs           // Auto-zoom, overlay, subtitle generation
│   │   └── HighlightReel.cs        // End-of-round compilation, voting
│   └── UI/
│       ├── TwitterUI.cs            // GoblinTwitter in-game interface
│       ├── ExchangeUI.cs           // Trading interface
│       ├── TokenCreatorUI.cs       // UGC token creation screen
│       ├── LeaderboardUI.cs        // End-of-round scoring
│       └── HUD.cs                  // Persistent overlay elements
├── Assets/
│   ├── Models/                     // Goblin, props, environment
│   ├── Materials/                  // Textures, shaders
│   ├── Sounds/                     // SFX, ambient, UI
│   └── UI/                         // Interface textures, fonts
└── Config/
    ├── economy_balance.json        // All economy tuning values
    ├── event_schedule.json         // Chaos Escalator timing/weights
    └── token_templates.json        // Preset token configurations
```

### Key Technical Decisions

**Server Authority:**
All economy state (token prices, player balances, trades) is server-authoritative. No client can modify prices locally. This prevents cheating in the one system where cheating would break the game.

**Physics Networking:**
Use S&box's built-in networked physics. Objects are server-authoritative for collision detection but client-predicted for visual smoothness. The Domino Effect scoring runs server-side (validates chain reactions before awarding points).

**GoblinTwitter as Data, Not UI:**
GoblinTwitter posts are structured data (author, content, timestamp, virality score), not rendered HTML. The UI is a C# panel that reads from a synced post list. This means the server can process virality calculations without rendering anything, and different clients can display the feed with client-side filtering.

**Clip Recorder Implementation:**
S&box doesn't have built-in screen recording. Two approaches, in priority order:
1. **Primary (MVP):** Use Source 2's demo recording system. Auto-record the game, bookmark clip moments, extract clips post-round from the demo file using Source 2's playback tools.
2. **Fallback:** Capture render target frames to a ring buffer in memory. On trigger, write buffer to disk as image sequence, then encode to MP4 using FFmpeg (bundled). This is more complex but gives more control over overlays and auto-editing.

**Performance Budget (per frame):**
| System | Target | Notes |
|--------|--------|-------|
| Physics (Rubikon) | 4ms | S&box handles this; ~50 active objects max |
| Economy tick | 0.5ms | Runs at 4Hz (every 250ms), not every frame |
| GoblinTwitter feed update | 0.2ms | UI refresh at 2Hz |
| Clip buffer write | 0.3ms | Background thread, ring buffer |
| Network sync | 1ms | S&box networking layer |
| **Total server overhead** | **~6ms** | Leaves headroom for 60fps on modest hardware |

---

## 13. MONETIZATION STRATEGY

### Revenue Stack (Ordered by Implementation Priority)

**1. S&box Play Fund (Day 1 — Zero Code Required)**
S&box's Play Fund distributes revenue to gamemode creators based on player hours. This is passive income from the moment the game launches. The only requirement is that people play. No monetization code, no shop UI, no payment processing. This is the MVP monetization strategy and it funds everything else.

**2. Cosmetic Shop (Month 1 Post-Launch)**
Chaos Tokens (earned via gameplay) buy cosmetic items. No real-money purchases at launch — this builds goodwill and avoids the "crypto game selling crypto" irony. Cosmetics are visual-only: goblin outfits, desk decorations, GoblinTwitter profile themes, custom rug pull animations.

**3. Season Pass — $9.99 (Season 1, ~Month 2)**
Seasonal content tied to real crypto events (see Section 13 Seasons). Each season pass includes: 1 new era variant, 5 cosmetic sets, 2 new event types, 1 new meme template pack. Free track exists with reduced rewards (70/30 split — enough free content to not feel punished).

**4. UGC Marketplace (Month 3+)**
Player-created content sold through S&box's workshop system. Custom goblin skins, WeWork decoration packs, meme template packs. Creators get a revenue share. This turns the community into a content pipeline.

**5. Steam Standalone (6+ Months Post-Launch)**
If S&box traction proves the concept, release as a standalone Steam title ($14.99–$19.99). This unlocks the 95% of Steam users who don't own S&box. Use the S&box version as the proven, refined prototype.

### Anti-Pay-to-Win Guarantee
No purchasable item affects gameplay. No XP boosts, no economy advantages, no exclusive mechanics behind paywalls. The entire game balance is identical for paying and non-paying players. This is both an ethical choice and a strategic one: the community will crucify a crypto satire game that uses predatory monetization.

---

## 14. SEASONS — TIED TO REAL CRYPTO EVENTS

### Season Structure

Each season lasts 6–8 weeks and is themed around a real crypto phenomenon (satirized, not endorsed).

**Season 0: "The ICO Era" (Launch)**
Theme: 2017 ICO boom. New meme templates reference "utility tokens" and "blockchain solutions." Special NPC: The Whitepaper Writer (generates absurd whitepapers for your token). Event: "ICO Frenzy" — all tokens get a one-time 50% price boost when "launched."

**Season 1: "NFT Mania" (Month 2)**
Theme: 2021 NFT boom. New mechanic: Goblins can mint "GoblinNFTs" (pixel art created in the token creator) and sell them to NPC collectors. New event: "Right-Click Save" — an NPC steals your NFT, crashing its value. Special environment: The WeWork gets an "Art Gallery" annex.

**Season 2: "DeFi Summer"**
Theme: Yield farming, liquidity pools, rug pulls within rug pulls. New mechanic: "Liquidity Pools" — pool your token with a rival's for shared risk/reward. New event: "Flash Loan Attack" — steal liquidity for one trade, then return it (or don't).

**Season 3: "Meme Coin Madness"**
Theme: Doge, Shiba, and the infinite animal coins. New mechanic: "Animal Token Generator" — random animal + random noun = new token ($CATBURGER, $DOGMOON). Community vote picks the season's official meme coin. New event: "Elon Tweeted" — a random token pumps 1000% for 60 seconds.

---

## 15. COMMUNITY BUILDING STRATEGY

### Pre-Launch (Now through April 28)

**Discord Server (Week 1 — IMMEDIATELY):**
48% of indie sleeper hits cited Discord as their number one success factor. This is not optional.

**Discord Structure:**
- `#announcements` — dev updates only
- `#devlog` — daily/weekly progress with screenshots and GIFs
- `#meme-coin-ideas` — community submits token names and taglines for in-game inclusion
- `#goblin-art` — fan art channel (boosts investment)
- `#playtest-signups` — early access builds
- `#feedback` — structured feedback after playtests
- `#clips` — players share recorded clips (post-launch)

**Content Calendar:**
| Week | Content | Platform |
|------|---------|----------|
| 1 | "What if you were a goblin running a crypto startup?" concept post | Reddit, Twitter |
| 2 | First gameplay GIF (goblin creating a token) | Twitter, Discord |
| 3 | "Name a meme coin" community vote | Discord, Reddit |
| 4 | Physics comedy clip (chair jousting) | Twitter, TikTok |
| 5 | Closed alpha signup announcement | Discord |
| 6 | Alpha gameplay clip (rug pull moment) | All platforms |
| 7 | "Meet the Goblins" character reveal | Twitter, Discord |
| 8 | Open playtest weekend announcement | All platforms |
| 9 | Playtest clips + community highlights | All platforms |
| 10 | Launch trailer + date confirmation | All platforms, Steam |

**Reddit Strategy:**
- Post in r/indiegaming, r/gamedev (devlog), r/cryptocurrency (the satire angle), r/gaming
- The crypto satire angle gives natural crossover appeal to non-gaming crypto communities
- Never shill — let the concept sell itself

### Post-Launch

**Streamer Seeding (see Section 18):** Send keys to 20–50 mid-tier streamers (1K–50K followers) who play Among Us, Content Warning, Lethal Company, or Schedule I. These communities have the exact right audience.

**Community Events:**
- Weekly "Goblin Olympics" — community tournament with themed rounds
- Monthly "Rug Pull Championship" — biggest rug pull wins a Discord role
- Season launch events with developer participation

---

## 16. DISCORD RICH PRESENCE — DAY 1

### Integration Points

Discord Rich Presence shows what players are doing in real-time, turning every player into a passive recruiter.

**Display States:**
- "Creating $DOGMOON in Goblin Chain" (token creation)
- "Shilling $DOGMOON — Price: $4.20 (+690%)" (active gameplay)
- "SEC RAID IN PROGRESS" (event)
- "Executing rug pull..." (climax)
- "Goblin of the Round! $42,069 profit" (end screen)

**Join Button:**
Discord Rich Presence supports a "Join Game" button. Friends see what you're playing and can join your lobby directly from Discord. This is free matchmaking infrastructure.

**Technical:**
S&box supports Discord Rich Presence natively. Implementation is a configuration file + a few API calls to update state on game events. Estimate: 2–4 hours of work.

---

## 17. DEV ROADMAP — 10 WEEKS TO MVP

### Week 1: Foundation
- S&box project setup, folder structure, build pipeline
- Goblin controller: movement, physics interaction, ragdoll
- Basic WeWork environment (one room, placeholder assets)
- Multiplayer lobby: join/leave, player sync
- **Deliverable:** 4 goblins walking around a room, pushing each other

### Week 2: Economy Core
- MarketSimulator: price engine with buy/sell pressure
- TokenFactory: create tokens with name/icon/supply
- WalletManager: per-player cash tracking
- Basic Exchange UI: buy/sell buttons, price display
- **Deliverable:** Players can create tokens and trade them, prices move

### Week 3: GoblinTwitter
- Post creation (text + meme template selection)
- Virality calculation engine
- Follower system with milestones
- GoblinTwitter UI on in-game laptop
- FUD posts and reply/ratio mechanic
- **Deliverable:** Full GoblinTwitter loop — post, gain followers, affect token prices

### Week 4: Physics Comedy
- Interactive objects: chairs, desks, coffee machine, server rack
- Domino Effect chain detection and scoring
- Object throwing and collision response tuning
- Environmental destruction (breakable props)
- **Deliverable:** Physics playground is fun on its own

### Week 5: Events & NPCs
- SEC raid event (patrol, investigation, hearing minigame)
- NPC investors (spawn, walk to desks, pitch interaction)
- Market Mood system (bull/bear/crab rotation)
- Whale Alert event
- Chaos Escalator event scheduler
- **Deliverable:** Rounds have rising tension and external pressure

### Week 6: Social Deduction
- Rug Puller role assignment
- Evidence system (transaction logs, blockchain anomalies)
- Goblin Council vote mechanic
- Rug Pull / Pivot execution and consequences
- **Deliverable:** The social layer works — paranoia, accusations, betrayal

### Week 7: Proximity Chat + Clip Recorder
- Vivox spatial audio integration
- Range tuning and zone-based overrides
- Clip recorder: frame buffer, trigger events, basic export
- End-of-round highlight reel
- **Deliverable:** Voice chat works spatially, cool moments are captured

### Week 8: Progression & Polish
- Era system (4 eras, environment transitions)
- Token Creator full UGC flow (pixel art editor)
- Scoring and leaderboard system
- Round flow polish (transitions, countdowns, results screen)
- **Deliverable:** Complete game loop from lobby to final scoring

### Week 9: Content & Tuning
- Economy balance pass (playtest and adjust all values in economy_balance.json)
- 30 physics objects modeled and integrated
- All 4 era visual variants dressed
- GoblinTwitter meme template library (20+ templates)
- Bot AI for backfill players
- Discord Rich Presence integration
- **Deliverable:** Content-complete, numbers feel right

### Week 10: Launch Prep
- Performance optimization pass
- Network stress test (8 players, all systems running)
- Bug fix sprint
- Steam/S&box store page (description, screenshots, trailer)
- Launch trailer (captured from gameplay using clip recorder)
- Discord community prep (channels, roles, welcome message)
- **Deliverable:** Ship it

---

## 18. DAY 1 FEATURES vs POST-LAUNCH

### Day 1 (April 28, 2026) — MUST SHIP

| Feature | Status |
|---------|--------|
| 4–8 player lobbies with matchmaking | Required |
| Token Creator (name, icon, tagline, supply) | Required |
| GoblinTwitter (posts, followers, virality) | Required |
| Market Simulator (buy/sell, price movement) | Required |
| Physics comedy (10+ interactive objects) | Required |
| Proximity chat (Vivox) | Required |
| SEC raids (basic) | Required |
| Rug Pull / Pivot mechanic | Required |
| Social Deduction (Rug Puller role, voting) | Required |
| Clip recorder (manual trigger + 3 auto triggers) | Required |
| 2 Era variants (Era 1 + Era 3) | Required |
| Bot backfill for small lobbies | Required |
| Discord Rich Presence | Required |
| Basic goblin customization (5 options per slot) | Required |

### Post-Launch Roadmap

**Month 1:**
- Era 2 and Era 4 environments
- Cosmetic shop (Chaos Token purchases)
- 10 additional clip auto-triggers
- Voice effects for proximity chat
- Community-submitted token names added

**Month 2:**
- Season 1: "NFT Mania" content
- $9.99 Season Pass
- Streamer Mode toggle (ticker overlay)
- Advanced clip editor (trim, caption, export formats)

**Month 3:**
- UGC Marketplace (community cosmetics)
- Season 2: "DeFi Summer"
- Custom game modifiers (lobby settings: no SEC, 10x volatility, etc.)
- Spectator mode

**Month 6:**
- Evaluate Steam standalone viability
- Tournament/competitive mode
- Mobile companion app (check token prices, post on GoblinTwitter from phone)

---

## 19. SCOPE CUT PRIORITIES

If behind schedule, cut in this order (bottom = cut first):

**NEVER CUT (game doesn't work without these):**
1. Token creation + trading
2. GoblinTwitter shilling
3. Multiplayer (4 players minimum)
4. Rug Pull mechanic
5. Basic physics interactions

**CUT RELUCTANTLY (significant quality loss):**
6. Social Deduction / Rug Puller role → Replace with: all players can rug, no secret role
7. Proximity chat → Replace with: text chat only (kills virality but game functions)
8. Clip recorder → Replace with: rely on OBS/Steam screenshots (kills viral loop but game functions)
9. SEC raids → Replace with: random market crashes (less funny but simpler)

**CUT IF NEEDED (polish items):**
10. Era progression (ship with one environment, reskin later)
11. Bot backfill (require 4 humans, no bots)
12. NPC investors (simplify to random buy events)
13. Discord Rich Presence (add post-launch)
14. Domino Effect scoring (physics still works, just no score tracking)

**CUT FREELY (nice-to-have):**
15. Meme post templates (text-only shills work fine)
16. Short selling mechanic
17. Wash trading mechanic
18. Fire system
19. Voice effects on proximity chat
20. Pixel art token icon editor (preset icons only)

---

## 20. STREAMER & VIRAL STRATEGY

### Why This Game Is Built for Streaming

Every mechanic generates a "moment":
- Rug pulls are betrayal moments (Among Us energy)
- Physics chaos is slapstick comedy (Human Fall Flat energy)
- Social deduction creates accusation drama (Mafia/Werewolf energy)
- Custom tokens create inside jokes (community-specific memes)
- Proximity chat creates overheard conversations (Lethal Company energy)
- The clip recorder packages these moments for sharing automatically

### Streamer Seeding Plan

**Target Streamers (20–50 keys):**
- **Tier 1 targets** (1K–10K viewers): Among Us regulars, Content Warning players, Schedule I streamers, Lethal Company community. These audiences overlap almost perfectly with our target demographic.
- **Tier 2 targets** (10K–50K): Variety streamers who cover indie multiplayer, crypto/finance commentary YouTubers who would appreciate the satire.
- **Avoid:** Mega-streamers (100K+) at launch. They won't cover an unknown S&box gamemode. Build from the middle.

**Key Distribution:**
- 2 weeks pre-launch: send keys to Tier 1 with a short pitch ("It's Among Us but you're goblins running a crypto scam")
- Launch day: send keys to Tier 2 with a clip compilation from Tier 1 streams as proof of entertainment value
- Post-launch: any streamer who asks gets a key, no questions asked

### Content-Generating Features (Viral Loop)

| Feature | Content Type | Platform |
|---------|-------------|----------|
| Custom token names | Screenshots, memes | Twitter, Discord, Reddit |
| Rug pull moments | Video clips (15–30s) | TikTok, YouTube Shorts, Twitter |
| Physics chaos | Video clips (10–20s) | TikTok, YouTube Shorts |
| Social deduction drama | Stream highlights (1–3 min) | YouTube, Twitch clips |
| Proximity chat conversations | Audio/video clips | TikTok, Twitter |
| GoblinTwitter posts | Screenshots | Twitter (meta-humor of sharing fake tweets on real Twitter) |
| Token price charts | Screenshots (fake charts) | Crypto Twitter, Reddit |
| End-of-round leaderboard | Screenshots | Discord, Reddit |

### The Meta-Humor Marketing Angle

This game's marketing IS the game's content. When players share "$GOONCOIN up 4000%!!!" on real Twitter, some people won't realize it's a game. This confusion IS the marketing. The line between the game's satire and real crypto culture is intentionally blurred. Every shared screenshot trains potential players to recognize the game.

---

## 21. RETENTION BENCHMARKS & TARGETS

### Industry Benchmarks (F2P/Indie Multiplayer)

| Metric | Median | Top 25% | Our Target |
|--------|--------|---------|------------|
| D1 Retention | 22.9% | 40% | 35% |
| D7 Retention | 8.7% | 15% | 12% |
| D30 Retention | 3.2% | 6% | 5% |
| Avg Session Length | 25 min | 45 min | 40 min (2 rounds) |
| Sessions/Week | 2.1 | 4.0 | 3.0 |

### Retention Levers

**Short-term (D1–D7):**
- 5-minute core loop means instant gratification
- Token Creator gives ownership ("MY coin, MY name")
- Social deduction creates "one more round" pull
- Clip sharing brings friends in (organic re-engagement)

**Medium-term (D7–D30):**
- Reputation system creates persistent progression
- GoblinTwitter follower milestones are weekly goals
- Era progression rewards repeated play with new environments
- Community events (weekly tournaments) create calendar anchors

**Long-term (D30+):**
- Seasonal content refreshes (new mechanics every 6–8 weeks)
- UGC marketplace (create and sell, not just consume)
- Competitive ladder / ranked lobbies
- Community-driven meta (dominant strategies shift with seasons)

---

## APPENDIX A: KEY REFERENCE GAMES & LESSONS

| Game | Lesson for Goblin Chain |
|------|------------------------|
| Content Warning | Clip recorder = game IS content. Hit 200K concurrent in 24 hours. |
| Among Us | Social deduction in simple settings. 4–10 players is the sweet spot. |
| Schedule I | Taboo theme + co-op chaos = 414K concurrent. Crypto scams have equal taboo appeal. |
| Human Fall Flat | Physics comedy + multiplayer = 55M copies. Multiplayer was the inflection point. |
| Lethal Company | Proximity chat creates emergent comedy. Solo dev. Breakout hit. |
| Game Dev Tycoon | Tycoon framing makes complex systems accessible and satisfying. |
| Overcooked | Chaos from simple systems interacting. 2–4 player lobbies work. |

## APPENDIX B: CRITICAL PATH DEPENDENCIES

```
Token Creator ──→ Exchange ──→ Market Simulator ──→ GoblinTwitter (price affects virality)
     │                              │
     ▼                              ▼
Token Card Export              Rug Pull Mechanic ──→ Social Deduction Layer
                                    │
                                    ▼
                              Clip Recorder (rug pull is top trigger)

Physics System ──→ Domino Effect ──→ Clip Recorder (chain reactions are triggers)
     │
     ▼
WeWork Environment ──→ Era Progression (visual state)

Proximity Chat ──→ Investor Pitches (voice = pitch mechanic)
     │
     ▼
Social Deduction (overhearing = evidence gathering)
```

Build order follows the dependency graph: Token system first, then market, then social layers, then polish systems (clips, progression, eras).

---

**END OF DOCUMENT**

*GOBLIN CHAIN: Crypto Chaos Tycoon — GDD v3.0 FINAL*
*Last updated: March 31, 2026*
*Target: April 28, 2026 launch on S&box*
