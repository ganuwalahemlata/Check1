namespace KontinuityCRM.Areas.HelpPage.ModelDescriptions
{
    /// <summary>
    /// Key Value Pair Model Description
    /// </summary>
    public class KeyValuePairModelDescription : ModelDescription
    {
        /// <summary>
        /// Indicates Key Model Description
        /// </summary>
        public ModelDescription KeyModelDescription { get; set; }
        /// <summary>
        /// Indicates Value Model Description
        /// </summary>
        public ModelDescription ValueModelDescription { get; set; }
    }
}