using System.Collections.ObjectModel;

namespace KontinuityCRM.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// Class ComplexTypeModelDescription.
    /// </summary>
    /// <seealso cref="KontinuityCRM.Areas.HelpPage.ModelDescriptions.ModelDescription" />
    public class ComplexTypeModelDescription : ModelDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexTypeModelDescription"/> class.
        /// </summary>
        public ComplexTypeModelDescription()
        {
            Properties = new Collection<ParameterDescription>();
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public Collection<ParameterDescription> Properties { get; private set; }
    }
}