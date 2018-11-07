using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraniteHouse.Services
{
    public interface IGraniteEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);

    }
}
