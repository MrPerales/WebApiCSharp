namespace WebApi.DTOs.Person
{
    public class CreatePersonDto
    {
        public required string Name { get; set; }
        public required int Age { get; set; }

        public required string Email { get; set; }
    }
}
