using System;

namespace KontinuityCRM.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// Describes a type model.
    /// </summary>
    public abstract class ModelDescription
    {
        /// <summary>
        /// Documentation of typed Model
        /// </summary>
        public string Documentation { get; set; }
        /// <summary>
        /// Model Type
        /// </summary>
        public Type ModelType { get; set; }
        /// <summary>
        /// Model Name
        /// </summary>
        public string Name { get; set; }
    }
}