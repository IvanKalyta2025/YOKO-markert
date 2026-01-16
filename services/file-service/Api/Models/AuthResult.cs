using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public record AuthResult(bool Success, string Message, User? User = null);
}