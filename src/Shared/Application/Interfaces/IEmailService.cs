#nullable enable
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace RhSensoERP.Shared.Application.Interfaces;
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, IEnumerable<string>? cc = null, CancellationToken ct = default);
}
