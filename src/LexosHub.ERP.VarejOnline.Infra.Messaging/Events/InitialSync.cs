using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Events
{
    public class InitialSync : BaseEvent
    {
        public string HubKey { get; set; } = null!;
    }
}
