using System;
using System.Collections.Generic;
using Core.DomainModel;
using Presentation.Web.Test.Controllers;

namespace SixtyDaysNotifier.Test
{
    class PersonMock : GenericRepositoryMock<MailNotificationSchedule>
    {
        protected override List<MailNotificationSchedule> Seed()
        {
            throw new NotImplementedException();
        }
    }
}
