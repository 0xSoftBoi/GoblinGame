namespace GoblinChain;

/// <summary>
/// Static data for all shill post components.
/// Players compose posts by picking one from each category.
/// The combination determines shill effectiveness, SEC risk, and virality.
/// </summary>
public static class ShillTemplates
{
	// ═══════════════════════════════════════
	//  COMPONENT DATA
	// ═══════════════════════════════════════

	public record ShillComponent( string Text, int HypePower, int Credibility, int CringeFactor, int Risk );

	// ═══════════════════════════════════════
	//  HYPE OPENERS
	// ═══════════════════════════════════════

	public static readonly ShillComponent[] HypeOpeners = new[]
	{
		new ShillComponent( "THIS IS NOT FINANCIAL ADVICE BUT…",         7, 3, 4, 2 ),
		new ShillComponent( "GOBLIN INSIDER INFO 🚨",                    9, 2, 6, 7 ),
		new ShillComponent( "Why is nobody talking about…",              5, 6, 2, 1 ),
		new ShillComponent( "Just mortgaged my cave for…",               8, 4, 8, 3 ),
		new ShillComponent( "My uncle works at the Blockchain Council…", 6, 3, 7, 5 ),
		new ShillComponent( "I shouldn't be telling you this but…",      8, 5, 3, 6 ),
		new ShillComponent( "THREAD 🧵 (1/47):",                         4, 7, 5, 1 ),
		new ShillComponent( "URGENT — READ BEFORE DELETED:",             9, 1, 9, 8 ),
	};

	// ═══════════════════════════════════════
	//  CLAIMS
	// ═══════════════════════════════════════

	public static readonly ShillComponent[] Claims = new[]
	{
		new ShillComponent( "going to 100x by tomorrow",                        9, 1, 7, 3 ),
		new ShillComponent( "backed by ancient goblin magic",                   6, 2, 8, 2 ),
		new ShillComponent( "the devs are literally wizards",                   5, 3, 6, 1 ),
		new ShillComponent( "this is the next GoblinCoin",                      7, 4, 5, 2 ),
		new ShillComponent( "partnership with the Blockchain Council incoming", 8, 3, 4, 7 ),
		new ShillComponent( "whale wallets loading up as we speak",             7, 5, 3, 4 ),
		new ShillComponent( "audit passed with flying colors (trust me)",       5, 6, 6, 5 ),
		new ShillComponent( "tokenomics are literally perfect",                 4, 7, 3, 1 ),
	};

	// ═══════════════════════════════════════
	//  SOCIAL PROOF
	// ═══════════════════════════════════════

	public static readonly ShillComponent[] SocialProofs = new[]
	{
		new ShillComponent( "my cousin's cousin works at GoblinSEC",         5, 2, 7, 6 ),
		new ShillComponent( "3 whales just aped in",                         7, 4, 4, 3 ),
		new ShillComponent( "trending on GoblinReddit",                      6, 5, 3, 1 ),
		new ShillComponent( "Elon Goblin just tweeted about it",             9, 1, 9, 4 ),
		new ShillComponent( "the smart money is already in",                 6, 6, 2, 2 ),
		new ShillComponent( "every alpha group is talking about this",       7, 3, 5, 3 ),
		new ShillComponent( "Vitazzle the Pale mentioned it at GoblinCon",  8, 4, 6, 5 ),
		new ShillComponent( "top 10 wallet just bought 500K",               7, 5, 3, 4 ),
	};

	// ═══════════════════════════════════════
	//  CALLS TO ACTION
	// ═══════════════════════════════════════

	public static readonly ShillComponent[] CTAs = new[]
	{
		new ShillComponent( "BUY NOW OR CRY LATER",                  8, 2, 7, 2 ),
		new ShillComponent( "last chance before moon",               7, 3, 5, 2 ),
		new ShillComponent( "don't say I didn't warn you",           5, 5, 4, 1 ),
		new ShillComponent( "NFA but also FA",                       6, 1, 9, 3 ),
		new ShillComponent( "set a reminder for next week",          3, 7, 2, 1 ),
		new ShillComponent( "screenshot this tweet",                 4, 4, 6, 1 ),
		new ShillComponent( "wagmi if you ape now 🦍",               7, 2, 8, 2 ),
		new ShillComponent( "your future self will thank you",       5, 5, 4, 1 ),
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
	/// </summary>
	public static string AssemblePostText( int openerIdx, int claimIdx, int proofIdx, int ctaIdx, string customText )
	{
		var opener = GetSafe( HypeOpeners, openerIdx );
		var claim = GetSafe( Claims, claimIdx );
		var proof = GetSafe( SocialProofs, proofIdx );
		var cta = GetSafe( CTAs, ctaIdx );

		string text = $"{opener.Text} {claim.Text} — {proof.Text}. {cta.Text}";

		if ( !string.IsNullOrWhiteSpace( customText ) )
			text += $"\n\n{customText}";

		return text;
	}

	private static ShillComponent GetSafe( ShillComponent[] arr, int idx )
		=> arr[Math.Clamp( idx, 0, arr.Length - 1 )];
}
