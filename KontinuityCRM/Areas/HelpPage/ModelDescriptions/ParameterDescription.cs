using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KontinuityCRM.Areas.HelpPage.ModelDescriptions
{
    public class ParameterDescription
    {
        public ParameterDescription()
        {
            Annotations = new Collection<ParameterAnnotation>();
        }
        /// <summary>
        /// Inidcates Annotations related to ParameterDesciption
        /// </summary>
        public Collection<ParameterAnnotation> Annotations { get; private set; }
        /// <summary>
        /// Documentation related to parameter description
        /// </summary>
        public string Documentation { get; set; }
        /// <summary>
        /// Name for Parameter Description
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Indicates Model Description
        /// </summary>
        public ModelDescription TypeDescription { get; set; }
    }
}