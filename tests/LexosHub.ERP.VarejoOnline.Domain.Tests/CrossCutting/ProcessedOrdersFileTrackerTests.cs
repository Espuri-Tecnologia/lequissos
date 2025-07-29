using System;
using System.IO;
using LexosHub.ERP.VarejoOnline.Infra.CrossCutting.Default;
using Xunit;

namespace LexosHub.ERP.VarejoOnline.Domain.Tests.CrossCutting
{
    public class ProcessedOrdersFileTrackerTests
    {
        [Fact]
        public void MarkProcessed_ShouldPersistAndAvoidDuplicates()
        {
            var path = Path.GetTempFileName();
            try
            {
                var tracker = new ProcessedOrdersFileTracker(path);
                tracker.MarkProcessed("123");
                tracker.MarkProcessed("123");
                tracker.MarkProcessed("456");

                Assert.True(tracker.IsProcessed("123"));
                Assert.True(tracker.IsProcessed("456"));
                Assert.False(tracker.IsProcessed("789"));

                var lines = File.ReadAllLines(path);
                Assert.Equal(2, lines.Length);

                // create a new instance to ensure persistence
                var tracker2 = new ProcessedOrdersFileTracker(path);
                Assert.True(tracker2.IsProcessed("123"));
                Assert.True(tracker2.IsProcessed("456"));
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
