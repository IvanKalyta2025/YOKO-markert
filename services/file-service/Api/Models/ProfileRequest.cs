using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class ProfileRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; } = 0; //update to version 2.1
        public string Gender { get; set; } = string.Empty; //update to version 2.1
        public string Hobby { get; set; } = string.Empty; //update to version 2.1
        public string MyPlaceOfBirth { get; set; } = string.Empty; //update to version 2.1
        public IFormFile? AvatarFile { get; set; } = null!;
    }
}