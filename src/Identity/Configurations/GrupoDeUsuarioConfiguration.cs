using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Domain.Entities;

namespace RhSensoERP.Identity.Infrastructure.Configurations;

/// <summary>
/// Mapeia <see cref="GrupoDeUsuario"/> para a tabela gurh1.
/// Regras principais:
/// - PK composta: (CdSistema, CdGrUser) — aderente ao legado
/// - Alternate Key: Id (BaseEntity) para integrações modernas
/// - Campos: DcGrUser (varchar(60)?)
/// - FK: Sistema(CdSistema)
/// </summary>
public sealed class GrupoDeUsuarioConfiguration : IEntityTypeConfiguration<GrupoDeUsuario>
{
    public void Configure(EntityTypeBuilder<GrupoDeUsuario> builder)
    {
        builder.ToTable("gurh1");

        // PK composta conforme padrão legado
        builder.HasKey(e => new { e.CdSistema, e.CdGrUser });

        // Alternate Key no Id para facilitar integrações internas modernas
        builder.HasAlternateKey(e => e.Id);

        builder.Property(e => e.CdGrUser)
            .HasColumnName("cdgruser")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.DcGrUser)
            .HasColumnName("dcgruser")
            .HasMaxLength(60);

        builder.Property(e => e.CdSistema)
            .HasColumnName("cdsistema")
            .HasMaxLength(10)
            .IsRequired()
            .IsFixedLength();

        // Se o banco tiver default (newsequentialid()) para a coluna Id, aplique:
        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.HasOne(e => e.Sistema)
            .WithMany()
            .HasForeignKey(e => e.CdSistema)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.GrupoFuncoes)
            .WithOne(gf => gf.GrupoDeUsuario!)
            .HasPrincipalKey(e => new { e.CdSistema, e.CdGrUser }) // principal = PK composta
            .HasForeignKey(gf => new { gf.CdSistema, gf.CdGrUser }) // FK composta no filho
            .OnDelete(DeleteBehavior.Cascade);
    }
}
