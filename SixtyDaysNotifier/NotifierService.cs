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
        private IGenericRepository<DriveReport> _reportRepo;

        public NotifierService(IGenericRepository<DriveReport> reportRepo)
        {
            _reportRepo = reportRepo;
        }

        internal void RunNotifierService()
        {
            var routeGeometry = _reportRepo.AsQueryable().Where(r => r.Purpose.Equals("MEGA TEST") );
            int i = 0;
        }

    }
}
