using Core.ApplicationServices;
using Core.DomainModel;
using Core.DomainServices;
using Ninject;

namespace SixtyDaysNotifier
{
    public class Program
    {
        static void Main(string[] args)
        {
            var ninjectKernel = NinjectWebKernel.CreateKernel();

            var service = new NotifierService(ninjectKernel.Get<IGenericRepository<DriveReport>>(), ninjectKernel.Get<IGenericRepository<Person>>());

            service.RunNotifierService();
            
        }
    }
}
