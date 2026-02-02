using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace Api.Contracts.Requests
{
    public class ProfileRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        [Range(16, 99, ErrorMessage = "Alder må være mellom 16 og 99 år")] //using System.ComponentModel.DataAnnotations;
        public int Age { get; set; } = 0; //update to version 2.1
        public Gender Gender { get; set; } = Gender.Other; //update to version 2.1.1 "from gender variable"
        public string Hobby { get; set; } = string.Empty; //update to version 2.1
        public string MyPlaceOfBirth { get; set; } = string.Empty; //update to version 2.1
        public IFormFile? AvatarFile { get; set; } = null!;
    }

    public enum Gender
    {
        Male,
        Female,
        Other
    }
}
