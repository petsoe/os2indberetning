using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Core.DomainModel;
using NSubstitute;
using Core.DomainServices;
using Core.ApplicationServices.MailerService.Interface;

namespace SixtyDaysNotifier.Test
{
    [TestFixture]
    public class SixtyDaysNotifierTest
    {
        private IGenericRepository<Person> personRepoMock;
        private IGenericRepository<DriveReport> driveReportRepoMock;
        private IMailService _mailServiceMock;

        private List<DriveReport> driveReports;
        private List<Person> persons;
        private Dictionary<Person, int> receivedFromService;

        [SetUp]
        public void SetUp()
        {
           
            driveReportRepoMock = NSubstitute.Substitute.For<IGenericRepository<DriveReport>>();
            personRepoMock = NSubstitute.Substitute.For<IGenericRepository<Person>>();
            _mailServiceMock = NSubstitute.Substitute.For<IMailService>();
            driveReports = new List<DriveReport>();
            persons = new List<Person>();

            receivedFromService = new Dictionary<Person, int>();

        }
        
        [Test]
        public void FilterDriveReportsNoDrivereportsTriggeredTest()
        {
            var amountOfDrivereportsToCreate = 3;
            driveReports = GenerateDriveReports(amountOfDrivereportsToCreate);
            persons = GeneratePersons();
            Assert.AreEqual(driveReports.Count, amountOfDrivereportsToCreate);
            driveReportRepoMock.AsQueryable().Returns(driveReports.AsQueryable());
            personRepoMock.AsQueryable().Returns(persons.AsQueryable());
            
            var notifierService = new NotifierService(driveReportRepoMock, personRepoMock, _mailServiceMock);
            receivedFromService = notifierService.GetReportsWhereSixtyDayRuleIsTriggered();
            //var personsFromService = receivedFromService.Keys;
            Assert.IsEmpty(receivedFromService);
        }

        [Test]
        public void FilterDriveReportsDriveReportsTriggeredTest()
        {
            var amountOfDrivereportsToCreate = 65;
            driveReports = GenerateDriveReports(amountOfDrivereportsToCreate);
            var drivereport = new DriveReport();
            drivereport.StartsAtHome = false;
            drivereport.EndsAtHome = false;

            driveReports.Add(drivereport);
            persons = GeneratePersons();
            
            driveReportRepoMock.AsQueryable().Returns(driveReports.AsQueryable());
            personRepoMock.AsQueryable().Returns(persons.AsQueryable());

            var notifierService = new NotifierService(driveReportRepoMock, personRepoMock, _mailServiceMock);
            receivedFromService = notifierService.GetReportsWhereSixtyDayRuleIsTriggered();
            Assert.IsEmpty(receivedFromService);
        }

        [Test]
        public void NotifierReceiveMailTest()
        {
            

            var amountOfDrivereportsToCreate = 1;
            driveReports = GenerateDriveReports(amountOfDrivereportsToCreate);
            persons = GeneratePersons();
            Assert.AreEqual(driveReports.Count, amountOfDrivereportsToCreate);
            driveReportRepoMock.AsQueryable().Returns(driveReports.AsQueryable());
            personRepoMock.AsQueryable().Returns(persons.AsQueryable());
           // var mailService = ;
            var notifierService = new NotifierService(driveReportRepoMock, personRepoMock, _mailServiceMock);
            notifierService.RunNotifierService();
            
        }
        [Test]
        public void MailTestTest()
        {
            var amountOfDrivereportsToCreate = 1;
            driveReports = GenerateDriveReports(amountOfDrivereportsToCreate);
            persons = GeneratePersons();
            Assert.AreEqual(driveReports.Count, amountOfDrivereportsToCreate);
            driveReportRepoMock.AsQueryable().Returns(driveReports.AsQueryable());
            personRepoMock.AsQueryable().Returns(persons.AsQueryable());

            var notifierService = new NotifierService(driveReportRepoMock, personRepoMock, _mailServiceMock);
            notifierService.RunNotifierService();

        }

        private List<Person> GeneratePersons()
        {
            var persons = new List<Person>();

            var person1 = new Person();
            person1.FirstName = "Kalla";
            person1.LastName = "Anka";
            person1.Id = 0;
            persons.Add(person1);

            var person2 = new Person();
            person2.FirstName = "Smølf";
            person2.LastName = "Nielsen";
            person2.Id = 1;
            persons.Add(person2);

            var person3 = new Person();
            person3.FirstName = "Milli";
            person3.LastName = "Vanilli";
            person3.Id = 2;
            persons.Add(person3);

            return persons;
        }

        private List<DriveReport> GenerateDriveReports(int amount)
        {
            var today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            var todayAsTimeStamp = ToUnixTime(today);
            var twelveMonthsBack = today.AddMonths(-12);
            var twelveMonthsBackAsTimestamp = ToUnixTime(twelveMonthsBack);

            var driveReports = new List<DriveReport>();

            for (var i = 0; i < amount; i++)
            {
                var drivereport = new DriveReport();
                drivereport.PersonId = 0;
                drivereport.CreatedDateTimestamp = todayAsTimeStamp;
                drivereport.StartsAtHome = true;
                var driveReportPoints = new List<DriveReportPoint>();
                var points = new DriveReportPoint();

                var driveReportPoint1 = new DriveReportPoint();
                driveReportPoint1.StreetName = "HanneBoelsGade222";
                var driveReportPoint2 = new DriveReportPoint();
                driveReportPoint2.StreetName = "HansiHinterSeersGade333";

                driveReportPoints.Add(driveReportPoint1);
                driveReportPoints.Add(driveReportPoint2);

                if (i % 2 != 0)
                {
                    
                    var driveReportPoint3 = new DriveReportPoint();
                    driveReportPoint3.StreetName = "JodleKjeldsGade444";
                    driveReportPoints.Add(driveReportPoint3);

                }
                drivereport.DriveReportPoints = driveReportPoints;
                driveReports.Add(drivereport);
            }
            return driveReports;
        }

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

