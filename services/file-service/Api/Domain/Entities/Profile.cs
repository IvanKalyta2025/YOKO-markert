using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Api.Contracts.Requests;

namespace Api.Domain.Entities
{
    public class Profile
    {
        [Key]
        public Guid UserId { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;

        [Range(16, 99, ErrorMessage = "Alder må være mellom 16 og 99 år")] //using System.ComponentModel.DataAnnotations;
        public int Age { get; set; } = 0; //update to version 2.1
        public Gender Gender { get; set; } = Gender.Other; //update to version 2.1
        public string Hobby { get; set; } = string.Empty; //update to version 2.1
        public string MyPlaceOfBirth { get; set; } = string.Empty; //update to version 2.1
        public string AvatarUrl { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

    }
    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
