namespace PharmacyApi.Data.DTO
{
    public class UserRegister: UserDto
    {
        public bool isAdmin { get; set; } = false;
    }
}
