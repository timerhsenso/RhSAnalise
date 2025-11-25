// =============================================================================
// RHSENSOERP WEB - USER PERMISSIONS CACHE SERVICE INTERFACE
// =============================================================================
using RhSensoERP.Web.Models.Account;

namespace RhSensoERP.Web.Services.Permissions;

public interface IUserPermissionsCacheService
{
    void Set(string cdUsuario, UserPermissionsViewModel permissions, TimeSpan? expiration = null);
    UserPermissionsViewModel? Get(string cdUsuario);
    Task<UserPermissionsViewModel?> GetOrFetchAsync(string cdUsuario, CancellationToken ct = default);
    Task<string> GetPermissionsForFunctionAsync(string cdUsuario, string cdFuncao, CancellationToken ct = default);
    Task<bool> HasPermissionAsync(string cdUsuario, string cdFuncao, char acao, CancellationToken ct = default);
    void Remove(string cdUsuario);
    Task<UserPermissionsViewModel?> RefreshAsync(string cdUsuario, CancellationToken ct = default);
}
