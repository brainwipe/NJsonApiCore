using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NJsonApiCore.Web.MVCCore.HelloWorld.Models
{
    public class Report
    {
        public Report()
        {
            StatisticsReport = new StatisticsReport();
        }

        public int Id { get; set; } = 1;

        public StatisticsReport StatisticsReport { get; set; }
    }
}
