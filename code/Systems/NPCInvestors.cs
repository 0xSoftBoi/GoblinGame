using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// 20 NPC retail investor bots that browse GoblinTwitter, buy/sell tokens,
/// and create organic market volume. Essential when playing with fewer than 8 humans.
/// Runs on host only.
/// </summary>
public sealed class NPCInvestors : Component
{
	public static NPCInvestors Instance { get; private set; }

	[Property] public int NPCCount { get; set; } = 50;
	[Property] public float TickInterval { get; set; } = 3f; // How often NPCs act
	[Property] public float BaseBuyChance { get; set; } = 0.3f;
	[Property] public float BaseSellChance { get; set; } = 0.15f;
	[Property] public float FOMOMultiplier { get; set; } = 2.0f; // Buy chance boost from trending
	[Property] public float FUDMultiplier { get; set; } = 2.5f; // Sell chance boost from FUD

	private List<NPCBot> _bots = new();
	private float _tickTimer;
	private Random _rng = new();

	public class NPCBot
	{
		public string Name;
		public string Handle;
		public float Balance;
		public Dictionary<Guid, float> Holdings = new(); // tokenId -> amount
		public float Greed; // 0-1: higher = more likely to buy
		public float Fear;  // 0-1: higher = more likely to panic sell
		public float Gullibility; // 0-1: higher = more affected by shills
		public float LastActionTime;

		public NPCBot( float startingBalance )
		{
			var rng = new Random();
			Name = NPCNames.Generate();
			Handle = NPCNames.GenerateHandle();
			Balance = startingBalance;
			Greed = (float)rng.NextDouble();
			Fear = (float)rng.NextDouble();
			Gullibility = 0.3f + (float)rng.NextDouble() * 0.7f; // NPCs are gullible (0.3-1.0)
		}
	}

	protected override void OnStart()
	{
		Instance = this;

		if ( IsProxy ) return;

		// Spawn NPC bots
		for ( int i = 0; i < NPCCount; i++ )
		{
			float startBalance = 200f + (float)_rng.NextDouble() * 800f; // 200-1000 GBC each
			_bots.Add( new NPCBot( startBalance ) );
		}

		Log.Info( $"Spawned {NPCCount} NPC retail investors" );
	}

	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;

		_tickTimer += Time.Delta;
		if ( _tickTimer < TickInterval ) return;
		_tickTimer -= TickInterval;

		// Each tick, a random subset of NPCs act
		int actorsThisTick = 5 + _rng.Next( 10 ); // 5-14 NPCs act per tick
		for ( int i = 0; i < actorsThisTick && i < _bots.Count; i++ )
		{
			var bot = _bots[_rng.Next( _bots.Count )];
			NPCAction( bot );
		}
	}

	private void NPCAction( NPCBot bot )
	{
		var tokenSystem = Scene.GetAllComponents<TokenSystem>().FirstOrDefault();
		var twitter = Scene.GetAllComponents<GoblinTwitter>().FirstOrDefault();
		if ( tokenSystem is null ) return;

		// Get available tokens
		var tokens = tokenSystem.GetActiveTokensSorted();
		if ( tokens.Count == 0 ) return;

		// Check trending posts for FOMO/FUD signals
		float fomoSignal = 0f;
		float fudSignal = 0f;
		if ( twitter is not null )
		{
			var trending = twitter.GetTrending();
			fomoSignal = trending.Count > 0 ? 0.3f : 0f;

			// Count FUD replies in recent posts
			var recentPosts = twitter.GetRecentPosts( 10 );
			int fudCount = recentPosts.Sum( p => p.FudReplies );
			fudSignal = MathF.Min( fudCount * 0.1f, 0.5f );
		}

		// Pick a random token to consider
		var targetToken = tokens[_rng.Next( tokens.Count )];
		if ( targetToken.IsRugged ) return; // Don't buy rugged tokens

		// Decision: buy, sell, or post on Twitter
		float buyChance = BaseBuyChance + (bot.Greed * 0.2f) + (fomoSignal * bot.Gullibility * FOMOMultiplier);
		float sellChance = BaseSellChance + (bot.Fear * 0.2f) + (fudSignal * bot.Fear * FUDMultiplier);

		// If token price is rising, NPCs FOMO in
		if ( targetToken.Price > 1.5f )
			buyChance *= 1.5f;

		// If token price is tanking, NPCs panic
		if ( targetToken.Price < 0.5f )
			sellChance *= 2f;

		float roll = (float)_rng.NextDouble();

		if ( roll < buyChance && bot.Balance > 10f )
		{
			// BUY: spend 5-20% of balance
			float spendPct = 0.05f + (float)_rng.NextDouble() * 0.15f;
			float spendAmount = bot.Balance * spendPct;
			float tokensReceived = spendAmount / MathF.Max( targetToken.Price, 0.01f );

			bot.Balance -= spendAmount;

			if ( !bot.Holdings.ContainsKey( targetToken.Id ) )
				bot.Holdings[targetToken.Id] = 0f;
			bot.Holdings[targetToken.Id] += tokensReceived;

			// Apply buy pressure to token
			tokenSystem.ApplyNPCBuyPressure( targetToken.Id, spendAmount );
		}
		else if ( roll < buyChance + sellChance )
		{
			// SELL: dump holdings if we have any
			if ( bot.Holdings.TryGetValue( targetToken.Id, out float held ) && held > 0f )
			{
				float sellPct = 0.3f + (float)_rng.NextDouble() * 0.7f; // Sell 30-100%
				float sellAmount = held * sellPct;
				float proceeds = sellAmount * targetToken.Price;

				bot.Holdings[targetToken.Id] -= sellAmount;
				bot.Balance += proceeds;

				tokenSystem.ApplyNPCSellPressure( targetToken.Id, proceeds );
			}
		}
		else if ( _rng.NextDouble() < 0.25 && twitter is not null )
		{
			// POST on GoblinTwitter (25% chance when not trading)
			NPCPost( bot, twitter, targetToken );
		}
	}

	private void NPCPost( NPCBot bot, GoblinTwitter twitter, TokenSystem.TokenData token )
	{
		string[] bullishPosts = {
			$"just aped into ${token.Ticker} 🚀 this is the one frens",
			$"${token.Ticker} looking BULLISH af rn. not financial advice but also yes",
			$"if you're not buying ${token.Ticker} at these prices you hate money",
			$"${token.Ticker} to the MOON 🌙 my cousin works at the exchange trust me",
			$"went all in on ${token.Ticker}. wife doesn't know yet lol",
			$"${token.Ticker} is the next GoblinCoin. screenshot this.",
			$"BREAKING: insider tells me ${token.Ticker} listing on major exchange soon 👀",
			$"just mortgaged my cave for more ${token.Ticker}. this IS financial advice."
		};

		string[] bearishPosts = {
			$"${token.Ticker} looking like a rug ngl... 🚩",
			$"who's still holding ${token.Ticker}?? couldn't be me 😂",
			$"sold all my ${token.Ticker}. something feels off about this one",
			$"${token.Ticker} devs went quiet... where's the roadmap??",
			$"reminder that ${token.Ticker} has no actual utility. just vibes."
		};

		bool isBullish = bot.Greed > bot.Fear || _rng.NextDouble() > 0.4;
		var posts = isBullish ? bullishPosts : bearishPosts;
		var content = posts[_rng.Next( posts.Length )];

		twitter.AddNPCPost( bot.Handle, token.Ticker, content );
	}

	// --- Public API ---

	/// <summary>
	/// Get total NPC buy volume for display purposes.
	/// </summary>
	public float GetTotalNPCBalance()
	{
		return _bots.Sum( b => b.Balance );
	}

	/// <summary>
	/// Get NPC count holding a specific token.
	/// </summary>
	public int GetHolderCount( Guid tokenId )
	{
		return _bots.Count( b => b.Holdings.ContainsKey( tokenId ) && b.Holdings[tokenId] > 0 );
	}

	/// <summary>
	/// Fires 2-3 NPC hype replies when a post hits the trending threshold (3 likes).
	/// </summary>
	public void TriggerTrendingReaction( string ticker )
	{
		var twitter = Scene.GetAllComponents<GoblinTwitter>().FirstOrDefault();
		if ( twitter is null || _bots.Count == 0 ) return;

		string[] hypeReplies = {
			$"${ticker} suddenly everywhere on my feed 👀",
			$"just aped into ${ticker} after seeing it trend. probably fine",
			$"${ticker} going viral rn. ngmi if you're not in already",
			$"who is buying all this ${ticker}... and why aren't I them",
		};

		int reactorCount = 2 + _rng.Next( 2 );
		var reactors = _bots.OrderBy( _ => _rng.Next() ).Take( reactorCount );
		foreach ( var bot in reactors )
			twitter.AddNPCPost( bot.Handle, ticker, hypeReplies[_rng.Next( hypeReplies.Length )] );
	}

	/// <summary>
	/// Floods GoblinTwitter with NPC rug-pull reactions. Call after a rug pull executes.
	/// </summary>
	public void TriggerRugPullReactions( string ticker )
	{
		var twitter = Scene.GetAllComponents<GoblinTwitter>().FirstOrDefault();
		if ( twitter is null ) return;

		string[] reactions = {
			$"just got RUGGED on ${ticker}. never again 😭",
			$"${ticker} rug confirmed. I trusted these goblins with my life savings",
			$"lmaooo ${ticker} just went to zero. gm everyone except ${ticker} holders",
			$"${ticker} rugged me and now I'm eating rocks for dinner. thanks",
			$"whoever launched ${ticker} is a GOBLIN. a literal goblin. wait.",
			$"SELL ${ticker} omg omg omg oh wait too late",
			$"${ticker}: 0 GBC. my cave: foreclosed. great investment guys",
			$"I KNEW ${ticker} was a rug. (I did not know)",
		};

		int reactorCount = 5 + _rng.Next( 6 ); // 5-10 NPCs react
		var reactors = _bots.OrderBy( _ => _rng.Next() ).Take( reactorCount );
		foreach ( var bot in reactors )
		{
			var content = reactions[_rng.Next( reactions.Length )];
			twitter.AddNPCPost( bot.Handle, ticker, content );
		}
	}
}
