# .NET-Serialization
A new serialization engine that provides serialization and deserialization services for complex graphs of objects, preserving references and allowing for polymorphism of trusted types, with a flexible and modular configuration. Works on .NET Standard 2.0.

The default services provide:

 - Reflection-based **constructor**, **property**, and **field** services.
 - Collection services for serializing `IEnumerable<T>`s and deserializing `IList<T>`, **arrays**, and other collections with `IEnumerable<T>` constructors.
 - Object initialization without a constructor, if none is available.
 - Primitive type serializers for `string`, `int`, `uint`, `long`, `ulong`, `float`, `double`, `bool`, `DateTime`, `Guid`, and `Vector2`, as well as the [BassClefStudio.NET](https://github.com/bassclefstudio/.NET-Libraries) primitives `DateTimeZone`, `DateTimeRange`, and `Color`.
 - `IgnoreSerialize` attribute for ignoring properties and fields while serializing.
 - Output `IGraphWriter` implementation for JSON.
 - Dependency injection support with Autofac (optional) with methods like `RegisterGraphSerializer()`, `RegisterDefaultGraphSerializer()` `RegisterGraphService()`, and `RegisterGraphConfiguration()`.

The core `GraphSerializer` also provides native support for **circular references**, **generic types**, **trusted types** (for polymorphism), and **reference preservation** through the use of IDs for objects and a full digraph framework for managing relationships between objects in the model.
