using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Metadata;

namespace KontinuityCRM.Helpers.IoC
{
    /// <summary>
    /// Custom ModelValidorProvider for di in the Web api
    /// </summary>
    public class CustomModelValidatorProvider : System.Web.Http.Validation.Providers.DataAnnotationsModelValidatorProvider
    {
        private readonly IContainer _container;

        public CustomModelValidatorProvider(IContainer container)
            : base()
        {
            _container = container;
        }
        /// <summary>
        /// Get Validators i.e. Provide validators for a model
        /// </summary>
        /// <param name="metadata">Model Meta Data</param>
        /// <param name="validatorProviders">Validator Providers</param>
        /// <param name="attributes">Attributes</param>
        /// <returns></returns>
        protected override IEnumerable<System.Web.Http.Validation.ModelValidator> GetValidators(ModelMetadata metadata, IEnumerable<System.Web.Http.Validation.ModelValidatorProvider> validatorProviders, IEnumerable<Attribute> attributes)
        {
            //go to db if you want
            //var repository = ((MyBaseController) context.Controller).RepositorySomething;

            //find user if you need it
            //var user = context.HttpContext.User;

            //if (!string.IsNullOrWhiteSpace(metadata.PropertyName) && metadata.PropertyName == "FirstName")
            //    attributes = new List<Attribute>() { new RequiredAttribute() };

            foreach (var attr in attributes)
                _container.BuildUp(attr);

            return base.GetValidators(metadata, validatorProviders, attributes);
        }
    }
}