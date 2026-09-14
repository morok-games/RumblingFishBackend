using System.ComponentModel.DataAnnotations;

namespace RumblingFishBackend.Models.DTO.Profile
{
    public record SetNicknameRequest(
        [StringLength(12, MinimumLength = 3)] string Nickname);
}
