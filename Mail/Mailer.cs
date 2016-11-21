using Core.ApplicationServices;
using Ninject;
using Core.ApplicationServices.Logger;

namespace Mail
{
    public class Mailer
    {
        
        public static void Main(string[] args)
        {
            
            ILogger _logger = NinjectWebKernel.CreateKernel().Get<ILogger>();
            _logger.Log($"************* Mail started ***************", "mail", 3);
            var service = NinjectWebKernel.CreateKernel().Get<ConsoleMailerService>();
            service.RunMailService();
            _logger.Log($"************* Mail ended ***************", "mail", 3);
        }

        
    }
}
