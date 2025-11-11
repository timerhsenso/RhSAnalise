using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities
{
    /// <summary>
    /// Configuração EF Core da tabela LANC3 (tabela sem PK).
    /// </summary>
    public class Lanc3Configuration : IEntityTypeConfiguration<Lanc3>
    {
        public void Configure(EntityTypeBuilder<Lanc3> builder)
        {
            builder.ToTable("lanc3");
            builder.HasNoKey(); // tabela sem PK

            builder.Property(x => x.NoMatric)
                   .HasColumnName("nomatric")
                   .HasMaxLength(8)
                   .IsRequired();

            builder.Property(x => x.CdEmpresa)
                   .HasColumnName("cdempresa")
                   .IsRequired();

            builder.Property(x => x.CdFilial)
                   .HasColumnName("cdfilial")
                   .IsRequired();

            builder.Property(x => x.NoProcesso)
                   .HasColumnName("noprocesso")
                   .HasMaxLength(6)
                   .IsRequired();

            builder.Property(x => x.CdConta)
                   .HasColumnName("cdconta")
                   .HasMaxLength(4)
                   .IsRequired();

            builder.Property(x => x.CdCcUsRes)
                   .HasColumnName("cdccusres")
                   .HasMaxLength(5);

            builder.Property(x => x.QtConta)
                   .HasColumnName("qtconta")
                   .HasColumnType("float");

            builder.Property(x => x.CdUsuario)
                   .HasColumnName("cdusuario")
                   .HasMaxLength(20);
        }
    }
}
