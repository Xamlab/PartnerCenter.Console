using System.Threading;
using System.Threading.Tasks;

namespace PartnerCenter.Console.Services
{
    public interface IUpdateService
    {
        Task UpdateFlightAsync(string flightName, string bundlePath, CancellationToken cancellationToken = default);
    }
}