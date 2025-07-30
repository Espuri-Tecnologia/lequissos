using LexosHub.ERP.VarejOnline.Infra.Data.Migrations.Models.Integration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data;

namespace LexosHub.ERP.VarejOnline.Infra.Data.Migrations.Context
{
    public interface IAppDbContext
    {
        public IDbConnection Connection { get; }
        DatabaseFacade Database { get; }

        public DbSet<Integration> Integration { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);


    }
}
