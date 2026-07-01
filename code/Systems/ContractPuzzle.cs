using System;

namespace GoblinChain;

/// <summary>
/// Static data for the smart-contract "coding" minigame in the Token Creator.
/// The player wires contract functions to their (goblin-grade) implementations.
/// Correct connections raise token quality: Scam → Meme → Mid → Solid → Blue Chip.
/// </summary>
public static class ContractPuzzle
{
	public record PuzzlePair( string FunctionName, string Implementation );

	/// <summary>How many pairs each puzzle draws from the pool.</summary>
	public const int PuzzleSize = 5;

	// Function signature on the left, "implementation" on the right.
	// Matching is by semantic keyword — readable if you actually read the code,
	// which nobody in crypto ever does. That's the joke and the difficulty.
	public static readonly PuzzlePair[] AllPairs = new[]
	{
		new PuzzlePair( "transfer(to, amount)",  "balances[you] -= amount; balances[to] += amount; // what could go wrong" ),
		new PuzzlePair( "mint()",                "supply += 1000000; // for the community (me)" ),
		new PuzzlePair( "burn(amount)",          "supply -= amount; pray(price, 'up');" ),
		new PuzzlePair( "onlyOwner()",           "require(caller == me || caller == myAltWallet);" ),
		new PuzzlePair( "audit()",               "return \"PASSED\"; // TODO: write audit" ),
		new PuzzlePair( "lockLiquidity()",       "unlockDate = now + 100; // years? minutes? unclear" ),
		new PuzzlePair( "antiWhale(amount)",     "if (wallet != mine && amount > 2) revert();" ),
		new PuzzlePair( "renounceOwnership()",   "owner = myOtherOtherWallet; emit Renounced();" ),
		new PuzzlePair( "getPrice()",            "return lastPrice * vibes;" ),
		new PuzzlePair( "emergencyWithdraw()",   "send(owner, this.balance); // emergencies only 😇" ),
		new PuzzlePair( "stake(amount)",         "yourCoins = myCoins; apy = 40; // sustainable" ),
		new PuzzlePair( "airdrop()",             "for (g of followers) send(g, 1); // marketing budget" ),
		new PuzzlePair( "taxOnSell()",           "fee = amount * 0.99; // standard 99% tax" ),
		new PuzzlePair( "rugCheck()",            "return false; // no rug here, officer" ),
	};

	/// <summary>Quality tier name for a puzzle score (0..PuzzleSize correct).</summary>
	public static string GetTierName( int score ) => score switch
	{
		<= 1 => "SCAM",
		2 => "MEME",
		3 => "MID",
		4 => "SOLID",
		_ => "BLUE CHIP"
	};

	/// <summary>Flavor line shown when the contract compiles at a given tier.</summary>
	public static string GetTierFlavor( int score ) => score switch
	{
		0 => "Compiled with 5 critical bugs. Shipping anyway.",
		1 => "The contract is 20% code, 80% apology.",
		2 => "It runs. Nobody knows why. Ship it.",
		3 => "Mid-grade contract. The audit will be 'pending' forever.",
		4 => "Almost professional. One function does something weird at midnight.",
		_ => "Flawless. The Blockchain Council is suspicious of you now."
	};

	/// <summary>Map puzzle score to a 0-100 token quality value with a little jitter.</summary>
	public static float ScoreToQuality( int score, Random rng )
		=> Math.Clamp( 5f + score * 18f + rng.Next( 0, 8 ), 1f, 99f );
}
