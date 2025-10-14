using ST10296167_PROG7312_POE.Data;
using ST10296167_PROG7312_POE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ST10296167_PROG7312_POE.Repository.User
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.User> _userManager;

        // Controller
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public UserRepository(ApplicationDbContext context, UserManager<Models.User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // Retrieve an employee from the database based on their email
        public async Task<Models.User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
        }

        // Add a new employee to the database
        public async Task<IdentityResult> CreateUserAsync(Models.User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                Console.WriteLine("User created!");
                await _userManager.AddToRoleAsync(user, "Employee");
            }

            return result;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//