using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http.Features;

[assembly: TypeForwardedTo(typeof(IFeatureCollection))]
[assembly: TypeForwardedTo(typeof(FeatureCollection))]
[assembly: TypeForwardedTo(typeof(FeatureReference<>))]
[assembly: TypeForwardedTo(typeof(FeatureReferences<>))]
