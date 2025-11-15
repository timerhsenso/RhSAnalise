using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Domain.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence.Configurations;

public sealed class UserGroupConfiguration : IEntityTypeConfiguration<UserGroup>
{
    public void Configure(EntityTypeBuilder<UserGroup> b)
    {
        b.ToTable("usrh1");

        // PK composta do legado
        b.HasKey(e => new { e.CdSistema, e.CdGrUser, e.CdUsuario });

        b.Property(e => e.CdUsuario)
            .HasColumnName("cdusuario")
            .HasMaxLength(30)
            .IsRequired();

        b.Property(e => e.CdGrUser)
            .HasColumnName("cdgruser")
            .HasMaxLength(30)
            .IsRequired();

        b.Property(e => e.CdSistema)
            .HasColumnName("cdsistema")
            .HasMaxLength(10)
            .IsFixedLength()
            .IsRequired();

        b.Property(e => e.IdGrupoDeUsuario)
            .HasColumnName("idgrupodeusuario");

        b.HasOne(e => e.Usuario)
            .WithMany(u => u.UserGroups)
            .HasForeignKey(e => e.CdUsuario)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(e => e.GrupoDeUsuario)
            .WithMany(g => g.UserGroups)
            .HasPrincipalKey(g => new { g.CdSistema, g.CdGrUser })
            .HasForeignKey(e => new { e.CdSistema, e.CdGrUser })
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(e => e.Sistema)
            .WithMany()
            .HasForeignKey(e => e.CdSistema)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
