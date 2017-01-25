using AutoMapper;
using KontinuityCRM.Helpers;
using KontinuityCRM.Helpers.MappingHelpers;
using KontinuityCRM.Models;
using KontinuityCRM.Models.AutoResponders;
using KontinuityCRM.Models.Fulfillments;
using KontinuityCRM.Models.Gateways;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace KontinuityCRM
{
    public static class DtoMapperConfig
    {
        public static void CreateMaps()
        {
            Mapper.CreateMap<KontinuityCRM.Models.APIModels.PartialCreateModel, Partial>();
            Mapper.CreateMap<KontinuityCRM.Models.APIModels.OrderCreateModel, Order>();

            Mapper.CreateMap<KontinuityCRM.Models.APIModels.OrderProduct, OrderProduct>().ReverseMap();

            Mapper.CreateMap<Partial, KontinuityCRM.Models.APIModels.PartialAPIModel>();
            Mapper.CreateMap<Order, KontinuityCRM.Models.APIModels.OrderAPIModel>()
                .ForMember(dest => dest.ShippingPrice, opt => opt.MapFrom(src => src.ShippingMethod.Price))
                .AfterMap((src, dest) =>
                {
                    foreach (var op in dest.OrderProducts)
                    {
                        var po = src.OrderProducts.Where(o => o.ProductId == op.ProductId).First();
                        op.ProductName = (po.Product == null)? string.Empty : po.Product.Name;
                    }
                });

            Mapper.CreateMap<Order, KontinuityCRM.Models.APIModels.OrderAPIModel>()
                .ForMember(dest => dest.ShippingPrice, opt => opt.MapFrom(src => src.ShippingMethod.Price));

            Mapper.CreateMap<Customer, Contact>();
            Mapper.CreateMap<Partial, Contact>();
            Mapper.CreateMap<Product, OrderProduct>();
            Mapper.CreateMap<OrderProduct, KontinuityCRM.Models.APIModels.OrderProductResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            Mapper.CreateMap<Order, KontinuityCRM.Models.APIModels.OrderCreationResponse>()
                .ForMember(dest => dest.Prepaid, opt => opt.MapFrom(src => src.IsPrepaid))
                .ForMember(dest => dest.ShippingPrice, opt => opt.MapFrom(src => src.ShippingMethod.Price))
                .AfterMap((src, dest) =>
                {
                    if (src.OrderProducts != null)
                    {
                        var orderProducts = new List<KontinuityCRM.Models.APIModels.OrderProductResponse>();
                        foreach (var op in src.OrderProducts)
                        {
                            orderProducts.Add(new KontinuityCRM.Models.APIModels.OrderProductResponse
                            {
                                Price = op.Price ?? 0,
                                ProductId = op.ProductId,
                                ProductName = (op.Product == null)? string.Empty : op.Product.Name,
                            });
                        }
                        dest.OrderProducts = orderProducts;                        
                    }

                    if (src.Transactions != null)
                    {
                        var transaction = src.Transactions.FirstOrDefault();
                        dest.Descriptor = transaction == null ? "" : transaction.Processor.Descriptor;
                        dest.CustomerServiceNumber = transaction == null ? "" : transaction.Processor.CustomerServiceNumber;
                        dest.TransactionResponse = transaction == null ? "" : transaction.Message;                        
                    }
                });



            #region AutoResponders
           
            Mapper.CreateMap<AutoResponderProvider, AutoResponder>()
               .Include<AutoResponderProvider, iContact>()
               .Include<AutoResponderProvider, GetResponse>() 
               .ReverseMap()
               .Include<iContact, AutoResponderProvider>()
               .Include<GetResponse, AutoResponderProvider>();

           
            Mapper.CreateMap<AutoResponderProvider, iContact>()
                 .ForMember(dest => dest.ClientFolder, opt => opt.MapFrom(src => src.ApiSecret))
                 .ReverseMap()
                 .ForMember(dest => dest.ApiSecret, opt => opt.MapFrom(src => src.ClientFolder));

          
            Mapper.CreateMap<AutoResponderProvider, GetResponse>()
                .ReverseMap();

            #endregion

           
            #region Fulfillment Providers

            Mapper.CreateMap<FulfillmentProvider, Fulfillment>()
              .Include<FulfillmentProvider, Shipwire>()
              .ReverseMap()
              .Include<Shipwire, FulfillmentProvider>();

            Mapper.CreateMap<FulfillmentProvider, Shipwire>()
               .ReverseMap();
  Mapper.CreateMap<FulfillmentProvider, Fulfillment>()
              .Include<FulfillmentProvider, ShipFusion>()
              .ReverseMap()
              .Include<ShipFusion, FulfillmentProvider>();

            Mapper.CreateMap<FulfillmentProvider, ShipFusion>()
               .ReverseMap();
            #endregion
            
            
            #region Processor

            Mapper.CreateMap<Processor, GatewayModel>()
               .Include<Processor, NMI>()
               .Include<Processor, Argus>()
               .Include<Processor, AuthorizeNET>()
               .Include<Processor, SecureTrading>()
               .ReverseMap()
               .Include<NMI, Processor>()
               .Include<Argus, Processor>()
               .Include<AuthorizeNET, Processor>()
               .Include<SecureTrading, Processor>();

            Mapper.CreateMap<Processor, AuthorizeNET>();

            Mapper.CreateMap<Processor, SecureTrading>()
            .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.SiteId))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
            .ReverseMap()
            .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.SiteId))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency));

            Mapper.CreateMap<Processor, Argus>()
               .ForMember(dest => dest.Auth, opt => opt.MapFrom(src => src.UsePreAuthorizationFilter))
               .ReverseMap()
               .ForMember(dest => dest.UsePreAuthorizationFilter, opt => opt.MapFrom(src => src.Auth));
              

            Mapper.CreateMap<Processor, NMI>()
                
                 .AfterMap((src, dest) =>
                 {
                     // deserialize mdf fields and set them to the model

                     if (src.Parameters != null)
                     {
                         var dictionary = (Dictionary<string, string>)KontinuityCRMHelper.ByteArrayToObject(src.Parameters);

                         foreach (var key in dictionary.Keys)
                         {
                             var propertyInfo = dest.GetType().GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
                             propertyInfo.SetValue(dest, dictionary[key]);

                         }
                     }
                 })
                .ReverseMap()
                .AfterMap((src, dest) =>
                {
                    // deserialize mdf fields and set them to the model

                    var dictionary = new Dictionary<string, string>();

                    foreach (var prop in src.GetType().GetProperties().Where(p => p.Name.StartsWith("MDF")))
                    {
                        var propvalue = prop.GetValue(src);

                        if (propvalue != null)
                        {
                            dictionary.Add(prop.Name, (string)propvalue);
                        }

                    }
                    if (dictionary.Any())
                    {
                        dest.Parameters = KontinuityCRMHelper.ObjectToByteArray(dictionary);
                    }
                });
                //.ForMember(dest => dest.Parameters, opt => opt.ResolveUsing<CustomResolver>().ConstructedBy(() => new CustomResolver()));

            #endregion

        }
    }

}
