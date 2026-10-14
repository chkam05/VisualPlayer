using System.Threading.Tasks;

namespace VisualPlayer.Interfaces;

public interface IInternalCloseGuard
{
    Task<bool> CanCloseAsync();
}
