using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolManagementSystem.Helpers;
using SchoolManagementSystem.Models;
using SchoolManagementSystem.Data.Entities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Azure;

namespace SchoolManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IConfiguration _configuration;

        public AccountController(
            IUserHelper userHelper,
            IMailHelper mailHelper,
            IConfiguration configuration)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
        }

        // ── LOGIN FLOW ──────────────────────────────────────────────────────────

        // BUG FIX: The old codebase had TWO Login() GET methods — the second one
        // (bare "Login" with no [HttpGet] attribute) shadowed the new selection
        // flow and caused the navbar "Login" link to land directly on the email
        // form instead of the choice screen. We now have ONE clean entry point:
        // GET /Account/Login → LoginSelection, plus named helpers EmailLogin /
        // AdminLogin that feed into the shared POST handler.

        /// <summary>
        /// GET /Account/Login  — redirects to the selection screen.
        /// This is what the navbar "Login" link hits.
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            // Always go to the choice screen first
            return RedirectToAction("LoginSelection");
        }

        /// <summary>
        /// GET /Account/LoginSelection  — shows the Email / Admin choice card.
        /// </summary>
        [HttpGet]
        public IActionResult LoginSelection()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View();
        }

        /// <summary>
        /// GET /Account/EmailLogin  — shows the login form pre-set for email users.
        /// </summary>
        [HttpGet]
        public IActionResult EmailLogin()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["LoginType"] = "email";
            return View("Login");
        }

        /// <summary>
        /// GET /Account/AdminLogin  — shows the login form pre-set for admins.
        /// </summary>
        [HttpGet]
        public IActionResult AdminLogin()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewData["LoginType"] = "admin";
            return View("Login");
        }

        /// <summary>
        /// POST /Account/Login  — processes credentials from either form variant.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string loginType = "email")
        {
            if (ModelState.IsValid)
            {
                var result = await _userHelper.LoginAsync(model);

                if (result.Succeeded)
                {
                    var user = await _userHelper.GetUserByEmailAsync(model.Username);
                    var userRole = await _userHelper.GetRoleAsync(user);

                    return userRole switch
                    {
                        "Admin" => RedirectToAction("AdminDashboard", "Dashboard"),
                        "Employee" => RedirectToAction("EmployeeDashboard", "Dashboard"),
                        "Teacher" => RedirectToAction("TeacherDashboard", "Dashboard"),
                        "Student" => RedirectToAction("StudentDashboard", "Dashboard"),
                        _ => RedirectToAction("Index", "Home"),
                    };
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }

            ViewData["LoginType"] = loginType;
            return View("Login", model);
        }

        // ── LOGOUT ──────────────────────────────────────────────────────────────

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ── REGISTRATION ────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterNewUserViewModel
            {
                TemporaryPassword = GenerateRandomPassword()
            };
            model.Password = model.TemporaryPassword;
            model.Confirm = model.TemporaryPassword;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                if (user == null)
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Username,
                        UserName = model.Username,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        DateCreated = DateTime.UtcNow
                    };

                    string temporaryPassword = GenerateRandomPassword();
                    model.TemporaryPassword = temporaryPassword;

                    var result = await _userHelper.AddUserAsync(user, temporaryPassword);
                    if (result != IdentityResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "The user could not be created.");
                        return View(model);
                    }

                    await _userHelper.AddUserToRoleAsync(user, "Pending");

                    string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    string tokenLink = Url.Action("ConfirmEmail", "Account",
                        new { userid = user.Id, token = myToken },
                        protocol: HttpContext.Request.Scheme);

                    string emailBody =
                        $"<h1>Account Created</h1>" +
                        $"<p>Your temporary password is: {temporaryPassword}</p>" +
                        $"<p>Click here to activate your account: <a href=\"{tokenLink}\">Activate Account</a></p>";

                    Helpers.Response response = _mailHelper.SendEmail(model.Username, "Account Created", emailBody);

                    if (response.IsSuccess)
                    {
                        ViewBag.Message = "User created successfully. An email was sent with further instructions.";
                        ViewBag.Links = new System.Collections.Generic.Dictionary<string, string>
                        {
                            { "Create Student",  Url.Action("Create", "Students")  },
                            { "Create Employee", Url.Action("Create", "Employees") },
                            { "Create Teacher",  Url.Action("Create", "Teachers")  }
                        };
                        return View(model);
                    }

                    ModelState.AddModelError(string.Empty, "Error sending confirmation email.");
                }
            }

            return View(model);
        }

        // ── PROFILE / CHANGE USER ────────────────────────────────────────────────

        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null) return RedirectToAction("NotAuthorized");

            var model = new ChangeUserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;
                    user.PhoneNumber = model.PhoneNumber;

                    var result = await _userHelper.UpdateUserAsync(user);
                    if (result.Succeeded)
                    {
                        await _userHelper.UpdateUserDataByRoleAsync(user);
                        return RedirectToAction("Index", "Home");
                    }
                    ModelState.AddModelError(string.Empty, "Failed to update user details.");
                }
                else
                {
                    return RedirectToAction("NotAuthorized");
                }
            }
            return View(model);
        }

        // ── EMAIL CONFIRMATION ───────────────────────────────────────────────────

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return NotFound();

            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return View("Error");

            return RedirectToAction("ChangeFirstPassword",
                new { email = user.Email, temporaryPassword = GenerateRandomPassword() });
        }

        public IActionResult ChangeFirstPassword(string email, string temporaryPassword)
        {
            var model = new ChangeFirstPasswordViewModel
            {
                Email = email,
                TemporaryPassword = temporaryPassword
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeFirstPassword(ChangeFirstPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user != null)
                {
                    var signInResult = await _userHelper.ValidatePasswordAsync(user, model.TemporaryPassword);
                    if (!signInResult.Succeeded)
                    {
                        ModelState.AddModelError(string.Empty, "The temporary password is incorrect.");
                        return View(model);
                    }

                    var result = await _userHelper.ResetPasswordWithoutTokenAsync(user, model.NewPassword);
                    if (result.Succeeded)
                    {
                        ViewBag.Message = "Your password has been changed successfully. You can now log in.";
                        return RedirectToAction("LoginSelection");
                    }

                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return View(model);
        }

        // ── PASSWORD CHANGE / RECOVERY ───────────────────────────────────────────

        public IActionResult ChangePassword() => View();

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded) return RedirectToAction("ChangeUser");
                    ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                }
            }
            return View(model);
        }

        public IActionResult RecoverPassword() => View();

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "The email does not correspond to a registered user.");
                    return View(model);
                }

                var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                var link = Url.Action("ResetPassword", "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);

                Helpers.Response response = _mailHelper.SendEmail(model.Email, "Password Reset",
                    $"<h1>Password Reset</h1>Click here to reset your password: <a href=\"{link}\">Reset Password</a>");

                if (response.IsSuccess)
                    ViewBag.Message = "Instructions to recover your password have been sent to your email.";

                return View();
            }
            return View(model);
        }

        public IActionResult ResetPassword(string token) => View();

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = await _userHelper.GetUserByEmailAsync(model.UserName);
            if (user != null)
            {
                var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded) { ViewBag.Message = "Password reset successfully."; return View(); }
                ViewBag.Message = "Error resetting the password.";
                return View(model);
            }
            ViewBag.Message = "User not found.";
            return View(model);
        }

        // ── MISC ─────────────────────────────────────────────────────────────────

        public IActionResult NotAuthorized() => View();
        public IActionResult AccessDenied() => View();

        private string GenerateRandomPassword(int length = 8)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()?_-";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
