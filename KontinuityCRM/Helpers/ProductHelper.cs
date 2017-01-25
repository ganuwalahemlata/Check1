using KontinuityCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Helpers
{
    public class ProductHelper
    {
        Product product;
        public ProductHelper(Product product) 
        {
            this.product = product;
        }
        public int ProductId
        {
            get { return product.ProductId; }
        }
        public string Name 
        { 
            get{return product.Name;}

            set { product.Name = value; }
        }
        public string Description 
        {
            get 
            {
                return product.Description;
            }
            set
            {
                product.Description = value;
            }
        }
        //public int CategoryId 
        //{
        //    get 
        //    {
        //        return product.CategoryId;
                    
        //    }
        //    set 
        //    {
        //        product.CategoryId = value;
        //    } 
        //}
        public string SKU
        {
            get { return product.SKU; }
            set { product.SKU = value; } 
        }
        public double Cost 
        {
            get 
            {
                return product.Cost;
            }
            set 
            {
                product.Cost = value;
            }
        }
        public double Price
        {
            get
            {
                return product.Price;
            }
            set
            {
                product.Price = value;
            }
        }
        public Currency Currency
        {
            get
            {
                return product.Currency;
            }
            set
            {
                product.Currency = value;
            }
        }
        public bool IsTaxable {  get 
            {
                return product.IsTaxable;
            }
            set 
            {
                product.IsTaxable = value;
            }
        }
        public bool IsShippable
        {
            get
            {
                return product.IsShippable;
            }
            set
            {
                product.IsShippable = value;
            }
        }
        public double Weight 
        {
            get
            {
                return product.Weight;
            }
            set
            {
                product.Weight = value;
            }
        }
        public double ShipValue
        {
            get
            {
                return product.ShipValue;
            }
            set
            {
                product.ShipValue = value;
            }
        }
        public bool IsSignatureConfirmation
        {
            get
            {
                return product.IsSignatureConfirmation;
            }
            set
            {
                product.IsSignatureConfirmation = value;
            }
        }
        public int FullfillmentProviderId
        {
            get
            {
                return product.FulfillmentProviderId;
            }
            set
            {
                product.FulfillmentProviderId = value;
            }
        }
        public bool IsDeliveryConfirmation
        {
            get
            {
                return product.IsDeliveryConfirmation;
            }
            set
            {
                product.IsDeliveryConfirmation = value;
            }
        }
        public int RecurringProductId 
        {
            get
            {
                return product.RecurringProductId;
            }
            set
            {
                product.RecurringProductId = value;
            }
        }
        public BillType BillType
        {
            get
            {
                return product.BillType;
            }
            set
            {
                product.BillType = value;
            }
        }
        private DateTime billdate = DateTime.Now;
        public DateTime BillDate 
        {
            get 
            {
               
                if (product.BillType == Models.BillType.Date)
                {
                    try
                    {
                        billdate = DateTime.Parse(product.BillValue);
                    }
                    catch { }
                } 
                return billdate; 
            }
            set 
            {
                billdate = value;
                if (BillType == Models.BillType.Date)
                    product.BillValue = billdate.ToString();
            }
        }

        private int billcycle;
        public int BillCycle
        {
            get
            {

                if (product.BillType == Models.BillType.Cycle)
                {
                    try
                    {
                        billcycle = int.Parse(product.BillValue);
                    }
                    catch { }
                }
                return billcycle;
            }
            set
            {
                billcycle = value;
                if (BillType == Models.BillType.Cycle)
                    product.BillValue = billcycle.ToString();
            }
        }

        private BillDay billday  = new BillDay(); 
        public BillDay BillDay
        {
            get
            {

                if (product.BillType == Models.BillType.Day)
                {
                    try
                    {
                        string [] aux = product.BillValue.Split(new char[1]{','});
                        billday = new BillDay();
                        billday.Week = int.Parse(aux[0]);
                        billday.Day = int.Parse(aux[1]);
                    }
                    catch { }
                }
                return billday;
            }
            set
            {
                billday = value;
                if (BillType == Models.BillType.Day)
                    product.BillValue = billday.Week + "," + billday.Day;
            }
        }

        public BillWeek Week 
        {
            get
            {
                return (BillWeek)billday.Week;
            }
            set 
            {
                billday.Week = (int)value;
                if (BillType == Models.BillType.Day)
                    product.BillValue = billday.Week + "," + billday.Day;
            }
        }
        public DayOfWeek Days
        {
            get
            {
                return (DayOfWeek)billday.Day;
            }
            set
            {
                billday.Day = (int)value;
                if (BillType == Models.BillType.Day)
                    product.BillValue = billday.Week + "," + billday.Day;
            }
        }

        public string VariantsId { get; set; }
        public string PostBackIds { get; set; }
        public string EventsIds { get; set; }

    }
    public class BillDay 
    {
        public int Week { get; set; }
        public int Day { get; set; }
    }
    public enum BillWeek 
    {
        First = 0,
        Second,
        Third,
        Fourth,
        Last,
    }
    public enum EnumDays
    {
        Sunday = 0,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,

    }
}