// src/Web/Models/Bancos/BancosListViewModel.cs

using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Bancos;

/// <summary>
/// ViewModel para a página de listagem de Bancos.
/// </summary>
public sealed class BancosListViewModel : BaseListViewModel
{
    public BancosListViewModel()
    {
        PageTitle = "Gestão de Bancos";
        PageSubtitle = "Cadastro e manutenção de bancos";
        PageIcon = "fas fa-university";
        ControllerName = "Bancos";
        CdFuncao = "BANCOS";
    }
}
