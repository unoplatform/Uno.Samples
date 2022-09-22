using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using TimeEntryRia.Web.Models;
using System.Collections.Generic;
using System.ServiceModel.DomainServices.Server;
using System.Security.Permissions;
using System.Threading;
using System.Web;

namespace TimeEntryRia.Web.Services
{
    [ServiceContract(Namespace = "http://www.custommayd.com/timeentry")]
    [SilverlightFaultBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class TimesheetSummaryService
    {
        public TimesheetSummaryService()
        {
            Thread.CurrentPrincipal = HttpContext.Current.User;
        }

        [OperationContract]
        [PrincipalPermission(SecurityAction.Demand, Authenticated=true)]
        public IEnumerable<SummaryRow> GetWeekSummary(int userId, DateTime weekStartDate)
        {
            var context = new TimeEntryEntities();
            weekStartDate = weekStartDate.Date;
            var weekEndDate = weekStartDate + TimeSpan.FromDays(7);

            var rows = context.TimeEntries.Include("Project").Where(te =>
                    te.UserId == userId &&
                    te.Date >= weekStartDate &&
                    te.Date < weekEndDate
                )
                .GroupBy(r => new { r.ProjectId, r.Project.Name, r.Date })
                .OrderBy(g => g.Key.ProjectId)
                .Select(g => new { ProjectId = g.Key.ProjectId, Name = g.Key.Name, Date = g.Key.Date, Total = g.Sum(r => r.Hours) })
                .ToList();



            var summaryRows = new Dictionary<int, SummaryRow>();
            foreach (var projectId in rows.Select(r => r.ProjectId).Distinct())
            {
                summaryRows.Add(projectId, new SummaryRow());
            }


            foreach (var row in rows)
            {
                summaryRows[row.ProjectId].IsTotalRow = false;
                summaryRows[row.ProjectId].ProjectId = row.ProjectId;
                summaryRows[row.ProjectId].Name = row.Name;

                var dayOffset = row.Date - weekStartDate;
                switch (dayOffset.Days)
                {
                    case 0:
                        summaryRows[row.ProjectId].MonTotal = row.Total;
                        break;
                    case 1:
                        summaryRows[row.ProjectId].TueTotal = row.Total;
                        break;
                    case 2:
                        summaryRows[row.ProjectId].WedTotal = row.Total;
                        break;
                    case 3:
                        summaryRows[row.ProjectId].ThuTotal = row.Total;
                        break;
                    case 4:
                        summaryRows[row.ProjectId].FriTotal = row.Total;
                        break;
                    case 5:
                        summaryRows[row.ProjectId].SatTotal = row.Total;
                        break;
                    case 6:
                        summaryRows[row.ProjectId].SunTotal = row.Total;
                        break;
                    default:
                        break;
                }
            }

            var result = summaryRows.Select(r => r.Value).ToList();

            result.Add(new SummaryRow()
            {
                IsTotalRow = true,
                Name = "Totals",
                MonTotal = result.Sum(r => r.MonTotal),
                TueTotal = result.Sum(r => r.TueTotal),
                WedTotal = result.Sum(r => r.WedTotal),
                ThuTotal = result.Sum(r => r.ThuTotal),
                FriTotal = result.Sum(r => r.FriTotal),
                SatTotal = result.Sum(r => r.SatTotal),
                SunTotal = result.Sum(r => r.SunTotal),
            });

            foreach (var row in result)
            {
                row.Total = row.MonTotal + row.TueTotal + row.WedTotal + row.ThuTotal + row.FriTotal + row.SatTotal + row.SunTotal;
            }


            return result;
        }
    }
}
