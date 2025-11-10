// Vprh2Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Infrastructure.Persistence.Configurations;

public sealed class Vprh2Configuration : IEntityTypeConfiguration<Vprh2>
{
    public void Configure(EntityTypeBuilder<Vprh2> b)
    {
        b.ToTable("vprh2");

        b.HasKey(x => x.Id);

        b.Property(x => x.CdValor).HasMaxLength(4).IsRequired();

        // FK interna: vprh2(idvalor) -> vprh1(id)
        b.HasOne<Vprh1>()
         .WithMany()
         .HasForeignKey(x => x.IdValor)
         .OnDelete(DeleteBehavior.Cascade)
         .HasConstraintName("FK_vprh2_vprh1_idvalor");

        // Índice conforme DDL
        b.HasIndex(x => x.IdValor).HasDatabaseName("IX_vprh2_idvalor");
    }
}
