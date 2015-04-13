﻿using System;
using System.Web.Http;
using Core.ApplicationServices;
using Core.ApplicationServices.FileGenerator;
using Core.DomainModel;
using Core.DomainServices;
using log4net;
using Ninject;

namespace OS2Indberetning.Controllers
{
    public class FileController : ApiController
    {
        private readonly IGenericRepository<DriveReport> _repo;

        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public FileController(IGenericRepository<DriveReport> repo)
        {
            _repo = repo;
        }

        //GET: Generate KMD File
        public IHttpActionResult Get()
        {
            try
            {
                new ReportGenerator(_repo, new ReportFileWriter()).WriteRecordsToFileAndAlterReportStatus();
                Logger.Info("Fil til KMD genereret.");
                return Ok();
            }
            catch (Exception)
            {
                Logger.Error("Fejl ved generering af fil til KMD", e);
                return InternalServerError();
            }
        }
    }
}