using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexosHub.ERP.VarejOnline.Domain.Services
{
    public class SyncProcessJobService : ISyncProcessJobService
    {
        public const string SyncStockContinuousJobId = "SyncStockContinuousJob";
        public SyncProcessJobService()
        {
        }
    }
}
