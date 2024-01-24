namespace WebApplication1.Models.Dto
{
    public class MemberDto
    {
        public string? Name { get; set; }

        public string? Email { get; set; } 

        public int? Age { get; set; } = 29;

        public IFormFile? Avatar { get; set; } 
    }
}
