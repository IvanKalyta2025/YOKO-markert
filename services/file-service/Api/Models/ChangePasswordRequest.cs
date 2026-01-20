using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public record ChangePasswordRequest(string Email, string CurrentPassword, string NewPassword);

}