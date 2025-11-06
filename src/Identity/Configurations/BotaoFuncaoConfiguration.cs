using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Domain.Entities;

namespace RhSensoERP.Identity.Infrastructure.Configurations;

/// <summary>
/// Mapeia <see cref="BotaoFuncao"/> para a tabela btfuncao.
/// Regras principais:
/// - PK composta: (CdSistema, CdFuncao, NmBotao)
/// - Campos: DcBotao (varchar(60) req.), CdAcao (char(1))
/// - FK: Funcao(CdSistema, CdFuncao)
/// </summary>
public sealed class BotaoFuncaoConfiguration : IEntityTypeConfiguration<BotaoFuncao>
{
    public void Configure(EntityTypeBuilder<BotaoFuncao> builder)
    {
        builder.ToTable("btfuncao");

        builder.HasKey(e => new { e.CdSistema, e.CdFuncao, e.NmBotao });

        builder.Property(e => e.CdSistema)
            .HasColumnName("cdsistema")
            .HasMaxLength(10)
            .IsRequired()
            .IsFixedLength();

        builder.Property(e => e.CdFuncao)
            .HasColumnName("cdfuncao")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.NmBotao)
            .HasColumnName("nmbotao")
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(e => e.DcBotao)
            .HasColumnName("dcbotao")
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(e => e.CdAcao)
            .HasColumnName("cdacao")
            .HasColumnType("char(1)");

        builder.HasOne(e => e.Funcao)
            .WithMany(f => f.Botoes)
            .HasForeignKey(e => new { e.CdSistema, e.CdFuncao })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
