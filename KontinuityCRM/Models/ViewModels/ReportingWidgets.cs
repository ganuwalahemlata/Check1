using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.ViewModels
{
    /// <summary>
    /// Reporting Widget that contains chart properties to be rendered on the widgets
    /// </summary>
    public class ReportingWidgets
    {
        public object[] XAxis { get; set; }
        public object[] YAxis { get; set; }
        public string XTitle { get; set; }

        public string YTitle { get; set; }
        public string ChartTitle { get; set; }
        public string ChartType { get; set; }

        public int id { get; set; }
        public int Col_Pos { get; set; }
        public int Row_Pos { get; set; }

        public string SeriesName { get; set; }
    }
    /// <summary>
    /// This tells the report type that will be rendered on the widget
    /// </summary>
    public enum ReportType
    {

        Rebill = 1,
        ScheduledRebill = 2,
        Profit = 3,
        CustomerLifeTime = 4
    }


}