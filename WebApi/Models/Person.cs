﻿namespace WebApi.Models
{
    public class Person
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required int Age { get; set; }

        public required string Email { get; set; }

    }
}
