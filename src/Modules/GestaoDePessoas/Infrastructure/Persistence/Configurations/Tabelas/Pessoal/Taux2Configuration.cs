using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations.Tabelas.Pessoal
{
    public class Taux2Configuration : IEntityTypeConfiguration<Taux2>
    {
        public void Configure(EntityTypeBuilder<Taux2> builder)
        {
            builder.ToTable("taux2");

            builder.HasKey(x => new { x.CdTpTabela, x.CdSituacao })
                   .HasName("PK__TAUX2__15F0184D");

            builder.Property(x => x.CdTpTabela)
                   .HasColumnName("cdtptabela")
                   .HasMaxLength(2)
                   .IsRequired();

            builder.Property(x => x.CdSituacao)
                   .HasColumnName("cdsituacao")
                   .HasMaxLength(2)
                   .IsRequired();

            builder.Property(x => x.DcSituacao)
                   .HasColumnName("dcsituacao")
                   .HasMaxLength(60)
                   .IsRequired();

            builder.Property(x => x.NoOrdem)
                   .HasColumnName("noordem");

            builder.Property(x => x.FlAtivoAux)
                   .HasColumnName("flativoaux")
                   .HasMaxLength(1)
                   .IsFixedLength();

            builder.Property(x => x.Id)
                   .HasColumnName("id")
                   .IsRequired(false);

            // FK -> TAUX1
            builder.HasOne(x => x.TipoTabela)
                   .WithMany(x => x.Situacoes)
                   .HasForeignKey(x => x.CdTpTabela)
                   .HasConstraintName("FK__TAUX2__cdtptabel__16E43C86")
                   .OnDelete(DeleteBehavior.Restrict);

            // Índice
            builder.HasIndex(x => new { x.CdTpTabela, x.CdSituacao })
                   .HasDatabaseName("taux2nx1");
        }
    }
}
