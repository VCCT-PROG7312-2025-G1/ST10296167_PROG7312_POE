using ST10296167_PROG7312_POE.Models;

namespace ST10296167_PROG7312_POE.Services.User
{
    public interface IUserService
    {
        Task<bool> LoginUserAsync(Login login);

        Task LogoutAsync();
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//