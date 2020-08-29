using System.Threading;
using System.Threading.Tasks;

namespace $safeprojectname$
{
    public interface IApp
    {
        Task Run(CancellationToken cancellationToken);
    }
}
