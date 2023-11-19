using Microsoft.AspNetCore.DataProtection;

namespace VulnerableApp4APISecurity.Infrastructure.Security;

public class PasswordSecurity
{
    private readonly IDataProtectionProvider _dataProtectionProvider;
    private readonly string _key;

    public PasswordSecurity(IDataProtectionProvider dataProtectionProvider)
    {
        _dataProtectionProvider = dataProtectionProvider;
        _key = "Harcodede key";
    }

    public string Encrypt(string input)
    {
        var protector = _dataProtectionProvider.CreateProtector(_key);
        return protector.Protect(input);
    }

    public string Decrypt(string input)
    {
        var protector = _dataProtectionProvider.CreateProtector(_key);
        return protector.Unprotect(input);
    }
}