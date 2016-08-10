using Core.ApplicationServices;
using Ninject;

namespace SixtyDaysNotifier
{
    class Program
    {
        static void Main(string[] args)
        {
           
            var service = NinjectWebKernel.CreateKernel().Get<NotifierService>();
            service.RunNotifierService();
            
        }
    }
}
