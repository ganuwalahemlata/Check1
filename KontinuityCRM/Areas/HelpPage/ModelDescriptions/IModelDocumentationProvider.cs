using System;
using System.Reflection;

namespace KontinuityCRM.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// Interface IModelDocumentationProvider
    /// </summary>
    public interface IModelDocumentationProvider
    {
        /// <summary>
        /// Gets the documentation.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>System.String.</returns>
        string GetDocumentation(MemberInfo member);

        /// <summary>
        /// Gets the documentation.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        string GetDocumentation(Type type);
    }
}