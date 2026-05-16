using System;
using System.Collections.Generic;

namespace GoblinChain;

/// <summary>
/// Procedural NPC name generator for retail investor bots.
/// Combines goblin-themed first names with crypto-bro last names.
/// </summary>
public static class NPCNames
{
	private static readonly string[] FirstNames = {
		"Blobrick", "Snargle", "Griznak", "Muckwort", "Dribble",
		"Scabsworth", "Fungrim", "Rotgut", "Blister", "Gnawbone",
		"Pustule", "Wormsworth", "Snivels", "Bogwart", "Crudmuffin",
		"Fleabottom", "Gunkface", "Horkle", "Itchwick", "Junkrat",
		"Kneecap", "Lumpkin", "Moldsworth", "Nubbin", "Oozeworth"
	};

	private static readonly string[] LastNames = {
		"McGains", "VanHODL", "DeFi-nitely", "TokenBro", "McPump",
		"LiquidPool", "GasFeeson", "Shillerton", "RugPullski", "NFTeeze",
		"VanMoon", "McBags", "Stakerson", "Yieldworth", "FOMOstein",
		"DumpItski", "LeverageMax", "McWhale", "PaperHands", "DiamondGrip",
		"ApeInson", "FloorPrice", "MintFresh", "BurnItAll", "McVolatile"
	};

	private static readonly string[] Titles = {
		"", "", "", "", "", // 50% chance of no title
		"Sir ", "Lord ", "Dr. ", "Professor ", "Captain "
	};

	private static readonly string[] Suffixes = {
		"", "", "", "", "", // 50% chance of no suffix
		" III", " Jr.", " PhD", " Esq.", " (anon)"
	};

	private static Random _rng = new();

	/// <summary>
	/// Generate a single random NPC name.
	/// </summary>
	public static string Generate()
	{
		var title = Titles[_rng.Next( Titles.Length )];
		var first = FirstNames[_rng.Next( FirstNames.Length )];
		var last = LastNames[_rng.Next( LastNames.Length )];
		var suffix = Suffixes[_rng.Next( Suffixes.Length )];
		return $"{title}{first} {last}{suffix}";
	}

	/// <summary>
	/// Generate a batch of unique NPC names.
	/// </summary>
	public static List<string> GenerateBatch( int count )
	{
		var names = new HashSet<string>();
		int attempts = 0;
		while ( names.Count < count && attempts < count * 10 )
		{
			names.Add( Generate() );
			attempts++;
		}
		return new List<string>( names );
	}

	/// <summary>
	/// Generate a display handle for GoblinTwitter (@name format).
	/// </summary>
	public static string GenerateHandle()
	{
		var handles = new string[] {
			$"@{FirstNames[_rng.Next( FirstNames.Length )].ToLower()}{_rng.Next( 99 )}",
			$"@crypto_{FirstNames[_rng.Next( FirstNames.Length )].ToLower()}",
			$"@{LastNames[_rng.Next( LastNames.Length )].ToLower().Replace( " ", "" )}",
			$"@degen{_rng.Next( 9999 )}",
			$"@goblin_investor_{_rng.Next( 999 )}",
			$"@ape_{FirstNames[_rng.Next( FirstNames.Length )].ToLower()}",
			$"@wagmi_{_rng.Next( 9999 )}",
			$"@ngmi_but_buying_{_rng.Next( 99 )}"
		};
		return handles[_rng.Next( handles.Length )];
	}
}
