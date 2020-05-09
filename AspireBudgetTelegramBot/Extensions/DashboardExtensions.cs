using System.Collections.Generic;
using System.Linq;
using System.Text;
using AspireBudgetApi.Models;

namespace AspireBudgetTelegramBot.Extensions
{
    public static class DashboardExtensions
    {
        public static string ToHtmlSummary(this List<DashboardRow> dashboard)
        {
            var sb = new StringBuilder();
            if (dashboard.Any())
            {
                sb.AppendLine("Available | Spent | Budgeted");
            }
            else
            {
                return  "No results.";
            }

            foreach (var row in dashboard)
            {
                if (row.Type == DashboardRowType.Group)
                {
                    sb.AppendLine();
                }
                sb.AppendLine(row.ToHtmlSummary());
            }

            return sb.ToString();
        }

        public static string ToHtmlSummary(this DashboardRow dashboardRow)
        {
            if (dashboardRow.Type == DashboardRowType.Group)
            {
                return $"<b>{dashboardRow.Name}</b>";
            }
            else
            {
                var emoji = dashboardRow.Available < 0 ? "❗️" : "";
                return $"{dashboardRow.Name}: {emoji}{dashboardRow.Available} | {dashboardRow.Spent} | {dashboardRow.Budgeted}";
            }
        }
    }
}
