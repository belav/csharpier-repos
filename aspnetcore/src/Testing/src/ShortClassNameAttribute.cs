using System;

namespace Microsoft.AspNetCore.Testing;

/// <summary>
/// Used to specify that <see cref="TestFileOutputContext.TestClassName"/> should used the
/// unqualified class name. This is needed when a fully-qualified class name exceeds
/// max path for logging.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
public class ShortClassNameAttribute : Attribute { }
