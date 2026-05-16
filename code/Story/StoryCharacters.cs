using System;
using System.Collections.Generic;

namespace GoblinChain.Story;

/// <summary>
/// Every named character in "The Goblin's Bargain" story mode.
/// Each character has a Circle portrait, relationship state, and chapter appearances.
/// The Circle is the REAL score — glowing portraits = relationships maintained,
/// dim/cracked = relationships lost to the Hoard.
/// </summary>
public static class StoryCharacters
{
	// ═══════════════════════════════════════
	//  CHARACTER DATA
	// ═══════════════════════════════════════

	public enum CircleState
	{
		Warm,       // Glowing, relationship healthy
		Fading,     // Dimming, being neglected
		Dim,        // Nearly gone, serious neglect
		Cracked,    // Broken event (death, betrayal, fight)
		Dark,       // Gone. Not dead — just gone. Worse than cracked.
		Hidden      // Not yet introduced
	}

	public enum Species
	{
		Goblin,
		Halfling,
		Elf,
		Orc,
		OrcGoblin,  // Hybrid
		Ogre,
		Ghost,       // Thotmas — never physically present
		Ancient      // The Goblin King
	}

	public record CharacterData(
		string Id,
		string Name,
		string Title,
		Species Species,
		string Description,
		string IntroQuote,
		int IntroChapter,
		CircleState DefaultState,
		string PortraitIcon  // Emoji placeholder until art assets
	);

	public static readonly CharacterData[] AllCharacters = new[]
	{
		// ═══ CORE CIRCLE ═══

		new CharacterData(
			"grix", "Grix", "The Protagonist",
			Species.Goblin,
			"Small green goblin. DACA kid from Appalachia. The one who keeps climbing.",
			"The Hoard counter was invented that day. Not by bankers. By me.",
			1, CircleState.Warm, "🟢"
		),

		new CharacterData(
			"grix_mother", "Grix's Mother", "Mamá",
			Species.Goblin,
			"The safety net. She catches him every time he falls. Her warmth is the baseline the entire game is measured against.",
			"We'll be okay, mijo. We've been okay before.",
			1, CircleState.Warm, "💚"
		),

		new CharacterData(
			"grix_father", "Grix's Father", "Papá",
			Species.Goblin,
			"Work boots by the door. Lost the house. Never lost his dignity.",
			"Help me fix this porch, son. Hold it steady.",
			1, CircleState.Warm, "🔨"
		),

		new CharacterData(
			"fenn", "Fenn", "The Best Friend",
			Species.Halfling,
			"Bright-eyed halfling. The warmest portrait in the Circle. The one who doesn't make it.",
			"Dude, you're literally a drug dealer operating from the woods. You're the coolest person in this town.",
			2, CircleState.Hidden, "✨"
		),

		new CharacterData(
			"borgg", "Borgg the Burned", "The Mentor",
			Species.OrcGoblin,
			"Ex-Moon Machinations (Sun Microsystems). Brilliant, alcoholic, broken by the opioid crisis that killed his son. Teaches Grix everything.",
			"The internet was supposed to free us, kid. It just built faster cages.",
			2, CircleState.Hidden, "🍺"
		),

		new CharacterData(
			"pierrot", "Pierrot le Fang", "The Anarchist",
			Species.Goblin,
			"Tall, gaunt goblin with a beret. French autistic anarchist. Speaks in manifestos. The funniest character in the game.",
			"Property is theft. Also, can I borrow twenty dollars?",
			3, CircleState.Hidden, "🎭"
		),

		new CharacterData(
			"lyra", "Lyra", "The Elf",
			Species.Elf,
			"The only non-goblin the protagonist falls for. Met twice, years apart. She represents everything outside the cave.",
			"You talk about money like it's oxygen. Like you'd die without it.",
			13, CircleState.Hidden, "🌿"
		),

		// ═══ CRYPTO WORLD ═══

		new CharacterData(
			"vitazzle", "Vitazzle the Pale", "The Visionary",
			Species.Elf,
			"Emaciated elf who pitches Aetherium at a forest festival. Either insane or the future.",
			"What if smart contracts could be... smart?",
			3, CircleState.Hidden, "💎"
		),

		new CharacterData(
			"thotmas", "Thotmas the Phantom", "The Ghost",
			Species.Ghost,
			"A ghost NPC — only interact through message scrolls. Never physically present until Miami. Creator of SafeStar.",
			"(message scroll) The wallet is almost ready. Wire the gold and we'll talk.",
			5, CircleState.Hidden, "👻"
		),

		new CharacterData(
			"bogan_tall", "Bogan Tall", "The Influencer",
			Species.Orc,
			"Massive orc influencer with a camera goblin following him everywhere. CryptoLair creator.",
			"The community loves me. I love the community. Can someone tell me what a blockchain is?",
			10, CircleState.Hidden, "📸"
		),

		new CharacterData(
			"dave_coinboy", "Dave Coinboy", "The Loudmouth",
			Species.Goblin,
			"Loud goblin with a pizza obsession. Screams about every price movement.",
			"GoldCoin is DEAD. Wait no it's ALIVE. Wait — who are you again?",
			10, CircleState.Hidden, "🍕"
		),

		new CharacterData(
			"blinkleboss_a", "Blinkleboss Twin A", "The Establishment",
			Species.Ogre,
			"One of two identical ogres who own the building above you. Speaks in synchronized sentences.",
			"Institutional adoption is inevitable. We own the building.",
			11, CircleState.Hidden, "🏢"
		),

		new CharacterData(
			"blinkleboss_b", "Blinkleboss Twin B", "The Establishment",
			Species.Ogre,
			"The other identical ogre. Finishes his brother's sentences.",
			"...inevitable. We also own the building next door.",
			11, CircleState.Hidden, "🏢"
		),

		new CharacterData(
			"goldtusk", "Herr Goldtusk", "The Swiss Investor",
			Species.Goblin,
			"Dapper goblin in a monocle. Old money meets new money on a train to Goblin York City.",
			"Tell me, young goblin — what is your coin actually worth? Not the chart. The truth.",
			5, CircleState.Hidden, "🧐"
		),

		new CharacterData(
			"skrag", "Skrag", "The Cofounder",
			Species.Goblin,
			"Rival goblin. Ambitious but not malicious. The cofounder fight tests whether Grix has grown.",
			"We built this together, Grix. Don't forget that when the money starts talking.",
			14, CircleState.Hidden, "⚔️"
		),

		new CharacterData(
			"burnweed", "Chief Burnweed", "The DAO Prophet",
			Species.Goblin,
			"Went to the Flaming Goblin Festival. Came back speaking only in governance proposals.",
			"I propose we decentralize the concept of proposals.",
			9, CircleState.Hidden, "🔥"
		),

		// ═══ INSTITUTIONAL / SHADOWY ═══

		new CharacterData(
			"crimson_mint", "The Crimson Mint Syndicate", "First Real Money",
			Species.Goblin,
			"First real institutional investor. Professional. Fair. The deal is clean.",
			"We invest in builders, not hype. Show us the code.",
			14, CircleState.Hidden, "🏛️"
		),

		new CharacterData(
			"cambric_weavers", "The Cambric Weavers", "The Shadow",
			Species.Goblin,
			"Shadowy figures who want to weave data into votes. The room is literally darker when they're in it.",
			"We have a proposal. Regarding... data. And elections. Nothing illegal. Mostly.",
			5, CircleState.Hidden, "🕸️"
		),

		new CharacterData(
			"professor", "The Professor", "Austrian Economics",
			Species.Goblin,
			"Goblin Mason Academy. Asks the question that changes everything.",
			"What if money didn't need a king?",
			3, CircleState.Hidden, "📚"
		),

		// ═══ THE EPILOGUE ═══

		new CharacterData(
			"goblin_king", "The Goblin King", "The Bargain",
			Species.Ancient,
			"Ancient, gnarled creature. Appears once, at the end. The thesis made flesh.",
			"You took my bargain, boy. Gold for blood. Fortune for friends. Fame for love. The terms were clear. You just didn't read the contract.",
			99, CircleState.Hidden, "👑"  // Chapter 99 = epilogue
		),
	};

	// ═══════════════════════════════════════
	//  LOOKUP
	// ═══════════════════════════════════════

	private static Dictionary<string, CharacterData> _lookup;

	public static CharacterData Get( string id )
	{
		if ( _lookup == null )
		{
			_lookup = new Dictionary<string, CharacterData>();
			foreach ( var c in AllCharacters )
				_lookup[c.Id] = c;
		}
		return _lookup.TryGetValue( id, out var data ) ? data : AllCharacters[0];
	}

	public static CharacterData[] GetChapterCharacters( int chapter )
	{
		var list = new List<CharacterData>();
		foreach ( var c in AllCharacters )
		{
			if ( c.IntroChapter <= chapter && c.Id != "grix" )
				list.Add( c );
		}
		return list.ToArray();
	}
}
