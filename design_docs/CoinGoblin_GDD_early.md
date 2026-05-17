# COINGOBLIN: Token Factory Tycoon

### Game Design Document v1.0

**Platform:** S&box (Source 2 Engine)
**Genre:** Satirical Comedy Simulator / Physics Sandbox
**Players:** 1–16 (Multiplayer)
**Dev Language:** C#
**Target:** Streamers, crypto-aware gamers, comedy game fans
**Tone:** Absurdist satire — think *Untitled Goose Game* meets *Wolf of Wall Street* meets a dumpster fire

---

## 1. ELEVATOR PITCH

You run a shady crypto startup out of a co-working space that's literally falling apart. Your job: launch tokens, hype them on social media, dodge regulators, attend increasingly unhinged conferences, and cash out before everything collapses — while the Source 2 physics engine ensures that everything around you is physically, literally falling apart too.

**The hook:** You're not a genius developer. You're a goblin. A literal, green, suit-wearing goblin with a MacBook, sitting in a WeWork that's structurally unsound. The satire isn't subtle. It doesn't need to be.

---

## 2. WHY THIS WORKS

### 2.1 Comedy Game Landscape (Research Findings)

2024 was a banner year for comedy games. The hits share common DNA:

- **Thank Goodness You're Here!** — rapid-fire slapstick, absurd character design, quickfire dialogue. Won PC Gamer's Best Comedy 2024.
- **Untitled Goose Game** — simple premise (be annoying), emergent chaos, infinite meme potential.
- **Job Simulator / Surgeon Simulator** — physical comedy through intentionally clunky controls + mundane tasks made absurd.
- **Papers Please** — bureaucratic tension as comedy. Repetitive tasks with escalating moral stakes.
- **TABS (Totally Accurate Battle Simulator)** — the name IS the joke. Presents itself seriously, plays ridiculously.

**Key insight:** The best comedy games don't tell jokes — they create systems where jokes emerge naturally. Procedural absurdity > scripted punchlines. Players create the funny moments, which is exactly what streamers need.

### 2.2 Crypto Satire Targets (Research Findings)

The crypto industry is a comedy writer's dream. Real events that actually happened:

- A 12-year-old livestreamed a rug pull on Pump.fun, flipped off the camera after pocketing $30K, and the community revenge-pumped his dead coin to $84M market cap out of spite.
- The "Hawk Tuah Girl" launched $HAWK to a $490M market cap. It crashed 90%+ within hours. 3–4% of supply was available to the public.
- Argentina's president tweeted about $LIBRA, pumped it to $4.5B, then the creator pulled $107M in liquidity.
- A Chainplay report found 55% of all meme coins analyzed were classified as "malicious."
- A billionaire fund manager publicly lamented at a Manhattan conference that he didn't own "dogwifhat."

You cannot make this stuff up. But you CAN make a game about it.

### 2.3 S&box Opportunity (Research Findings)

S&box launches April 28, 2026 — less than a month away. Key advantages:

- **Source 2 physics (Rubikon engine)** — physical comedy built into the DNA.
- **C# scripting with hot-reload** — fast iteration, modern dev experience.
- **Multiplayer built-in** — co-op and competitive out of the box.
- **Standalone game licensing** — Facepunch signed a deal with Valve allowing standalone Steam releases, royalty-free. This means CoinGoblin could ship as its own product.
- **First-mover advantage** — the platform is fresh. Early hits will dominate the ecosystem.
- **Cloud assets** — drag-and-drop asset pipeline for rapid prototyping.

---

## 3. CORE GAME LOOP

```
LAUNCH TOKEN → HYPE IT → DODGE REGULATORS → CASH OUT → UPGRADE → REPEAT
    |              |              |                |           |
 (physics      (social        (stealth/        (timing     (cosmetics,
  minigame)    media sim)     chase scene)     mechanic)   office upgrades)
```

### Phase 1: THE LAUNCH (Token Factory)

You physically build your token at a workbench. This is a crafting/physics minigame:

- **Pick a name** from a slot machine of absurd word fragments: "MOON" + "DOGE" + "ELON" + "SAFE" + "ROCKET" + "INU" + "PEPE" + "TRUMP" + "AI" + "QUANTUM"
- **Design the logo** by literally throwing paint at a canvas (physics-based — splatter goes everywhere)
- **Write the whitepaper** by mashing keys on a typewriter that's on fire. The faster you type, the more buzzwords appear, but the typewriter physically shakes and letters fly off
- **Set the tokenomics** with physical slider machines — but the sliders are greased, wobbly, and hard to control. "Team allocation" slider keeps sliding back to 95%. "Public sale" slider is comically tiny

**Comedy beat:** No matter what you do, the whitepaper always comes out as gibberish buzzword soup. You can see it printed: "Leveraging quantum-resistant AI blockchain synergies to democratize DeFi yield optimization through cross-chain interoperability consensus mechanisms."

### Phase 2: THE HYPE (Social Media War Room)

Your office has a wall of screens showing a fake Twitter/X feed. You must:

- **Post tweets** by physically typing on keyboards around the office. Each keyboard controls a different sock puppet account
- **Manage influencers** — NPCs wander in and demand payment. Literally throw bags of money at them. If you hit them in the face, they promote you MORE
- **Fake partnerships** — drag and drop corporate logos onto your website (a physical corkboard). The logos are comically oversized and keep falling off
- **Chart manipulation** — a giant physical chart on the wall. You can literally push the line up with your hands, but gravity keeps pulling it down
- **Community management** — a Discord chat scrolls on a monitor. Angry messages physically print out of a receipt printer. The pile grows. You can set the pile on fire

**Hype Meter:** A physical mercury thermometer on the wall. Too cold = nobody cares. Too hot = SEC notices. You need to keep it in the "sweet spot" (a hilariously narrow band).

### Phase 3: THE REGULATORS (Escape & Evasion)

When the Hype Meter gets too high, SEC agents (men in black suits and sunglasses, moving in synchronized formation) approach your building. You must:

- **Shred documents** — feed papers into a shredder, but papers keep jamming. Physics-based paper-feeding chaos
- **Hide evidence** — physically shove monitors, hard drives, and printed charts into increasingly absurd hiding spots (ceiling tiles, potted plants, inside a pizza box)
- **Disguise your office** — flip a switch and your "Crypto Startup" sign rotates to reveal "Totally Normal Accounting Firm." The office furniture physically rearranges (badly)
- **The Chase** — if they breach the office, a full Source 2 physics chase through a collapsing co-working space. Knock over water coolers, ride office chairs down hallways, throw printers at agents

**Comedy beat:** The SEC agents are always slightly incompetent. They trip over furniture. They stop to read your whitepaper and get confused. One of them is clearly also trading crypto on his phone.

### Phase 4: THE CASHOUT (Timing Minigame)

The big moment. A giant "SELL" button sits on your desk. But:

- The button is covered in a glass case that requires solving a physical puzzle to open (smash glass, pick lock, find key, etc.)
- Your "community" (a crowd of NPC investors visible through the window) watches your every move. If they see you reaching for the button, panic selling begins
- You must time the sell perfectly — a giant price ticker dominates the wall. Sell too early = small profit. Sell too late = it already crashed. Sell at the peak = legendary status
- **The Rug Pull animation:** When you finally sell, the floor literally opens up beneath the NPC investors (they're standing on a rug). The rug gets pulled. They fall into a pit. It's slapstick, not cruel — they bounce on trampolines at the bottom and land in a ball pit

### Phase 5: THE UPGRADE (Spend Your Gains)

Between rounds, spend your ill-gotten crypto on:

- **Office upgrades:** Start in a closet, end in a penthouse. Each upgrade is more absurdly lavish
- **Lambo collection:** A garage fills with increasingly ridiculous vehicles. Eventually you're driving a gold-plated tank
- **Conference tickets:** Unlock access to bigger, wilder industry events (see Events section)
- **Cosmetics:** Goblin suits, watches, chains. A "humility" stat that goes down as your outfit gets more expensive
- **Better scam tools:** Upgraded typewriters, faster shredders, more sock puppet keyboards

---

## 4. MULTIPLAYER MODES

### 4.1 Co-op: "The Startup"

2–4 players run one crypto startup together. Division of labor:

- **The Dev** — handles token creation and "tech"
- **The Hype Man** — manages social media and influencers
- **The Finance Goblin** — controls the sell button and manages money
- **The Fixer** — handles SEC evasion and document shredding

The comedy comes from miscommunication. The Hype Man is pumping while the Finance Goblin is trying to sell. The Dev accidentally launches a second token. The Fixer is hiding evidence that the Hype Man is actively creating.

### 4.2 Competitive: "Token Wars"

4–8 players each run competing crypto startups in the same co-working building. Sabotage mechanics:

- **Steal each other's influencers** by offering bigger money bags (literal throwing mechanics)
- **Report competitors to the SEC** anonymously — but the SEC might come to YOUR office instead
- **FUD campaigns** — print fake news and slide it under competitor doors
- **Hack the chart** — sneak into another player's office and push their price line down

### 4.3 Asymmetric: "The Investigation"

1 player is an SEC agent. 4–8 players are crypto goblins. Papers Please meets Among Us:

- The SEC agent reviews token applications at a desk. Each application is a physical folder with documents
- They must spot red flags: team allocation over 90%, whitepapers that are just Lorem Ipsum, roadmaps that end at "Step 3: Profit"
- Approved tokens enter the market. Rejected ones go to "crypto jail" (a playpen with a sad trombone sound)
- The crypto goblins can disguise their applications, bribe the agent (slide money under the desk), or distract them

---

## 5. EVENTS & SETPIECES

### 5.1 The Conference

A recurring event where all players gather in a convention center. Minigames:

- **The Panel:** Sit on stage and answer questions by pressing buttons that play random buzzword audio clips. The audience reaction meter goes up for confidence, down for making sense
- **The Afterparty:** Physics-based party scene. Champagne physics. Throw business cards at people. A DJ plays increasingly distorted EDM
- **The Pitch Competition:** Shark Tank parody. Present your token to a panel of NPCs using a physical presentation board. You can draw on it, but the markers don't work well and your hands are shaky
- **Networking:** Physically hand business cards to NPCs. They judge you based on card thickness, font choice, and whether it has "CEO / Visionary / Thought Leader" on it

### 5.2 The Bull Run

Periodically, a bull (literal, physical bull) runs through the office building. During this event:

- All token prices skyrocket. The chart goes vertical
- The office shakes. Things fall off desks
- NPCs outside start throwing money through your windows
- If you can ride the bull, your token gets a massive bonus. The bull follows Source 2 physics. It is very hard to ride

### 5.3 The Crash

The opposite. A bear (literal bear) appears. During this event:

- All prices plummet. Your chart catches fire
- Your office starts physically collapsing — ceiling tiles fall, water pipes burst
- NPCs gather outside with torches and pitchforks
- You must survive by barricading your office. Board up windows. Push furniture against doors
- "WAGMI" graffiti on the walls starts peeling off and rearranging to "NGMI"

### 5.4 The Congressional Hearing

You're called to testify. Minigame:

- Sit at a desk facing a row of angry NPC senators
- They ask questions. You have three response buttons: "I don't recall," "I plead the fifth," and "We're building the future"
- A TRUTH METER slowly rises. If it fills, you go to crypto jail
- You can distract senators by physically throwing objects from your desk (water glass, microphone, your own whitepaper)
- One senator keeps accidentally unmuting their phone, which plays crypto podcast ads

---

## 6. PHYSICAL COMEDY SYSTEMS (SOURCE 2 LEVERAGE)

### 6.1 Ragdoll Everything

- Your goblin character has intentionally wonky physics. Slightly too much momentum on turns. Head bobble when walking fast
- All furniture is physics-enabled and destructible. Accidentally knock over your monitor while typing? Classic
- Document physics: papers flutter, scatter, and pile up. The office gets messier over time. The mess level is a gameplay stat that affects SEC suspicion

### 6.2 The Domino Effect System

Chain-reaction physics events:

- Knock over one Red Bull can → it hits a monitor → monitor falls on keyboard → keyboard types a tweet → tweet goes viral → SEC shows up
- These cascading failure chains are procedurally generated from the current state of your physical office
- Players will discover (and try to recreate) insane chain reactions. This is the Streamer Clip Factory

### 6.3 Breakable Office

The office degrades over time and events:

- Bull runs crack the walls
- Bear attacks damage the roof
- SEC raids break doors
- Each round, you repair OR upgrade (but the building gets increasingly structurally questionable)
- Endgame offices are held together with duct tape and dreams

---

## 7. HUMOR SYSTEMS (KEEPING IT FRESH)

### 7.1 Procedural Token Names

A name generator that combines fragments to create tokens that sound horrifyingly real:

| Prefix | Root | Suffix | Modifier |
|--------|------|--------|----------|
| Safe | Moon | Inu | Classic |
| Baby | Doge | Coin | 2.0 |
| Mega | Elon | Token | Pro |
| Ultra | Pepe | Chain | Max |
| Quantum | Shib | Swap | AI |
| Dark | Floki | Fi | X |
| Turbo | Wojak | DAO | Plus |
| Chad | Ape | Verse | Reloaded |

Result examples: "SafeFlokiSwap AI", "BabyWojakVerse 2.0", "TurboElonDAO Reloaded", "QuantumShibFi Pro Max"

### 7.2 Dynamic Whitepaper Generator

Procedurally generated buzzword documents that get increasingly unhinged:

**Level 1 (early game):** "A decentralized platform for cross-chain asset management."
**Level 5:** "Leveraging quantum-resistant zero-knowledge proofs to synergize metaverse yield farming."
**Level 10:** "Harnessing the blockchain to solve world hunger through AI-powered tokenomics and community-driven lunar colonization."
**Level 20:** "We are building a sentient blockchain that will transcend the physical plane and achieve crypto-nirvana for all HODLers."

### 7.3 NPC Investor Personalities

Procedurally generated investor NPCs with satirical archetypes:

- **The Diamond Hands Guy** — refuses to sell, ever, even when the price is literally zero. Holds a sign saying "HODL" while standing in rubble
- **The "Do Your Own Research" Guy** — his "research" is watching a 45-minute YouTube video by a guy in a Lambo
- **The VC Bro** — shows up in a Patagonia vest, only talks about "thesis" and "conviction"
- **The Influencer** — will promote anything for money. Literally anything. Has a ring light permanently attached to their face
- **The Boomer** — confused but enthusiastic. Keeps calling everything "the Bitcoin"
- **The Doomer** — constantly predicts the crash. When the crash comes, they're somehow still broke
- **The "I'm In It For The Tech" Guy** — has never read a whitepaper. Portfolio is 100% meme coins
- **The Whale** — a literal whale NPC. Massive. Crashes through walls. When they sell, the screen shakes

### 7.4 Environmental Storytelling

The office tells jokes through its environment:

- A motivational poster that says "BELIEVE" but the B and E fell off, so it says "LIEVE" (lie + ve)
- A bookshelf with titles: "The Art of the Rug," "Tokenomics for Dummies," "How to Win Friends and Rugpull People," "The 4-Hour Rug Pull"
- A whiteboard with crossed-out startup ideas: "Uber for Blockchains," "AI-Powered NFT Dreams," "Decentralized Oxygen"
- Post-it notes: "TODO: Learn what blockchain actually is"
- A framed picture on the wall of the goblin shaking hands with an obvious cardboard cutout of a famous tech CEO
- Increasingly desperate motivational quotes appear on the walls as your token crashes

---

## 8. PROGRESSION & META GAME

### 8.1 The Goblin Career Ladder

```
LEVEL 1: "Crypto Curious"     — Closet office, laptop on a cardboard box
LEVEL 5: "Token Peddler"      — Small office, one desk, one monitor
LEVEL 10: "Blockchain Bro"    — WeWork-style space, ping pong table, neon signs
LEVEL 15: "Protocol Visionary" — Open floor plan, bean bags, that one guy playing guitar
LEVEL 20: "DeFi Deity"        — Penthouse office, floor-to-ceiling windows, koi pond
LEVEL 25: "Crypto Overlord"   — Literal villain lair. Volcano optional. Multiple Lambos
LEVEL 30: "Satoshi Himself"   — Your office is on the moon. The moon is also an office
```

### 8.2 Achievement System (All Comedy)

- **"It's Called a Feature"** — Ship a token with a bug in the smart contract. Nobody notices for 24 hours
- **"Due Diligence"** — Read an entire whitepaper (takes 0.3 seconds because it's all buzzwords)
- **"Community Driven"** — Get 1,000 NPC investors. Ignore all of their suggestions
- **"Sustainable Exit"** — Rug pull and immediately launch a new token using the same logo flipped horizontally
- **"Diamond Hands"** — Hold your own token through a 99% crash. Why?
- **"The Zuckerberg"** — Testify before Congress without your truth meter filling up
- **"Not Financial Advice"** — Give financial advice to 100 NPCs
- **"WAGMI"** — Have every player in a multiplayer session go bankrupt simultaneously
- **"The Vitalik"** — Launch a token that actually does something useful. (HIDDEN — extremely difficult)
- **"Full Circle"** — Get rug-pulled by one of your own investors

### 8.3 Prestige System: "The Rebrand"

When you reach max level, you can "rebrand" — reset to level 1, but now your company has a different name and a suspiciously similar logo. Each rebrand adds a modifier:

- Rebrand 1: Add "Labs" to your company name
- Rebrand 2: Add "Protocol"
- Rebrand 3: Add "Foundation"
- Rebrand 4: Just add "2.0"
- Rebrand 5+: Add random Greek letters

---

## 9. STREAMER-BAIT DESIGN

Every system is designed to generate sharable moments:

### 9.1 Clip Factories

- **Domino chain reactions** — accidental Rube Goldberg machines of office destruction
- **Bull/Bear attacks** — sudden chaos events that interrupt everything
- **Close calls** — SEC agent opens the closet you're hiding in, but gets distracted
- **Multiplayer betrayals** — one player sells while the others are still hyping
- **Token name reveals** — "Our new project is... *reads procedural name* ...TurboPepeVerse AI Reloaded"

### 9.2 "Did That Just Happen?" Moments

Rare events that create water-cooler moments:

- An NPC whale investor crashes through the ceiling
- The price ticker overflows and starts displaying in scientific notation
- Two SEC agents start arguing about whether your token is a security
- Your office catches fire and the fire department arrives, but they're also investing in your token
- The bear event and bull event trigger simultaneously (the animals fight)

### 9.3 Social Features

- **Token leaderboard** — global rankings of the most successful (and most disastrous) tokens
- **Replay system** — save and share your best rug pulls
- **Photo mode** — pose your goblin with their Lambos, charts, and burning offices
- **Token gallery** — a museum of every token you've ever launched, with their final chart (mostly vertical red lines)

---

## 10. SOUND DESIGN

Sound sells the comedy:

- **Typing sounds** that get increasingly frantic as deadlines approach
- **Sad trombone** on every price crash
- **Airhorn** on every price pump
- **"EMOTIONAL DAMAGE"** -style sound cue when you get caught by the SEC
- **MLG-style montage parody sounds** for domino chain reactions
- **Lo-fi beats** in the office that get more distorted as chaos increases
- **The "To Be Continued" Roundabout riff** when the SEC arrives
- **Elevator music** during Congressional hearings

---

## 11. VISUAL STYLE

- **Characters:** Goblins in business casual. Exaggerated heads, tiny bodies. Think *Rabbids* meets Wall Street
- **Office:** Realistic enough to read as a startup office, stylized enough to be funny. Cluttered, messy, screens everywhere
- **UI:** Designed to look like a Bloomberg Terminal had a baby with a meme page. Charts use Comic Sans. Warning popups are written in broken English
- **Token logos:** MS Paint aesthetic. Intentionally bad. The worse they look, the more "authentic" they feel
- **Color palette:** Neon greens and reds (charts), sterile office whites and grays, with splashes of gold (Lambo) and orange (SEC raid warning)

---

## 12. TECHNICAL ARCHITECTURE (S&BOX SPECIFIC)

### 12.1 C# Component Structure

```
CoinGoblin/
├── Core/
│   ├── GameManager.cs          — Round state machine, phase transitions
│   ├── TokenFactory.cs         — Procedural token generation
│   ├── MarketSimulator.cs      — Price ticker simulation, volatility
│   └── EventSystem.cs          — Bull/Bear/SEC event triggers
├── Player/
│   ├── GoblinController.cs     — Player movement, interaction
│   ├── InventorySystem.cs      — Money bags, documents, tools
│   └── GoblinCosmetics.cs      — Outfit/accessory system
├── Office/
│   ├── OfficeManager.cs        — Office state, degradation, upgrades
│   ├── FurniturePhysics.cs     — Destructible furniture, domino chains
│   ├── InteractableStation.cs  — Typewriter, shredder, sell button
│   └── MesrinessTracker.cs     — Environmental mess → SEC suspicion
├── Social/
│   ├── FakeTwitterFeed.cs      — Procedural tweet generation
│   ├── InfluencerAI.cs         — NPC influencer behavior
│   ├── InvestorCrowdAI.cs      — Crowd simulation outside window
│   └── DiscordSimulator.cs     — Fake chat feed with angry messages
├── Regulators/
│   ├── SECAgentAI.cs           — Patrol, search, chase behaviors
│   ├── SuspicionSystem.cs      — Heat meter, threshold triggers
│   └── CongressionalHearing.cs — Testimony minigame
├── Events/
│   ├── BullRun.cs              — Bull spawning, riding physics
│   ├── BearAttack.cs           — Bear spawning, office destruction
│   ├── ConferenceEvent.cs      — Convention center scenes
│   └── CrashEvent.cs           — Market crash setpiece
├── Multiplayer/
│   ├── LobbyManager.cs         — Room creation, role assignment
│   ├── SabotageSystem.cs       — Cross-player interference
│   └── TokenWarsScoring.cs     — Competitive scoring
└── UI/
    ├── BloombergTerminal.cs    — HUD styled as trading terminal
    ├── TokenNameGenerator.cs   — Name slot machine UI
    └── AchievementPopups.cs    — Comedy achievement notifications
```

### 12.2 Leveraging S&box Features

- **Rubikon Physics:** Every object in the office is a physics prop. Source 2's physics engine handles chain reactions natively. We add custom "domino trigger" components that detect collision chains
- **Multiplayer Networking:** S&box's built-in networking handles state sync. Token prices, office states, and player positions use S&box's standard replication
- **Hot Reload:** Rapid iteration on comedy timing. Tweak the bull's charge speed, the SEC agent's reaction time, the paper physics — all without restart
- **Scene System:** Each office level is a scene. Upgrade = load new scene with persistent player data
- **Cloud Assets:** Community can upload custom token logos, office decorations, goblin outfits

### 12.3 Modding Support

S&box is built for modding. We should embrace it:

- Custom token name word lists
- Custom NPC investor types
- Custom office layouts
- Custom events (community-created chaos events)
- Full Steam Workshop integration via S&box's asset system

---

## 13. MONETIZATION CONSIDERATIONS

Since S&box now allows standalone Steam releases (royalty-free through Facepunch's Valve license), two paths:

**Path A: Free S&box Game Mode**
- Launch as a free game mode within S&box
- Builds audience during S&box's launch window
- Cosmetic microtransactions for goblin outfits (optional)

**Path B: Standalone Steam Release**
- Polish into a full standalone game
- Premium price ($15–20)
- No microtransactions (the irony of a crypto satire having loot boxes would be too on-the-nose... or would it?)

**Recommended: Path A first, Path B if it takes off.** Ride the S&box launch wave, build community, then standalone if warranted.

---

## 14. LEGAL SAFETY NOTES

The game satirizes the crypto industry broadly, not specific individuals or companies:

- No real person names — use obvious archetypes instead
- No real token names — the procedural generator handles this
- No real company logos — generic parodies only ("Conbase," "Bynance," "OpenSea-Floor")
- Parody is protected speech, but we avoid anything that could be mistaken for actual financial advice
- Add a disclaimer: "This game is not financial advice. Nothing in this game is financial advice. Please do not take financial advice from a goblin."
- The game itself contains no real cryptocurrency, blockchain, or NFT technology. It is purely fictional slapstick comedy

---

## 15. DEVELOPMENT ROADMAP

### Phase 1: Prototype (4 weeks)
- Core goblin controller + office physics
- Basic token launch → hype → sell loop
- One office level, placeholder art
- Single player only

### Phase 2: Comedy Polish (4 weeks)
- Token name generator + whitepaper generator
- NPC investors + influencers
- SEC agent AI + chase mechanics
- Bull and Bear events
- Sound design pass

### Phase 3: Multiplayer (4 weeks)
- Co-op mode (The Startup)
- Competitive mode (Token Wars)
- Asymmetric mode (The Investigation)
- Lobby system + matchmaking

### Phase 4: Content & Launch (4 weeks)
- Conference events
- Congressional hearing
- Full progression system (30 levels)
- Achievement system
- Modding support
- Community playtest + streamer outreach

**Total estimated timeline: ~16 weeks to launch-ready**

---

## 16. COMPETITIVE ANALYSIS

| Game | What It Does Well | What CoinGoblin Does Differently |
|------|------------------|----------------------------------|
| Untitled Goose Game | Simple premise, emergent chaos | More systems depth, multiplayer, progression |
| Job Simulator | Physical comedy in mundane settings | Real-world satire target, competitive modes |
| Surgeon Simulator | Clunky controls = comedy | Intentional chaos vs. accidental chaos |
| Papers Please | Bureaucratic tension | Lighter tone, more slapstick, less moral weight |
| Among Us | Social deduction, betrayal | Physical world, persistent progression, less talking more doing |
| Content Warning | Streamer-bait design, clip factory | Crypto-specific humor, broader appeal through cultural relevance |

---

## 17. RISK ASSESSMENT

| Risk | Likelihood | Mitigation |
|------|-----------|------------|
| "Too niche" — non-crypto people don't get jokes | Medium | Core gameplay (physics comedy, multiplayer chaos) works without crypto knowledge. Crypto is the skin, chaos is the skeleton |
| S&box launches with bugs/low playercount | Medium | Build standalone-ready from day one. S&box is the launchpad, not the dependency |
| Crypto becomes uncool/irrelevant | Low | Crypto discourse is self-perpetuating. Every crash makes the satire more relevant, not less |
| Legal issues from crypto companies | Very Low | Pure satire with no real names. Parody is well-protected |
| Comedy doesn't land | Medium | Playtest-driven development. If a joke doesn't get laughs in playtest, cut it. Physics comedy is universal |

---

## 18. SUCCESS METRICS

**Launch Month Targets:**
- 10K+ unique players in first week (S&box game mode)
- 50+ streamer clips shared organically
- Top 10 on S&box "Most Popular" within 48 hours
- Community creates 100+ custom token name mods

**3-Month Targets:**
- 100K+ total players
- Active modding community
- Featured in at least one major gaming outlet
- Standalone Steam page live (if metrics justify)

---

## APPENDIX A: SAMPLE TOKEN NAMES (Generated)

For testing and marketing materials:

1. SafeMoonInu Classic
2. BabyDogeChain 2.0
3. MegaElonSwap Pro
4. UltraPepeDAO Max
5. QuantumShibFi AI
6. DarkFlokiVerse X
7. TurboWojakToken Plus
8. ChadApeSwap Reloaded
9. SafeFlokiDAO 2.0 AI
10. BabyMoonVerse Pro Max Reloaded

---

## APPENDIX B: SAMPLE ACHIEVEMENT LIST

| Achievement | Description | Rarity |
|------------|-------------|--------|
| First Rug | Complete your first token lifecycle | Common |
| Speed Run | Launch and rug a token in under 60 seconds | Uncommon |
| Paper Hands | Sell at a loss. Somehow | Common |
| Diamond Hands | Hold through a 99% crash | Rare |
| The Zuckerberg | Survive a Congressional hearing | Rare |
| Bull Rider | Successfully ride the bull | Epic |
| Bear Wrestler | Survive a bear attack without hiding | Epic |
| The Vitalik | Create a token that's actually useful | Legendary |
| Full Circle | Get rug pulled by your own investor | Legendary |
| WAGMI | Everyone in multiplayer goes bankrupt at once | Legendary |
| Not Financial Advice | Give financial advice 100 times | Uncommon |
| Decentralized | Play with 16 players simultaneously | Rare |
| Floor Price | Your token reaches a price of exactly $0.00 | Common |
| To The Moon | Your token reaches $1M market cap | Rare |
| The Rebrand | Prestige for the first time | Uncommon |
| Identity Crisis | Prestige 5 times | Epic |
| Touch Grass | Go outside your office (there is no outside) | Impossible |

---

*Document authored for CoinGoblin development. This is a living document — update as playtesting reveals what's funny and what isn't. Comedy is iterative. Ship, test, laugh, adjust.*
