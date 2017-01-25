using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    public class Widgets
    {
        /// <summary>
        /// Widget Id as primary key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Type of the Widget Rebill, CLV, Schedule Rebill
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// Indicates whether the widget is reporting or not
        /// </summary>
        public bool isReportingWidget { get; set; }
        /// <summary>
        /// Row Position of Widget for rendering
        /// </summary>
        public int Row_Position { get; set; }
        /// <summary>
        /// Column Position of Widget for rendering
        /// </summary>
        public int Col_Position { get; set; }
        /// <summary>
        /// Created Date of Widget
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// UserId who created the widget
        /// </summary>
        public int CreatedBy { get; set; }
    }
}