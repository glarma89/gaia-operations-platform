
using Xunit;

namespace Gaia.Tests.Integration;

[CollectionDefinition("DatabaseIntegration")]
public sealed class DatabaseTestCollection
    : ICollectionFixture<GaiaApiFactory>
{
}
