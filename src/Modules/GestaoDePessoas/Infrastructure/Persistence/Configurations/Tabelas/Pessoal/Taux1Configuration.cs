using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations.Tabelas.Pessoal
{
    public class Taux1Configuration : IEntityTypeConfiguration<Taux1>
    {
        public void Configure(EntityTypeBuilder<Taux1> builder)
        {
            builder.ToTable("taux1");

            builder.HasKey(x => x.CdTpTabela)
                   .HasName("PK__TAUX1__1407CFDB");

            builder.Property(x => x.CdTpTabela)
                   .HasColumnName("cdtptabela")
                   .HasMaxLength(2)
                   .IsRequired();

            builder.Property(x => x.DcTabela)
                   .HasColumnName("dctabela")
                   .HasMaxLength(60)
                   .IsRequired();

            // Id técnico (seu BaseEntity)
            builder.Property(x => x.Id)
                   .HasColumnName("id")
                   .ValueGeneratedNever()
                   .IsRequired(false); // não existe na DDL; deixar opcional

            // Índice
            builder.HasIndex(x => x.CdTpTabela)
                   .HasDatabaseName("taux1nx1");
        }
    }
}
