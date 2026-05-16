using System;
using System.Collections.Generic;

namespace GoblinChain.Story;

/// <summary>
/// Complete chapter data for "The Goblin's Bargain" — all 14 chapters + epilogue.
/// Each chapter defines: era, tone, gameplay genre, scenes with dialogue/choices,
/// Circle effects, Hoard changes, and Grind Gauge impacts.
/// This is the SOUL of the game. The anti-power fantasy made manifest.
/// </summary>
public static class StoryChapters
{
	// ═══════════════════════════════════════
	//  ENUMS
	// ═══════════════════════════════════════

	public enum ChapterTone { Warm, Dark, Energetic, Manic, Thriller, Carnival, Noir, Desperate, Triumphant, Seductive, Hollow, Peaceful, Devastating, Reckoning }
	public enum GameplayType { WalkingSim, HackingPuzzle, Tycoon, DialogueRPG, PartyGame, FactoryAutomation, CorporateManagement, StockTrading, ExpoManagement, CasinoTycoon, DeliveryDriving, DeFiGambling, SocialMedia, PartySim, Exploration, RelationshipSim, StartupManagement, CrisisManagement }
	public enum MusicStyle { AppalachianFolk, DarkElectronic, IndieRock, EDM, CorporateThriller, Carnival, JazzNoir, SparsePiano, HypeBeast, MiamiBass, AmbientDrone, ClinicalElectronic, ThaiAmbient, BrokenLoveTheme, Silence }

	// ═══════════════════════════════════════
	//  SCENE / DIALOGUE / CHOICE STRUCTURES
	// ═══════════════════════════════════════

	public record DialogueLine(
		string SpeakerId,          // Character ID or "narrator" or "internal"
		string Text,
		bool IsInternal = false    // Grix's internal monologue (italic, different style)
	);

	public record Choice(
		string Text,               // What player sees
		string ResultText,         // Narration after choosing
		string CircleEffect,       // "fenn:+10" or "pierrot:-5" or "" for none
		int HoardChange,           // Gold delta
		int GrindChange,           // Burnout delta
		string FlagSet = ""        // Story flag to set: "took_laptop", "invested_aetherium", etc.
	);

	public record Scene(
		string Id,
		string Title,
		string Description,        // Scene-setting narration
		GameplayType Gameplay,
		DialogueLine[] Dialogue,
		Choice[] Choices,          // Empty = no choice, linear scene
		string[] CircleUpdates,    // Auto circle changes: "fenn:cracked", "borgg:warm"
		int HoardDelta,            // Auto hoard change for the scene
		int GrindDelta             // Auto grind change
	);

	public record ChapterData(
		int Number,
		string Title,
		string Subtitle,
		string Era,
		int PlaytimeMinutes,
		ChapterTone Tone,
		GameplayType PrimaryGameplay,
		MusicStyle Music,
		string OpeningText,        // Title card narration
		string ClosingText,        // End of chapter card
		Scene[] Scenes,
		string KeyInstrument       // For the music/tone map
	);

	// ═══════════════════════════════════════
	//  ALL 14 CHAPTERS + EPILOGUE
	// ═══════════════════════════════════════

	public static readonly ChapterData[] AllChapters = new[]
	{
		// ─────────────────────────────────────
		// CHAPTER 1: THE HOUSE THAT FELL
		// ─────────────────────────────────────
		new ChapterData(
			1, "THE HOUSE THAT FELL", "The Goblin's Bargain begins with a loss.",
			"2008", 20, ChapterTone.Warm, GameplayType.WalkingSim, MusicStyle.AppalachianFolk,
			"A small Appalachian town. Warm autumn colors. The most beautiful environment in the entire game.",
			"He made just enough. He lost more than enough.",
			new Scene[]
			{
				new Scene( "1_home", "The Family Home",
					"The family home is modest, warm, full of detail: family photos on the wall, mother's cooking on the stove, father's work boots by the door.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "narrator", "Every object in this house has a memory. Touch them. Remember them. You won't feel this warm again for a long time." ),
						new( "grix_mother", "Dinner's almost ready, mijo. Go help your father with the porch." ),
					},
					Array.Empty<Choice>(),
					new[] { "grix_mother:warm", "grix_father:warm" }, 0, -10 // Depletes grind — this is peace
				),

				new Scene( "1_porch", "Fix the Porch",
					"Simple mini-game: help Dad fix the porch. Hammer nails in rhythm. The ONLY time in the game a work mini-game feels warm and collaborative rather than lonely.",
					GameplayType.WalkingSim, // Rhythm mini-game within walking sim
					new DialogueLine[] {
						new( "grix_father", "Hold it steady, son. Like that. Good." ),
						new( "internal", "This is the only time in my life that work felt like love.", true ),
					},
					Array.Empty<Choice>(),
					new[] { "grix_father:warm" }, 0, -5
				),

				new Scene( "1_foreclosure", "The Notice",
					"The FORECLOSURE NOTICE arrives. The Hoard counter appears for the first time, starting at $0, flashing red with DEBT.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "grix_mother", "We'll be okay, mijo. We've been okay before." ),
						new( "internal", "The Hoard counter was invented that day. Not by bankers. By me. Sitting in a box truck watching my house shrink in the mirror.", true ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 0, 20 // Grind spikes — trauma
				),

				new Scene( "1_packing", "Packing Boxes",
					"Player watches — not plays — as the family packs boxes. Powerless. The warm autumn colors begin to desaturate.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "narrator", "You cannot help. You cannot stop this. This is what powerless feels like. Remember it." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 0, 15
				),
			},
			"Acoustic guitar"
		),

		// ─────────────────────────────────────
		// CHAPTER 2: THE UNDERBAZAAR
		// ─────────────────────────────────────
		new ChapterData(
			2, "THE UNDERBAZAAR", "The dark web. The friend. The loss.",
			"2013-2015", 75, ChapterTone.Dark, GameplayType.HackingPuzzle, MusicStyle.DarkElectronic,
			"Beast Buys. Freak Squad. A stolen laptop. A friend named Fenn. A bazaar beneath the bazaar.",
			"He made just enough for school. He lost more than enough of himself.",
			new Scene[]
			{
				new Scene( "2_beast_buys", "Freak Squad",
					"Grix works at Beast Buys in the Freak Squad. Borgg the Burned teaches him real skills disguised as goblin magic.",
					GameplayType.HackingPuzzle,
					new DialogueLine[] {
						new( "borgg", "See this grimoire? Kali. The Cloak of Shadows — that's Tor. The Vanishing Rune — Tails. Pay attention, kid. This is the real education." ),
						new( "borgg", "The internet was supposed to free us, kid. It just built faster cages." ),
						new( "internal", "Borgg smelled like barrel ale and broken dreams. He was the smartest person I'd ever met.", true ),
					},
					Array.Empty<Choice>(),
					new[] { "borgg:warm", "fenn:warm" }, 50, 10
				),

				new Scene( "2_the_theft", "The Laptop",
					"A customer's laptop sits unclaimed. The game presents it simply.",
					GameplayType.HackingPuzzle,
					new DialogueLine[] {
						new( "narrator", "It's been 90 days. Nobody came back for it. It sits on the shelf, humming." ),
					},
					new Choice[] {
						new( "TAKE", "You slide it into your bag. It weighs nothing. It weighs everything.", "borgg:+5", 0, 5, "took_laptop" ),
						new( "LEAVE", "You leave it. Borgg takes it for you anyway. The outcome is the same. But the choice is logged.", "borgg:+5", 0, 5, "left_laptop" ),
					},
					Array.Empty<string>(), 0, 5
				),

				new Scene( "2_the_hike", "The Woods",
					"A brief traversal — Grix walks through Appalachian woods at night with a stolen laptop, finding a spot near library WiFi. Beautiful and menacing.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "internal", "The woods were beautiful at night. So was the signal strength near the library. Both felt stolen.", true ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 0, 10
				),

				new Scene( "2_underbazaar", "The Market Below",
					"The Underbazaar — a procedurally generated underground goblin marketplace. List products, set prices, manage supply chains, handle encrypted messages.",
					GameplayType.Tycoon,
					new DialogueLine[] {
						new( "narrator", "Vendor rating: 4.8 stars. Reviews: 'Fast shipping, good potion quality, would buy again.' The Hoard starts climbing." ),
						new( "internal", "Every sale made the same sound. Cha-ching. I'd hear it in my sleep. I still do.", true ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 5000, 25
				),

				new Scene( "2_fenn_early", "Good Times",
					"Fenn appears in side missions — hanging out, skateboarding through goblin town, late-night talks. He's the warmest portrait in the Circle.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "fenn", "Dude, you're literally a drug dealer operating from the woods. You're the coolest person in this town." ),
						new( "fenn", "Let's just skate, man. Forget the bazaar for one night." ),
					},
					new Choice[] {
						new( "Skate with Fenn", "You put the laptop away. The night is warm. The gold can wait.", "fenn:+15", -100, -15, "skated_with_fenn" ),
						new( "Keep working", "You tell him maybe tomorrow. Tomorrow never comes enough times and it becomes never.", "fenn:-5", 200, 10, "kept_working" ),
					},
					Array.Empty<string>(), 0, 0
				),

				new Scene( "2_fenn_decline", "The Fade",
					"Fenn starts using. Subtle at first — dialogue changes, he's late, his portrait flickers. Nothing you do works. This is not a puzzle with a solution.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "fenn", "I just need a little more, Grix. I'm fine. I promise I'm fine." ),
						new( "internal", "His eyes were different. The brightness was chemical now. I tried everything. Nothing is the correct amount of everything when someone is disappearing.", true ),
					},
					new Choice[] {
						new( "Confront him", "He smiles. Says he'll stop. He doesn't stop.", "fenn:-5", 0, 15, "confronted_fenn" ),
						new( "Give him money", "He takes it. He thanks you. The gratitude is real. The destination is the same.", "fenn:+5", -500, 20, "gave_fenn_money" ),
						new( "Say nothing", "Sometimes silence is all you have. Sometimes it's not enough either.", "fenn:-10", 0, 10, "said_nothing_fenn" ),
					},
					Array.Empty<string>(), 0, 20
				),

				new Scene( "2_fenn_death", "The Call",
					"A phone call. Screen goes black. The portrait cracks. The Underbazaar interface is still open in the background with sales still ticking in. The juxtaposition is the point.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "narrator", "The phone rings. You already know." ),
						new( "narrator", "The Underbazaar keeps running. Sales tick in. Cha-ching. Cha-ching. Cha-ching. The sound doesn't stop for the dead." ),
					},
					Array.Empty<Choice>(),
					new[] { "fenn:cracked" }, 0, 40 // Massive grind spike — grief
				),

				new Scene( "2_exit", "Walking Away",
					"Grix walks away from the Underbazaar with exactly enough gold for school. The tycoon interface closes permanently.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "narrator", "He made just enough. He lost more than enough." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 0, -10
				),
			},
			"Synth bass + silence"
		),

		// ─────────────────────────────────────
		// CHAPTER 3: THE INVISIBLE HAND
		// ─────────────────────────────────────
		new ChapterData(
			3, "THE INVISIBLE HAND", "Austrian economics, anarchists, and the festival that changes everything.",
			"2017", 50, ChapterTone.Energetic, GameplayType.DialogueRPG, MusicStyle.IndieRock,
			"Goblin Mason Academy. A wrestling scholarship funded partly by drug money the game doesn't let you forget.",
			"The first real believers. The last honest party.",
			new Scene[]
			{
				new Scene( "3_classroom", "The Question",
					"A lecture. The professor draws diagrams on a chalkboard. The player connects concepts: money, state, separation, freedom.",
					GameplayType.HackingPuzzle,
					new DialogueLine[] {
						new( "professor", "What if money didn't need a king?" ),
						new( "internal", "I sold acid in the woods using money that didn't need a king. I just didn't have the vocabulary yet.", true ),
					},
					Array.Empty<Choice>(),
					new[] { "professor:warm" }, 0, -5
				),

				new Scene( "3_pierrot", "The Anarchist",
					"Meet Pierrot le Fang. Tall French goblin with a beret. Immediately, obviously, the funniest character in the game.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "pierrot", "Property is theft. Also, can I borrow twenty dollars?" ),
						new( "pierrot", "The state is a monopoly on violence. Anyway, are you coming to the Halloween party? I'm going as Murray Rothbard." ),
					},
					Array.Empty<Choice>(),
					new[] { "pierrot:warm" }, 0, -10 // Friendship depletes grind
				),

				new Scene( "3_501c3", "The Drunk Nonprofit",
					"A party. Pierrot and Grix, smashed on Shrub Light, fill out a nonprofit application. Timed mini-game where you're drunk — cursor wobbles, text misspells, submit button moves. Hilarious. It also works.",
					GameplayType.PartyGame,
					new DialogueLine[] {
						new( "pierrot", "If we're going to change the world, we should probably incorporate first. Hand me another Shrub Light." ),
						new( "internal", "We filed a 501(c)(3) while blackout drunk. It was the most honest thing either of us had ever done.", true ),
					},
					Array.Empty<Choice>(),
					new[] { "pierrot:warm" }, -200, -15
				),

				new Scene( "3_porkfeast", "PorkFeast",
					"A libertarian goblin festival. Renaissance faire meets crypto conference. The comedy peaks. Vitazzle pitches Aetherium.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "vitazzle", "What if smart contracts could be... smart?" ),
						new( "pierrot", "He's either insane or the future." ),
					},
					new Choice[] {
						new( "Invest in Aetherium", "You put 500 gold into a dream pitched by an emaciated elf. It pays off 1000x by Chapter 5. Your GoldCoin maxi friends resent you.", "vitazzle:+10,pierrot:-5", -500, 5, "invested_aetherium" ),
						new( "Stay GoldCoin Maxi", "You pass. Pure. Principled. Poor. The GoldCoin maxis respect you. Vitazzle shrugs and finds another believer.", "vitazzle:-5,pierrot:+5", 0, 0, "stayed_maxi" ),
					},
					new[] { "vitazzle:warm" }, 0, -5
				),
			},
			"Electric guitar"
		),

		// ─────────────────────────────────────
		// CHAPTER 4: TEMPLATE MAGIC
		// ─────────────────────────────────────
		new ChapterData(
			4, "TEMPLATE MAGIC", "The factory floor. Copy-paste. Gold rains.",
			"2017-2018", 45, ChapterTone.Manic, GameplayType.FactoryAutomation, MusicStyle.EDM,
			"San Francisco. A goblin conference. Speakers bragging about raising 150 million, 4 BILLION gold pieces. Grix's Hoard is tiny.",
			"Every template was the same. The gold didn't care.",
			new Scene[]
			{
				new Scene( "4_conference", "The Big Room",
					"A massive convention hall. NPCs flash Hoard counters above their heads — all enormous. Grix's is tiny. The disparity is visceral.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "narrator", "150 million. 4 billion. The numbers float above their heads like halos. Yours reads: 12,400." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 0, 15
				),

				new Scene( "4_factory", "The Conveyor Belt",
					"Smart contract templates roll down a conveyor belt. Stamp them with different project names. Ship. Gold pours in. Cookie-clicker dopamine. Deliberately hollow.",
					GameplayType.FactoryAutomation,
					new DialogueLine[] {
						new( "internal", "COPY PREVIOUS CONTRACT? Y/N. I stopped pressing N after the third one. The gold was the same either way.", true ),
						new( "narrator", "The factory noise is numbing. The counter climbs. It feels amazing and it means nothing. Both things are true." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 15000, 30
				),

				new Scene( "4_networking", "The Social Dungeon",
					"Conference after-parties as a social dungeon crawler. Each room is a conversation. Match keywords to unlock deeper connections.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "narrator", "Governance. Scalability. Paradigm shift. The words are keys. The doors open to rooms with more doors." ),
						new( "pierrot", "I'm working the governance angle. These people are insane. I love it." ),
					},
					Array.Empty<Choice>(),
					new[] { "pierrot:warm" }, 2000, 10
				),
			},
			"Arpeggiator"
		),

		// ─────────────────────────────────────
		// CHAPTER 5: THE GOLDEN TICKET
		// ─────────────────────────────────────
		new ChapterData(
			5, "THE GOLDEN TICKET", "The $2.5M offer. The train. The layoffs. The empty apartment.",
			"2018", 65, ChapterTone.Thriller, GameplayType.CorporateManagement, MusicStyle.CorporateThriller,
			"A message scroll arrives: $2.5M ACQUISITION OFFER. The Hoard counter EXPLODES.",
			"140 names. 140 faces. The Hoard doesn't care who fills it.",
			new Scene[]
			{
				new Scene( "5_the_offer", "The Message",
					"In class. A message scroll: $2.5M ACQUISITION OFFER. Confetti. Particle effects. Sound design goes nuts.",
					GameplayType.CorporateManagement,
					new DialogueLine[] {
						new( "narrator", "2.5 MILLION GOLD PIECES. The Hoard counter has never looked like this. Confetti rains. You feel like a king." ),
					},
					new Choice[] {
						new( "Take the midterm", "You finish the exam. The deal waits. You're still a student. For now.", "", 0, -5, "took_midterm" ),
						new( "Skip for the deal", "You walk out of the exam hall. The professor watches you go. You don't look back.", "", 0, 10, "skipped_midterm" ),
					},
					Array.Empty<string>(), 2500000, 20
				),

				new Scene( "5_train", "The Train to Goblin York",
					"Herr Goldtusk, the Swiss investor. Verbal sparring mini-game on a train as the city skyline grows in the window.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "goldtusk", "Tell me, young goblin — what is your coin actually worth? Not the chart. The truth." ),
					},
					Array.Empty<Choice>(),
					new[] { "goldtusk:warm" }, 0, 10
				),

				new Scene( "5_cambric", "The Dark Room",
					"The Cambric Weavers. A creepy side meeting. Shadowy goblins. Data-and-voting ICO. The room is literally darker.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "cambric_weavers", "We have a proposal. Regarding... data. And elections. Nothing illegal. Mostly." ),
					},
					new Choice[] {
						new( "Engage", "The gold is good. The feeling is bad. A 'Compromise' marker appears on the Circle.", "cambric_weavers:+5", 50000, 15, "engaged_cambric" ),
						new( "Walk away", "You leave the dark room. The light in the hallway feels earned.", "", 0, -5, "rejected_cambric" ),
					},
					Array.Empty<string>(), 0, 0
				),

				new Scene( "5_pierrot_fork", "The Divergence",
					"Pierrot announces he's staying on the governance path. He won't come to Toronto. The first Circle separation not caused by death.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "pierrot", "You're chasing their gold now, Grix. I'm trying to build something that doesn't need gold." ),
						new( "internal", "That's easy to say when you're not the one who lost a house.", true ),
					},
					Array.Empty<Choice>(),
					new[] { "pierrot:fading" }, 0, 15
				),

				new Scene( "5_layoffs", "140 Names",
					"The darkest gameplay. Fire employees one by one. Each has a name, a portrait, a family. Click TERMINATE. There is no way to save them all.",
					GameplayType.CorporateManagement,
					new DialogueLine[] {
						new( "narrator", "Employee #12: Has two kids. Employee #47: Just bought a house. Employee #89: Moved from overseas for this job." ),
						new( "narrator", "Click TERMINATE. The portrait vanishes. The Hoard doesn't flinch." ),
						new( "internal", "I came from nothing. Now I'm the one taking everything from people who have nothing. The Hoard doesn't care who fills it.", true ),
					},
					Array.Empty<Choice>(), // No choice — you must fire them
					Array.Empty<string>(), 0, 40 // Massive grind
				),

				new Scene( "5_airbnb", "The Empty Apartment",
					"After hours. Alone. A DRINK button appears. Each press fills the Grind Gauge and blurs the screen. The gold counter is enormous. The Circle is dim.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "narrator", "The apartment costs more per night than Employee #47's monthly mortgage. You press DRINK. The screen blurs. You press it again." ),
					},
					new Choice[] {
						new( "Keep drinking", "The Grind Gauge fills. The screen desaturates. The gold counter glows like a dying star.", "", 0, 25, "drank_alone" ),
						new( "Call Mom", "She picks up on the first ring. She always does. The Circle portrait glows, faintly, through the blur.", "grix_mother:+10", 0, -15, "called_mom" ),
					},
					Array.Empty<string>(), 0, 0
				),

				new Scene( "5_thotmas", "The Phantom",
					"A message scroll arrives from an NPC you've never met in person. Thotmas — brilliant, efficient, entirely virtual. His portrait is a silhouette.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "thotmas", "(message scroll) I have a project. You have skills. The numbers work. We should talk." ),
						new( "internal", "I couldn't see his face. That should have been the first warning.", true ),
					},
					Array.Empty<Choice>(),
					new[] { "thotmas:warm" }, 0, 5
				),
			},
			"Piano + strings"
		),

		// ─────────────────────────────────────
		// CHAPTER 6: THE BIG SHOW
		// ─────────────────────────────────────
		new ChapterData(
			6, "THE BIG SHOW", "E3. The carnival. The bear.",
			"2018-2019", 35, ChapterTone.Carnival, GameplayType.ExpoManagement, MusicStyle.Carnival,
			"The only crypto company at the biggest game expo. A goblin carnival in a convention hall.",
			"CRYPTO WINTER. Population: you.",
			new Scene[]
			{
				new Scene( "6_expo", "The Booth",
					"TokenForge at E3. Booth next to the Harmonia Engine (Unity). Attract visitors, give pitches, compete for foot traffic.",
					GameplayType.ExpoManagement,
					new DialogueLine[] {
						new( "narrator", "Your booth has a cardboard sign. The booth next to you has holograms. You have passion. They have budget." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 5000, 15
				),

				new Scene( "6_cryptocritters", "The NFT Craze",
					"CryptoCritters era. Breed, trade, flip digital creatures. Brief. Fun. Shallow.",
					GameplayType.Tycoon,
					new DialogueLine[] {
						new( "narrator", "The creatures are adorable. The prices are absurd. Both things fuel each other." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 8000, 10
				),

				new Scene( "6_the_bear", "The Bear",
					"A literal bear crashes through the expo hall. Not a market metaphor — an actual massive bear. (It IS a metaphor. But it's also a bear.)",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "narrator", "The gold counter starts ticking DOWN. Coins fly away from you. The music cuts. NPCs run. The carnival is over." ),
						new( "narrator", "CRYPTO WINTER. Population: you." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), -50000, 30
				),
			},
			"Calliope organ"
		),

		// ─────────────────────────────────────
		// CHAPTER 7: BONES AND DICE
		// ─────────────────────────────────────
		new ChapterData(
			7, "BONES AND DICE", "Block production. BetBones. The globe. The burn.",
			"2019-2020", 65, ChapterTone.Noir, GameplayType.CasinoTycoon, MusicStyle.JazzNoir,
			"The ECHOES chain. A gambling platform. $200M per week. Then: nothing.",
			"I spent two years trying to go legit. Turns out 'legit' is just 'criminal' with better stationery.",
			new Scene[]
			{
				new Scene( "7_echoes", "Block Producer",
					"ECHOES chain governance sim — vote on proposals, manage infrastructure, play politics.",
					GameplayType.Tycoon,
					new DialogueLine[] {
						new( "narrator", "Block production. The most boring way to print money. Until BetBones." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 20000, 15
				),

				new Scene( "7_betbones", "The Rain",
					"BetBones launches. $200M/week volume. The screen is literally raining gold. Maximum dopamine.",
					GameplayType.CasinoTycoon,
					new DialogueLine[] {
						new( "narrator", "Two hundred million per week. The gold doesn't rain. It POURS. The screen can barely contain it." ),
						new( "internal", "I watched the counter and felt nothing. That's when I should have been scared.", true ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 500000, 25
				),

				new Scene( "7_betrayal", "The Screw",
					"A contract clause you missed. The BetBones founder reveals the betrayal. The gold flow STOPS. Coins suspended in air, then shatter.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "narrator", "The rain stops mid-animation. Coins hang in the air for one terrible second. Then they all fall. And shatter on the ground." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), -200000, 35
				),

				new Scene( "7_licensing", "The World Tour",
					"Malta, Fiji, Macau, Hong Kong, Isle of Man. Each location a legitimacy dungeon — bureaucratic puzzles. Every regulator is on the take.",
					GameplayType.HackingPuzzle,
					new DialogueLine[] {
						new( "narrator", "Malta: a fortress of golden stone. Macau: a neon cavern. Fiji: tropical goblin paradise. Each one a different flavor of 'give us your money.'" ),
						new( "internal", "I spent two years trying to go legit. Turns out 'legit' is just 'criminal' with better stationery.", true ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), -2500000, 40
				),
			},
			"Saxophone"
		),

		// ─────────────────────────────────────
		// CHAPTER 8: ROCK BOTTOM
		// ─────────────────────────────────────
		new ChapterData(
			8, "ROCK BOTTOM", "DungeonDash. COVID. Mom's house. DeFi Summer.",
			"2020", 35, ChapterTone.Desperate, GameplayType.DeliveryDriving, MusicStyle.SparsePiano,
			"The shortest chapter with the biggest emotional swing.",
			"That's either the future of finance or the most elaborate lie in history. Probably both.",
			new Scene[]
			{
				new Scene( "8_dungeondash", "Delivering Food",
					"Drive a beat-up cart through empty plague streets. Deliver food. Earn copper pieces. The same cha-ching sound plays for 3 gold that once played for 3 million.",
					GameplayType.DeliveryDriving,
					new DialogueLine[] {
						new( "narrator", "The Hoard counter, which once showed millions, now ticks up in single digits. The same satisfying cha-ching. For 3 gold. The sound doesn't know the difference." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 50, 30
				),

				new Scene( "8_home_again", "Mom's House",
					"The family home from Chapter 1 — smaller, sadder. Muted colors. Mom still cooks. The Circle portraits of family glow, faintly.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "grix_mother", "You can stay as long as you need, mijo." ),
						new( "internal", "She said the same thing in 2008. I'm twenty-something years old and my mom is still catching me when I fall. When do I start catching her?", true ),
					},
					Array.Empty<Choice>(),
					new[] { "grix_mother:warm" }, 0, -20 // Home depletes grind
				),

				new Scene( "8_defi_summer", "The Gamble",
					"DeFi Summer arrives. UNISWAMP FORK AVAILABLE. All-in or play safe. Liquidity pool management — fast, addictive, terrifying.",
					GameplayType.DeFiGambling,
					new DialogueLine[] {
						new( "narrator", "ANNUAL PERCENTAGE YIELD: 40,000%. That's either the future of finance or the most elaborate lie in history." ),
						new( "internal", "Probably both.", true ),
					},
					new Choice[] {
						new( "Go all in on DeFi", "Everything you have left. Into the pools. The yield farms. The future, maybe.", "", -1000, 20, "defi_all_in" ),
						new( "Play it safe", "You keep delivering. The yields mock you from your phone screen. But you still have tomorrow.", "", 0, 5, "defi_safe" ),
					},
					Array.Empty<string>(), 0, 0
				),

				new Scene( "8_airdrop", "The Miracle",
					"The Uniswamp Airdrop. Tokens rain from the sky. Free money. The Hoard JUMPS. Divine intervention.",
					GameplayType.DeFiGambling,
					new DialogueLine[] {
						new( "narrator", "Tokens rain from the sky. Free. Actual free money. The Hoard JUMPS. You scream. The screen erupts. It feels like divine intervention." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 100000, -10
				),
			},
			"Solo piano"
		),

		// ─────────────────────────────────────
		// CHAPTER 9: THE COMEBACK KID
		// ─────────────────────────────────────
		new ChapterData(
			9, "THE COMEBACK KID", "Crypto Twitter. Head of DeFi. Chief Burnweed.",
			"2020-2021", 50, ChapterTone.Triumphant, GameplayType.SocialMedia, MusicStyle.HypeBeast,
			"The return. The timeline. The clout. The unraveling.",
			"What if the company was a vibe? Can we vote on that?",
			new Scene[]
			{
				new Scene( "9_crypto_twitter", "The Timeline",
					"Grix becomes a Crypto Twitter personality. Write Goblin Tweets. Optimize for engagement. The Reputation meter SOARS.",
					GameplayType.SocialMedia,
					new DialogueLine[] {
						new( "narrator", "Followers: 100. 1,000. 10,000. Each notification is a hit. The timeline is a slot machine and you keep winning." ),
						new( "internal", "I was building a persona. The persona was building a prison. Same bars, different gold.", true ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 10000, 20
				),

				new Scene( "9_head_of_defi", "The Revival",
					"Head of DeFi at a dying L1 blockchain from 2018. Revive the chain — launch GameFi, design tokenomics, attract liquidity.",
					GameplayType.Tycoon,
					new DialogueLine[] {
						new( "narrator", "The chain was dead. You brought it back. Numbers went up. It felt heroic. It felt real." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 50000, 15
				),

				new Scene( "9_burnweed", "The Prophet Returns",
					"Burnweed goes to the Flaming Goblin Festival. Comes back speaking only in governance proposals.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "burnweed", "I propose we decentralize the concept of proposals." ),
						new( "burnweed", "What if the company was a vibe? Can we vote on that?" ),
						new( "narrator", "Meetings become governance theater. Nothing ships. Your comeback unravels because one goblin ate too many mushrooms in the desert." ),
					},
					Array.Empty<Choice>(),
					new[] { "burnweed:warm" }, -20000, 25
				),
			},
			"808s"
		),

		// ─────────────────────────────────────
		// CHAPTER 10: LOST IN THE SAUCE
		// ─────────────────────────────────────
		new ChapterData(
			10, "LOST IN THE SAUCE", "SafeStar. Miami. The penthouse. The rot.",
			"2021", 50, ChapterTone.Seductive, GameplayType.PartySim, MusicStyle.MiamiBass,
			"This is the chapter where the anti-power fantasy turns the knife.",
			"The penthouse has fourteen rooms and I'm in none of them.",
			new Scene[]
			{
				new Scene( "10_safestar", "The Phantom's Token",
					"Thotmas reveals himself as SafeStar's creator. Offers $1M gold for the SafeStar wallet. The deal tracker turns green.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "thotmas", "(message scroll) The wallet is ready. The community is ready. Are you ready for a million gold?" ),
						new( "narrator", "Milestones pass. Payment doesn't come. The tracker turns yellow. Then red. But the ASSOCIATION keeps you in the spotlight." ),
					},
					Array.Empty<Choice>(),
					new[] { "thotmas:fading" }, 50000, 15
				),

				new Scene( "10_miami", "The Party",
					"Neon goblin paradise. Penthouses, yacht parties, celebrity NPCs. Dave Coinboy doing pizza reviews on a gold-plated yacht. Every 19-year-old thinks this is the peak. That's the trap.",
					GameplayType.PartySim,
					new DialogueLine[] {
						new( "dave_coinboy", "GoldCoin is DEAD. Wait no it's ALIVE. Wait — who are you again?" ),
						new( "narrator", "The vibes are immaculate. The music is fire. The gold is flowing. Check the Circle. Every portrait except the Miami crew is dim." ),
					},
					new Choice[] {
						new( "Call home", "Mom picks up. She's happy to hear your voice. The party continues without you. It doesn't notice.", "grix_mother:+10", 0, -10, "called_home_miami" ),
						new( "Stay at the party", "Another drink. Another deal. Another face you won't remember. The Circle dims. The Hoard glows.", "", 5000, 15, "stayed_at_party" ),
					},
					Array.Empty<string>(), 20000, 20
				),

				new Scene( "10_cryptolair", "The Worse Deal",
					"Building for Bogan Tall's CryptoLair. Obviously a scam. The money is promised. The code is junk. The products hurt real people.",
					GameplayType.FactoryAutomation,
					new DialogueLine[] {
						new( "bogan_tall", "The community loves me. I love the community. Can someone tell me what a blockchain is?" ),
						new( "internal", "The factory conveyor belt is back. But the products falling off it are broken. And they're hurting real goblins buying NFTs with their savings.", true ),
					},
					Array.Empty<Choice>(),
					new[] { "bogan_tall:warm" }, 30000, 30
				),

				new Scene( "10_balcony", "3 AM",
					"Miami balcony. Alone. The silence after the bass. The anti-power fantasy delivers its payload.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "internal", "The penthouse has fourteen rooms and I'm in none of them. I'm on a balcony looking at a city that doesn't know my name, holding a phone that won't ring with a call I actually want.", true ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 0, 25 // Deep grind
				),
			},
			"Bass + silence"
		),

		// ─────────────────────────────────────
		// CHAPTER 11: THE GLASS TOWER
		// ─────────────────────────────────────
		new ChapterData(
			11, "THE GLASS TOWER", "The Soho Penthouse. The Blinkleboss Twins. The decision.",
			"2021-2022", 35, ChapterTone.Hollow, GameplayType.WalkingSim, MusicStyle.AmbientDrone,
			"A million-gold penthouse. Every room echoes. The kitchen has never been used.",
			"I've never had a dentist.",
			new Scene[]
			{
				new Scene( "11_penthouse", "The Echo",
					"Grix's penthouse in Goblin York. Huge, detailed, EMPTY. Walk through it. Every room echoes. A single pizza box on a marble countertop worth more than his childhood home.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "narrator", "The penthouse is 3,000 square feet of silence. The marble echoes your footsteps back at you. There's no one else to make sound." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 0, 20
				),

				new Scene( "11_blinkleboss", "The Elevator",
					"The Blinkleboss Twins. Two identical ogres. Synchronized sentences about institutional adoption.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "blinkleboss_a", "Institutional adoption is inevitable. We own the building." ),
						new( "blinkleboss_b", "...inevitable. We also own the building next door." ),
					},
					Array.Empty<Choice>(),
					new[] { "blinkleboss_a:warm", "blinkleboss_b:warm" }, 0, 10
				),

				new Scene( "11_decision", "The Fork",
					"CONTINUE crypto or TAKE A NORMIE JOB. VP of DeFi — salary, benefits, health insurance. For the first time, 'responsible' is genuinely appealing.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "internal", "I'm 24 years old. I've never had health insurance. I've had a penthouse, a Lamborghini, and a meeting with everyone who matters in crypto. But I've never had a dentist.", true ),
					},
					new Choice[] {
						new( "Take the normie job", "Salary. Benefits. A W-2 with your real name on it. The Hoard slows to a steady drip. It feels... stable. Weird. New.", "", 0, -20, "took_normie_job" ),
						new( "Stay in crypto", "The gold flows faster. The Circle dims faster. Both things are true.", "", 50000, 15, "stayed_crypto" ),
					},
					Array.Empty<string>(), 0, 0
				),
			},
			"Reverb"
		),

		// ─────────────────────────────────────
		// CHAPTER 12: THE NORMIE
		// ─────────────────────────────────────
		new ChapterData(
			12, "THE NORMIE", "The W-2. Terra. FTX. SVB. Survival.",
			"2022-2023", 65, ChapterTone.Thriller, GameplayType.CrisisManagement, MusicStyle.ClinicalElectronic,
			"Grix's first real job. Signing a contract. Getting a company email. Setting up direct deposit. It feels... stable.",
			"18 months of constant crisis. Even the victory feels exhausting.",
			new Scene[]
			{
				new Scene( "12_w2", "Direct Deposit",
					"The most understated celebration in the game: signing an employment contract. The Hoard changes from chaotic gold to steady, modest paychecks.",
					GameplayType.CrisisManagement,
					new DialogueLine[] {
						new( "narrator", "The Hoard counter changes texture. No more rain, no explosions. A steady, quiet +2,400 every two weeks. It looks boring. It feels like oxygen." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 2400, -15
				),

				new Scene( "12_terra", "Terra Moona Collapse",
					"The stablecoin death spiral. Watch a peg break in real-time. Triage: pull liquidity, hedge positions. Time pressure extreme.",
					GameplayType.CrisisManagement,
					new DialogueLine[] {
						new( "narrator", "The peg breaks at 2:47 AM. By 3:15 AM, forty billion gold has evaporated. You're managing a treasury through an extinction event." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), -100000, 35
				),

				new Scene( "12_fgx", "FGX Collapse",
					"The exchange disappears overnight. Locked funds. Click WITHDRAW. PROCESSING... forever. A horror sequence.",
					GameplayType.CrisisManagement,
					new DialogueLine[] {
						new( "narrator", "WITHDRAW. PROCESSING... WITHDRAW. PROCESSING... WITHDRAW. ERROR: SERVICE UNAVAILABLE. The doors are closed. Everyone inside is locked in." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), -200000, 40
				),

				new Scene( "12_svv", "Sorcerer's Valley Vault",
					"The bank run. Circle stablecoins depeg. Manage a treasury through a banking crisis.",
					GameplayType.CrisisManagement,
					new DialogueLine[] {
						new( "narrator", "A bank run. In 2023. With 'decentralized' money. The irony would be funny if it weren't happening to your balance sheet." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), -50000, 30
				),

				new Scene( "12_aftermath", "The Survivor",
					"You survived. Made money, even. But the Grind Gauge is maxed. Screen desaturated. Music muffled. Even victory is exhausting.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "narrator", "The Hoard is healthy. The Circle is dim. You optimized for survival and neglected everything else. The game doesn't comment. It just shows the meters." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 75000, 20
				),
			},
			"Alarm tones"
		),

		// ─────────────────────────────────────
		// CHAPTER 13: THE TEMPLE AND THE ELF
		// ─────────────────────────────────────
		new ChapterData(
			13, "THE TEMPLE AND THE ELF", "Thailand. Lyra. The real thing.",
			"2023-2024", 50, ChapterTone.Peaceful, GameplayType.Exploration, MusicStyle.ThaiAmbient,
			"A complete tonal shift. For the first time since Chapter 1, the world feels beautiful without an asterisk.",
			"I don't care about the Hoard. I care about the goblin.",
			new Scene[]
			{
				new Scene( "13_thailand", "The Temple",
					"Lush goblin-temple paradise. Golds, greens, deep blues. Walk through night markets, temples, beaches. No tycoon games. No trading. The Grind Gauge DEPLETES.",
					GameplayType.Exploration,
					new DialogueLine[] {
						new( "narrator", "No objectives. No counters. No timers. Just walk. The Grind Gauge depletes with every step. The screen re-saturates. Color returns to the world." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 0, -40 // Massive grind depletion
				),

				new Scene( "13_lyra_meets", "The Elf",
					"Lyra. The only non-goblin. She was at the NYC conference years ago. He didn't notice. Now, in Thailand, they meet properly.",
					GameplayType.RelationshipSim,
					new DialogueLine[] {
						new( "lyra", "You talk about money like it's oxygen. Like you'd die without it." ),
						new( "internal", "I almost did. More than once.", true ),
						new( "lyra", "That's not the same thing, Grix." ),
					},
					Array.Empty<Choice>(),
					new[] { "lyra:warm" }, 0, -20
				),

				new Scene( "13_lyra_deep", "The Real Conversation",
					"Dialogue-heavy, choice-driven. The conversations are real. Being vulnerable grows the connection. Being guarded protects the Hoard identity.",
					GameplayType.RelationshipSim,
					new DialogueLine[] {
						new( "lyra", "I don't care about the Hoard. I care about the goblin." ),
					},
					new Choice[] {
						new( "Be vulnerable", "You tell her about Fenn. About the house. About the Underbazaar. About all of it. Her portrait blazes.", "lyra:+20", 0, -15, "vulnerable_with_lyra" ),
						new( "Stay guarded", "You tell her the highlight reel. She nods. She knows there's more. She doesn't push. Her portrait glows, but not as bright.", "lyra:+5", 0, 0, "guarded_with_lyra" ),
					},
					Array.Empty<string>(), 0, 0
				),

				new Scene( "13_flashback", "She Was Always There",
					"Lyra mentions the NYC conference. Flashback to Chapter 5 — the same room, the same crowd, but the camera finds her in the background.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "narrator", "The scene replays. The conference. Chapter 5. The same room. But now the camera pans left and there she is. In the background. She was always there. You weren't looking." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 0, 0
				),

				new Scene( "13_building", "The Real Code",
					"Grix starts coding a perp DEX. The coding mini-game returns — cleaner, more mature. He's building something real.",
					GameplayType.StartupManagement,
					new DialogueLine[] {
						new( "internal", "The code felt different this time. Not templates. Not factory stamping. Real architecture. Real purpose. Maybe that's what Thailand did.", true ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 5000, 10
				),
			},
			"Flute + guitar"
		),

		// ─────────────────────────────────────
		// CHAPTER 14: THE TOP OF THE CAVE
		// ─────────────────────────────────────
		new ChapterData(
			14, "THE TOP OF THE CAVE", "The raise. The cofounder. The decay. The loss.",
			"2024-2025", 55, ChapterTone.Devastating, GameplayType.StartupManagement, MusicStyle.BrokenLoveTheme,
			"Everything converges. The finale.",
			"You finally made it, Grix. You're at the top. Look around. Is anyone here?",
			new Scene[]
			{
				new Scene( "14_crimson_mint", "Real Money",
					"The Crimson Mint Syndicate. First real institutional investor. Professional. Fair. Clean. The Hoard grows the right way.",
					GameplayType.StartupManagement,
					new DialogueLine[] {
						new( "crimson_mint", "We invest in builders, not hype. Show us the code." ),
						new( "narrator", "The deal is clean. No rat holes. No schemes. Just building. The manner of growth feels different for the first time." ),
					},
					Array.Empty<Choice>(),
					new[] { "crimson_mint:warm" }, 500000, 10
				),

				new Scene( "14_skrag", "The Cofounder",
					"Skrag clashes with Grix. Conflict management: escalate or de-escalate. For the first time, Grix can handle this like an adult.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "skrag", "We built this together, Grix. Don't forget that when the money starts talking." ),
					},
					new Choice[] {
						new( "De-escalate", "Handshake. Mutual respect. A quiet achievement: GROWN UP. Skrag's portrait dims but doesn't crack.", "skrag:fading", -50000, -5, "deescalated_skrag" ),
						new( "Escalate", "Bitter split. Legal costs. The old pattern repeats. The Hoard bleeds from lawyer fees.", "skrag:cracked", -150000, 20, "escalated_skrag" ),
					},
					new[] { "skrag:warm" }, 0, 0
				),

				new Scene( "14_decay", "The Relationship Meter",
					"Every late night, every skipped call, every 'just one more sprint' — the meter drops. The game forces the choice: Hoard or Circle. There is no way to max both.",
					GameplayType.StartupManagement,
					new DialogueLine[] {
						new( "narrator", "The DEX needs you. Lyra needs you. The investor call is at 9 PM — the same time you promised to call her. The game is rigged. Just like real life." ),
					},
					new Choice[] {
						new( "Prioritize Lyra", "You close the laptop. You call her. The metrics dip. The investors worry. But her portrait blazes.", "lyra:+15", -50000, -10, "prioritized_lyra" ),
						new( "Prioritize the DEX", "One more sprint. One more call. She doesn't pick up anymore. The Hoard glows. The portrait fades.", "lyra:-20", 100000, 15, "prioritized_dex" ),
					},
					Array.Empty<string>(), 0, 0
				),

				new Scene( "14_the_loss", "The Loss",
					"Lyra leaves. If you prioritized her, it's gentle and later. If you neglected her, it's abrupt. But she leaves. The fixed point. The story is autobiographical.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "lyra", "You finally made it, Grix. You're at the top. Look around. Is anyone here?" ),
						new( "narrator", "Lyra's portrait goes dark. Not cracked — dark. She's not dead. She's just gone. And that's worse." ),
					},
					Array.Empty<Choice>(),
					new[] { "lyra:dark" }, 0, 50 // Maximum grind
				),

				new Scene( "14_hong_kong", "The Rooftop",
					"The final scene. Hong Kong rooftop. Neon megalopolis below. Hoard highest ever. Reputation maxed. The Circle is almost empty.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "narrator", "The city stretches below. The gold counter ticks upward. Nobody is watching." ),
						new( "narrator", "Hold for ten seconds. Fade to black." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 0, 0
				),
			},
			"Detuned melody"
		),

		// ─────────────────────────────────────
		// EPILOGUE: THE BARGAIN
		// ─────────────────────────────────────
		new ChapterData(
			99, "THE BARGAIN", "The Goblin King. The Chain. The Choice.",
			"Timeless", 10, ChapterTone.Reckoning, GameplayType.DialogueRPG, MusicStyle.AppalachianFolk,
			"After credits. The Appalachian woods. Night. The same spot from Chapter 2.",
			"What am I actually building my life around?",
			new Scene[]
			{
				new Scene( "ep_woods", "The Return",
					"The Appalachian woods from Chapter 2. Night. The same spot. But now he's older, richer, and more alone.",
					GameplayType.WalkingSim,
					new DialogueLine[] {
						new( "narrator", "The same woods. The same spot. The WiFi signal from the library still reaches here. Everything is the same. You are not." ),
					},
					Array.Empty<Choice>(),
					Array.Empty<string>(), 0, 0
				),

				new Scene( "ep_king", "The Goblin King",
					"An ancient, gnarled creature. The thesis made flesh.",
					GameplayType.DialogueRPG,
					new DialogueLine[] {
						new( "goblin_king", "You took my bargain, boy. Gold for blood. Fortune for friends. Fame for love. The terms were clear. You just didn't read the contract." ),
						new( "internal", "I didn't sign a contract.", true ),
						new( "goblin_king", "Everyone signs. They just call it 'ambition.' Come. Join the chain." ),
						new( "narrator", "He gestures to a chain of goblins — an infinite line of creatures, each clutching gold, each alone, stretching into darkness. THE GOBLIN CHAIN." ),
					},
					new Choice[] {
						new( "JOIN THE CHAIN",
							"Grix takes his place. The camera zooms out — the Goblin Chain stretches across a world map. This transitions directly into multiplayer. You ARE a goblin in the chain. The satire is the gameplay.",
							"", 0, 0, "joined_chain" ),
						new( "WALK AWAY",
							"Grix turns around. Through the trees, a warm light: the family home. Mom on the porch. The Hoard ticks down with each step. The Circle brightens. 'Come inside, mijo. It's cold.'",
							"grix_mother:+50", -999999, -100, "walked_away" ),
					},
					Array.Empty<string>(), 0, 0
				),
			},
			"Acoustic guitar"
		),
	};

	// ═══════════════════════════════════════
	//  LOOKUP
	// ═══════════════════════════════════════

	public static ChapterData GetChapter( int number )
	{
		foreach ( var ch in AllChapters )
			if ( ch.Number == number ) return ch;
		return AllChapters[0];
	}

	public static int[] GetChapterNumbers()
	{
		var nums = new int[AllChapters.Length];
		for ( int i = 0; i < AllChapters.Length; i++ )
			nums[i] = AllChapters[i].Number;
		return nums;
	}
}
