using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models.ViewModels
{
    #region Response of Create Shipment API ShipFusion

    public class ResponseShipFusion
    {
        public string responseStatus { get; set; }
        public string shipmentId { get; set; }
        public string message { get; set; }
    }

    #endregion

    #region PostBack response of  API ShipFusion

    public class ShipFusionPostBackResponse
    {
        public string id { get; set; }
        public string type { get; set; }
        public string shipmentId { get; set; }
        public string orderNumber { get; set; }
        public string timestamp { get; set; }
        public ShipFusionTracking tracking { get; set; }
    }
    public class ShipFusionTracking
    {
        public int totalPackages { get; set; }
        public List<ShipFusionTrackingPackages> packages { get; set; }
    }
    public class ShipFusionTrackingPackages
    {
        public string shippingCarrier { get; set; }
        public string shippingService { get; set; }
        public string shipDate { get; set; }
        public string shipTimestamp { get; set; }
        public string trackingNumber { get; set; }
    }

    #endregion

    #region Response of Retrieve Shipment API ShipFusion

    public class ResponseShipmentShipFusion
    {
        public string shipmentId { get; set; }
        public string orderNumber { get; set; }
        public string warehouse { get; set; }
        public string orderDate { get; set; }
        public string orderTimestamp { get; set; }
        public string shipmentStatus { get; set; }
        public ShipmentAddress address { get; set; }
        public List<ShipmentItems> shipmentItems { get; set; }
        public dynamic tracking { get; set; }

    }
    public class ShipmentAddress
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string zip { get; set; }
        public string phoneNumber { get; set; }
        public string emailAddress { get; set; }
    }
    public class ShipmentItems
    {
        public string shipmentItemId { get; set; }
        public string SKU { get; set; }
        public string status { get; set; }
        public string tag { get; set; }
    }
    public class ShipmentTracking
    {
        public string shippingCarrier { get; set; }
        public string shippingService { get; set; }
        public string shipDate { get; set; }
        public string shipTimeStamp { get; set; }
        public string[] trackingNumbers { get; set; }
    }

    #endregion

    public enum FullFillmentProvidersEnum
    {
        Shipiwire,
        Shipfusion
    }

    public enum ShipfusionShipmentStatus
    {
        inQueue,
        processing,
        shipped,
        backOrder,
        onHold,
        cancelled,
        all
    }
}
