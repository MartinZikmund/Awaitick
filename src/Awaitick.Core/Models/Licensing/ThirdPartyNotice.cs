namespace Awaitick.Core.Models.Licensing;

/// <summary>
/// A single third-party component shipped with Awaitick.
/// </summary>
public sealed record ThirdPartyNotice(string Name, string License, string? Copyright = null, string? Url = null);

/// <summary>
/// Components that share one licence.
/// </summary>
public sealed record ThirdPartyNoticeGroup(string License, string? Url, IReadOnlyList<string> Components)
{
	public string ComponentsText => string.Join(", ", Components);
}
