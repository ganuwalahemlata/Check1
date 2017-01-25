using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace KontinuityCRM.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// Class EnumTypeModelDescription.
    /// </summary>
    /// <seealso cref="KontinuityCRM.Areas.HelpPage.ModelDescriptions.ModelDescription" />
    public class EnumTypeModelDescription : ModelDescription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumTypeModelDescription"/> class.
        /// </summary>
        public EnumTypeModelDescription()
        {
            Values = new Collection<EnumValueDescription>();
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public Collection<EnumValueDescription> Values { get; private set; }
    }
}