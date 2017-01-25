using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Helpers.IoC
{
    public abstract class ServiceLocatorImplBase : IServiceLocator
    {
        public virtual object GetService(Type serviceType)
        {
            return GetInstance(serviceType, null);
        }
        /// <summary>
        /// Get Instance
        /// </summary>
        /// <param name="serviceType">Type</param>
        /// <returns></returns>
        public virtual object GetInstance(Type serviceType)
        {
            return GetInstance(serviceType, null);
        }
        /// <summary>
        /// Get Instance with key
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual object GetInstance(Type serviceType, string key)
        {
            try
            {
                return DoGetInstance(serviceType, key);
            }
            catch (Exception ex)
            {
                throw new ActivationException(
                    FormatActivationExceptionMessage(ex, serviceType, key),
                    ex);
            }
        }
        /// <summary>
        /// Get All Instances
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public virtual IEnumerable<object> GetAllInstances(Type serviceType)
        {
            try
            {
                return DoGetAllInstances(serviceType);
            }
            catch (Exception ex)
            {
                throw new ActivationException(
                    FormatActivateAllExceptionMessage(ex, serviceType),
                    ex);
            }
        }
        /// <summary>
        /// Get Instance
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public virtual TService GetInstance<TService>()
        {
            return (TService)GetInstance(typeof(TService), null);
        }
        /// <summary>
        /// Get Instance with key
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TService GetInstance<TService>(string key)
        {
            return (TService)GetInstance(typeof(TService), key);
        }
        /// <summary>
        /// Get All Instances
        /// </summary>
        /// <typeparam name="TService">Service Type</typeparam>
        /// <returns></returns>
        public virtual IEnumerable<TService> GetAllInstances<TService>()
        {
            foreach (object item in GetAllInstances(typeof(TService)))
            {
                yield return (TService)item;
            }
        }
        /// <summary>
        /// Get Instance with key
        /// </summary>
        /// <param name="serviceType">Type</param>
        /// <param name="key">key</param>
        /// <returns></returns>
        protected abstract object DoGetInstance(Type serviceType, string key);
        /// <summary>
        /// Get All Instances
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        protected abstract IEnumerable<object> DoGetAllInstances(Type serviceType);
        /// <summary>
        /// Format Activation Exception Messages
        /// </summary>
        /// <param name="actualException">Exception</param>
        /// <param name="serviceType">Type</param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected virtual string FormatActivationExceptionMessage(Exception actualException, Type serviceType, string key)
        {
            //return string.Format(CultureInfo.CurrentUICulture, Resources.ActivationExceptionMessage, serviceType.Name, key);
            return string.Format(CultureInfo.CurrentUICulture, actualException.Message, serviceType.Name, key);
        }
        /// <summary>
        /// Format Activate Exception Messages
        /// </summary>
        /// <param name="actualException">Exception</param>
        /// <param name="serviceType">Type</param>
        /// <returns></returns>
        protected virtual string FormatActivateAllExceptionMessage(Exception actualException, Type serviceType)
        {
            //return string.Format(CultureInfo.CurrentUICulture, Resources.ActivateAllExceptionMessage, serviceType.Name);
            return string.Format(CultureInfo.CurrentUICulture, actualException.Message, serviceType.Name);
        }
    }
}
