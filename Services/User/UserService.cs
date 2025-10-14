using ST10296167_PROG7312_POE.Models;
using ST10296167_PROG7312_POE.Repository.User;
using Microsoft.AspNetCore.Identity;

namespace ST10296167_PROG7312_POE.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<Models.User> _signInManager;

        // Controller
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public UserService(IUserRepository userRepository, SignInManager<Models.User> signInManager)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
        
        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // Log in a user based on the entered email and password 
        public async Task<bool> LoginUserAsync(Login login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Logs a user out of their account 
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//