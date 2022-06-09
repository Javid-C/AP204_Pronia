using AP204_Pronia.Models;
using AP204_Pronia.Utilities;
using AP204_Pronia.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AP204_Pronia.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = new AppUser
            {
                Firstname = register.Firstname,
                Lastname = register.Lastname,
                Email = register.Email,
                UserName = register.Username
            };

            IdentityResult result = await _userManager.CreateAsync(user, register.Password);

            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            await _userManager.AddToRoleAsync(user, Roles.Member.ToString());

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(LoginVM login)
        {
            AppUser user = await _userManager.FindByNameAsync(login.Username);
            if (user == null) return View();

            IList<string> roles = await _userManager.GetRolesAsync(user);

            string role = roles.FirstOrDefault(r=>r.ToLower().Trim() == Roles.Member.ToString().ToLower().Trim());
            //if (role == null)
            //{
            //    ModelState.AddModelError("", "Something went wrong. Please contact with admins");
            //    return View();
            //}
            //else
            //{
                if (login.RememberMe)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, login.Password, true, true);
                    if (!result.Succeeded)
                    {
                        if (result.IsLockedOut)
                        {
                            ModelState.AddModelError("", "You have been dismissed for 5 minutes");
                            return View();
                        }
                        ModelState.AddModelError("", "Username or password is incorrect");
                        return View();
                    }
                }
                else
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(user, login.Password, false, true);
                    if (!result.Succeeded)
                    {
                        if (result.IsLockedOut)
                        {
                            ModelState.AddModelError("", "You have been dismissed for 5 minutes");
                            return View();
                        }
                        ModelState.AddModelError("", "Username or password is incorrect");
                        return View();
                    }
                }
                return RedirectToAction("Index", "Home");
            //}
            
        }

        
        //[Authorize(Roles = "Member")]
        public async Task<IActionResult> Edit()
        {
            
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null) return NotFound();

            EditUserVM edit = new EditUserVM
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Username = user.UserName
            };

            return View(edit);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Edit(EditUserVM user)
        {
            
            AppUser existed = await _userManager.FindByNameAsync(User.Identity.Name);
            EditUserVM edit = new EditUserVM
            {
                Firstname = existed.Firstname,
                Lastname = existed.Lastname,
                Email = existed.Email,
                Username = existed.UserName
            };
            if (!ModelState.IsValid) return View(edit);

            bool result = user.Password == null && user.ConfirmPassword == null && user.CurrentPassword !=null;
            if(user.Email == null || user.Email != existed.Email)
            {
                ModelState.AddModelError("", "You can not change your email");
                return View(edit);
            }
            if (result)
            {
                existed.UserName = user.Username;
                existed.Firstname = user.Firstname;
                existed.Lastname = user.Lastname;
                await _userManager.UpdateAsync(existed);
            }
            else
            {
                existed.UserName = user.Username;
                existed.Firstname = user.Firstname;
                existed.Lastname = user.Lastname;
                if (user.CurrentPassword == user.Password)
                {
                    ModelState.AddModelError("", "You can not change password with the same password");
                    return View();
                }
                   

                IdentityResult resultEdit = await _userManager.ChangePasswordAsync(existed,user.CurrentPassword,user.Password);

                if (!resultEdit.Succeeded)
                {
                    foreach (IdentityError err in resultEdit.Errors)
                    {
                        ModelState.AddModelError("", err.Description);
                    }
                    return View(edit);
                }

            }

            return RedirectToAction("Index", "Home");
        }
        
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Show()
        {
            return Content(User.Identity.IsAuthenticated.ToString());
        }

        public async Task CreateRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole {Name = Roles.Member.ToString() });
            await _roleManager.CreateAsync(new IdentityRole {Name = Roles.Admin.ToString() });
            await _roleManager.CreateAsync(new IdentityRole {Name = Roles.SuperAdmin.ToString() });
        }
        

    }
}
