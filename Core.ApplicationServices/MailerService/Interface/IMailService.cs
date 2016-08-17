using System;
using System.Collections.Generic;

namespace Core.ApplicationServices.MailerService.Interface
{
    public interface IMailService
    {
        void SendMails(DateTime payRoleDateTime);
        void SendTestMail(string to, string subject, string text);  
        IEnumerable<string> GetLeadersWithPendingReportsMails();
    }
}
