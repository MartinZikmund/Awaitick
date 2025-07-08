using System.Text.Json.Serialization;

namespace Awaitick.Core.Models;

[JsonSerializable(typeof(EventCountdown))]
[JsonSerializable(typeof(EventCountdown[]))]
[JsonSerializable(typeof(List<EventCountdown>))]
public partial class EventCountdownSerializerContext : JsonSerializerContext
{
}
