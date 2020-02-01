using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using AspireBudgetApi.Models;

namespace AspireBudgetTelegramBot.Extensions
{
    public static class DashboardExtensions
    {
        public static string ToSummary(this List<DashboardRow> dashboard)
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
                    sb.AppendLine($"<b>{row.Name}</b>");
                }
                else
                {
                    var emoji = row.Available < 0 ? "❗️" : "";
                    sb.AppendLine($"{row.Name}: {emoji}{row.Available} | {row.Spent} | {row.Budgeted}");
                }
            }

            return sb.ToString();
        }
    }
}
