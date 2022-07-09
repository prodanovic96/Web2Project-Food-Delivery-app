using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web2Project.Models;

namespace Web2Project.Helper
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
    }
}
