using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models
{
    public class Profile
    {
        [Key]
        public Guid UserId { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public int Age { get; set; } //update to version 2.1
        public string Gender { get; set; } = string.Empty; //update to version 2.1
        public string Hobby { get; set; } = string.Empty; //update to version 2.1
        public string MyPlaceOfBirth { get; set; } = string.Empty; //update to version 2.1
        public string AvatarUrl { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

    }
}
