using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransferService.Application.DTO
{
    public class AuthenticateRequest
    {
        public string Username { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
}
