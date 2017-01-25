using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.ViewModels
{
    public class ExportTemplateModel
    {
        public ExportTemplateModel()
        {
            ExportFields = new List<ExportFields>();
        }
        public int Id { get; set; }

        [Required]
        public string TemplateName { get; set; }
        //This property contains all the Feilds.
        public OrderSearch TemplateFields { get; set; }

        public List<ExportFields> ExportFields { get; set; }
    }

    public class ExportTemplateListModel
    {
        public List<ExportTemplate> ExportTemplates { get; set; }
    }
}