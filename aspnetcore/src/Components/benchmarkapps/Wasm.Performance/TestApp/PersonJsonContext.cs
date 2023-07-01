using System.Text.Json.Serialization;

namespace Wasm.Performance.TestApp;

[JsonSerializable(typeof(Person))]
internal sealed partial class PersonJsonContext : JsonSerializerContext { }
