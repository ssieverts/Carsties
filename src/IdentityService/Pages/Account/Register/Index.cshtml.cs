using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityService.Models;
using IdentityService.Pages.Account.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace IdentityService.Pages.Register;

[SecurityHeaders]
[AllowAnonymous]
public class Index : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<Index> _logger;

    [BindProperty]
    public RegisterViewModel Input { get; set; }

    [BindProperty]
    public bool RegisterSuccess { get; set; } = false;

    public Index(UserManager<ApplicationUser> userManager, ILogger<Index> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public IActionResult OnGet(string returnUrl)
    {
        Input = new RegisterViewModel { ReturnUrl = returnUrl };

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (Input.Button != "register")
            return Redirect("~/");

        if (ModelState.IsValid)
        {
            var user = new ApplicationUser
            {
                Email = Input.Email,
                UserName = Input.Username,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await _userManager.AddClaimsAsync(
                    user,
                    new Claim[] { new Claim(JwtClaimTypes.Name, Input.FullName) }
                );

                RegisterSuccess = true;
            }
        }

        return Page();
    }
}
