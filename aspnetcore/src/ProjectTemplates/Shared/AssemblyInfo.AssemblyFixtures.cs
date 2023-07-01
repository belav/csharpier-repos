using Microsoft.AspNetCore.Testing;
using Templates.Test.Helpers;
using Xunit;

[assembly: AssemblyFixture(typeof(ProjectFactoryFixture))]
[assembly: CollectionBehavior(DisableTestParallelization = true)]
