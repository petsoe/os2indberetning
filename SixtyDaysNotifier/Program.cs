using Core.ApplicationServices;
using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using Ninject;
using System;
using System.Collections.Generic;

namespace SixtyDaysNotifier
{
    public class Program
    {
        static void Main(string[] args)
        {
            var ninjectKernel = NinjectWebKernel.CreateKernel();

            //var service = new NotifierService(ninjectKernel.Get<IGenericRepository<DriveReport>>(), ninjectKernel.Get<IGenericRepository<Person>>(),ninjectKernel.Get<IMailSender>());
            var service = ninjectKernel.Get<NotifierService>();
            var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            Dictionary<Person, int> personsAndAmount = service.GetReportsWhereSixtyDayRuleIsTriggered(today);
            service.SendMailAboutSixtyDaysViolation(personsAndAmount);
            //service.RunNotifierService();
            
        }
    }
}
