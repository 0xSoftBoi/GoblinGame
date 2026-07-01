using System;

namespace GoblinChain;

/// <summary>
/// Static data for all shill post components.
/// Players compose posts by picking one from each category:
/// Hype Opener + Claim + Social Proof + CTA.
/// The combination determines shill effectiveness, SEC risk, and virality.
///
/// Post shape: "{Opener} ${TICKER} {claim} — {proof}. {cta}"
/// Claims are verb phrases that follow the ticker. Proofs are noun clauses.
/// </summary>
public static class ShillTemplates
{
	// ═══════════════════════════════════════
	//  COMPONENT DATA
	// ═══════════════════════════════════════

	// HypePower: raw attention. Credibility: how believable. CringeFactor: viral spread
	// (cringe travels). Risk: how much GoblinSEC cares. All 1-9.
	public record ShillComponent( string Text, int HypePower, int Credibility, int CringeFactor, int Risk );

	// ═══════════════════════════════════════
	//  HYPE OPENERS
	// ═══════════════════════════════════════

	public static readonly ShillComponent[] HypeOpeners = new[]
	{
		new ShillComponent( "THIS IS NOT FINANCIAL ADVICE BUT…",                          7, 3, 4, 2 ),
		new ShillComponent( "GOBLIN INSIDER INFO 🚨",                                     9, 2, 6, 7 ),
		new ShillComponent( "Why is nobody talking about",                                5, 6, 2, 1 ),
		new ShillComponent( "Just mortgaged my cave for",                                 8, 4, 8, 3 ),
		new ShillComponent( "My uncle works at the Blockchain Council and says",          6, 3, 7, 5 ),
		new ShillComponent( "I shouldn't be telling you this but",                        8, 5, 3, 6 ),
		new ShillComponent( "THREAD 🧵 (1/47):",                                          4, 7, 5, 1 ),
		new ShillComponent( "URGENT — READ BEFORE DELETED:",                              9, 1, 9, 8 ),
		new ShillComponent( "deleting this in 10 minutes:",                               8, 2, 6, 4 ),
		new ShillComponent( "ok. I've been quiet about this long enough.",                6, 6, 3, 2 ),
		new ShillComponent( "you're going to want to sit down for this one:",             6, 4, 5, 2 ),
		new ShillComponent( "was up all night reading the contract and I'm shaking:",     7, 6, 4, 2 ),
		new ShillComponent( "my alpha group is FURIOUS I'm posting this:",                8, 3, 6, 5 ),
		new ShillComponent( "the chart is literally begging you to look at it:",          5, 4, 5, 1 ),
		new ShillComponent( "imagine fading this. couldn't be me:",                       5, 3, 7, 1 ),
		new ShillComponent( "I don't shill. you KNOW I don't shill. but:",                7, 5, 5, 3 ),
		new ShillComponent( "PSA for the real ones still here:",                          5, 5, 3, 1 ),
		new ShillComponent( "not a drill. NOT a drill:",                                  8, 2, 7, 3 ),
		new ShillComponent( "my bags are packed and so is my conscience:",                6, 3, 6, 4 ),
		new ShillComponent( "in 6 months you'll pretend you saw this coming:",            6, 5, 4, 1 ),
		new ShillComponent( "the smartest goblin I know just went all in on",             7, 6, 3, 2 ),
		new ShillComponent( "get in before the influencers find it:",                     6, 5, 4, 2 ),
		new ShillComponent( "leaked from a private telegram (don't ask):",                8, 2, 5, 8 ),
		new ShillComponent( "market makers don't want you seeing this:",                  7, 3, 5, 5 ),
		new ShillComponent( "I did the math so you don't have to:",                       5, 7, 3, 1 ),
		new ShillComponent( "photo of the dev's whiteboard (real):",                      7, 2, 7, 6 ),
		new ShillComponent( "1000x or I delete my account:",                              8, 1, 9, 3 ),
		new ShillComponent( "gm to everyone except the paper hands:",                     5, 3, 7, 1 ),
		new ShillComponent( "your last chance to be early to literally anything:",        7, 4, 6, 2 ),
		new ShillComponent( "FINAL WARNING (this time I mean it):",                       8, 1, 8, 4 ),
	};

	// ═══════════════════════════════════════
	//  CLAIMS — verb phrase following "$TICKER"
	// ═══════════════════════════════════════

	public static readonly ShillComponent[] Claims = new[]
	{
		new ShillComponent( "is going to 100x by tomorrow, maybe sooner",                                            9, 1, 7, 6 ),
		new ShillComponent( "is backed by ancient goblin magic AND a Swiss foundation",                              6, 2, 8, 3 ),
		new ShillComponent( "has devs who are literally wizards. I checked their github. it's spells.",              5, 3, 8, 1 ),
		new ShillComponent( "is the next GoblinCoin and for once I'm actually early",                                7, 4, 5, 2 ),
		new ShillComponent( "has a Blockchain Council partnership dropping any day now",                             8, 3, 4, 8 ),
		new ShillComponent( "is being loaded into whale wallets as we speak",                                        7, 5, 3, 4 ),
		new ShillComponent( "passed its audit with flying colors (self-audit, but still)",                           5, 6, 6, 5 ),
		new ShillComponent( "has literally perfect tokenomics. burn on every transaction. deflationary. beautiful.", 4, 7, 4, 1 ),
		new ShillComponent( "is about to announce a major exchange listing (I've seen the emails)",                  9, 2, 4, 9 ),
		new ShillComponent( "has more TVL than your entire bloodline's net worth",                                   6, 4, 6, 2 ),
		new ShillComponent( "is the only token here with real utility (utility TBA)",                                5, 5, 7, 2 ),
		new ShillComponent( "just quietly flipped RUGDAO in market cap",                                             6, 6, 3, 1 ),
		new ShillComponent( "has liquidity locked for 100 years (intern set it to 100 minutes, they're fixing it)",  6, 3, 8, 5 ),
		new ShillComponent( "is heads-down building while everyone else farms engagement",                           4, 8, 2, 1 ),
		new ShillComponent( "has a deflationary supply and an inflationary community",                               5, 6, 4, 1 ),
		new ShillComponent( "will make the '47 bull run look like a soundcheck",                                     8, 3, 5, 3 ),
		new ShillComponent( "is what SatoshiGoblin would have built with better funding",                            6, 5, 5, 2 ),
		new ShillComponent( "has zero team allocation (team holds via 14 unmarked wallets)",                         5, 2, 7, 8 ),
		new ShillComponent( "cannot mathematically go down. I ran the numbers twice.",                               8, 1, 9, 7 ),
		new ShillComponent( "is one influencer away from a god candle",                                              7, 5, 4, 2 ),
		new ShillComponent( "has a roadmap so good the devs keep it secret",                                         5, 3, 8, 3 ),
		new ShillComponent( "does 40% APY. sustainably. forever. somehow.",                                          8, 2, 7, 8 ),
		new ShillComponent( "is the safest 1000x you will ever be offered",                                          9, 1, 8, 8 ),
		new ShillComponent( "already survived two rugs, which makes it rug-proof now",                               5, 3, 7, 3 ),
		new ShillComponent( "is powered by real revenue (the revenue is selling the token)",                         5, 4, 8, 5 ),
		new ShillComponent( "will be the reserve currency of the entire Underhive",                                  7, 3, 5, 3 ),
		new ShillComponent( "has a dev who replied to me personally. we're basically cofounders.",                   4, 4, 8, 1 ),
		new ShillComponent( "is at pre-viral prices for approximately the next 20 minutes",                          8, 2, 6, 5 ),
		new ShillComponent( "fixed everything wrong with GOBETH in a single contract",                               6, 4, 4, 2 ),
		new ShillComponent( "is exactly what the Blockchain Council doesn't want you holding",                       7, 3, 5, 4 ),
	};

	// ═══════════════════════════════════════
	//  SOCIAL PROOF — noun clause after the dash
	// ═══════════════════════════════════════

	public static readonly ShillComponent[] SocialProofs = new[]
	{
		new ShillComponent( "my cousin's cousin works at GoblinSEC",                                    5, 2, 7, 6 ),
		new ShillComponent( "3 whales just aped in",                                                    7, 4, 4, 3 ),
		new ShillComponent( "it's trending on GoblinReddit",                                            6, 5, 3, 1 ),
		new ShillComponent( "Elon Goblin just tweeted about it",                                        9, 1, 9, 4 ),
		new ShillComponent( "the smart money is already in",                                            6, 6, 2, 2 ),
		new ShillComponent( "every alpha group is talking about this",                                  7, 3, 5, 3 ),
		new ShillComponent( "Vitazzle the Pale mentioned it at GoblinCon",                              8, 4, 6, 5 ),
		new ShillComponent( "a top 10 wallet just bought 500K",                                         7, 5, 3, 4 ),
		new ShillComponent( "the telegram gained 4,000 members overnight, all real, I checked three",   6, 3, 6, 3 ),
		new ShillComponent( "a wallet that front-ran the last six rugs just bought in",                 7, 6, 3, 5 ),
		new ShillComponent( "my barber asked me about it, which historically means sell, but ignore that", 4, 5, 7, 1 ),
		new ShillComponent( "the dev's mom liquidated her retirement for this",                         6, 3, 8, 4 ),
		new ShillComponent( "CoinGoblin ranks it #1 trending (I refreshed until it was)",               6, 4, 7, 2 ),
		new ShillComponent( "an anonymous whale DM'd me a single 👀",                                   7, 2, 6, 3 ),
		new ShillComponent( "the chart looks exactly like GBC in '47 before the run",                   7, 5, 3, 2 ),
		new ShillComponent( "insiders are accumulating (I am insiders)",                                6, 2, 8, 8 ),
		new ShillComponent( "the Blockchain Council tried to subpoena the devs. bullish.",              7, 4, 6, 4 ),
		new ShillComponent( "it's the only thing my alpha group has ever agreed on",                    5, 6, 3, 2 ),
		new ShillComponent( "two exchange listings confirmed by a guy who's never been wrong twice",    6, 3, 7, 5 ),
		new ShillComponent( "on-chain data says accumulation and my heart says lambo",                  6, 5, 6, 1 ),
		new ShillComponent( "someone paid 900 GBC in gas just to buy the top. that's conviction.",      5, 4, 7, 1 ),
		new ShillComponent( "the community is unpaid and relentless, like me",                          5, 5, 5, 1 ),
		new ShillComponent( "GoblinNomura initiated coverage with a rating of '???'",                   6, 5, 5, 2 ),
		new ShillComponent( "the discord mods have completely stopped rugging people. growth.",         5, 3, 8, 3 ),
		new ShillComponent( "a fortune cookie told me first and the chart confirmed",                   5, 2, 8, 1 ),
		new ShillComponent( "it was on seven podcasts this week, all recorded in the same basement",    5, 4, 7, 1 ),
		new ShillComponent( "the top holder has never sold. can't. lost the keys. bullish.",            6, 4, 7, 2 ),
		new ShillComponent( "Auntie Hagatha's investment club is in, and they have never lost",         6, 4, 6, 2 ),
		new ShillComponent( "even the wash traders picked THIS one. they know something.",              6, 3, 6, 7 ),
		new ShillComponent( "GoblinSEC hasn't sued yet, which is basically an endorsement",             6, 4, 6, 6 ),
	};

	// ═══════════════════════════════════════
	//  CALLS TO ACTION
	// ═══════════════════════════════════════

	public static readonly ShillComponent[] CTAs = new[]
	{
		new ShillComponent( "BUY NOW OR CRY LATER",                                        8, 2, 7, 2 ),
		new ShillComponent( "last chance before moon",                                     7, 3, 5, 2 ),
		new ShillComponent( "don't say I didn't warn you",                                 5, 5, 4, 1 ),
		new ShillComponent( "NFA but also FA",                                             6, 1, 9, 3 ),
		new ShillComponent( "set a reminder for next week",                                3, 7, 2, 1 ),
		new ShillComponent( "screenshot this tweet",                                       4, 4, 6, 1 ),
		new ShillComponent( "wagmi if you ape now 🦍",                                     7, 2, 8, 2 ),
		new ShillComponent( "your future self will thank you",                             5, 5, 4, 1 ),
		new ShillComponent( "like and repost so the algorithm blesses you too",            5, 3, 7, 1 ),
		new ShillComponent( "DYOR (the R is this tweet)",                                  5, 2, 8, 2 ),
		new ShillComponent( "sell your rig, buy the dip",                                  6, 2, 7, 3 ),
		new ShillComponent( "tell your grandkids you were here",                           5, 4, 6, 1 ),
		new ShillComponent( "the exit liquidity thanks you in advance",                    4, 3, 9, 5 ),
		new ShillComponent( "few understand. fewer will after this tweet.",                5, 4, 7, 1 ),
		new ShillComponent( "if this ages badly, delete your memory of it",                4, 4, 7, 1 ),
		new ShillComponent( "buy first, understand later, brag forever",                   6, 2, 8, 3 ),
		new ShillComponent( "don't fade goblins twice",                                    5, 4, 5, 1 ),
		new ShillComponent( "the dip is a gift. regift it to me.",                         5, 3, 7, 2 ),
		new ShillComponent( "mute the haters, max the bags",                               6, 3, 6, 2 ),
		new ShillComponent( "I'll pin this when it hits. pins never lie.",                 5, 4, 6, 1 ),
		new ShillComponent( "get in before retail (you are the retail)",                   5, 3, 8, 2 ),
		new ShillComponent( "this is your sign. it was also the last 40 signs.",           5, 3, 8, 1 ),
		new ShillComponent( "smash buy and log off. serenity.",                            6, 3, 6, 1 ),
		new ShillComponent( "trust me. I've been wrong before, but never like this.",      6, 4, 6, 2 ),
		new ShillComponent( "bookmark this for the museum",                                4, 4, 6, 1 ),
		new ShillComponent( "yolo responsibly (or not)",                                   5, 3, 7, 1 ),
		new ShillComponent( "when it moons, remember who shilled you",                     6, 4, 5, 1 ),
		new ShillComponent( "generational wealth or another funny story. win-win.",        5, 4, 6, 1 ),
		new ShillComponent( "load your bags and hide your ledger",                         6, 3, 5, 4 ),
		new ShillComponent( "act now. regret is for the unleveraged.",                     6, 2, 7, 3 ),
	};

	// ═══════════════════════════════════════
	//  EFFECTIVENESS CALCULATION
	// ═══════════════════════════════════════

	/// <summary>
	/// Calculate total shill power from selected components.
	/// Returns value roughly 0-10 scale.
	/// </summary>
	public static float CalculateEffectiveness(
		int openerIdx, int claimIdx, int proofIdx, int ctaIdx,
		float reputationMultiplier )
	{
		var opener = GetSafe( HypeOpeners, openerIdx );
		var claim = GetSafe( Claims, claimIdx );
		var proof = GetSafe( SocialProofs, proofIdx );
		var cta = GetSafe( CTAs, ctaIdx );

		float avgHype = (opener.HypePower + claim.HypePower + proof.HypePower + cta.HypePower) / 4f;
		float avgCred = (opener.Credibility + claim.Credibility + proof.Credibility + cta.Credibility) / 4f;
		float avgCringe = (opener.CringeFactor + claim.CringeFactor + proof.CringeFactor + cta.CringeFactor) / 4f;

		// Cringe = viral potential (spreads wider but less believable)
		float viralSpread = avgCringe * 0.7f;

		float effectiveness = (avgHype * 0.3f) + (avgCred * 0.3f) + (viralSpread * 0.2f) + (reputationMultiplier * 2f * 0.2f);

		return effectiveness;
	}

	/// <summary>
	/// Calculate total SEC risk from selected components.
	/// </summary>
	public static float CalculateRisk( int openerIdx, int claimIdx, int proofIdx, int ctaIdx )
	{
		var opener = GetSafe( HypeOpeners, openerIdx );
		var claim = GetSafe( Claims, claimIdx );
		var proof = GetSafe( SocialProofs, proofIdx );
		var cta = GetSafe( CTAs, ctaIdx );

		return (opener.Risk + claim.Risk + proof.Risk + cta.Risk) / 4f;
	}

	/// <summary>
	/// Assemble the full post text from component indices.
	/// The ticker is woven in after the opener, crypto-twitter style.
	/// </summary>
	public static string AssemblePostText( int openerIdx, int claimIdx, int proofIdx, int ctaIdx, string ticker, string customText )
	{
		var opener = GetSafe( HypeOpeners, openerIdx );
		var claim = GetSafe( Claims, claimIdx );
		var proof = GetSafe( SocialProofs, proofIdx );
		var cta = GetSafe( CTAs, ctaIdx );

		string tick = string.IsNullOrWhiteSpace( ticker ) ? "???" : ticker;
		string text = $"{opener.Text} ${tick} {claim.Text} — {proof.Text}. {cta.Text}";

		if ( !string.IsNullOrWhiteSpace( customText ) )
			text += $"\n\n{customText}";

		return text;
	}

	private static ShillComponent GetSafe( ShillComponent[] arr, int idx )
		=> arr[Math.Clamp( idx, 0, arr.Length - 1 )];
}
