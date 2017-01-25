using StructureMap;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM.Helpers.IoC
{
    /// <summary>
    /// Data Annotations Model Validator
    /// </summary>
    public class StructureMapDataAnnotationsModelValidator : DataAnnotationsModelValidator  
    {
        private readonly IContainer _container;

        public StructureMapDataAnnotationsModelValidator(ModelMetadata metadata, ControllerContext context, ValidationAttribute attribute, IContainer container) 
            : base(metadata, context, attribute)
        {
            _container = container;
        }
        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="container">container</param>
        /// <returns></returns>
        public override IEnumerable<ModelValidationResult> Validate(object container)
        {
            //if (!string.IsNullOrWhiteSpace(Metadata.PropertyName) && Metadata.PropertyName == "CreditCardNumber")
            //{
            //    var creditcardattribute

            //}
            //    attributes = new List<Attribute>() { new RequiredAttribute() };


            /* Ask StructureMap to do setter injection for all properties decorated with SetterProperty attribute*/
            _container.BuildUp(Attribute);
            return base.Validate(container);
        }

    }

    /// <summary>
    /// Not used
    /// </summary>
    public class CustomModelProviderMVC : DataAnnotationsModelValidatorProvider
    {
        private readonly IContainer _container;

        public CustomModelProviderMVC(IContainer container) 
            : base()
        {
            _container = container;
        }
        /// <summary>
        /// Get Validators
        /// </summary>
        /// <param name="metadata">model meta data</param>
        /// <param name="context">Controller Context</param>
        /// <param name="attributes">Attributes</param>
        /// <returns></returns>
        protected override IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ControllerContext context, IEnumerable<Attribute> attributes)
        {
            foreach (var attr in attributes)
                _container.BuildUp(attr);

            //if (!string.IsNullOrWhiteSpace(metadata.PropertyName) && metadata.PropertyName == "CreditCardNumber")
            //{
            //    attributes = new List<Attribute> { new CreditCardAttribute() };
            //}

            return base.GetValidators(metadata, context, attributes);
        }
    }
}


   
