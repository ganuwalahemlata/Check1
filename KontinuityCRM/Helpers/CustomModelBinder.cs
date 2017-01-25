using KontinuityCRM.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace KontinuityCRM.Helpers
{
    /// <summary>
    /// Custom Model Binder
    /// </summary>
    public class CustomModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// creates model based on model type
        /// </summary>
        /// <param name="controllerContext">Controller Context</param>
        /// <param name="bindingContext">Binding Context</param>
        /// <param name="modelType">Model Type</param>
        /// <returns></returns>
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            var typeValue = bindingContext.ValueProvider.GetValue("ModelType");
            var type = Type.GetType(
                (string)typeValue.ConvertTo(typeof(string)),
                true
            );
            
            if (!modelType.IsAssignableFrom(type))
            //if (!typeof(KontinuityCRM.Models.ViewModels.AutoResponder).IsAssignableFrom(type))
            {
                throw new InvalidOperationException("Bad Type");
            }
            var model = Activator.CreateInstance(type);
            bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, type);
            return model;
        }
    }
    /// <summary>
    /// Order action model binder
    /// </summary>
    public class UrlOrderActionModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// binds model
        /// </summary>
        /// <param name="controllerContext">Controller Action</param>
        /// <param name="bindingContext">Binding Context</param>
        /// <returns></returns>
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (value != null)
            {
                var rawValues = value.RawValue as string[];
                if (rawValues != null)
                {
                    UrlOrderAction result;                    
                    if (Enum.TryParse<UrlOrderAction>(string.Join(",", rawValues), out result))
                    {
                        return result;
                    }
                }
            }
            return base.BindModel(controllerContext, bindingContext);
        }
    }
    /// <summary>
    /// Comma Seperated Model Binder
    /// </summary>
    public class CommaSeparatedValuesModelBinder : DefaultModelBinder
    {
        /// <summary>
        /// Method Info
        /// </summary>
        private static readonly MethodInfo ToArrayMethod = typeof(Enumerable).GetMethod("ToArray");
        /// <summary>
        /// Return the value of property
        /// </summary>
        /// <param name="controllerContext">Controller Context</param>
        /// <param name="bindingContext">Bind Context</param>
        /// <param name="propertyDescriptor">Propert Descriptor</param>
        /// <param name="propertyBinder">Property Binder</param>
        /// <returns></returns>
        protected override object GetPropertyValue(ControllerContext controllerContext, ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor, IModelBinder propertyBinder)
        {
            if (propertyDescriptor.PropertyType.GetInterface(typeof(IEnumerable).Name) != null)
            {
                var actualValue = bindingContext.ValueProvider.GetValue(propertyDescriptor.Name);

                if (actualValue != null && !String.IsNullOrWhiteSpace(actualValue.AttemptedValue) &&
                    actualValue.AttemptedValue.Contains(","))
                {
                    var valueType = propertyDescriptor.PropertyType.GetElementType() ??
                                    propertyDescriptor.PropertyType.GetGenericArguments().FirstOrDefault();

                    if (valueType != null && valueType.GetInterface(typeof(IConvertible).Name) != null)
                    {
                        var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(valueType));

                        foreach (var splitValue in actualValue.AttemptedValue.Split(new[] { ',' }))
                        {
                            if (valueType.IsEnum)
                            {
                                try
                                {
                                    list.Add(Enum.Parse(valueType, splitValue));
                                }
                                catch { }
                            }
                            else
                            {
                                list.Add(Convert.ChangeType(splitValue, valueType));
                            }
                        }

                        if (propertyDescriptor.PropertyType.IsArray)
                        {
                            return ToArrayMethod.MakeGenericMethod(valueType).Invoke(this, new[] { list });
                        }
                        return list;
                    }
                }
            }

            return base.GetPropertyValue(controllerContext, bindingContext, propertyDescriptor, propertyBinder);
        }
    }

    //public class OrderSearchBinder : IModelBinder
    //{

    //    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
    //    {
    //        ValueProviderResult value = bindingContext.ValueProvider.GetValue("id");

    //        OrderSearch model = new OrderSearch() { id = new () };
    //        model.Details.Id = int.Parse(value.AttemptedValue);

    //        //Or you can load the information from the database based on the Id, whatever you want.

    //        return model;
    //    }

    //}
}