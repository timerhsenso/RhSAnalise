// src/Modules/GestaoDePessoas/Infrastructure/Persistence/Configurations/SindicatoConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations
{
    public class SindicatoConfiguration : IEntityTypeConfiguration<Sindicato>
    {
        public void Configure(EntityTypeBuilder<Sindicato> builder)
        {
            builder.ToTable("sind1");

            builder.HasKey(s => s.CodigoSindicato);

            builder.Property(s => s.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newsequentialid()");

            builder.Property(s => s.CodigoSindicato)
                .HasColumnName("cdsindicat")
                .HasColumnType("char(2)")
                .IsRequired();

            builder.Property(s => s.DescricaoSindicato)
                .HasColumnName("dcsindicat")
                .HasMaxLength(40)
                .IsRequired();

            builder.Property(s => s.Endereco)
                .HasColumnName("dcendereco")
                .HasMaxLength(30);

            builder.Property(s => s.CNPJ)
                .HasColumnName("cgcsindicat")
                .HasMaxLength(14);

            builder.Property(s => s.CodigoEntidade)
                .HasColumnName("cdentidade")
                .HasColumnType("char(20)");

            builder.Property(s => s.DataBase)
                .HasColumnName("data_base")
                .HasColumnType("char(2)");

            builder.Property(s => s.FlagTipo)
                .HasColumnName("fltipo");

            builder.Property(s => s.CodigoTabelaBase)
                .HasColumnName("cdtabbase")
                .HasColumnType("char(3)");
        }
    }
}