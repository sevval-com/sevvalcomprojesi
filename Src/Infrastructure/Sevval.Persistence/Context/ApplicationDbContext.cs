using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sevval.Application.Interfaces.IService.Common;
using Sevval.Domain.Entities;
using Sevval.Domain.Entities.Common;
using Sevval.Domain.Enums;
using Sevval.Persistence.Context.sevvalemlak.Models;
using Sevval.Web.Models;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Claims;
using YourProjectName.Models;


namespace Sevval.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, Role, string>, IApplicationDbContext, IDataProtectionKeyContext
    {

        private readonly IHttpContextAccessor _httpContextAccessor;


        public ApplicationDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) :
            base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            Database.ExecuteSqlRaw("PRAGMA foreign_keys=OFF;");
            Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
            Database.ExecuteSqlRaw("PRAGMA busy_timeout=5000;");
        }

        private const int DefaultDecimalPrecision = 18;
        private const int DefaultDecimalScale = 3;
        private const int DefaultDoublePrecision = 18;
        private const int DefaultDoubleScale = 3;
        //private const string SqlForNewGuidAsString = "NEWID()";
        private const string SqlForNewGuidAsString = "lower(substr(hex(randomblob(16)), 1, 8) || '-' ||    substr(hex(randomblob(16)), 9, 4) || '-' ||    substr(hex(randomblob(16)), 13, 4) || '-' ||    substr(hex(randomblob(16)), 17, 4) || '-' ||    substr(hex(randomblob(16)), 21, 12)  )";
 
        private const string SqlForCreatedDateTime = "datetime('now')";
        //private const string SqlForCreatedDateTime = "CAST(GETUTCDATE() AS datetime2(7))";

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        public DbSet<Audit> AuditLogs { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<ForgettenPassword> ForgettenPasswords { get; set; }
        public DbSet<VideolarSayfasi> VideolarSayfasi { get; set; }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<IlanModel> IlanBilgileri { get; set; }
        // Veritabanında tabloları temsil eden DbSet özellikleri
        public DbSet<Sepet> Sepet { get; set; }

        public DbSet<SmsSendHistory> SmsSendHistory { get; set; }
        public DbSet<AfisTalep> AfisTalepler { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<SatisTalep> SatisTalepleri { get; set; }
        public DbSet<Blog> Blogs { get; set; }

        public DbSet<HaftalikBegeniler> HaftalikBegeniler { get; set; }

        public DbSet<HaftalikArama> HaftalikAramalar { get; set; }

        public DbSet<HaftalikGoruntulenme> HaftalikGoruntulenmeler { get; set; }
        public DbSet<VideoLike> VideoLikes { get; set; }
        public DbSet<VideoYorum> VideoYorumlari { get; set; }
        public DbSet<VideoWatch> VideoWatches { get; set; }
        public DbSet<Category> Categories { get; set; }

        public DbSet<BireyselIlanTakibi> BireyselIlanTakipleri { get; set; }
        public DbSet<GununIlanModel> GununIlanlari { get; set; }

        public DbSet<GununIlaniTalep> GununIlaniTalepler { get; set; }


        public DbSet<VideoModel> Videos { get; set; }

        // Toplam ziyaretçi sayısını saklamak için DbSet tanımı
        public DbSet<VisitorCount> VisitorCounts { get; set; }
        public DbSet<Visitor> Visitors { get; set; } // Burayı ekleyin


        //public DbSet<KullaniciYetkisi> KullaniciYetkileri { get; set; }
        //// ... diğer DbSet'ler ...


        // Yeni eklenen DbSet
        public DbSet<AboutUsContent> AboutUsContents { get; set; }

        // Fotoğraflar için DbSet tanımı
        public DbSet<PhotoModel> Photos { get; set; } // Fotoğraflar için DbSet

        public DbSet<YorumModel> Yorumlar { get; set; }

        public DbSet<UserVerification> UserVerifications { get; set; }
        public DbSet<KurumsalRegister> KurumsalRegisters { get; set; }
        public DbSet<ConsultantInvitation> ConsultantInvitations { get; set; }
        public DbSet<MembershipChangeRequest> MembershipChangeRequests { get; set; }
        public DbSet<TempEstateVerification> TempEstateVerifications { get; set; }

        public DbSet<ApplicationUser> Users { get; set; }

        public DbSet<IdentityUserRole<string>> UserRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RecentlyVisitedAnnouncement> RecentlyVisitedAnnouncements { get; set; }
        
        public DbSet<DeletedAccount> DeletedAccounts { get; set; }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {

            OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }

        private void OnBeforeSaveChanges()
        {

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            userId= userId??"-";
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries<IBaseAuditableEntity>())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;
                var auditEntry = new AuditEntry(entry);
                auditEntry.TableName = entry.Entity.GetType().Name;
                auditEntry.UserId = userId;
                auditEntries.Add(auditEntry);
                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.AuditType = AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = AuditType.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }
            foreach (var auditEntry in auditEntries)
            {
                AuditLogs.Add(auditEntry.ToAudit());
            }
        }

        #region With IDENTITY_INSERT ON and OFF async

        public async Task<int> SaveChangesWithIdentityInsertAsync<T>(CancellationToken cancellationToken)
        {
            // if (context == null) throw new ArgumentNullException(nameof(context));
            await using var transaction = await base.Database.BeginTransactionAsync(cancellationToken);
            await EnableIdentityInsertAsync<T>();
            var c = await base.SaveChangesAsync(cancellationToken);
            await DisableIdentityInsertAsync<T>();
            await transaction.CommitAsync(cancellationToken);
            return c;
        }

        public async Task EnableIdentityInsertAsync<T>() => await SetIdentityInsertAsync<T>(true);
        public async Task DisableIdentityInsertAsync<T>() => await SetIdentityInsertAsync<T>(false);

        private async Task SetIdentityInsertAsync<T>(bool enable)
        {
            var entityType = Model.FindEntityType(typeof(T));
            var value = enable ? "ON" : "OFF";
            await base.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} {value}");
        }

        #endregion

        #region Context configurations

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //if (Database.IsSqlite())
            //{
            //    Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
            //    Database.ExecuteSqlRaw("PRAGMA synchronous=NORMAL;");
            //    Database.ExecuteSqlRaw("PRAGMA busy_timeout=30000;");
            //}

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                ApplyDefaultTypes(modelBuilder, entityType.ClrType);
                ApplyIdConfiguration(modelBuilder, entityType.ClrType);
                ApplyDefaultDecimalPrecisionConfiguration(modelBuilder, entityType.ClrType);
                ApplyDefaultDoublePrecisionConfiguration(modelBuilder, entityType.ClrType);
                ApplyGlobalFilters(modelBuilder, entityType.ClrType);
            }



            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());







            modelBuilder.Entity<ApplicationUser>()
               .HasOne(u => u.ConsultantCompany)
               .WithMany(u => u.Consultants)
               .HasForeignKey(u => u.ConsultantCompanyId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConsultantInvitation>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.InvitationToken).IsRequired();
                entity.Property(e => e.Status).IsRequired();
                entity.Property(e => e.CompanyName).IsRequired();
                entity.Property(e => e.InvitedBy).IsRequired(false);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.ExpiryDate).IsRequired();

                entity.HasIndex(e => e.InvitationToken).IsUnique();
                entity.HasIndex(e => e.Email);

                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(e => e.InvitedBy)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ConsultantInvitation>()
                .HasIndex(c => c.InvitationToken)
                .IsUnique();

            modelBuilder.Entity<ConsultantInvitation>()
                .HasIndex(c => c.Email);

            // DeletedAccount configuration
            modelBuilder.Entity<DeletedAccount>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(128);
                entity.Property(e => e.DeletedAt).IsRequired();
                entity.Property(e => e.DeletionReason).IsRequired(false).HasMaxLength(500);
                entity.Property(e => e.RecoveryToken).IsRequired(false).HasMaxLength(100);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.DeletedAt);
                entity.HasIndex(e => e.RecoveryToken);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // VisitorCount tablosu için konfigürasyon
            modelBuilder.Entity<VisitorCount>()
                .HasKey(vc => vc.Id); // Id'yi anahtar olarak belirle
            modelBuilder.Entity<VisitorCount>()
                .Property(vc => vc.ActiveVisitors)
                .IsRequired(); // ActiveVisitors için zorunlu alan
            modelBuilder.Entity<VisitorCount>()
                .Property(vc => vc.TotalVisitors)
                .IsRequired(); // TotalVisitors için zorunlu alan
                               // İlanModel konfigürasyonu
                               // İlanModel konfigürasyonu
            modelBuilder.Entity<IlanModel>()
                .ToTable("IlanBilgileri")
                .HasKey(i => i.Id);



            base.OnModelCreating(modelBuilder);
        }

        protected virtual void ApplyDefaultTypes(ModelBuilder modelBuilder, Type type)
        {
            if (typeof(ICreatedEntity).IsAssignableFrom(type))
            {
                modelBuilder.Entity(type)
                    .Property(nameof(ICreatedEntity.CreatedBy))
                    .HasMaxLength(128)
                    .HasDefaultValueSql(SqlForNewGuidAsString)
                    .ValueGeneratedOnAdd();

                modelBuilder.Entity(type)
                    .Property(nameof(ICreatedEntity.CreatedDate))
                    .HasDefaultValueSql(SqlForCreatedDateTime)
                    .ValueGeneratedOnAdd();
            }

            if (typeof(ILastModifiedEntity).IsAssignableFrom(type))
            {
                modelBuilder.Entity(type)
                    .Property(nameof(ILastModifiedEntity.LastModifiedBy))
                    .IsRequired(false)
                    .HasMaxLength(128);

                modelBuilder.Entity(type)
                .Property(nameof(ILastModifiedEntity.LastModifiedDate))
                    .IsRequired(false);
            }
        }

        protected virtual void ApplyGlobalFilters(ModelBuilder modelBuilder, Type type)
        {
            if (typeof(ISoftDelete).IsAssignableFrom(type))
            {
                var filter = CreateSoftDeleteFilterExpression(type);
                modelBuilder.Entity(type).HasQueryFilter(filter);
            }
        }

        protected LambdaExpression CreateSoftDeleteFilterExpression(Type type)
        {
            LambdaExpression expression = null;

            if (typeof(ISoftDelete).IsAssignableFrom(type))
            {
                var softDeleteFilter = CreateLambdaExpression(type, nameof(IAuditableEntity<string>.IsDeleted), false);
                expression = expression == null
                    ? softDeleteFilter
                    : CombineLambdaExpressions(expression, softDeleteFilter);
            }

            return expression;
        }

        protected virtual LambdaExpression CreateLambdaExpression(Type entityType, string propertyName, object filterValue)
        {
            var param = Expression.Parameter(entityType, "p");
            var body = Expression.Equal(Expression.Property(param, propertyName), Expression.Constant(filterValue));
            var lambda = Expression.Lambda(body, param);
            return lambda;
        }

        protected virtual LambdaExpression CombineLambdaExpressions(LambdaExpression expression, LambdaExpression softDeleteFilter)
        {
            var body = Expression.AndAlso(expression.Body, softDeleteFilter.Body);
            var lambda = Expression.Lambda(body, expression.Parameters[0]);
            return lambda;
        }

        protected virtual void ApplyIdConfiguration(ModelBuilder modelBuilder, Type type)
        {
            if (typeof(IBaseEntity<string>).IsAssignableFrom(type) ||
              typeof(IBaseEntity<long>).IsAssignableFrom(type) ||
              typeof(IBaseEntity<int>).IsAssignableFrom(type) ||
              typeof(IBaseEntity<Guid>).IsAssignableFrom(type))
            {
                modelBuilder.Entity(type).HasKey(nameof(IBaseEntity.Id));
                modelBuilder.Entity(type).Property(nameof(IBaseEntity.Id)).ValueGeneratedOnAdd();
            }

            if (typeof(IBaseEntity<Guid>).IsAssignableFrom(type))
            {
                modelBuilder.Entity(type).Property(nameof(IBaseEntity<Guid>.Id))
                    .HasDefaultValue(Guid.NewGuid()).ValueGeneratedOnAdd();
            }
            if (typeof(IBaseEntity<string>).IsAssignableFrom(type))
            {
                modelBuilder.Entity(type).Property(nameof(IBaseEntity<string>.Id))
                    .HasDefaultValueSql(SqlForNewGuidAsString).ValueGeneratedOnAdd();
            }
        }

        protected virtual void ApplyDefaultDecimalPrecisionConfiguration(ModelBuilder modelBuilder, Type type)
        {
            var properties = type.GetProperties().Where(w => w.PropertyType == typeof(decimal) || w.PropertyType == typeof(decimal?));
            foreach (var property in properties)
            {
                modelBuilder.Entity(type).Property(property.Name).HasPrecision(DefaultDecimalPrecision, DefaultDecimalScale);
            }
        }

        protected virtual void ApplyDefaultDoublePrecisionConfiguration(ModelBuilder modelBuilder, Type type)
        {
            var properties = type.GetProperties().Where(w => w.PropertyType == typeof(double) || w.PropertyType == typeof(double?));
            foreach (var property in properties)
            {
                modelBuilder.Entity(type).Property(property.Name).HasPrecision(DefaultDoublePrecision, DefaultDoubleScale);
            }
        }



        #endregion
    }

    namespace sevvalemlak.Models
    {
        public class AboutUsContent
        {
            public int Id { get; set; }
            public string Key { get; set; } // Metnin benzersiz anahtarı (örn: "intro-text", "color-choice")
            public string Content { get; set; } // Metnin içeriği
        }
    }
}
