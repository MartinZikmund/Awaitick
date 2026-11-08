using System.Text.Json.Serialization;

namespace Awaitick.Core.Services.Settings;

[JsonSerializable(typeof(ElementTheme))]
public partial class PreferencesSerializerContext : JsonSerializerContext
{
}
