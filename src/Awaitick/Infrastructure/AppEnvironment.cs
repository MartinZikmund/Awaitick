namespace Awaitick.Infrastructure;

public static partial class AppEnvironment
{
#if APP_CHANNEL_DEV
	public const bool IsDevChannel = true;
	public const string ChannelLabel = "DEV";
#else
	public const bool IsDevChannel = false;
	public const string ChannelLabel = "";
#endif

	/// <summary>
	/// The channel label, plus the worktree when this build came from one. The generated part of
	/// this class supplies <see cref="WorktreeName"/>.
	/// </summary>
	public static string ChannelBadgeLabel =>
		WorktreeName is { Length: > 0 } worktree ? $"{ChannelLabel} · {worktree}" : ChannelLabel;
}
