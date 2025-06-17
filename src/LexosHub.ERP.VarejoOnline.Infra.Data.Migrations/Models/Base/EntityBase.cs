using System;

namespace LexosHub.ERP.VarejoOnline.Infra.Data.Migrations.Models.Base
{
    public abstract class EntityBase
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
