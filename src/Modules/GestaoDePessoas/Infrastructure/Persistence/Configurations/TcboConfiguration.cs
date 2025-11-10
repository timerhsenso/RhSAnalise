using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations;

public class TcboConfiguration : IEntityTypeConfiguration<Tcbo>
{
    public void Configure(EntityTypeBuilder<Tcbo> builder)
    {
        builder.ToTable("tcbo1");

        builder.HasKey(x => x.CdCbo);

        builder.Property(x => x.CdCbo)
               .HasColumnName("cdcbo")
               .HasColumnType("char(6)")
               .IsRequired();

        builder.Property(x => x.DcCbo)
               .HasColumnName("dccbo")
               .HasMaxLength(80)
               .IsRequired();

        builder.Property(x => x.Sinonimo)
               .HasColumnName("sinonimo")
               .HasMaxLength(4000);

        builder.Property(x => x.Id)
               .HasColumnName("id")
               .HasDefaultValueSql("newsequentialid()");

        // Índice não clusterizado em cdcbo (além da PK)
        builder.HasIndex(x => x.CdCbo)
               .HasDatabaseName("tcbo1nx1");

        // Índice único não clusterizado em dccbo
        builder.HasIndex(x => x.DcCbo)
               .HasDatabaseName("UX_tcbo1_dccbo")
               .IsUnique();
    }
}
