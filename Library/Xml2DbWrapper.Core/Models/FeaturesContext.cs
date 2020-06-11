using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Xml2DbMapper.Core.Models
{
    public partial class FeaturesContext : DbContext
    {
        public FeaturesContext()
        {
        }

        public FeaturesContext(DbContextOptions<FeaturesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Application> Application { get; set; }
        public virtual DbSet<BiopsyKits> BiopsyKits { get; set; }
        public virtual DbSet<Bundle> Bundle { get; set; }
        public virtual DbSet<Certifier> Certifier { get; set; }
        public virtual DbSet<CertifierVersion> CertifierVersion { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<CountryDistributor> CountryDistributor { get; set; }
        public virtual DbSet<CountryLicense> CountryLicense { get; set; }
        public virtual DbSet<CountryVersion> CountryVersion { get; set; }
        public virtual DbSet<Dbconfiguration> Dbconfiguration { get; set; }
        public virtual DbSet<Deprecated> Deprecated { get; set; }
        public virtual DbSet<Distributor> Distributor { get; set; }
        public virtual DbSet<DummyTable> DummyTable { get; set; }
        public virtual DbSet<Feature> Feature { get; set; }
        public virtual DbSet<FeatureRelation> FeatureRelation { get; set; }
        public virtual DbSet<License> License { get; set; }
        public virtual DbSet<LicenseRelation> LicenseRelation { get; set; }
        public virtual DbSet<LicenseRelationException> LicenseRelationException { get; set; }
        public virtual DbSet<LogicalModel> LogicalModel { get; set; }
        public virtual DbSet<MinorVersionAssociation> MinorVersionAssociation { get; set; }
        public virtual DbSet<NormalRule> NormalRule { get; set; }
        public virtual DbSet<Option> Option { get; set; }
        public virtual DbSet<PartNumbersAssociations> PartNumbersAssociations { get; set; }
        public virtual DbSet<PhysicalModel> PhysicalModel { get; set; }
        public virtual DbSet<Probe> Probe { get; set; }
        public virtual DbSet<ProbePreset> ProbePreset { get; set; }
        public virtual DbSet<ProbeSettingsFamily> ProbeSettingsFamily { get; set; }
        public virtual DbSet<ProbeTransducers> ProbeTransducers { get; set; }
        public virtual DbSet<RegulatoryFeature> RegulatoryFeature { get; set; }
        public virtual DbSet<SettingFamily> SettingFamily { get; set; }
        public virtual DbSet<Swpack> Swpack { get; set; }
        public virtual DbSet<TwinLicenses> TwinLicenses { get; set; }
        public virtual DbSet<Uirule> Uirule { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//                optionsBuilder.UseSqlServer("Server=PC\\SQLExpress;Database=Features;Trusted_Connection=True;");
//            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Application>(entity =>
            {
                entity.Property(e => e.Abbreviation)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Localization).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ProbeDescrName).HasMaxLength(50);

                //entity.HasOne(d => d.Feature)
                //    .WithMany(p => p.Application)
                //    .HasForeignKey(d => d.FeatureId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Feature_Id_Application_FeatureId_Relationship_1_N");
            });

            modelBuilder.Entity<BiopsyKits>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                //entity.HasOne(d => d.Feature)
                //    .WithMany(p => p.BiopsyKits)
                //    .HasForeignKey(d => d.FeatureId)
                //    .HasConstraintName("Feature_Id_BiopsyKits_FeatureId_Relationship_1_N");
            });

            modelBuilder.Entity<Bundle>(entity =>
            {
                //entity.HasOne(d => d.Feature)
                //    .WithMany(p => p.BundleFeature)
                //    .HasForeignKey(d => d.FeatureId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Feature_Id_Bundle_FeatureId_Relationship_1_N");

                //entity.HasOne(d => d.ParentFeature)
                //    .WithMany(p => p.BundleParentFeature)
                //    .HasForeignKey(d => d.ParentFeatureId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Feature_Id_Bundle_ParentFeatureId_Relationship_1_N");
            });

            modelBuilder.Entity<Certifier>(entity =>
            {
                entity.HasIndex(e => new { e.Name, e.Code })
                    .HasName("Certifier_Name,Code_Index")
                    .IsUnique();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<CertifierVersion>(entity =>
            {
                entity.HasIndex(e => new { e.CertifierId, e.DistributorId, e.LogicalModelId })
                    .HasName("CertifierVersion_CertifierId,DistributorId,LogicalModelId_Index")
                    .IsUnique();
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasIndex(e => new { e.Name, e.Code })
                    .HasName("Country_Name,Code_Index")
                    .IsUnique();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                //entity.HasOne(d => d.Certifier)
                //    .WithMany(p => p.Country)
                //    .HasForeignKey(d => d.CertifierId)
                //    .HasConstraintName("Certifier_Id_Country_CertifierId_Relationship_1_N");
            });

            modelBuilder.Entity<CountryDistributor>(entity =>
            {
                entity.ToTable("Country_Distributor");

                entity.HasIndex(e => new { e.CountryId, e.DistributorId })
                    .HasName("Country_Distributor_CountryId,DistributorId_Index")
                    .IsUnique();

                //entity.HasOne(d => d.Country)
                //    .WithMany(p => p.CountryDistributor)
                //    .HasForeignKey(d => d.CountryId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Country_Id_Country_Distributor_CountryId_Relationship_1_N");

                //entity.HasOne(d => d.Distributor)
                //    .WithMany(p => p.CountryDistributor)
                //    .HasForeignKey(d => d.DistributorId)
                //    .HasConstraintName("Distributor_Id_Country_Distributor_DistributorId_Relationship_1_N");
            });

            modelBuilder.Entity<CountryLicense>(entity =>
            {
                entity.HasIndex(e => new { e.CountryId, e.DistributorId, e.LicenseId })
                    .HasName("CountryLicense_CountryId,DistributorId,LicenseId_Index")
                    .IsUnique();

                //entity.HasOne(d => d.Country)
                //    .WithMany(p => p.CountryLicense)
                //    .HasForeignKey(d => d.CountryId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Country_Id_CountryLicense_CountryId_Relationship_1_N");

                //entity.HasOne(d => d.Distributor)
                //    .WithMany(p => p.CountryLicense)
                //    .HasForeignKey(d => d.DistributorId)
                //    .HasConstraintName("Distributor_Id_CountryLicense_DistributorId_Relationship_1_N");

                //entity.HasOne(d => d.License)
                //    .WithMany(p => p.CountryLicense)
                //    .HasForeignKey(d => d.LicenseId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("License_Id_CountryLicense_LicenseId_Relationship_1_N");
            });

            modelBuilder.Entity<CountryVersion>(entity =>
            {
                entity.HasIndex(e => new { e.CountryId, e.DistributorId, e.LogicalModelId })
                    .HasName("CountryVersion_CountryId,DistributorId,LogicalModelId_Index")
                    .IsUnique();

                //entity.HasOne(d => d.Country)
                //    .WithMany(p => p.CountryVersion)
                //    .HasForeignKey(d => d.CountryId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Country_Id_CountryVersion_CountryId_Relationship_1_N");

                //entity.HasOne(d => d.Distributor)
                //    .WithMany(p => p.CountryVersion)
                //    .HasForeignKey(d => d.DistributorId)
                //    .HasConstraintName("Distributor_Id_CountryVersion_DistributorId_Relationship_1_N");

                //entity.HasOne(d => d.LogicalModel)
                //    .WithMany(p => p.CountryVersion)
                //    .HasForeignKey(d => d.LogicalModelId)
                //    .HasConstraintName("LogicalModel_Id_CountryVersion_LogicalModelId_Relationship_1_N");
            });

            modelBuilder.Entity<Dbconfiguration>(entity =>
            {
                entity.ToTable("DBConfiguration");
            });

            modelBuilder.Entity<Deprecated>(entity =>
            {
                //entity.HasOne(d => d.DeprecatedFeature)
                //    .WithMany(p => p.DeprecatedDeprecatedFeature)
                //    .HasForeignKey(d => d.DeprecatedFeatureId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Feature_Id_Deprecated_DeprecatedFeatureId_Relationship_1_N");

                //entity.HasOne(d => d.SubstituteFeature)
                //    .WithMany(p => p.DeprecatedSubstituteFeature)
                //    .HasForeignKey(d => d.SubstituteFeatureId)
                //    .HasConstraintName("Feature_Id_Deprecated_SubstituteFeatureId_Relationship_1_N");
            });

            modelBuilder.Entity<Distributor>(entity =>
            {
                entity.HasIndex(e => new { e.Name, e.Code })
                    .HasName("Distributor_Name,Code_Index")
                    .IsUnique();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<DummyTable>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Feature>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.NameInCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                //entity.HasOne(d => d.License)
                //    .WithMany(p => p.Feature)
                //    .HasForeignKey(d => d.LicenseId)
                //    .HasConstraintName("License_Id_Feature_LicenseId_Relationship_1_N");
            });

            modelBuilder.Entity<FeatureRelation>(entity =>
            {
                //entity.HasOne(d => d.Feature)
                //    .WithMany(p => p.FeatureRelationFeature)
                //    .HasForeignKey(d => d.FeatureId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Feature_Id_FeatureRelation_FeatureId_Relationship_1_N");

                //entity.HasOne(d => d.ParentFeature)
                //    .WithMany(p => p.FeatureRelationParentFeature)
                //    .HasForeignKey(d => d.ParentFeatureId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Feature_Id_FeatureRelation_ParentFeatureId_Relationship_1_N");
            });

            modelBuilder.Entity<License>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<LicenseRelation>(entity =>
            {
                //entity.HasOne(d => d.Feature)
                //    .WithMany(p => p.LicenseRelationFeature)
                //    .HasForeignKey(d => d.FeatureId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Feature_Id_LicenseRelation_FeatureId_Relationship_1_N");

                //entity.HasOne(d => d.ParentFeature)
                //    .WithMany(p => p.LicenseRelationParentFeature)
                //    .HasForeignKey(d => d.ParentFeatureId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Feature_Id_LicenseRelation_ParentFeatureId_Relationship_1_N");
            });

            modelBuilder.Entity<LicenseRelationException>(entity =>
            {
                //entity.HasOne(d => d.LicenseRelation)
                //    .WithMany(p => p.LicenseRelationException)
                //    .HasForeignKey(d => d.LicenseRelationId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("LicenseRelation_Id_LicenseRelationException_LicenseRelationId_Relationship_1_N");

                //entity.HasOne(d => d.LogicalModel)
                //    .WithMany(p => p.LicenseRelationException)
                //    .HasForeignKey(d => d.LogicalModelId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("LogicalModel_Id_LicenseRelationException_LogicalModelId_Relationship_1_N");
            });

            modelBuilder.Entity<LogicalModel>(entity =>
            {
                entity.Property(e => e.IsDefault).HasColumnName("isDefault");

                entity.Property(e => e.ModelFamily).HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.StyleName).HasMaxLength(50);

                //entity.HasOne(d => d.License)
                //    .WithMany(p => p.LogicalModel)
                //    .HasForeignKey(d => d.LicenseId)
                //    .HasConstraintName("License_Id_LogicalModel_LicenseId_Relationship_1_N");
            });

            modelBuilder.Entity<MinorVersionAssociation>(entity =>
            {
                entity.Property(e => e.BuildVersion).HasMaxLength(50);
            });

            modelBuilder.Entity<NormalRule>(entity =>
            {
                entity.HasIndex(e => e.ApplicationId)
                    .HasName("NormalRule_ApplicationId_Index");

                entity.HasIndex(e => e.CountryId)
                    .HasName("NormalRule_CountryId_Index");

                entity.HasIndex(e => e.DistributorId)
                    .HasName("NormalRule_DistributorId_Index");

                entity.HasIndex(e => e.KitId)
                    .HasName("NormalRule_KitId_Index");

                entity.HasIndex(e => e.LogicalModelId)
                    .HasName("NormalRule_LogicalModelId_Index");

                entity.HasIndex(e => e.OptionId)
                    .HasName("NormalRule_OptionId_Index");

                entity.HasIndex(e => e.ProbeId)
                    .HasName("NormalRule_ProbeId_Index");

                entity.HasIndex(e => e.TransducerType)
                    .HasName("NormalRule_TransducerType_Index");

                entity.HasIndex(e => e.UserLevel)
                    .HasName("NormalRule_UserLevel_Index");

                entity.HasIndex(e => e.Version)
                    .HasName("NormalRule_Version_Index");

                entity.Property(e => e.UiruleId).HasColumnName("UIRuleId");

                //entity.HasOne(d => d.Application)
                //    .WithMany(p => p.NormalRule)
                //    .HasForeignKey(d => d.ApplicationId)
                //    .HasConstraintName("Application_Id_NormalRule_ApplicationId_Relationship_1_N");

                //entity.HasOne(d => d.Country)
                //    .WithMany(p => p.NormalRule)
                //    .HasForeignKey(d => d.CountryId)
                //    .HasConstraintName("Country_Id_NormalRule_CountryId_Relationship_1_N");

                //entity.HasOne(d => d.Distributor)
                //    .WithMany(p => p.NormalRule)
                //    .HasForeignKey(d => d.DistributorId)
                //    .HasConstraintName("Distributor_Id_NormalRule_DistributorId_Relationship_1_N");

                //entity.HasOne(d => d.Kit)
                //    .WithMany(p => p.NormalRule)
                //    .HasForeignKey(d => d.KitId)
                //    .HasConstraintName("BiopsyKits_Id_NormalRule_KitId_Relationship_1_N");

                //entity.HasOne(d => d.LogicalModel)
                //    .WithMany(p => p.NormalRule)
                //    .HasForeignKey(d => d.LogicalModelId)
                //    .HasConstraintName("LogicalModel_Id_NormalRule_LogicalModelId_Relationship_1_N");
            });

            modelBuilder.Entity<Option>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                //entity.HasOne(d => d.Feature)
                //    .WithMany(p => p.Option)
                //    .HasForeignKey(d => d.FeatureId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Feature_Id_Option_FeatureId_Relationship_1_N");
            });

            modelBuilder.Entity<PartNumbersAssociations>(entity =>
            {
                entity.Property(e => e.FeatureBosname)
                    .HasColumnName("FeatureBOSName")
                    .HasMaxLength(100);

                entity.Property(e => e.ModeTypeToExport).HasMaxLength(50);

                entity.Property(e => e.PartNumber)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ToExport).HasColumnName("toExport");

                //entity.HasOne(d => d.Feature)
                //    .WithMany(p => p.PartNumbersAssociations)
                //    .HasForeignKey(d => d.FeatureId)
                //    .HasConstraintName("Feature_Id_PartNumbersAssociations_FeatureId_Relationship_1_N");

                //entity.HasOne(d => d.LogicalModel)
                //    .WithMany(p => p.PartNumbersAssociations)
                //    .HasForeignKey(d => d.LogicalModelId)
                //    .HasConstraintName("LogicalModel_Id_PartNumbersAssociations_LogicalModelId_Relationship_1_N");
            });

            modelBuilder.Entity<PhysicalModel>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<Probe>(entity =>
            {
                entity.HasIndex(e => e.SaleName)
                    .HasName("Probe_SaleName_Index")
                    .IsUnique();

                entity.Property(e => e.HwCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ProbeDescription).HasMaxLength(50);

                entity.Property(e => e.ProbeStringCode).HasMaxLength(50);

                entity.Property(e => e.SaleName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TeeFamily).HasMaxLength(50);

                //entity.HasOne(d => d.Feature)
                //    .WithMany(p => p.Probe)
                //    .HasForeignKey(d => d.FeatureId)
                //    .HasConstraintName("Feature_Id_Probe_FeatureId_Relationship_1_N");
            });

            modelBuilder.Entity<ProbePreset>(entity =>
            {
                entity.ToTable("Probe_Preset");

                entity.Property(e => e.PresetFileNameFrontal).HasMaxLength(50);

                entity.Property(e => e.PresetFileNameLateral).HasMaxLength(50);

                entity.Property(e => e.PresetFolder).HasMaxLength(50);

                entity.Property(e => e.ProbeDefaultEnum)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                //entity.HasOne(d => d.Application)
                //    .WithMany(p => p.ProbePreset)
                //    .HasForeignKey(d => d.ApplicationId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Application_Id_Probe_Preset_ApplicationId_Relationship_1_N");

                //entity.HasOne(d => d.Probe)
                //    .WithMany(p => p.ProbePreset)
                //    .HasForeignKey(d => d.ProbeId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Probe_Id_Probe_Preset_ProbeId_Relationship_1_N");

                //entity.HasOne(d => d.SettingsFamily)
                //    .WithMany(p => p.ProbePreset)
                //    .HasForeignKey(d => d.SettingsFamilyId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("SettingFamily_Id_Probe_Preset_SettingsFamilyId_Relationship_1_N");
            });

            modelBuilder.Entity<ProbeSettingsFamily>(entity =>
            {
                entity.ToTable("Probe_SettingsFamily");

                entity.Property(e => e.ProbeDataFileNameFrontal).HasMaxLength(50);

                entity.Property(e => e.ProbeDataFileNameLateral).HasMaxLength(50);

                entity.Property(e => e.ProbeDescFileName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ProbeFolder)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                //entity.HasOne(d => d.Probe)
                //    .WithMany(p => p.ProbeSettingsFamily)
                //    .HasForeignKey(d => d.ProbeId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Probe_Id_Probe_SettingsFamily_ProbeId_Relationship_1_N");

                //entity.HasOne(d => d.SettingsFamily)
                //    .WithMany(p => p.ProbeSettingsFamily)
                //    .HasForeignKey(d => d.SettingsFamilyId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("SettingFamily_Id_Probe_SettingsFamily_SettingsFamilyId_Relationship_1_N");
            });

            modelBuilder.Entity<ProbeTransducers>(entity =>
            {
                entity.ToTable("Probe_Transducers");

                entity.HasIndex(e => new { e.ProbeId, e.TransducerType })
                    .HasName("Probe_Transducers_ProbeId,TransducerType_Index")
                    .IsUnique();

                //entity.HasOne(d => d.Probe)
                //    .WithMany(p => p.ProbeTransducers)
                //    .HasForeignKey(d => d.ProbeId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Probe_Id_Probe_Transducers_ProbeId_Relationship_1_N");
            });

            modelBuilder.Entity<RegulatoryFeature>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                //entity.HasOne(d => d.Feature)
                //    .WithMany(p => p.RegulatoryFeature)
                //    .HasForeignKey(d => d.FeatureId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("Feature_Id_RegulatoryFeature_FeatureId_Relationship_1_N");
            });

            modelBuilder.Entity<SettingFamily>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ProbeListFile)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.SwpackId).HasColumnName("SWpackId");

                //entity.HasOne(d => d.Swpack)
                //    .WithMany(p => p.SettingFamily)
                //    .HasForeignKey(d => d.SwpackId)
                //    .HasConstraintName("SWpack_Id_SettingFamily_SWpackId_Relationship_1_N");
            });

            modelBuilder.Entity<Swpack>(entity =>
            {
                entity.ToTable("SWpack");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<TwinLicenses>(entity =>
            {
                entity.HasIndex(e => e.LicenseId)
                    .HasName("TwinLicenses_LicenseId_Index")
                    .IsUnique();

                entity.HasIndex(e => e.TwinLicenseId)
                    .HasName("TwinLicenses_TwinLicenseId_Index")
                    .IsUnique();

                //entity.HasOne(d => d.License)
                //    .WithOne(p => p.TwinLicensesLicense)
                //    .HasForeignKey<TwinLicenses>(d => d.LicenseId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("License_Id_TwinLicenses_LicenseId_Relationship_1_N");

                //entity.HasOne(d => d.TwinLicense)
                //    .WithOne(p => p.TwinLicensesTwinLicense)
                //    .HasForeignKey<TwinLicenses>(d => d.TwinLicenseId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("License_Id_TwinLicenses_TwinLicenseId_Relationship_1_N");
            });

            modelBuilder.Entity<Uirule>(entity =>
            {
                entity.ToTable("UIRule");

                entity.Property(e => e.RuleName).HasMaxLength(50);

                entity.Property(e => e.Version).HasMaxLength(50);

                // ANTO FIX
                //entity.HasOne(d => d.Application)
                //    .WithMany(p => p.Uirule)
                //    .HasForeignKey(d => d.ApplicationId)
                //    .HasConstraintName("Application_Id_UIRule_ApplicationId_Relationship_1_N");

                //entity.HasOne(d => d.Certifier)
                //    .WithMany(p => p.Uirule)
                //    .HasForeignKey(d => d.CertifierId)
                //    .HasConstraintName("Certifier_Id_UIRule_CertifierId_Relationship_1_N");

                //entity.HasOne(d => d.Country)
                //    .WithMany(p => p.Uirule)
                //    .HasForeignKey(d => d.CountryId)
                //    .HasConstraintName("Country_Id_UIRule_CountryId_Relationship_1_N");

                //entity.HasOne(d => d.Distributor)
                //    .WithMany(p => p.Uirule)
                //    .HasForeignKey(d => d.DistributorId)
                //    .HasConstraintName("Distributor_Id_UIRule_DistributorId_Relationship_1_N");

                //entity.HasOne(d => d.Kit)
                //    .WithMany(p => p.Uirule)
                //    .HasForeignKey(d => d.KitId)
                //    .HasConstraintName("BiopsyKits_Id_UIRule_KitId_Relationship_1_N");

                //entity.HasOne(d => d.LogicalModel)
                //    .WithMany(p => p.Uirule)
                //    .HasForeignKey(d => d.LogicalModelId)
                //    .HasConstraintName("LogicalModel_Id_UIRule_LogicalModelId_Relationship_1_N");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
