
namespace Reclamos.Application.DTOs
{
    public class UserDto
    {
        public string id { get; init; }
        public string name { get; init; }
        public string lastName { get; init; }
        public string email { get; init; }
        public string roleId { get; set; }
        public string address { get; init; }
        public string phone { get; init; }
    }
}
