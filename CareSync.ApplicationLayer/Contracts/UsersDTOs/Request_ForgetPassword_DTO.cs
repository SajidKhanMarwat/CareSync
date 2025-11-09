

namespace CareSync.ApplicationLayer.Contracts.UsersDTOs
{
    public class Request_ForgetPassword_DTO
    {
        public required string Email { get; set; }
        public required string NewPassword { get; set; }
    }
}
