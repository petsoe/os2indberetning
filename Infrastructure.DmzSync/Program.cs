using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.ApplicationServices;
using Core.ApplicationServices.Interfaces;
using Core.ApplicationServices.Logger;
using Core.DmzModel;
using Core.DomainModel;
using Core.DomainServices;
using Core.DomainServices.RoutingClasses;
using Infrastructure.AddressServices;
using Infrastructure.DmzDataAccess;
using Infrastructure.DataAccess;
using Core.DomainServices.Encryption;
using Infrastructure.DmzSync.Services.Impl;
using Ninject;
using DriveReport = Core.DmzModel.DriveReport;
using Rate = Core.DomainModel.Rate;


namespace Infrastructure.DmzSync
{
    class Program
    {
        static void Main(string[] args)
        {

            var logger = NinjectWebKernel.CreateKernel().Get<ILogger>();
       
            // hacks because of error with Entity Framework.
            // This forces the dmzconnection to use MySql.
            new DataContext();

            var personSync = new PersonSyncService(new GenericDmzRepository<Profile>(new DmzContext()),
                new GenericRepository<Person>(new DataContext()),
                NinjectWebKernel.CreateKernel().Get<IPersonService>());

            var tokenSync = new TokenSyncService(new GenericDmzRepository<Token>(new DmzContext()),
                new GenericRepository<MobileToken>(new DataContext()));

            var driveSync = new DriveReportSyncService(new GenericDmzRepository<DriveReport>(new DmzContext()),
               new GenericRepository<Core.DomainModel.DriveReport>(new DataContext()), new GenericRepository<Rate>(new DataContext()), new GenericRepository<LicensePlate>(new DataContext()), NinjectWebKernel.CreateKernel().Get<IDriveReportService>(), NinjectWebKernel.CreateKernel().Get<IRoute<RouteInformation>>(), NinjectWebKernel.CreateKernel().Get<IAddressCoordinates>(), NinjectWebKernel.CreateKernel().Get<IGenericRepository<Core.DomainModel.Employment>>(), NinjectWebKernel.CreateKernel().Get<ILogger>());

            var rateSync = new RateSyncService(new GenericDmzRepository<Core.DmzModel.Rate>(new DmzContext()),
                new GenericRepository<Rate>(new DataContext()));

            var userSync = new UserAuthSyncService(new GenericRepository<AppLogin>(new DataContext()),
                new GenericDmzRepository<UserAuth>(new DmzContext()));

            try
            {
                Console.WriteLine("TokenSyncFromDmz");
                tokenSync.SyncFromDmz();

            }
            catch (Exception ex)
            {
                logger.Log("Failed to sync Tokens from DMZ", "dmz", ex);
                throw;
            }

            try
            {
                Console.WriteLine("DriveReportsSyncFromDmz");
                driveSync.SyncFromDmz();

            }
            catch (Exception ex)
            {
                logger.Log("Failed to sync Drive Reports from DMZ", "dmz", ex);
                throw;
            }

            try
            {
                Console.WriteLine("TokenClearDmz");
                tokenSync.ClearDmz();
            }
            catch (Exception ex)
            {
                logger.Log("Failed to clear Tokens on DMZ", "dmz", ex);
                throw;
            }

            try
            {
                Console.WriteLine("PersonClearDmz");
                personSync.ClearDmz();
            }
            catch (Exception ex)
            {
                logger.Log("Failed to clear Persons on DMZ", "dmz", ex);
                throw;
            }

            try
            {
                Console.WriteLine("RateClearDmz");
                rateSync.ClearDmz();
            }
            catch (Exception ex)
            {
                logger.Log("Failed to clear Rates on DMZ", "dmz", ex);
                throw;
            }

            try
            {
                Console.WriteLine("PersonSyncToDmz");
                personSync.SyncToDmz();

            }
            catch (Exception ex)
            {
                logger.Log("Failed to sync Persons to DMZ", "dmz", ex);
                throw;
            }

            try
            {
                Console.WriteLine("TokenSyncToDmz");
                tokenSync.SyncToDmz();

            }
            catch (Exception ex)
            {
                logger.Log("Failed to sync Tokens to DMZ", "dmz", ex);
                throw;
            }

            try
            {
                Console.WriteLine("UserAuthSync");
                userSync.SyncToDmz();

            }
            catch (Exception ex)
            {
                logger.Log("Failed to sync UserAuth to DMZ", "dmz", ex);
                throw;
            }

            try
            {
                Console.WriteLine("RateSyncToDmz");
                rateSync.SyncToDmz();
            }
            catch (Exception ex)
            {
                logger.Log("Failed to sync Rates to DMZ", "dmz", ex);
                throw;
            }
            

            Console.WriteLine("Done");



        }
    }
}
