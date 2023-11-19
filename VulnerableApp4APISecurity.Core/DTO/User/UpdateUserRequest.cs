namespace VulnerableApp4APISecurity.Core.DTO.User;

public class UpdateUserRequest
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Password { get; set; }
}