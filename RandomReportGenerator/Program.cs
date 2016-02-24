using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.DomainModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject;
using System;
using System.Collections.Generic;
using System.Net;
using Core.DomainServices;
using System.Linq;

namespace RandomReportGenerator
{
    class Program
    {
        static Random _random = new Random();
        static void Main(string[] args)
        {
            var service = NinjectWebKernel.CreateKernel().Get<IDriveReportService>();
            IGenericRepository<Person> _personRepo = NinjectWebKernel.CreateKernel().Get<IGenericRepository<Person>>();

            for (int i = 0; i < 10000; i++)
            {
                try {
                    Console.WriteLine("Generating report " + i + " of " + 10000);
                    service.Create(GetReport(_personRepo));
                }
                catch (Exception)
                {
                    Console.WriteLine("ERROR");
                }
            }
        }

        private static DriveReport GetReport(IGenericRepository<Person> personRepo)
        {

            var person = _random.Next(1, 5);
            var rateType = _random.Next(1, 5);
            var date = (Int32)(DateTime.UtcNow.AddDays((_random.Next(1, 365)) * -1).Subtract(new DateTime(1970, 1, 1))).TotalSeconds;


            var result = new DriveReport
            {
                DriveReportPoints = GetAddressesForReport(),
                PersonId = person,
                EmploymentId = person,
                Purpose = "Testformål",
                IsRoundTrip = Convert.ToBoolean(_random.Next(0, 1)),
                KilometerAllowance = KilometerAllowance.Calculated,
                LicensePlate = "AB12312",
                TFCode = "4861",
                KmRate = 363,
                DriveDateTimestamp = date,
                CreatedDateTimestamp = date - 1000,
                FullName = personRepo.AsQueryable().FirstOrDefault(x => x.Id == person).FullName,
                Comment = "Ingen kommentar"
            };
            return result;
        }

        private static List<DriveReportPoint> GetAddressesForReport()
        {
            var result = new List<DriveReportPoint>();
            var numberOfAddresses = _random.Next(2, 10);
            for(int i = 0; i < numberOfAddresses; i++)
            {
                result.Add(GetRandomAddress());
            }
            return result;
        }

        private static DriveReportPoint GetRandomAddress()
        {
            // This method gets two random letters and returns the first address found from DAWA AWS using these two letters as query.
            var result = new DriveReportPoint();

            var input = GetLetter() + "" + GetLetter();

            using (WebClient webClient = new System.Net.WebClient())
            {
                WebClient n = new WebClient();
                n.Encoding = System.Text.Encoding.UTF8;
                var json = n.DownloadString("https://dawa.aws.dk/adgangsadresser/autocomplete?fuzzy&q=" + input);

                JArray jArr = JArray.Parse(json);
                JObject jObj = JObject.Parse(jArr.First.ToString());
                JToken jUser = jObj["adgangsadresse"];
                var street = jUser["vejnavn"].ToString();
                var number = jUser["husnr"].ToString();
                var zipCode = jUser["postnr"].ToString();
                result.StreetName = street;
                result.StreetNumber = number;
                result.ZipCode = Convert.ToInt32(zipCode);
            }
            return result;
        }

        private static char GetLetter()
        {
            // This method returns a random lowercase letter.
            // ... Between 'a' and 'z' inclusize.
            int num = _random.Next(0, 26); // Zero to 25
            char let = (char)('a' + num);
            return let;
        }
    }
}
