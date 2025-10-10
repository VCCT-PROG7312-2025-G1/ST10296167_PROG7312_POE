using ST10296167_PROG7312_POE.Models;
using Microsoft.AspNetCore.Identity;

namespace ST10296167_PROG7312_POE.Repository.User
{
    public interface IUserRepository
    {
        Task<Models.User> GetUserByEmailAsync(string email);

        Task<IdentityResult> CreateUserAsync(Models.User user, string password);
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//