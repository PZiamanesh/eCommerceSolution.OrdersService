namespace OrdersMicroservice.Core.DTOs;
public record UserDTO
{
    public Guid UserID { get; set; }
    public string? Email { get; set; }
    public string? PersonName { get; set; }
    public GenderOptions? Gender { get; set; }
}
