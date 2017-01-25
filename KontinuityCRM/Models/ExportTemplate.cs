using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    /// <summary>
    /// Export Template 
    /// </summary>
    public class ExportTemplate
    {
        /// <summary>
        /// Primary Key of the Export Template
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name of Export Template
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// All the fields for filtering orders to be saved in an export template
    /// </summary>
    public class ExportTemplateFields
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Export Template For Referential Integrity
        /// </summary>
        public virtual ExportTemplate ExportTemplate { get; set; }

        /// <summary>
        /// Name of the Field which will be applied for filtering orders
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// Value of the Field which wll be applied for filtering orders
        /// </summary>
        public string FieldValue { get; set; }

    }

    /// <summary>
    /// Export Fields that go in exported csv file
    /// </summary>
    public class ExportFields
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name that goes in export file
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value i.e. Field is allowed or not
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// Sort order of the field
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Referential Integrity
        /// </summary>
        public virtual ExportTemplate ExportTemplate { get; set; }

        /// <summary>
        /// Display Name
        /// </summary>
        [NotMapped]
        public string DisplayName { get; set; }
    }
}