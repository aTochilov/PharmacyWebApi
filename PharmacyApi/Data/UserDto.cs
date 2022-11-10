namespace PharmacyApi.Data
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool isAdmin { get; set; } = false;
    }
}
