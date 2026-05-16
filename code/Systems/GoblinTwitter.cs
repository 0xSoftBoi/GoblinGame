using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoblinChain;

/// <summary>
/// In-game social media system. Players compose shill posts about tokens,
/// other players interact (like/repost/FUD/report), and engagement drives
/// token prices. The heart of the game.
/// </summary>
public sealed class GoblinTwitter : Component
{
	public static GoblinTwitter Instance { get; private set; }

	// --- Synced State ---
	[Sync] public NetList<PostData> Feed { get; set; } = new();

	// --- Config ---
	[Property] public int MaxFeedSize { get; set; } = 50;
	[Property] public float TrendingBonusBuyPressure { get; set; } = 0.15f;
	[Property] public float LikePriceImpact { get; set; } = 0.005f;
	[Property] public float RepostPriceImpact { get; set; } = 0.015f;
	[Property] public float FudEffectivenessReduction { get; set; } = 0.3f;
	[Property] public float ReportThreshold { get; set; } = 3f;
	[Property] public float SECHeatPerReport { get; set; } = 10f;

	// Track who already interacted with each post (prevent spam)
	private Dictionary<Guid, HashSet<Guid>> _likedBy = new();
	private Dictionary<Guid, HashSet<Guid>> _repostedBy = new();
	private Dictionary<Guid, HashSet<Guid>> _fuddedBy = new();
	private Dictionary<Guid, HashSet<Guid>> _reportedBy = new();

	protected override void OnStart()
	{
		Instance = this;
	}

	// ═══════════════════════════════════════
	//  POST CREATION
	// ═══════════════════════════════════════

	[Rpc.Host]
	public void RequestPost( Guid tokenId, int openerIdx, int claimIdx,
		int proofIdx, int ctaIdx, string customText )
	{
		var caller = Rpc.Caller;
		var player = FindPlayer( caller );
		if ( player is null ) return;

		// Must be during Shill (Trading) phase or Chaos
		var state = GameStateManager.Instance;
		if ( state is not null && state.CurrentPhase != GamePhase.Shill && state.CurrentPhase != GamePhase.Chaos )
		{
			Log.Warning( $"{caller.DisplayName} tried to post outside Shill phase" );
			return;
		}

		// Validate token exists
		var tokenSys = TokenSystem.Instance;
		var token = tokenSys?.GetToken( tokenId );
		if ( token is null || token.Value.IsRugged ) return;

		// Rate limit: max 1 post per 10 seconds per player
		var recentPost = Feed.Where( p => p.AuthorId == player.Id )
			.OrderByDescending( p => p.CreatedAt )
			.FirstOrDefault();
		if ( recentPost.AuthorId == player.Id && Time.Now - recentPost.CreatedAt < 10f )
		{
			Log.Warning( "Posting too fast" );
			return;
		}

		// Truncate custom text
		if ( customText?.Length > 80 )
			customText = customText[..80];

		// Calculate effectiveness
		var rep = player.Components.Get<ReputationTracker>();
		float repMult = rep?.ReputationMultiplier ?? 1f;

		// Check for Rugger shill boost
		var deduction = Scene.GetAllComponents<SocialDeduction>().FirstOrDefault();
		if ( deduction is not null && deduction.IsRugger( player ) )
			repMult *= 1.2f; // Hidden 20% boost

		float effectiveness = ShillTemplates.CalculateEffectiveness(
			openerIdx, claimIdx, proofIdx, ctaIdx, repMult );

		float risk = ShillTemplates.CalculateRisk( openerIdx, claimIdx, proofIdx, ctaIdx );

		string postText = ShillTemplates.AssemblePostText(
			openerIdx, claimIdx, proofIdx, ctaIdx, customText );

		// Create post
		var post = new PostData
		{
			PostId = Guid.NewGuid(),
			AuthorId = player.Id,
			AuthorName = caller.DisplayName,
			TokenId = tokenId,
			TokenTicker = token.Value.Ticker,
			Content = postText,
			Likes = 0,
			Reposts = 0,
			FudReplies = 0,
			Reports = 0,
			ShillPower = effectiveness,
			CreatedAt = Time.Now
		};

		Feed.Add( post );

		// Trim feed
		while ( Feed.Count > MaxFeedSize )
			Feed.RemoveAt( 0 );

		// Initialize interaction tracking
		_likedBy[post.PostId] = new HashSet<Guid>();
		_repostedBy[post.PostId] = new HashSet<Guid>();
		_fuddedBy[post.PostId] = new HashSet<Guid>();
		_reportedBy[post.PostId] = new HashSet<Guid>();

		// Immediate shill pressure on token
		tokenSys?.AddShillPressure( tokenId, effectiveness );

		// SEC heat from risky posts
		if ( risk > 5f )
		{
			var sec = Scene.GetAllComponents<SECSystem>().FirstOrDefault();
			sec?.AddHeat( player, risk );
		}

		BroadcastNewPost( caller.DisplayName, token.Value.Ticker, effectiveness );

		Log.Info( $"POST: @{caller.DisplayName} shills ${token.Value.Ticker} (power: {effectiveness:F1}, risk: {risk:F1})" );
	}

	// ═══════════════════════════════════════
	//  POST INTERACTIONS
	// ═══════════════════════════════════════

	[Rpc.Host]
	public void RequestLike( Guid postId )
	{
		var caller = Rpc.Caller;
		var player = FindPlayer( caller );
		if ( player is null ) return;

		int idx = FindPostIndex( postId );
		if ( idx < 0 ) return;

		// Prevent double-like
		if ( !_likedBy.TryGetValue( postId, out var likers ) )
			_likedBy[postId] = likers = new();
		if ( likers.Contains( player.Id ) ) return;
		likers.Add( player.Id );

		var post = Feed[idx];
		post.Likes++;
		Feed[idx] = post;

		// Price impact
		var tokenSys = TokenSystem.Instance;
		tokenSys?.AddShillPressure( post.TokenId, LikePriceImpact * post.ShillPower );

		// At 3 likes the post is trending — fire NPC hype replies
		if ( post.Likes == 3 && post.TokenTicker?.Length > 0 )
			NPCInvestors.Instance?.TriggerTrendingReaction( post.TokenTicker );
	}

	[Rpc.Host]
	public void RequestRepost( Guid postId )
	{
		var caller = Rpc.Caller;
		var player = FindPlayer( caller );
		if ( player is null ) return;

		int idx = FindPostIndex( postId );
		if ( idx < 0 ) return;

		if ( !_repostedBy.TryGetValue( postId, out var reposters ) )
			_repostedBy[postId] = reposters = new();
		if ( reposters.Contains( player.Id ) ) return;
		reposters.Add( player.Id );

		var post = Feed[idx];
		post.Reposts++;
		Feed[idx] = post;

		// Stronger price impact
		var tokenSys = TokenSystem.Instance;
		tokenSys?.AddShillPressure( post.TokenId, RepostPriceImpact * post.ShillPower );
	}

	[Rpc.Host]
	public void RequestFud( Guid postId )
	{
		var caller = Rpc.Caller;
		var player = FindPlayer( caller );
		if ( player is null ) return;

		int idx = FindPostIndex( postId );
		if ( idx < 0 ) return;

		if ( !_fuddedBy.TryGetValue( postId, out var fudders ) )
			_fuddedBy[postId] = fudders = new();
		if ( fudders.Contains( player.Id ) ) return;
		fudders.Add( player.Id );

		var post = Feed[idx];
		post.FudReplies++;
		// Reduce ongoing shill effectiveness
		post.ShillPower *= (1f - FudEffectivenessReduction);
		Feed[idx] = post;

		// Negative price pressure
		var tokenSys = TokenSystem.Instance;
		tokenSys?.AddShillPressure( post.TokenId, -0.01f * post.ShillPower );
	}

	[Rpc.Host]
	public void RequestReport( Guid postId )
	{
		var caller = Rpc.Caller;
		var player = FindPlayer( caller );
		if ( player is null ) return;

		int idx = FindPostIndex( postId );
		if ( idx < 0 ) return;

		if ( !_reportedBy.TryGetValue( postId, out var reporters ) )
			_reportedBy[postId] = reporters = new();
		if ( reporters.Contains( player.Id ) ) return;
		reporters.Add( player.Id );

		var post = Feed[idx];
		post.Reports++;
		Feed[idx] = post;

		// 3+ reports = SEC heat on author
		if ( post.Reports >= ReportThreshold )
		{
			var author = GoblinPlayer.All
				.FirstOrDefault( p => p.Id == post.AuthorId );
			if ( author is not null )
			{
				var sec = Scene.GetAllComponents<SECSystem>().FirstOrDefault();
				sec?.AddHeat( author, SECHeatPerReport );
			}
		}
	}

	// ═══════════════════════════════════════
	//  NPC POST (called by NPCInvestors)
	// ═══════════════════════════════════════

	public void AddNPCPost( string npcName, string tokenTicker, string content )
	{
		if ( IsProxy ) return;

		var post = new PostData
		{
			PostId = Guid.NewGuid(),
			AuthorId = Guid.Empty, // NPC marker
			AuthorName = npcName,
			TokenId = Guid.Empty,
			TokenTicker = tokenTicker,
			Content = content,
			Likes = 0,
			Reposts = 0,
			FudReplies = 0,
			Reports = 0,
			ShillPower = 0f,
			CreatedAt = Time.Now
		};

		Feed.Add( post );
		while ( Feed.Count > MaxFeedSize )
			Feed.RemoveAt( 0 );
	}

	// ═══════════════════════════════════════
	//  TRENDING
	// ═══════════════════════════════════════

	/// <summary>
	/// Returns top 3 trending tokens by GoblinTwitter engagement.
	/// </summary>
	public List<TrendingToken> GetTrending()
	{
		var tokenScores = new Dictionary<Guid, float>();

		foreach ( var post in Feed )
		{
			if ( post.TokenId == Guid.Empty ) continue;

			float timeSince = MathF.Max( 1f, Time.Now - post.CreatedAt );
			float score = (post.Likes + post.Reposts * 2f + post.FudReplies * 1.5f)
				/ MathF.Sqrt( timeSince );

			if ( !tokenScores.ContainsKey( post.TokenId ) )
				tokenScores[post.TokenId] = 0f;
			tokenScores[post.TokenId] += score;
		}

		var tokenSys = TokenSystem.Instance;
		return tokenScores
			.OrderByDescending( kv => kv.Value )
			.Take( 3 )
			.Select( kv =>
			{
				var token = tokenSys?.GetToken( kv.Key );
				return new TrendingToken(
					kv.Key,
					token?.Ticker ?? "???",
					token?.Name ?? "Unknown",
					token?.Price ?? 0f,
					kv.Value );
			} )
			.ToList();
	}

	public record TrendingToken( Guid TokenId, string Ticker, string Name, float Price, float Score );

	public List<PostData> GetRecentPosts( int count )
		=> Feed.Skip( Math.Max( 0, Feed.Count - count ) ).ToList();

	// ═══════════════════════════════════════
	//  HELPERS
	// ═══════════════════════════════════════

	private int FindPostIndex( Guid postId )
	{
		for ( int i = 0; i < Feed.Count; i++ )
			if ( Feed[i].PostId == postId ) return i;
		return -1;
	}

	private GoblinPlayer FindPlayer( Connection conn )
		=> GoblinPlayer.All
			.FirstOrDefault( p => p.Network.Owner == conn );

	[Rpc.Broadcast]
	private void BroadcastNewPost( string author, string ticker, float power )
	{
		// Subtle notification — don't spam
		if ( power > 6f )
		{
			Log.Info( $"🔥 @{author}'s shill about ${ticker} is FIRE (power: {power:F1})" );
		}
	}
}

// ═══════════════════════════════════════
//  POST DATA (network-serializable)
// ═══════════════════════════════════════

public struct PostData : INetworkSerializable
{
	public Guid PostId;
	public Guid AuthorId;
	public string AuthorName;
	public Guid TokenId;
	public string TokenTicker;
	public string Content;
	public int Likes;
	public int Reposts;
	public int FudReplies;
	public int Reports;
	public float ShillPower;
	public float CreatedAt;

	public bool IsNPC => AuthorId == Guid.Empty;

	public void Read( ref NetRead read )
	{
		PostId = read.Read<Guid>();
		AuthorId = read.Read<Guid>();
		AuthorName = read.Read<string>();
		TokenId = read.Read<Guid>();
		TokenTicker = read.Read<string>();
		Content = read.Read<string>();
		Likes = read.Read<int>();
		Reposts = read.Read<int>();
		FudReplies = read.Read<int>();
		Reports = read.Read<int>();
		ShillPower = read.Read<float>();
		CreatedAt = read.Read<float>();
	}

	public void Write( NetWrite write )
	{
		write.Write( PostId );
		write.Write( AuthorId );
		write.Write( AuthorName );
		write.Write( TokenId );
		write.Write( TokenTicker );
		write.Write( Content );
		write.Write( Likes );
		write.Write( Reposts );
		write.Write( FudReplies );
		write.Write( Reports );
		write.Write( ShillPower );
		write.Write( CreatedAt );
	}
}
