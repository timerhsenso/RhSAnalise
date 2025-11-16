using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations.Tabelas.Pessoal;

/// <summary>
/// Configuração EF Core para Taux1 (Tipos de Tabelas Auxiliares).
/// Tabela legada: taux1 | Chave: cdtptabela (varchar(2))
/// </summary>
public class Taux1Configuration : IEntityTypeConfiguration<Taux1>
{
    public void Configure(EntityTypeBuilder<Taux1> builder)
    {
        builder.ToTable("taux1");

        // Chave Primária (legada)
        builder.HasKey(x => x.CdTpTabela)
               .HasName("PK__TAUX1__1407CFDB");

        // Propriedades
        builder.Property(x => x.CdTpTabela)
               .HasColumnName("cdtptabela")
               .HasColumnType("varchar(2)")
               .IsRequired();

        builder.Property(x => x.DcTabela)
               .HasColumnName("dctabela")
               .HasMaxLength(60)
               .IsRequired();

        // ✅ REMOVIDO: Linhas Ignore() já que Taux1 NÃO herda BaseEntity

        // Índice
        builder.HasIndex(x => x.CdTpTabela)
               .HasDatabaseName("taux1nx1");

        // Navegação
        builder.HasMany(x => x.Situacoes)
               .WithOne(x => x.TipoTabela)
               .HasForeignKey(x => x.CdTpTabela)
               .HasPrincipalKey(x => x.CdTpTabela)
               .OnDelete(DeleteBehavior.Cascade);
    }
}