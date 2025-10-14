using ST10296167_PROG7312_POE.Models;
using ST10296167_PROG7312_POE.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ST10296167_PROG7312_POE.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        // Controller
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Views
        //------------------------------------------------------------------------------------------------------------------------------------------//
        public IActionResult Login()
        {
            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            return View();
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//

        // Methods
        //------------------------------------------------------------------------------------------------------------------------------------------//
        // Employee login
        [HttpPost]
        public async Task<IActionResult> LoginUser(Login login)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", login);
            }

            var isValidUser = await _userService.LoginUserAsync(login);

            if (isValidUser)
            {
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Invalid email or password";
            return View("Login", login);
        }

        // Logout employee
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
        //------------------------------------------------------------------------------------------------------------------------------------------//
    }
}
//--------------------------------------------------------X END OF FILE X-------------------------------------------------------------------//