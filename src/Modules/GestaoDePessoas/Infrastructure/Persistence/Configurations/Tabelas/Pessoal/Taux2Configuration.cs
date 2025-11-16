using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations.Tabelas.Pessoal;

/// <summary>
/// Configuração EF Core para Taux2 (Situações Auxiliares).
/// Tabela legada: taux2 | Chave composta: (cdtptabela, cdsituacao)
/// </summary>
public class Taux2Configuration : IEntityTypeConfiguration<Taux2>
{
    public void Configure(EntityTypeBuilder<Taux2> builder)
    {
        builder.ToTable("taux2");

        // Chave Primária Composta (legada)
        builder.HasKey(x => new { x.CdTpTabela, x.CdSituacao })
               .HasName("PK__TAUX2__15F0184D");

        // Propriedades
        builder.Property(x => x.CdTpTabela)
               .HasColumnName("cdtptabela")
               .HasColumnType("varchar(2)")
               .IsRequired();

        builder.Property(x => x.CdSituacao)
               .HasColumnName("cdsituacao")
               .HasColumnType("varchar(2)")
               .IsRequired();

        builder.Property(x => x.DcSituacao)
               .HasColumnName("dcsituacao")
               .HasMaxLength(60)
               .IsRequired();

        builder.Property(x => x.NoOrdem)
               .HasColumnName("noordem");

        builder.Property(x => x.FlAtivoAux)
               .HasColumnName("flativoaux")
               .HasColumnType("char(1)")
               .IsFixedLength();

        // ✅ REMOVIDO: builder.Property(x => x.Id).IsRequired(false);
        // Taux2 não herda mais BaseEntity, então não tem propriedade Id

        // Relacionamento FK -> TAUX1
        builder.HasOne(x => x.TipoTabela)
               .WithMany(x => x.Situacoes)
               .HasForeignKey(x => x.CdTpTabela)
               .HasPrincipalKey(x => x.CdTpTabela)
               .HasConstraintName("FK__TAUX2__cdtptabel__16E43C86")
               .OnDelete(DeleteBehavior.Restrict);

        // Índice
        builder.HasIndex(x => new { x.CdTpTabela, x.CdSituacao })
               .HasDatabaseName("taux2nx1");
    }
}