// Lanc3Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Configurations;

public sealed class Lanc3Configuration : IEntityTypeConfiguration<Lanc3>
{
    public void Configure(EntityTypeBuilder<Lanc3> b)
    {
        b.ToTable("lanc3");

        b.HasNoKey(); // sem PK

        b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
        b.Property(x => x.NoProcesso).HasMaxLength(6).IsRequired();
        b.Property(x => x.CdConta).HasMaxLength(4).IsRequired();
        b.Property(x => x.CdCcUsRes).HasMaxLength(5);
        b.Property(x => x.CdUsuario).HasMaxLength(20);

        b.HasIndex(x => new { x.CdEmpresa, x.CdFilial, x.NoMatric, x.NoProcesso });
    }
}
