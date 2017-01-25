using AutoMapper;
using KontinuityCRM.Models.Gateways;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Helpers.MappingHelpers
{
    /// <summary>
    /// Custom Resolver
    /// </summary>
    public class CustomResolver : ValueResolver<NMI, byte[]>
    {
        /// <summary>
        /// Resolve Core
        /// </summary>
        /// <param name="gatewayModel">Gateway Model</param>
        /// <returns></returns>
        protected override byte[] ResolveCore(NMI gatewayModel)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var prop in gatewayModel.GetType().GetProperties().Where(p => p.Name.StartsWith("MDF")))
            {
                var propvalue = prop.GetValue(gatewayModel);

                if (propvalue != null)
                {
                    dictionary.Add(prop.Name, (string)propvalue);
                }
                                
            }
            if (dictionary.Any())
            {
                return KontinuityCRMHelper.ObjectToByteArray(dictionary);
            }

            return null;
        }
    }

    
}