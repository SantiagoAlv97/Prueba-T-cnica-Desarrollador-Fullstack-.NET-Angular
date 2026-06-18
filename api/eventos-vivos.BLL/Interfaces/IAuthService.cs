using System.Threading.Tasks;
using eventos_vivos.BDO.DTOs.Auth;

namespace eventos_vivos.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginGoogleAsync(GoogleLoginRequest request);
    }
}
