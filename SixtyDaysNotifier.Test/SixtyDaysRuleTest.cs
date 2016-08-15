using System;
using NUnit.Framework;
using Core.DomainModel;
using System.Collections.Generic;

namespace SixtyDaysNotifier.Test
{
    [TestFixture]
    public class SixtyDaysRuleTest
    {
        //private PersonMock personRepoMock;

        //private DriveReportMock driveReportRepoMock;
        private List<DriveReport> driveReports;

        [SetUp]
        public void SetUp()
        {
            //  personRepoMock = new PersonMock();
          //  driveReportRepoMock = new DriveReportMock();
            driveReports = GenerateDriveReports(55);
        }

        private List<DriveReport> GenerateDriveReports(int amount)
        {

            List<DriveReport> driveReports = new List<DriveReport>();
            
            for (var i = 0; i < amount; i++)
            {
                var drivereport = new DriveReport();

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

        [Test]
        public void FilterDriveReportsTest(List<DriveReport> driveReports)
        {
            
            Assert.IsFalse(1==1);
        }
    }

   
}

