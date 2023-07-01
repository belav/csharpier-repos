using System;

namespace Microsoft.AspNetCore.Identity;

/// <summary>
/// Used to indicate that a something is considered personal data.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class PersonalDataAttribute : Attribute { }
