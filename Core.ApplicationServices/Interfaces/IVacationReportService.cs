﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModel;

namespace Core.ApplicationServices.Interfaces
{
    public interface IVacationReportService : IReportService<VacationReport>
    {
        void PrepareReport(VacationReport report);
        void ApproveReport(VacationReport report, Person approver, string emailText);
        VacationReport EditReport(VacationReport report);
    }
}