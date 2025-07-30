using System;

namespace LexosHub.ERP.VarejOnline.Infra.Data.Migrations.Models.Base
{
    public abstract class EntityBase
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
