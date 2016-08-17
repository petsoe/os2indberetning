using Core.ApplicationServices.MailerService.Interface;
using Core.DomainModel;
using Core.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SixtyDaysNotifier
{
    public class NotifierService
    {
        private IGenericRepository<DriveReport> _reportRepo;
        private IGenericRepository<Person> _personRepo;
        //private IMailSender _mailSender;
        private IMailService _mailService;

        public NotifierService(IGenericRepository<DriveReport> reportRepo, IGenericRepository<Person> personRepo, IMailService mailService)
        {
            _mailService = mailService;
            _reportRepo = reportRepo;
            _personRepo = personRepo;
        }

        public void RunNotifierService()
        {
           // var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);

           // _mailService.SendMails(today);
            _mailService.SendTestMail("mhn@miracle.dk", "nothing", "hej");
            //Dictionary<Person, int> personsAndAmount = GetReportsWhereSixtyDayRuleIsTriggered();
            
            //foreach (var person in personsAndAmount.Keys)
            //{
            //    if (personsAndAmount[person] >= 60)
            //    {
                    
            //    }
            //} 
            
        }

        public Dictionary<Person, int> GetReportsWhereSixtyDayRuleIsTriggered()
        {
            var personToDriveReports = GetPersonToDriveReports();

            var triggeredDriveReports = new Dictionary<Person, int>();

            foreach (var person in personToDriveReports.Keys)
            {
                var filteredDriveReports = new List<RoutePointPair>();

                //Mail needs to be sent if 60 or more reports 
                if (personToDriveReports[person].Count() >= 60)
                {
                var driveReports = personToDriveReports[person];
                filteredDriveReports = FilterDriveReports(driveReports);
                 }

                if (filteredDriveReports.Count >= 60)
                {
                    //Rasmus Rosendal kan du gøre noget smart her ud fra hvad vi snakkede om?
                    //Ved ikke hvad den skal returnere :-)
                    //triggeredDriveReports.Add(person, filteredDriveReports.Count);
                }
            }

            return triggeredDriveReports;
        }

        private List<RoutePointPair> FilterDriveReports(List<DriveReport> driveReports)
        {
            var triggeredDriveReports = new List<RoutePointPair>();

            foreach (var report in driveReports)
            {
                //Det er ikke endepunktet, der tages højde for, men den første adresse man kører til hjemmefra 
                //– eller den sidste man har været på, inden man kører hjem.
                // Det, der er vigtigt er, at hjemmeadressen skal være i den ene ende - 
                //om det er start eller slut er lige meget. Men det er hjemmeadressen, der er vigtig. 
                //Det er også vigtigt, at hvis den samme rute (med hjemmeadressen i den ene ende) køres flere gange på én dag,
                //så tæller det stadig kun som en gang. Så det er vigtigt, at der tælles på dage og ikke på antal ruter.

                var driveReportPoints = report.DriveReportPoints;
                //something something...
                DriveReportPoint nextPoint;
                DriveReportPoint homeAdressPoint;
                //We can be sure that the homeadress is present but it will either be in the start or in the end
                if (report.StartsAtHome)
                {
                    homeAdressPoint = driveReportPoints.AsQueryable().ElementAt(0);
                    //if startsathome -> the first adress after home is next point
                    nextPoint = driveReportPoints.AsQueryable().ElementAt(1);
                }else
                {
                    homeAdressPoint = driveReportPoints.AsQueryable().ElementAt(driveReportPoints.Count - 1);
                    //if endsathome -> the adress before home is next point
                    nextPoint = driveReportPoints.AsQueryable().ElementAt(driveReportPoints.Count - 2);
                }
                //Travelling from homeadress to endadress or from endadress to homeadress is considered the same in the sixtydays-rule
                triggeredDriveReports.Add(new RoutePointPair(homeAdressPoint, nextPoint, FromUnixTime(report.CreatedDateTimestamp)));
                //Tag højde for, at en rute maks kan tælles én gang om dagen. To ens ruter på samme dag, tæller altså kun én gang.
            }

            return triggeredDriveReports;
        }

        protected class RoutePointPair{
            public DriveReportPoint home { get; set; }
            public DriveReportPoint next { get; set; }
            public DateTime createdDate { get; set; }
            
            public RoutePointPair(DriveReportPoint home, DriveReportPoint next, DateTime createdDate)
            {
                this.home = home;
                this.next = next;
                this.createdDate = createdDate;
                
            }
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
