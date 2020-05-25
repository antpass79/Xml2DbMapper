using Microsoft.EntityFrameworkCore;
using Xml2DbMapper.Core;
using Xml2DbMapper.Core.Models;
using Xunit;

namespace Xml2DbMapper.Library.Tests
{
    [Trait("Database", "Lifecycle")]
    public class DatabaseLifecycleTests
    {
        [Fact]
        public void CreateDatabase()
        {
            var options = new DbContextOptionsBuilder<FeaturesContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            using (var databaseLifecycle = new DatabaseLifecycle(options).Scope())
            {
                Assert.True(databaseLifecycle.Created);
            }
        }

        [Fact]
        public void DeleteDatabase()
        {
            var options = new DbContextOptionsBuilder<FeaturesContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            IDatabaseLifecycle databaseLifecycle;
            using (databaseLifecycle = new DatabaseLifecycle(options).Scope())
            {
                Assert.True(databaseLifecycle.Created);
            }

            Assert.False(databaseLifecycle.Created);
        }
    }
}
