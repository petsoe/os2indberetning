using Core.DomainModel;
using Core.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyDaysNotifier
{
    class NotifierService
    {
        private IGenericRepository<DriveReport> reportRepo;

        public NotifierService(IGenericRepository<DriveReport> reportRepo)
        {
            this.reportRepo = reportRepo;
        }

        internal void RunNotifierService()
        {
            var routeGeometry = reportRepo.AsQueryable().Where(r => r.Purpose.Equals("MEGA TEST") );
            
        }

    }
}
