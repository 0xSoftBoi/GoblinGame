# NOTES.md — Session Log & Decisions

Running log of what was done and *why*, so a future session can pick up the
thread without re-deriving everything. Newest session at the top.

---

## Session: 2026-07-01 — Audit-and-repair pass + contract puzzle

Previous sessions had already built the skeleton of all five brief systems
(GoblinTwitter, TokenSystem, SocialDeduction, SECSystem, MarketManipulation,
EraManager). This session ran two fresh-context audits (a brief-vs-code gap
check and a cross-file wiring audit), then fixed everything load-bearing that
they found. No compiler exists in this environment — verification was done by
static cross-checking every call site, so **the first in-editor compile may
still surface small issues.**

### New features built

1. **Contract Compiler minigame** (`ContractPuzzle.cs`, `TokenCreator.razor`)
   — the brief's item #2, which had been silently cut. Token creation is now
   3 stages: name/ticker/icon → wire 5 contract functions to their goblin
   implementations (pool of 14 pairs, matching by actually reading the code)
   → compile result (SCAM/MEME/MID/SOLID/BLUE CHIP) → launch.
   *Decision:* went with function↔implementation matching instead of a
   node-graph circuit board — same "code the contract" fantasy, feasible in
   Razor without canvas drawing, and the pairs carry the comedy
   (`audit() → return "PASSED"; // TODO: write audit`). Score is
   client-computed and sent with RequestCreateToken (cheatable, acceptable
   for a party game). Quality = 5 + score×18 + jitter; feeds volatility and
   initial price.

2. **ShillTemplates expanded 8 → 30 options per slot**, and the composer now
   deals a **hand of 5 per slot with a 🎲 REROLL button** (flat list of 30×4
   was unusable). Post text now weaves the ticker in
   (`{opener} ${TICK} {claim} — {proof}. {cta}`) — previously the ticker
   never appeared in the tweet body. Live tweet preview added to composer.

3. **Rugger is now playable** (was orphaned code):
   - `[Rpc.Owner]` misuse fixed — role notification now uses
     `Rpc.FilterInclude(c => c == target)` + `[Rpc.Broadcast]`, sets
     client-side `SocialDeduction.LocalIsRugger`, pushes a notification.
   - Secret 🎭 phone tab (Rugger's client only): GBC share vs 50% threshold,
     perk list, **GRAND RUG button** (RequestGrandRug finally has a caller).
   - **Shadow Wallet actually works**: host publishes `PublicBalances`
     (NetDictionary, 1s tick) with the Rugger's balance discounted 30%;
     HUD leaderboard renders those instead of raw balances. Raw wallet
     balances are still [Sync] (visible to a determined cheater) — fixing
     that means restructuring CryptoWallet sync; deferred.
   - **End-of-match reveal**: `RevealRugger()` sets synced `FinalRuggerReveal`
     at DetermineWinner; ResultsScreen shows "THE AUDIT IS FINAL" card.

4. **Audit votes complete the loop**: synced CurrentAccusedId/Guilty/Innocent
   tallies, panel ticks the timer from synced state, shows the verdict, and
   auto-hides after 6s (all three were previously never called).
   *Semantics decision:* vote-for-accused = guilty, anything else = innocent;
   guilty verdict on a clean goblin fines the **guilty voters** (UI already
   promised this; old code fined the accused). *Design decision:* acquittal
   reveals nothing — only a guilty verdict opens the books. Otherwise one
   cheap audit ends the deduction game.

5. **SEC raids match the brief's five options**: added **HIDE** (50/50 — walk
   free with heat still at 90, or full fine + humiliation) and **FLEE** (you
   escape the fine, your token bags get seized + 10% travel fine). Added the
   **Auditor miniboss as a character**: Auditor Grimsby Ledgerbane, with a
   synced per-raid quip on the raid panel. A physical auditor NPC entity is
   future work.
   **SEC heat is now visible** — heat bar + number in the HUD wallet block
   (was: console-log only, which killed the risk-management loop).

6. **NPCs actually read the feed** (brief: "buy the hype, panic on FUD").
   Rewrote NPCAction: per-token hype/FUD signals from the last 15 posts
   (resolving NPC posts' tickers to tokens), gullible bots chase the loudest
   token, FUD on a held bag flips them to panic-selling *that* token.
   Trending reactions now include 3 real NPC buys plus the previously-dead
   `TrendingBonusBuyPressure` config.

### Blockers fixed (from the wiring audit)

- **Input actions did not exist** — no InputSettings block in .sbproj, so T/Y/M/C/Q/V/Tab did nothing and most UI was unreachable. Added the full block (custom + standard actions — defining InputSettings replaces engine defaults). **Voice moved V → B** (V is CallAudit). `input_actions.txt` rewritten to match.
- **TextEntry one-way binding** — TokenName/Ticker/_customText were never written back, so token creation could never succeed and custom post text was always dropped. Fixed by pulling `entry.Text` in OnUpdate.
- Compile-breakers: `RemoveTokenHolding(tokenId)` missing amount arg (rug path); `TokenSystem.TokenData` → `TokenData`; `caller.SteamId` assigned to `Guid` (3 sites in MarketManipulation, now `player.Id`); `MathF.Clamp` → `Math.Clamp` (5 sites); stray `</div>` in AuditVote.razor.
- Double-bound inputs: TradePanel closed on "Trade" while PlayerInput opened the phone on it; MarketOverlay self-toggled against PlayerInput. Panels no longer listen — PlayerInput owns all toggles.
- Stale phase names (`MINING`/`TRADING`) in GoblinHud PhaseClass/PhaseHint and PhaseAnnouncement CSS → CREATE/SHILL.
- `TwitchIntegration` wrote `[Sync] IsConnected` from the IRC background thread → now signals via Interlocked flag drained in OnFixedUpdate.
- `TokenSystem.CleanupRound()` was never called → now runs between rounds. Rug pulls now phase-gated to Shill/Chaos (were possible in Pregame).
- Pivot renamed tokens to "Name V2" always → random suffix pool (" (Audited)", " Classic", " AI", " Trust Edition", …).

### Free cloud assets (added after the audit pass)

The office and rigs now use **free Facepunch models from sbox.game**, loaded
at runtime via `Cloud.Model("facepunch.x")` and listed in .sbproj
`PackageReferences`. Every ident was verified to exist against sbox.game
(fetch the package page with a crawler UA; real packages return a
"<Name> from Facepunch" title). Verified idents used:
office_desk, office_chair, tv, cardboard_box, wooden_crate, pallet,
pizza_box, traffic_cone, couch, microwave, watermelon, atm, fridge.

- `OfficeSetup.SpawnCloudOffice()`: desk grid (desk + chair + TV terminal)
  plus per-era clutter — garage era gets pallets and pizza boxes, exchange
  and penthouse eras get **ATMs as decor**. Fallback chain: editor prefabs →
  cloud assets → dev boxes (and the old dev-box fallback was actually dead:
  the `< 5` check ran after the market board + 8 posters spawned, so it
  never fired — the office shipped empty. Fixed by counting furniture only).
- Mining rigs are now canonically **a microwave with a GPU inside**
  (`NetworkedRigSpawner.DressRig`, model swapped before NetworkSpawn so the
  snapshot syncs; dev-box remains if the cloud is unreachable).
- Idents that do NOT exist (probed, don't waste time): server_rack,
  coffee_machine, whiteboard, potted_plant, keyboard, monitor,
  filing_cabinet, desk_lamp, water_cooler, computer variants.

### Known gaps / future work

- **Era environments are now cloud-furnished but not architectural** — same
  room, richer clutter per era. Walls/lighting per era still future work.
  Era numbers (volatility, raid threshold) progress as before.
- **Insider leaks** (stretch item) not built — short selling was the
  substitute.
- No physical Auditor NPC / raid chase sequence; raid is a decision UI with
  the Grimsby character layered on.
- Twitch raid votes only map to actions 0–3 (not Hide/Flee).
- `RequestAudit` (untargeted audit start) has no UI path; V-key raycast
  accusation (`RequestAuditVote`) is the only entry.
- Rugger assignment is per-match, not per-round (brief says per round; match
  = 5 rounds ≈ one social-deduction arc, feels right — revisit after
  playtest).
- BotAI doesn't create tokens or shill; NPC investors carry the economy.

### Conventions this repo actually uses (learned the hard way)

- Targeted client RPC = `using (Rpc.FilterInclude(c => c == conn)) { SomeBroadcast(); }` — `[Rpc.Owner]` on a host-owned singleton runs on the host, not the target player.
- Struct network types (`TokenData`, `PostData`) are top-level in namespace `GoblinChain`, INetworkSerializable with manual Read/Write — keep field order identical in both.
- UI panels are PanelComponents found via `Scene.GetAllComponents<UI.X>().FirstOrDefault()`; systems are singletons via `Instance` set in OnStart, always null-guarded at call sites.
- s&box UI CSS is a flexbox subset — no `float` (use flex + margin-left:auto).
- TextEntry does NOT two-way bind `Text` — pull `entry.Text` back in OnUpdate.
