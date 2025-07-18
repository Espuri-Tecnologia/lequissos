using LexosHub.ERP.VarejoOnline.Infra.Data.Migrations.Models.Base;
using LexosHub.ERP.VarejoOnline.Infra.Data.Migrations.Models.Integration;
using LexosHub.ERP.VarejoOnline.Infra.Data.Migrations.Models.Webhook;
using LexosHub.ERP.VarejoOnline.Infra.Data.Migrations.Models.SyncProcess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Reflection;

namespace LexosHub.ERP.VarejoOnline.Infra.Data.Migrations.Context
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Integration> Integration => Set<Integration>();
        public DbSet<Webhook> Webhook => Set<Webhook>();
        public DbSet<SyncProcess> SyncProcess => Set<SyncProcess>();
        public IDbConnection Connection => Database.GetDbConnection();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder();
                builder.SetBasePath(Directory.GetCurrentDirectory());
                builder.AddJsonFile("migrationsettings.json");
                IConfiguration configuration = builder.Build();

                optionsBuilder.UseSqlServer(configuration.GetConnectionString("ErpDBConn"));
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            AddTimestamps();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.State is EntityState.Added or EntityState.Modified);

            foreach (var entity in entities)
            {
                var now = DateTime.Now;

                if (entity.State == EntityState.Added)
                {
                    ((EntityBase)entity.Entity).CreatedDate = now;
                }
                ((EntityBase)entity.Entity).UpdatedDate = now;
            }
        }

        public void DetachAllEntities() //Salvar para a posteridade
        {
            var changedEntriesCopy = this.ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Modified or EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }
    }
}
