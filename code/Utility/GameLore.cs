using System;

namespace GoblinChain;

/// <summary>
/// Central lore repository for all in-game flavor text.
/// Static — no component needed. Call GameLore.GetRandomTip(), etc.
/// </summary>
public static class GameLore
{
	private static readonly Random _rng = new();

	// ═══════════════════════════════════════
	//  COIN DESCRIPTIONS (for Market UI)
	// ═══════════════════════════════════════

	public static readonly CoinLore[] Coins = new[]
	{
		new CoinLore(
			"GBC", "GoblinCoin",
			"The coin for the rest of us.",
			"Founded 2047. No utility, no roadmap, no problem. Backed by collective delusion and hash power."
		),
		new CoinLore(
			"GOBETH", "Goblin Ethereum",
			"Smart contracts, dumb decisions.",
			"A fork of a fork of a fork. The codebase is a palimpsest of conflicting smart contracts."
		),
		new CoinLore(
			"RUGDAO", "Rug Pull DAO",
			"Transparency through obscurity.",
			"Rug-pulled 14 times. Relaunched 14 times. Governance by Magic 8-Ball."
		),
		new CoinLore(
			"PONZI", "PonziToken",
			"Early investors win. That's you, right?",
			"The whitepaper admits it's a Ponzi scheme. The community considers this transparency."
		),
		new CoinLore(
			"COPE", "CopeCoin",
			"Everything is fine.",
			"Price goes up when everything else goes down. The best hedge is monetized sadness."
		),
	};

	public record CoinLore( string Symbol, string FullName, string Tagline, string Description );

	// ═══════════════════════════════════════
	//  CRASH HEADLINES (expanded)
	// ═══════════════════════════════════════

	public static readonly string[] CrashHeadlines = new[]
	{
		"BREAKING: GoblinCoin founder seen buying ramen in bulk",
		"PANIC: Whale dumps entire wallet, cites 'bad vibes'",
		"ALERT: Smart contract found to contain only 'lol'",
		"CRASH: Market manipulation? No, just regular manipulation",
		"DUMP: Anonymous dev tweets 'oops' then deletes account",
		"SELL: Audit reveals blockchain is just a Google Sheet",
		"RUG: Liquidity pool drained by 'definitely_not_admin'",
		"MELTDOWN: GOBETH node catches fire, price catches cold",
		"PANIC: Fortune cookie predicts doom, goblins believe it",
		"ALERT: RUGDAO rug-pulled for the 15th time this quarter",
		"CRASH: Someone asked 'what does this coin actually DO?'",
		"DUMP: Blockchain Council intern accidentally hits sell-all",
		"SELL: PONZI community realizes name was literal",
		"RUG: Exchange hot wallet secured with password 'password123'",
		"MELTDOWN: AI trading bot achieves sentience, immediately sells everything",
	};

	// ═══════════════════════════════════════
	//  MOON HEADLINES (expanded)
	// ═══════════════════════════════════════

	public static readonly string[] MoonHeadlines = new[]
	{
		"MOON: Influencer accidentally promotes GoblinCoin instead of rival",
		"PUMP: Elon tweets goblin emoji, markets go wild",
		"SURGE: GoblinCoin accidentally listed on real exchange",
		"BOOM: AI bot stuck in buy loop, refuses to stop",
		"RALLY: 'Diamond hands' meme goes viral, price follows",
		"UP: Fortune cookie predicted GoblinCoin would moon today",
		"MOON: COPE holders sell, accidentally triggering a GBC bull run",
		"PUMP: Anonymous whale buys 10M GBC, memo reads 'yolo'",
		"SURGE: Blockchain Council accidentally endorses GBC in press release",
		"BOOM: Mining difficulty drops 80% due to 'clerical error'",
		"RALLY: RUGDAO founder returns, says 'my bad, here's the money back'",
		"UP: GoblinCoin mentioned in a fortune cookie fortune. AGAIN.",
	};

	// ═══════════════════════════════════════
	//  NORMAL HEADLINES (expanded)
	// ═══════════════════════════════════════

	public static readonly string[] NormalHeadlines = new[]
	{
		"Markets stable. For now.",
		"GoblinCoin holds steady. Suspicious.",
		"Low volume. Everyone's AFK.",
		"Sideways trading. Goblins are confused.",
		"Price discovery in progress. (It's lost.)",
		"Calm before the storm. Or just calm. Who knows.",
		"RUGDAO governance vote ends in tie. Magic 8-Ball consulted.",
		"COPE community releases statement: 'It's fine.'",
		"PonziToken releases quarterly report. It's a triangle.",
		"Someone asks what GOBETH does. No one answers.",
		"Blockchain Council denies existence. Again.",
		"Sector 7G power grid holding at 58% capacity. New record.",
		"Exchange fees normal. Intern's code behaving. Temporarily.",
	};

	// ═══════════════════════════════════════
	//  LOADING SCREEN TIPS
	// ═══════════════════════════════════════

	public static readonly string[] LoadingTips = new[]
	{
		"GoblinCoin has no intrinsic value. Neither does anything else.",
		"The best time to place a rig was 30 seconds ago. The second best time is now.",
		"If you can't spot the whale in the room, you're the liquidity.",
		"RUGDAO has been rug-pulled 14 times and counting.",
		"The Underhive's power grid runs on 40% stolen electricity and 60% optimism.",
		"COPE's price goes up when everything else goes down. This is called 'hedging your sadness.'",
		"SatoshiGoblin's last words were 'good luck lol.' We're still working on the luck part.",
		"PonziToken's whitepaper admits it's a Ponzi scheme. The community considers this transparency.",
		"In the Underhive, 'decentralized' means 'no one to call when things break.'",
		"Every EMP you throw is an EMP you can't throw later. Unless you buy more.",
		"The leaderboard updates in real time so you can watch yourself lose in high definition.",
		"GOBETH's smart contracts have bugs that accidentally became features. So does everything here.",
		"Pro tip: if someone offers you a trade during Chaos phase, they're probably lying.",
		"The Blockchain Council insists it doesn't exist. It also insists you pay your taxes.",
		"Mining doesn't require pickaxes anymore. It requires GPUs and a willingness to ignore your electricity bill.",
		"Sector 7G was supposed to be demolished. The demolition bot was bribed. We live here now.",
		"Your rig's durability is a suggestion, not a guarantee.",
		"The Exchange is technically a loading dock. The screens are bolted to the walls.",
		"When in doubt, blame the intern who wrote the fee algorithm.",
		"Hash rate is the one number that doesn't lie. Everything else in the Underhive does.",
		"The 'G' in GBC officially stands for nothing.",
		"PONZI holders greet each other with 'to the top of the pyramid.'",
		"Placing rigs near other goblins' rigs is a power move. Or a terrible idea. Sometimes both.",
		"The Furnace level mines faster but your rig might literally catch fire.",
		"Fun fact: GBC's all-time high was caused by a typo in a whale's sell order.",
	};

	// ═══════════════════════════════════════
	//  PHASE FLAVOR TEXT
	// ═══════════════════════════════════════

	public static readonly string[] PregameQuips = new[]
	{
		"Goblins are filing into Sector 7G. Someone's already arguing about tokenomics.",
		"The rigs are cold. The wallets are empty. The vibes are questionable.",
		"Another round in the Underhive. May the best scammer win.",
		"Loading capitalism simulator... please wait.",
	};

	public static readonly string[] MiningQuips = new[]
	{
		"The rigs are humming. The power grid is groaning. The Underhive is open for business.",
		"Build rigs. Mine coins. Stack hash rate. Ignore the burning smell.",
		"Every hash brings you closer to glory. Or bankruptcy. Usually bankruptcy.",
		"The GPUs are screaming. That's normal. Probably.",
	};

	public static readonly string[] TradingQuips = new[]
	{
		"The Exchange is live. Trust no one. Especially the ones who say 'trust me.'",
		"Open market. Make deals. Exploit everyone. Standard procedure.",
		"Trades are binding. Regret is free.",
		"Remember: there's a sucker in every trade. Check the mirror.",
	};

	public static readonly string[] ChaosQuips = new[]
	{
		"All bets are off. All EMPs are armed. The Blockchain Council has left the chat.",
		"Sabotage. Crash markets. Survive. In that order.",
		"The Underhive descends into anarchy. So, a normal Tuesday.",
		"Everything is on fire. COPE is mooning.",
	};

	public static readonly string[] ResultsQuips = new[]
	{
		"The ledger is final. The numbers don't lie. Everything else in the Underhive does.",
		"Another round complete. Fortunes made. Friendships destroyed.",
		"The blockchain has spoken. No refunds.",
		"Winners celebrate. Losers cope. COPE holders profit.",
	};

	// ═══════════════════════════════════════
	//  RANDOM EVENT DESCRIPTIONS (lore-rich)
	// ═══════════════════════════════════════

	public static readonly EventLore[] EventDescriptions = new[]
	{
		new EventLore( "HASH BOOST", "SOLAR FLARE",
			"Deep space radiation supercharges all rigs! Scientists say impossible. Goblins say scientists are poor.",
			"Solar flare supercharges all mining rigs! 2x hash rate for 15s!",
			"positive" ),

		new EventLore( "POWER OUTAGE", "ROLLING BLACKOUT",
			"The Underhive's power grid — 40% stolen electricity, 60% wishful thinking — hiccups again.",
			"Rolling blackout! {0} rig(s) went dark!",
			"negative" ),

		new EventLore( "GOLD RUSH", "RARE BLOCK",
			"A once-in-a-thousand block appears. Denser. Richer. The goblin with the most rigs finds it first.",
			"{0} found a rare block! +{1} GBC bonus!",
			"positive" ),

		new EventLore( "TAX AUDIT", "THE IRS REMEMBERS",
			"Nobody knows how the IRS still exists. A drone just delivered a tax notice to the richest goblin.",
			"IRS targets {0}! -{1} GBC confiscated!",
			"negative" ),

		new EventLore( "WHALE ALERT", "ANONYMOUS BUYER",
			"A massive buy order from wallet 0xDEAD...BEEF. No one knows who owns it. The price spikes.",
			"Anonymous whale just bought 10M GBC! Price pumping!",
			"positive" ),

		new EventLore( "LIQUIDITY CRISIS", "FEE ALGORITHM",
			"The intern's fee algorithm strikes again. All transactions now cost extra. The intern does not accept feedback.",
			"Exchange fees spike! -{0} GBC from every wallet!",
			"negative" ),

		new EventLore( "AIRDROP", "MYSTERY DROP",
			"An unidentified smart contract distributes free coins. Nobody knows where they came from. Both excitement and terror are correct.",
			"Mystery airdrop! Everyone gets +{0} GBC!",
			"positive" ),

		new EventLore( "SERVER FIRE", "THERMAL RUNAWAY",
			"The Furnace level lives up to its name. Cooling pipes burst. Every rig emergency-shuts.",
			"Data center fire! ALL rigs offline for 8 seconds!",
			"negative" ),

		new EventLore( "SEC RAID", "REGULATORS INBOUND",
			"The Blockchain Council's enforcement division — yes, the decentralized system has one — identifies the richest goblin.",
			"Regulators seize 25% of {0}'s wallet! Redistributed to all!",
			"negative" ),

		new EventLore( "VIRAL MOMENT", "GOING VIRAL",
			"One goblin's hastily photoshopped meme hits the social feed and goes nuclear. Being funny is a viable financial strategy.",
			"{0}'s goblin meme goes viral! +{1} GBC from fans!",
			"positive" ),
	};

	public record EventLore( string Tag, string Title, string LoreText, string GameplayText, string Tone );

	// ═══════════════════════════════════════
	//  WINNER TITLES
	// ═══════════════════════════════════════

	public static string GetWinnerTitle( float winnerBalance, float runnerUpBalance, int winnerRigs, bool survivedCrash )
	{
		float margin = winnerBalance - runnerUpBalance;

		if ( winnerRigs == 0 )
			return "THE WOLF OF GOBLIN STREET";
		if ( margin > 200 )
			return "CRYPTO OVERLORD";
		if ( survivedCrash )
			return "DIP BUYER SUPREME";
		if ( winnerRigs >= 5 )
			return "HASH KING";

		return "THE ULTIMATE GOBLIN";
	}

	// ═══════════════════════════════════════
	//  HELPER METHODS
	// ═══════════════════════════════════════

	public static string GetRandomTip()
		=> LoadingTips[_rng.Next( LoadingTips.Length )];

	public static string GetRandomCrashHeadline()
		=> CrashHeadlines[_rng.Next( CrashHeadlines.Length )];

	public static string GetRandomMoonHeadline()
		=> MoonHeadlines[_rng.Next( MoonHeadlines.Length )];

	public static string GetRandomNormalHeadline()
		=> NormalHeadlines[_rng.Next( NormalHeadlines.Length )];

	public static string GetPhaseQuip( GamePhase phase )
	{
		var pool = phase switch
		{
			GamePhase.Pregame => PregameQuips,
			GamePhase.Create => MiningQuips,
			GamePhase.Shill => TradingQuips,
			GamePhase.Chaos => ChaosQuips,
			GamePhase.Results => ResultsQuips,
			_ => PregameQuips
		};
		return pool[_rng.Next( pool.Length )];
	}

	public static CoinLore GetCoinLore( string symbol )
	{
		foreach ( var c in Coins )
			if ( c.Symbol == symbol ) return c;
		return Coins[0];
	}
}
