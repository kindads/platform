using Captivate.Comun.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivate.DataAccess
{
    public partial class KindadsContext : DbContext
    {
        public KindadsContext()
            : base("name=KindAdsDBContext")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            

            modelBuilder.Entity<ProductEntity>().ToTable("PRODUCTS", schemaName:"dbo");
            modelBuilder.Entity<AspNetUserEntity>().ToTable("AspNetUsers", schemaName: "dbo");
            modelBuilder.Entity<CategoryEntity>().ToTable("CATEGORIES", schemaName: "dbo");
            modelBuilder.Entity<PartnerSettingsEntity>().ToTable("PARTNER_PRODUCT_SETTINGS", schemaName: "dbo");
            modelBuilder.Entity<PartnerEntity>().ToTable("PARTNERS", schemaName: "dbo");
            modelBuilder.Entity<ProductTypeEntity>().ToTable("PRODUCT_TYPE", schemaName: "dbo");
            modelBuilder.Entity<SiteEntity>().ToTable("SITES", schemaName: "dbo");
            modelBuilder.Entity<TagEntity>().ToTable("TAGS", schemaName: "dbo");
            modelBuilder.Entity<TransactionTypeEntity>().ToTable("TRANSACTION_TYPE", schemaName: "dbo");
            modelBuilder.Entity<TransactionsCaptEntity>().ToTable("TRANSACTIONS_CAPT", schemaName: "dbo");
            modelBuilder.Entity<CampaignEntity>().ToTable("CAMPAIGNs1", schemaName: "dbo");
            modelBuilder.Entity<AspNetUserClaimEntity>().ToTable("AspNetUserClaims", schemaName: "dbo");
            modelBuilder.Entity<AspNetUserLoginEntity>().ToTable("AspNetUserLogins", schemaName: "dbo");
            modelBuilder.Entity<AspNetRoleEntity>().ToTable("AspNetRoles", schemaName: "dbo");
            modelBuilder.Entity<TransactionsExternalEntity>().ToTable("TRANSACTIONS_EXTERNAL", schemaName: "dbo");
            modelBuilder.Entity<CatalogoCampaignStatusEntity>().ToTable("CAT_CAMPAIGN_STATUS", schemaName: "dbo");
            modelBuilder.Entity<PartnerSettingsEntity>().ToTable("PARTNER_SETTINGS", schemaName: "dbo");
            modelBuilder.Entity<ProductSettingsEntity>().ToTable("PRODUCT_SETTINGS", schemaName: "dbo");

            modelBuilder.Entity<CampaignSettingsEntity>().ToTable("CAMPAIGN_SETTINGS", schemaName: "dbo");
            modelBuilder.Entity<CampaignChatEntity>().ToTable("CAMPAIGN_CHAT", schemaName: "dbo");

            modelBuilder.Entity<CategorySiteEntity>().ToTable("CATEGORYSITE", schemaName: "dbo");
        }

       

        public virtual DbSet<AspNetUserEntity> AspNetUsers { get; set; }
        public virtual DbSet<CategoryEntity> Categories { get; set; }
        public virtual DbSet<PartnerProductSettingsEntity> PartnerProductSettings { get; set; }
        public virtual DbSet<PartnerEntity> Partners { get; set; }
        public virtual DbSet<ProductTypeEntity> ProductTypes { get; set; }
        public virtual DbSet<ProductEntity> Products { get; set; }
        public virtual DbSet<SiteEntity> Sites { get; set; }
        public virtual DbSet<TagEntity> Tags { get; set; }
        public virtual DbSet<TransactionTypeEntity> TransactionTypes { get; set; }
        public virtual DbSet<TransactionsCaptEntity> TransactionsCapt { get; set; }
        public virtual DbSet<CampaignEntity> Campaigns { get; set; }
        public virtual DbSet<AspNetUserClaimEntity> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLoginEntity> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetRoleEntity> AspNetRoles { get; set; }
        public virtual DbSet<TransactionsExternalEntity> TransactionExternals { get; set; }
        public virtual DbSet<CatalogoCampaignStatusEntity> CatalogoCampaignStatus { get; set; }
        public virtual DbSet<PartnerSettingsEntity> PartnerSettings { get; set; }
        public virtual DbSet<ProductSettingsEntity> ProductSettings { get; set; }
        public virtual DbSet<CampaignSettingsEntity> CampaignSettings { get; set; }
        public virtual DbSet<CampaignChatEntity> CampaignChat { get; set; }
        public virtual DbSet<CategorySiteEntity> CategorySites { get; set; }
    }
}
