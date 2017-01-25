using System;

namespace KontinuityCRM.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// Parameter Annotation
    /// </summary>
    public class ParameterAnnotation
    {
        /// <summary>
        /// Annotation Attribute
        /// </summary>
        public Attribute AnnotationAttribute { get; set; }
        /// <summary>
        /// Documentation for parameter Annotation
        /// </summary>
        public string Documentation { get; set; }
    }
}