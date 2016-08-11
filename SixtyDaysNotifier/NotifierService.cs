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
        private IGenericRepository<Person> _personRepo;

        public NotifierService(IGenericRepository<DriveReport> reportRepo, IGenericRepository<Person> personRepo)
        {
            _reportRepo = reportRepo;
            _personRepo = personRepo;
        }

        public void RunNotifierService()
        {
            if (GetReportsWhereSixtyDayRuleIsTriggered().Count() > 0)
            {
                //send mail om aktuelle antal kørseler
            }

        }

        private Dictionary<Person, int> GetReportsWhereSixtyDayRuleIsTriggered()
        {
            var personToDriveReports = GetPersonToDriveReports();

            var triggeredDriveReports = new Dictionary<Person, int>();

            foreach (var person in personToDriveReports.Keys)
            {
                //Mail needs to be sent if 55 or more reports 
                //if (personToDriveReports[person].Count() >= 55)
                //{
                var driveReports = personToDriveReports[person];
                var filteredDriveReports = FilterDriveReports(driveReports);
                // }
                if (filteredDriveReports.Count >= 55)
                {
                    triggeredDriveReports.Add(person, filteredDriveReports.Count);
                }
            }

            return triggeredDriveReports;
        }

        private List<DriveReport> FilterDriveReports(List<DriveReport> driveReports)
        {
            var triggeredDriveReports = new List<DriveReport>();

            foreach (var report in driveReports)
            {
                //Det er ikke endepunktet, der tages højde for, men den første adresse man kører til hjemmefra 
                //– eller den sidste man har været på,
                //inden man kører hjem. Så i dette tilfælde køres der fra A til C og fra A til D.
                var driveReportPoints = report.DriveReportPoints;
                //something something...

            }

            return triggeredDriveReports;
        }

        private Dictionary<Person, List<DriveReport>> GetPersonToDriveReports()
        {
            var personToDriveReports = new Dictionary<Person, List<DriveReport>>();

            var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            var twelveMonthsBack = today.AddMonths(-12);
            var twelveMonthsBackAsTimestamp = ToUnixTime(twelveMonthsBack);

            //We are only interested in reports made within the last 12 months and which starts or ends at home
            var driveReportsTwelveMonths = _reportRepo.AsQueryable().Where(r => r.CreatedDateTimestamp >= twelveMonthsBackAsTimestamp && (r.StartsAtHome || r.EndsAtHome));
            var persons = _personRepo.AsQueryable();

            foreach (var person in persons.ToList())
            {
                var list = new List<DriveReport>();
                foreach (var drive in driveReportsTwelveMonths.ToList())
                {
                    if (drive.PersonId == person.Id)
                    {
                        list.Add(drive);
                    }
                }
                personToDriveReports.Add(person, list);
            }
            return personToDriveReports;
        }

        //}

        /// <summary>
        /// Converts DateTime to timestamp
        /// </summary>
        /// <param name="date">DateTime to convert</param>
        /// <returns>long timestamp</returns>
        public long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        /// <summary>
        /// Converts timestamp to datetime
        /// </summary>
        /// <param name="unixTime">Timestamp to convert</param>
        /// <returns>DateTime</returns>
        public DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

    }
}
