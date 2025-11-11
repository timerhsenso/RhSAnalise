// src/Modules/GestaoDePessoas/Infrastructure/Persistence/Configurations/FilialConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations
{
    public class FilialConfiguration : IEntityTypeConfiguration<Filial>
    {
        public void Configure(EntityTypeBuilder<Filial> builder)
        {
            builder.ToTable("test1");

            // Chave Primária Composta
            builder.HasKey(f => new { f.CodigoEmpresa, f.CodigoFilial });

            builder.Property(f => f.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newsequentialid()");

            builder.Property(f => f.CodigoEmpresa)
                .HasColumnName("cdempresa")
                .IsRequired();

            builder.Property(f => f.CodigoFilial)
                .HasColumnName("cdfilial")
                .IsRequired();

            builder.Property(f => f.NomeFantasia)
                .HasColumnName("nmfantasia")
                .HasMaxLength(30);

            builder.Property(f => f.DescricaoEstabelecimento)
                .HasColumnName("dcestab")
                .HasMaxLength(60);

            builder.Property(f => f.Endereco)
                .HasColumnName("dcendereco")
                .HasMaxLength(60);

            builder.Property(f => f.Bairro)
                .HasColumnName("dcbairro")
                .HasMaxLength(60);

            builder.Property(f => f.SiglaEstado)
                .HasColumnName("sgestado")
                .HasColumnType("char(2)");

            builder.Property(f => f.CEP)
                .HasColumnName("nocep")
                .HasMaxLength(9);

            builder.Property(f => f.Telefone)
                .HasColumnName("notelefone")
                .HasMaxLength(15);

            builder.Property(f => f.Fax)
                .HasColumnName("nofax")
                .HasMaxLength(10);

            builder.Property(f => f.MatriculaINPS)
                .HasColumnName("nomatinps")
                .HasColumnType("char(15)");

            builder.Property(f => f.CNPJ)
                .HasColumnName("cdcgc")
                .HasColumnType("char(15)");

            builder.Property(f => f.CodigoMunicipio)
                .HasColumnName("cdmunicip")
                .HasColumnType("char(5)");

            builder.Property(f => f.CodigoAtividadeINPS)
                .HasColumnName("cdatvinps")
                .HasMaxLength(7);

            builder.Property(f => f.CodigoAtividadeIBGE)
                .HasColumnName("cdativibge")
                .HasColumnType("char(5)");

            builder.Property(f => f.CodigoNaturezaJuridica)
                .HasColumnName("cdnatjus")
                .HasColumnType("char(4)");

            builder.Property(f => f.InscricaoEstadual)
                .HasColumnName("noinscriest")
                .HasMaxLength(15);

            builder.Property(f => f.CodigoMunicipioRAIS)
                .HasColumnName("cdmunirais")
                .HasColumnType("char(7)");

            builder.Property(f => f.NumeroProprietario)
                .HasColumnName("noproprie")
                .HasMaxLength(2);

            builder.Property(f => f.InscricaoCEI)
                .HasColumnName("noinscricei")
                .HasColumnType("char(15)");

            builder.Property(f => f.CodigoAtividadeIR)
                .HasColumnName("cdativir")
                .HasColumnType("char(4)");

            builder.Property(f => f.InscricaoMunicipal)
                .HasColumnName("noinscrimun")
                .HasMaxLength(15);

            builder.Property(f => f.CodigoTabelaSalarial)
                .HasColumnName("cdtbsal")
                .HasColumnType("char(2)");

            builder.Property(f => f.CodigoCalculoDigital)
                .HasColumnName("cdcalcdig")
                .HasColumnType("char(2)");

            builder.Property(f => f.NumeroEndereco)
                .HasColumnName("numero")
                .HasMaxLength(6);

            builder.Property(f => f.CodigoBancoFGTS)
                .HasColumnName("cdbcofgts")
                .HasColumnType("char(3)");

            builder.Property(f => f.CodigoAgenciaFGTS)
                .HasColumnName("cdagefgts")
                .HasColumnType("char(4)");

            builder.Property(f => f.CodigoIdentificadorEmpresaCEF)
                .HasColumnName("cdidempcef")
                .HasColumnType("char(13)");

            builder.Property(f => f.FlagRecolhimentoFGTS)
                .HasColumnName("flrecfgts")
                .HasColumnType("char(2)");

            builder.Property(f => f.CodigoSimples)
                .HasColumnName("cdsimples")
                .HasColumnType("char(2)");

            builder.Property(f => f.FlagCNAE)
                .HasColumnName("flcnae")
                .HasColumnType("char(1)");

            builder.Property(f => f.CodigoFPAS)
                .HasColumnName("cdfpas")
                .HasColumnType("char(3)");

            builder.Property(f => f.CodigoAreaTrabalho)
                .HasColumnName("cdactrab")
                .HasColumnType("char(7)");

            builder.Property(f => f.CodigoTerceiros)
                .HasColumnName("cdterc")
                .HasColumnType("char(4)");

            builder.Property(f => f.CodigoGPS)
                .HasColumnName("cdgps")
                .HasColumnType("char(4)");

            builder.Property(f => f.PercentualConvenio)
                .HasColumnName("pcconv")
                .HasColumnType("numeric(12,2)");

            builder.Property(f => f.PercentualSAT)
                .HasColumnName("pcsat")
                .HasColumnType("numeric(12,2)");

            builder.Property(f => f.PercentualTerceiros)
                .HasColumnName("pcterc")
                .HasColumnType("numeric(12,2)");

            builder.Property(f => f.PercentualEmpresa)
                .HasColumnName("pcemp")
                .HasColumnType("numeric(12,2)");

            builder.Property(f => f.NumeroCAGED)
                .HasColumnName("nocaged")
                .HasColumnType("char(7)");

            builder.Property(f => f.Declaracao)
                .HasColumnName("declara")
                .HasColumnType("char(2)");

            builder.Property(f => f.Alteracao)
                .HasColumnName("alteracao")
                .HasColumnType("char(2)");

            builder.Property(f => f.InscricaoTerceiros)
                .HasColumnName("noinscter")
                .HasMaxLength(14);

            builder.Property(f => f.NomeServico)
                .HasColumnName("nmtservico")
                .HasMaxLength(40);

            builder.Property(f => f.EnderecoServico)
                .HasColumnName("dcendetser")
                .HasMaxLength(60);

            builder.Property(f => f.BairroServico)
                .HasColumnName("dcbairtser")
                .HasMaxLength(60);

            builder.Property(f => f.CEPServico)
                .HasColumnName("noceptser")
                .HasMaxLength(8);

            builder.Property(f => f.CidadeServico)
                .HasColumnName("dccidtser")
                .HasMaxLength(20);

            builder.Property(f => f.EstadoServico)
                .HasColumnName("sgesttser")
                .HasColumnType("char(2)");

            builder.Property(f => f.TipoInscricaoTerceiros)
                .HasColumnName("tpinscter")
                .HasColumnType("char(1)");

            builder.Property(f => f.UltimaMatricula)
                .HasColumnName("noultmatric")
                .HasColumnType("char(8)");

            builder.Property(f => f.UltimaFicha)
                .HasColumnName("noultrficha")
                .HasColumnType("char(6)");

            builder.Property(f => f.CheckMatricula)
                .HasColumnName("chmatric")
                .HasColumnType("char(1)");

            builder.Property(f => f.CheckNumeroRegistro)
                .HasColumnName("chnoregist")
                .HasColumnType("char(1)");

            builder.Property(f => f.CodigoContaAdicional)
                .HasColumnName("cdcontaadn")
                .HasColumnType("char(4)");

            builder.Property(f => f.PercentualConvenioFloat)
                .HasColumnName("pc_conv");

            builder.Property(f => f.PercentualSATFloat)
                .HasColumnName("pc_sat");

            builder.Property(f => f.PercentualTerceirosFloat)
                .HasColumnName("pc_terc");

            builder.Property(f => f.PercentualEmpresaFloat)
                .HasColumnName("pc_emp");

            builder.Property(f => f.CodigoFornecedor)
                .HasColumnName("cdfornec");

            builder.Property(f => f.CodigoTipoInscricao)
                .HasColumnName("cdtpinscri");

            builder.Property(f => f.CodigoMotivoOcorrenciaHE)
                .HasColumnName("cdmotocHE")
                .HasColumnType("char(4)");

            builder.Property(f => f.CodigoMotivoOcorrenciaFalta)
                .HasColumnName("cdmotocFALTA")
                .HasColumnType("char(4)");

            builder.Property(f => f.TipoOcorrenciaHE)
                .HasColumnName("tpocorrHE");

            builder.Property(f => f.TipoOcorrenciaFalta)
                .HasColumnName("tpocorrFALTA");

            builder.Property(f => f.FlagAdicionalNoturno)
                .HasColumnName("FLADTNOT");

            builder.Property(f => f.InicioAdicionalNoturno)
                .HasColumnName("INIADTNOT")
                .HasMaxLength(5);

            builder.Property(f => f.FimAdicionalNoturno)
                .HasColumnName("FIMADTNOT")
                .HasMaxLength(5);

            builder.Property(f => f.FlagLimiteTroca)
                .HasColumnName("FLLIMTROCA");

            builder.Property(f => f.LimiteTroca)
                .HasColumnName("LIMTROCA");

            builder.Property(f => f.CodigoEmpresaContabil)
                .HasColumnName("cdempctb");

            builder.Property(f => f.CodigoFilialContabil)
                .HasColumnName("cdfilctb");

            builder.Property(f => f.ValorHoraAdicional)
                .HasColumnName("VALORHORAADN")
                .HasColumnType("numeric(12,2)");

            builder.Property(f => f.FlagDescontaAlmoco)
                .HasColumnName("FLDESCONTAALMOCO");

            builder.Property(f => f.FlagMinimoHE)
                .HasColumnName("FLMINHE");

            builder.Property(f => f.ValorMinimoHE)
                .HasColumnName("VLMINHE");

            builder.Property(f => f.TipoOcorrenciaAtraso)
                .HasColumnName("TPOCORRATRAZO");

            builder.Property(f => f.CodigoMotivoOcorrenciaAtraso)
                .HasColumnName("CDMOTOCATRAZO")
                .HasColumnType("char(4)");

            builder.Property(f => f.Email)
                .HasColumnName("EMAIL")
                .HasMaxLength(30);

            builder.Property(f => f.QuantidadeHoraMaxima)
                .HasColumnName("CQTHORAMAX")
                .HasMaxLength(30);

            builder.Property(f => f.Login)
                .HasColumnName("CLGN")
                .HasMaxLength(30);

            builder.Property(f => f.Senha)
                .HasColumnName("USESENH")
                .HasMaxLength(30);

            builder.Property(f => f.Host)
                .HasColumnName("CHOST")
                .HasMaxLength(30);

            builder.Property(f => f.PercentualFAP)
                .HasColumnName("pc_fap")
                .HasColumnType("numeric(12,6)");

            builder.Property(f => f.CodigoSindicatoResponsavel)
                .HasColumnName("cdsindicatres")
                .HasColumnType("char(2)");

            builder.Property(f => f.FlagAtivoFilial)
                .HasColumnName("flativofilial");

            builder.Property(f => f.DDD)
                .HasColumnName("noddd")
                .HasMaxLength(3);

            builder.Property(f => f.CodigoTipoSistemaRAIS)
                .HasColumnName("cod_tipo_sisponto_rais")
                .HasColumnType("char(2)");

            builder.Property(f => f.DataInicioValidade)
                .HasColumnName("dtinivalidade");

            builder.Property(f => f.PercentualFilantropia)
                .HasColumnName("pcfilantropia")
                .HasColumnType("numeric(5,2)");

            builder.Property(f => f.ComplementoEndereco)
                .HasColumnName("DCEND_COMP")
                .HasMaxLength(60);

            builder.Property(f => f.IdEmpresa)
                .HasColumnName("idempresa");

            builder.Property(f => f.IdMunicipioEndereco)
                .HasColumnName("idmunicipioendereco");

            builder.Property(f => f.IdSindicato)
                .HasColumnName("idsindicato");

            builder.Property(f => f.IdLotacaoTributaria)
                .HasColumnName("idlotacaotributaria");

            builder.Property(f => f.IndicadorSubstituicaoPatronalObra)
                .HasColumnName("indsubstpatrobra");

            builder.Property(f => f.NumeroProcessoPCD)
                .HasColumnName("numeroprocessoapcd")
                .HasMaxLength(20);

            builder.Property(f => f.NumeroProcessoAprendiz)
                .HasColumnName("numeroprocessoaprendiz")
                .HasMaxLength(20);

            builder.Property(f => f.TipoCAEPessoaFisica)
                .HasColumnName("tpcaepf");

            // Índices
            builder.HasIndex(f => new { f.CodigoEmpresa, f.CodigoFilial })
                .HasDatabaseName("test1nx1");

            // Relacionamentos
            builder.HasOne(f => f.Empresa)
                .WithMany(e => e.Filiais)
                .HasForeignKey(f => f.CodigoEmpresa)
                .HasConstraintName("FK_test1_temp1_cdempresa");

            builder.HasOne(f => f.MunicipioEndereco)
                .WithMany(m => m.Filiais)
                .HasForeignKey(f => f.IdMunicipioEndereco)
                .HasConstraintName("FK_test1_muni1_idmunicipioendereco");

            builder.HasOne(f => f.Sindicato)
                .WithMany(s => s.Filiais)
                .HasForeignKey(f => f.IdSindicato)
                .HasConstraintName("FK_test1_sind1_idsindicato");
        }
    }
}