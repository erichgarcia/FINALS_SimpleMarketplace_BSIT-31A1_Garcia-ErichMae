using Microsoft.AspNetCore.Identity;
using SimpleMarketplace.Models;

namespace SimpleMarketplace.Services
{
    public class CustomPasswordValidator : IPasswordValidator<ApplicationUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user, string? password)
        {
            var errors = new List<IdentityError>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordRequired",
                    Description = "Password is required."
                });
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            // Count uppercase letters
            int uppercaseCount = password.Count(char.IsUpper);
            if (uppercaseCount < 2)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordRequiresUpper",
                    Description = "Password must contain at least 2 uppercase letters."
                });
            }

            // Count digits
            int digitCount = password.Count(char.IsDigit);
            if (digitCount < 3)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordRequiresDigit",
                    Description = "Password must contain at least 3 numbers."
                });
            }

            // Count non-alphanumeric characters (symbols)
            int symbolCount = password.Count(ch => !char.IsLetterOrDigit(ch));
            if (symbolCount < 3)
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordRequiresNonAlphanumeric",
                    Description = "Password must contain at least 3 symbols."
                });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
