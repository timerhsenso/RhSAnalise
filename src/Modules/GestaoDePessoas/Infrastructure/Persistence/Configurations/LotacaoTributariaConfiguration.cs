using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;
using RhSensoERP.Modules.Esocial.Core.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations;

public class LotacaoTributariaConfiguration : IEntityTypeConfiguration<LotacaoTributaria>
{
    public void Configure(EntityTypeBuilder<LotacaoTributaria> builder)
    {
        builder.ToTable("lotacoestributarias");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
               .HasColumnName("id")
               .HasDefaultValueSql("newsequentialid()");

        builder.Property(x => x.CodLotacao)
               .HasColumnName("codlotacao")
               .HasMaxLength(30)
               .IsRequired();

        builder.Property(x => x.Descricao)
               .HasColumnName("descricao")
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(x => x.TpLotacao)
               .HasColumnName("tplotacao")
               .HasColumnType("char(2)")
               .IsRequired();

        builder.Property(x => x.TpInsc)
               .HasColumnName("tpinsc")
               .HasColumnType("char(1)");

        builder.Property(x => x.NrInsc)
               .HasColumnName("nrinsc")
               .HasMaxLength(14);

        builder.Property(x => x.FPAS)
               .HasColumnName("fpas")
               .HasColumnType("char(3)")
               .IsRequired();

        builder.Property(x => x.CodTercs)
               .HasColumnName("codtercs")
               .HasColumnType("char(4)")
               .IsRequired();

        builder.Property(x => x.CodTercsSusp)
               .HasColumnName("codtercssusp")
               .HasColumnType("char(4)");

        builder.Property(x => x.TpInscContrat).HasColumnName("tpinsccontrat");
        builder.Property(x => x.NrInscContrat).HasColumnName("nrInscContrat").HasMaxLength(14);
        builder.Property(x => x.TpInscProp).HasColumnName("tpinscprop");
        builder.Property(x => x.NrInscProp).HasColumnName("nrinscprop").HasMaxLength(14);
        builder.Property(x => x.AliqRat).HasColumnName("aliqRat").HasColumnType("char(1)");
        builder.Property(x => x.Fap).HasColumnName("fap");

        // Índices
        builder.HasIndex(x => x.FPAS).HasDatabaseName("IX_lotacoestributarias_fpas");
        builder.HasIndex(x => x.TpLotacao).HasDatabaseName("IX_lotacoestributarias_tplotacao");

        // ===== Relações (FKs) =====
        // tplotacao (char(2)) -> tab10_esocial(tab10_codigo)
        builder.HasOne<Tab10Esocial>()
               .WithMany()
               .HasForeignKey(x => x.TpLotacao)
               .HasConstraintName("FK_lotacoestributarias_tab10_esocial_tplotacao")
               .OnDelete(DeleteBehavior.NoAction);

        // fpas (char(3)) -> tab4_esocial(tab4_codigo)
        builder.HasOne<Tab4Esocial>()
               .WithMany()
               .HasForeignKey(x => x.FPAS)
               .HasConstraintName("FK_lotacoestributarias_tab4_esocial_fpas")
               .OnDelete(DeleteBehavior.NoAction);
    }
}
