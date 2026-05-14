using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JWolfsEventPlanning.Pages;


public class LoginModel : PageModel
{
    private const string TelegramScheme = "Telegram";

    public IActionResult OnGet(string? returnUrl = null)
    {
        var redirectUrl = string.IsNullOrWhiteSpace(returnUrl)
       ? "/"
       : returnUrl;

        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl
        };

        return Challenge(properties, TelegramScheme);
    }
}
