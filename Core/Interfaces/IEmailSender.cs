using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(List<string> Recipients, string subject, string message);
    }
}
