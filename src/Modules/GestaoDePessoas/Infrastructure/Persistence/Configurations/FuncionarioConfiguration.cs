// src/Modules/GestaoDePessoas/Infrastructure/Persistence/Configurations/FuncionarioConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations
{
    public class FuncionarioConfiguration : IEntityTypeConfiguration<Funcionario>
    {
        public void Configure(EntityTypeBuilder<Funcionario> builder)
        {
            builder.ToTable("func1");

            // Chave Primária Composta
            builder.HasKey(f => new { f.Matricula, f.CodigoEmpresa, f.CodigoFilial });

            // Mapeamento das Propriedades
            builder.Property(f => f.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newsequentialid()");

            builder.Property(f => f.Matricula)
                .HasColumnName("nomatric")
                .HasColumnType("char(8)")
                .IsRequired();

            builder.Property(f => f.CodigoEmpresa)
                .HasColumnName("cdempresa")
                .IsRequired();

            builder.Property(f => f.CodigoFilial)
                .HasColumnName("cdfilial")
                .IsRequired();

            builder.Property(f => f.NomeColaborador)
                .HasColumnName("nmcolab")
                .HasMaxLength(60);

            builder.Property(f => f.NomeGuerra)
                .HasColumnName("nmguerra")
                .HasMaxLength(15);

            builder.Property(f => f.CodigoCentroCusto)
                .HasColumnName("cdccusto")
                .HasColumnType("char(5)")
                .IsRequired();

            builder.Property(f => f.TipoColaborador)
                .HasColumnName("tpcolab");

            builder.Property(f => f.DataAdmissao)
                .HasColumnName("dtadmissao")
                .IsRequired();

            builder.Property(f => f.DataDemissao)
                .HasColumnName("dtdemissao");

            builder.Property(f => f.DataAviso)
                .HasColumnName("dtaviso");

            // Dados Bancários - Recebimento
            builder.Property(f => f.CodigoBancoRecebimento)
                .HasColumnName("cdbanrec")
                .HasColumnType("char(3)");

            builder.Property(f => f.CodigoAgenciaRecebimento)
                .HasColumnName("cdagerec")
                .HasColumnType("char(4)");

            builder.Property(f => f.NumeroContaRecebimento)
                .HasColumnName("noctarec")
                .HasMaxLength(13);

            builder.Property(f => f.DigitoVerificadorAgenciaRecebimento)
                .HasColumnName("dvagerec")
                .HasColumnType("char(1)");

            builder.Property(f => f.DigitoVerificadorContaRecebimento)
                .HasColumnName("dvctarec")
                .HasMaxLength(2);

            builder.Property(f => f.TipoContaRecebimento)
                .HasColumnName("tpctarec")
                .HasColumnType("char(1)");

            // FGTS
            builder.Property(f => f.CodigoBancoFGTS)
                .HasColumnName("cdbanfgts")
                .HasColumnType("char(3)");

            builder.Property(f => f.CodigoAgenciaFGTS)
                .HasColumnName("cdagefgts")
                .HasColumnType("char(4)");

            builder.Property(f => f.NumeroContaFGTS)
                .HasColumnName("noctafgts")
                .HasColumnType("char(13)");

            builder.Property(f => f.CodigoOpcaoFGTS)
                .HasColumnName("cdopcfgts")
                .HasColumnType("char(1)");

            builder.Property(f => f.DigitoVerificadorAgenciaFGTS)
                .HasColumnName("dvagefgts")
                .HasColumnType("char(1)");

            builder.Property(f => f.DataOpcaoFGTS)
                .HasColumnName("dtopcfgts");

            builder.Property(f => f.SaldoFGTS)
                .HasColumnName("vlsaldofgts");

            builder.Property(f => f.RecebeuFGTS)
                .HasColumnName("flrecfgts");

            // Endereço
            builder.Property(f => f.Endereco)
                .HasColumnName("dcendereco")
                .HasMaxLength(60);

            builder.Property(f => f.NumeroEndereco)
                .HasColumnName("end_numero")
                .HasMaxLength(10);

            builder.Property(f => f.ComplementoEndereco)
                .HasColumnName("end_comp")
                .HasMaxLength(60);

            builder.Property(f => f.Bairro)
                .HasColumnName("dcbairro")
                .HasMaxLength(40);

            builder.Property(f => f.CodigoMunicipio)
                .HasColumnName("cdmunicip")
                .HasColumnType("char(5)");

            builder.Property(f => f.SiglaEstado)
                .HasColumnName("sgestado")
                .HasColumnType("char(2)");

            builder.Property(f => f.CEP)
                .HasColumnName("nocep")
                .HasColumnType("char(8)");

            builder.Property(f => f.PontoReferencia)
                .HasColumnName("dcptreferencia")
                .HasMaxLength(100);

            // Contatos
            builder.Property(f => f.Telefone)
                .HasColumnName("notelefone")
                .HasMaxLength(30);

            builder.Property(f => f.DDD)
                .HasColumnName("noddd")
                .HasMaxLength(3);

            builder.Property(f => f.Telefone2)
                .HasColumnName("notelefone2")
                .HasMaxLength(15);

            builder.Property(f => f.DDD2)
                .HasColumnName("noddd2")
                .HasMaxLength(3);

            builder.Property(f => f.DDD3)
                .HasColumnName("noddd3")
                .HasColumnType("char(2)");

            builder.Property(f => f.Telefone3)
                .HasColumnName("notelefone3")
                .HasColumnType("char(30)");

            builder.Property(f => f.Contato2)
                .HasColumnName("contato2")
                .HasColumnType("char(60)");

            builder.Property(f => f.Contato3)
                .HasColumnName("contato3")
                .HasColumnType("char(60)");

            builder.Property(f => f.Email)
                .HasColumnName("dcemail")
                .HasMaxLength(80);

            builder.Property(f => f.EmailAlternativo)
                .HasColumnName("emailalternativo")
                .HasMaxLength(60);

            builder.Property(f => f.Ramal)
                .HasColumnName("dcramal")
                .HasMaxLength(9);

            builder.Property(f => f.RamalTransporte)
                .HasColumnName("cdramal_transp")
                .HasColumnType("char(5)");

            // Dados Pessoais
            builder.Property(f => f.EstadoCivil)
                .HasColumnName("cdestcivil")
                .HasColumnType("char(1)");

            builder.Property(f => f.DataNascimento)
                .HasColumnName("dtnasc");

            builder.Property(f => f.Sexo)
                .HasColumnName("cdsexo")
                .HasColumnType("char(1)");

            builder.Property(f => f.NomePai)
                .HasColumnName("nmpaicolab")
                .HasMaxLength(60);

            builder.Property(f => f.NomeMae)
                .HasColumnName("nmmaecolab")
                .HasMaxLength(60);

            builder.Property(f => f.TipoSanguineo)
                .HasColumnName("cdtpsangue")
                .HasColumnType("char(2)");

            builder.Property(f => f.CodigoRaca)
                .HasColumnName("cod_raca");

            builder.Property(f => f.CodigoDeficiente)
                .HasColumnName("cod_deficiente")
                .HasColumnType("char(1)");

            // Documentos
            builder.Property(f => f.CPF)
                .HasColumnName("nocpf")
                .HasMaxLength(11);

            builder.Property(f => f.PIS)
                .HasColumnName("nopis")
                .HasMaxLength(11);

            builder.Property(f => f.DataInscricaoPIS)
                .HasColumnName("dtinscpis");

            builder.Property(f => f.RG)
                .HasColumnName("norg")
                .HasColumnType("char(15)");

            builder.Property(f => f.OrgaoEmissorRG)
                .HasColumnName("sgorgrg")
                .HasMaxLength(20);

            builder.Property(f => f.EstadoRG)
                .HasColumnName("sgestrg")
                .HasColumnType("char(2)");

            builder.Property(f => f.DataRG)
                .HasColumnName("dtrg");

            builder.Property(f => f.DataValidadeRGEstrangeiro)
                .HasColumnName("dtvrgext");

            builder.Property(f => f.CartaoSUS)
                .HasColumnName("nocartaosus")
                .HasMaxLength(20);

            // Documentos Profissionais
            builder.Property(f => f.CodigoCargo)
                .HasColumnName("cdcargo")
                .HasColumnType("char(5)");

            builder.Property(f => f.NumeroCarteiraTrabalho)
                .HasColumnName("nocartprof")
                .HasMaxLength(10);

            builder.Property(f => f.SerieCarteiraTrabalho)
                .HasColumnName("noserie")
                .HasColumnType("char(5)");

            builder.Property(f => f.EstadoCarteiraTrabalho)
                .HasColumnName("sgestcart")
                .HasColumnType("char(2)");

            builder.Property(f => f.DataCarteiraTrabalho)
                .HasColumnName("dtcartprof");

            // Título Eleitoral
            builder.Property(f => f.NumeroTituloEleitoral)
                .HasColumnName("notitelei")
                .HasColumnType("char(12)");

            builder.Property(f => f.SecaoTituloEleitoral)
                .HasColumnName("nosecaotit")
                .HasColumnType("char(4)");

            builder.Property(f => f.ZonaTituloEleitoral)
                .HasColumnName("nozonatit")
                .HasColumnType("char(3)");

            builder.Property(f => f.EstadoTituloEleitoral)
                .HasColumnName("sgesttit")
                .HasColumnType("char(2)");

            builder.Property(f => f.DataTituloEleitoral)
                .HasColumnName("dttitelei");

            // Dados Trabalhistas
            builder.Property(f => f.CodigoCategoria)
                .HasColumnName("cdcategori")
                .HasColumnType("char(2)");

            builder.Property(f => f.CodigoCausaRescisao)
                .HasColumnName("cdcausres")
                .HasColumnType("char(2)");

            builder.Property(f => f.HorasSemanais)
                .HasColumnName("nohssem");

            builder.Property(f => f.HorasMensais)
                .HasColumnName("nohsmes");

            builder.Property(f => f.CodigoInstrucao)
                .HasColumnName("cdinstruc")
                .HasColumnType("char(2)");

            builder.Property(f => f.CodigoNacionalidade)
                .HasColumnName("cdnacion")
                .HasColumnType("char(2)");

            builder.Property(f => f.AnoChegadaPais)
                .HasColumnName("aachegpais")
                .HasColumnType("char(4)");

            builder.Property(f => f.CodigoSindicato)
                .HasColumnName("cdsindicat")
                .HasColumnType("char(2)");

            builder.Property(f => f.FlagSindicato)
                .HasColumnName("flsindicat")
                .HasDefaultValue(0);

            builder.Property(f => f.FlagContribuicaoSindical)
                .HasColumnName("flcontsin")
                .HasColumnType("char(1)");

            builder.Property(f => f.CodigoSituacao)
                .HasColumnName("cdsituacao")
                .HasColumnType("char(2)");

            builder.Property(f => f.CodigoVinculo)
                .HasColumnName("cdvincul")
                .HasColumnType("char(2)");

            builder.Property(f => f.CodigoNivel)
                .HasColumnName("cdnivel")
                .HasColumnType("char(5)");

            builder.Property(f => f.FlagFrequencia)
                .HasColumnName("flfreq");

            builder.Property(f => f.CodigoEmpregado)
                .HasColumnName("cdempreg")
                .HasColumnType("char(2)");

            builder.Property(f => f.CodigoTurma)
                .HasColumnName("cdturma")
                .HasColumnType("char(2)");

            builder.Property(f => f.CodigoCarreira)
                .HasColumnName("cdcarreira");

            // Local Nascimento
            builder.Property(f => f.MunicipioNascimento)
                .HasColumnName("dcmuninasc")
                .HasColumnType("char(20)");

            builder.Property(f => f.CodigoMunicipioNascimento)
                .HasColumnName("cdmuninasc")
                .HasColumnType("char(5)");

            builder.Property(f => f.EstadoNascimento)
                .HasColumnName("sgestadonasc")
                .HasColumnType("char(2)");

            // Estrangeiro
            builder.Property(f => f.TipoVisto)
                .HasColumnName("cdtpvisto")
                .HasColumnType("char(10)");

            builder.Property(f => f.DataValidadeCapacidadeExtrangeiro)
                .HasColumnName("dtvcapfext");

            // Controle
            builder.Property(f => f.MatriculaAnterior)
                .HasColumnName("nomatant")
                .HasColumnType("char(8)");

            builder.Property(f => f.CodigoPessoa)
                .HasColumnName("nopessoa")
                .HasColumnType("char(6)");

            builder.Property(f => f.CodigoColaborador)
                .HasColumnName("cdcolab")
                .HasColumnType("char(6)");

            builder.Property(f => f.CodigoOcorrencia)
                .HasColumnName("cdocorr")
                .HasColumnType("char(2)");

            builder.Property(f => f.DataUltimaExperiencia)
                .HasColumnName("dtultexper");

            builder.Property(f => f.DataUltimaMovimentacao)
                .HasColumnName("dtultmov");

            builder.Property(f => f.CodigoUsuario)
                .HasColumnName("cdusuario")
                .HasColumnType("char(20)");

            builder.Property(f => f.FlagContratoExperiencia)
                .HasColumnName("flcexperie");

            builder.Property(f => f.DataContratoExperiencia)
                .HasColumnName("dtcexperie");

            builder.Property(f => f.FlagPrevidenciaComplementar)
                .HasColumnName("flprevcomp");

            builder.Property(f => f.CodigoGrupoPPP)
                .HasColumnName("cdgrupo_ppp");

            builder.Property(f => f.DataTransferencia)
                .HasColumnName("dttransf");

            builder.Property(f => f.DataVencimentoContrato2)
                .HasColumnName("dtvenccontr2");

            builder.Property(f => f.DataHomologacao)
                .HasColumnName("dthomologacao");

            builder.Property(f => f.DataLimiteAcesso)
                .HasColumnName("dtlimite_acesso");

            builder.Property(f => f.DataAdmissaoHistorica)
                .HasColumnName("dtadmhis");

            builder.Property(f => f.MatriculaSAP)
                .HasColumnName("matsap")
                .HasColumnType("char(30)");

            // Foreign Key IDs
            builder.Property(f => f.IdCentroCusto)
                .HasColumnName("idcentrodecusto");

            builder.Property(f => f.IdCargo)
                .HasColumnName("idcargo");

            builder.Property(f => f.IdMunicipioNaturalidade)
                .HasColumnName("idmunicipionaturalidade");

            builder.Property(f => f.IdGrauInstrucao)
                .HasColumnName("idgraudeinstrucao");

            builder.Property(f => f.IdSituacao)
                .HasColumnName("idsituacao");

            builder.Property(f => f.IdMunicipioEndereco)
                .HasColumnName("idmunicipioendereco");

            builder.Property(f => f.IdVinculoEmpregaticio)
                .HasColumnName("idvinculoempregaticio");

            builder.Property(f => f.IdSindicato)
                .HasColumnName("idsindicato");

            builder.Property(f => f.IdBancoRecebimento)
                .HasColumnName("idbancorecebimento");

            builder.Property(f => f.IdAgenciaRecebimento)
                .HasColumnName("idagenciarecebimento");

            builder.Property(f => f.IdFilial)
                .HasColumnName("idfilial");

            builder.Property(f => f.IdMotivoRescisao)
                .HasColumnName("idmotivorescisao");

            // Índice único
            builder.HasIndex(f => f.Id)
                .HasDatabaseName("uk_func1_id")
                .IsUnique();

            // Relacionamentos
            builder.HasOne(f => f.Empresa)
                .WithMany(e => e.Funcionarios)
                .HasForeignKey(f => f.CodigoEmpresa)
                .HasConstraintName("FK_func1_temp1_cdempresa");

            builder.HasOne(f => f.Filial)
                .WithMany(fil => fil.Funcionarios)
                .HasForeignKey(f => new { f.CodigoEmpresa, f.CodigoFilial })
                .HasConstraintName("FK_func1_test1_cdempresa_cdfilial");

            builder.HasOne(f => f.CentroCusto)
                .WithMany(cc => cc.Funcionarios)
                .HasForeignKey(f => f.IdCentroCusto)
                .HasConstraintName("fk_func1_idcentrodecusto");

            builder.HasOne(f => f.Cargo)
                .WithMany(c => c.Funcionarios)
                .HasForeignKey(f => f.IdCargo)
                .HasConstraintName("fk_func1_idcargo");

            builder.HasOne(f => f.MunicipioNaturalidade)
                .WithMany(m => m.FuncionariosNaturalidade)
                .HasForeignKey(f => f.IdMunicipioNaturalidade)
                .HasConstraintName("FK_func1_muni1_idmunicipionaturalidade");

            builder.HasOne(f => f.MunicipioEndereco)
                .WithMany(m => m.FuncionariosEndereco)
                .HasForeignKey(f => f.IdMunicipioEndereco)
                .HasConstraintName("FK_func1_muni1_idmunicipioendereco");

            builder.HasOne(f => f.GrauInstrucao)
                .WithMany(gi => gi.Funcionarios)
                .HasForeignKey(f => f.IdGrauInstrucao)
                .HasConstraintName("FK_func1_tgin1_idgraudeinstrucao");

            builder.HasOne(f => f.Situacao)
                .WithMany(s => s.Funcionarios)
                .HasForeignKey(f => f.IdSituacao)
                .HasConstraintName("FK_func1_tsitu1_idsituacao");

            builder.HasOne(f => f.VinculoEmpregaticio)
                .WithMany(v => v.Funcionarios)
                .HasForeignKey(f => f.IdVinculoEmpregaticio)
                .HasConstraintName("FK_func1_tvin1_idvinculoempregaticio");

            builder.HasOne(f => f.Sindicato)
                .WithMany(s => s.Funcionarios)
                .HasForeignKey(f => f.IdSindicato)
                .HasConstraintName("FK_func1_sind1_idsindicato");

            builder.HasOne(f => f.BancoRecebimento)
                .WithMany(b => b.Funcionarios)
                .HasForeignKey(f => f.IdBancoRecebimento)
                .HasConstraintName("FK_func1_tban1_idbancorecebimento");

            builder.HasOne(f => f.AgenciaRecebimento)
                .WithMany(a => a.Funcionarios)
                .HasForeignKey(f => f.IdAgenciaRecebimento)
                .HasConstraintName("FK_func1_tage1_idagenciarecebimento");

            builder.HasOne(f => f.MotivoRescisao)
                .WithMany(m => m.Funcionarios)
                .HasForeignKey(f => f.IdMotivoRescisao)
                .HasConstraintName("FK_func1_tcre1_idmotivorescisao");
        }
    }
}