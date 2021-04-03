using System.Threading;
using System.Threading.Tasks;
using PartnerCenter.Console.Models;

namespace PartnerCenter.Console.Services
{
    public interface ITokenService
    {
        Task<PartnerCenterToken> GetPartnerCenterTokenAsync(CancellationToken cancellationToken = default);
    }
}