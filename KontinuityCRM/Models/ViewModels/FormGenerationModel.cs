using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using KontinuityCRM.Helpers;
using System.ComponentModel.DataAnnotations;

namespace KontinuityCRM.Models.ViewModels

{

    public class FormFieldInfo
    {
        private int _order = 1;
        private int _pageIndex = 1;
        private string _label;
        private HtmlInputType _inputType = HtmlInputType.text;
        public bool Required { get; set; }
        /// <summary>
        /// Indicates Order related to FormFieldInfo
        /// </summary>
        public int Order
        {
            get { return _order; }
            set { _order = value; }
        }
        /// <summary>
        /// Indicates pageIndex
        /// </summary>
        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }
        /// <summary>
        /// Indicates Label
        /// </summary>
        public string Label
        {
            get { return string.IsNullOrEmpty(_label) ? Name : _label; }
            set { _label = value; }
        }
        /// <summary>
        /// Indicates Name of FormFieldInfo
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Indicates whether the FormFieldInfo is visible or not
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// Validation expression for the formFieldInfo
        /// </summary>
        public string ValidationExpression { get; set; }
        /// <summary>
        /// Indicates the formFieldInfo Input Type
        /// </summary>
        public HtmlInputType InputType
        {
            get { return _inputType; }
            set { _inputType = value; }
        }
    }

    public class formname
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
    public class Customerinfo
    {
        [StringLength(100)]
        [Display(Description = "First Name")]
        public string FirstName { get; set; }
        [StringLength(100)]
        [Display(Description = "Last Name")]
        public string LastName { get; set; }
        [StringLength(100)]
        [Display(Description = "Address Line1")]
        public string Address1 { get; set; }
        [StringLength(100)]
        [Display(Description = "Address Line2")]
        public string Address2 { get; set; }
        [StringLength(100)]
        [Display(Description = "City")]
        public string City { get; set; }
        [StringLength(100)]
        [Display(Description = "State")]
        public string State { get; set; }
        [StringLength(100)]
        [Display(Description = "Country")]
        public string Country { get; set; }
        [StringLength(100)]
        [Display(Description = "Postal Code")]
        public string PostalCode { get; set; }
        [StringLength(100)]
        [Display(Description = "Email")]
        public string Email { get; set; }
        [StringLength(100)]
        [Display(Description = "Phone")]
        public string Phone { get; set; }

    }

    public class Cardinfo
    {
        [StringLength(100)]
        [Display(Description = "Credit Card Number")]
        public string CreditCardNumber { get; set; }

        [StringLength(100)]
        [Display(Description = "Expiry Date")]
        public string CCExpiryDate { get; set; }

        [StringLength(100)]
        [Display(Description = "Expiry Date")]
        public string CreditCardCVV { get; set; }

      

    }

    public class FormGenerationModel1
    {
        private IList<Product> _products = new List<Product>();

        public Customerinfo _Customerinfo = new Customerinfo();

        public Cardinfo _Cardinfo = new Cardinfo();
        /// <summary>
        /// Defines the page the shipping method data will be displayed
        /// </summary>


        /// <summary>
        /// List of products associated to the form generation process
        /// </summary>
        public IList<Product> Products
        {
            get { return _products; }
            set { _products = value; }
        }
        public Customerinfo Customers
        {
            get { return _Customerinfo; }
            set { _Customerinfo = value; }
        }

        public Cardinfo CardInfo
        {
            get { return _Cardinfo; }
            set { _Cardinfo = value; }
        }





    }
    public class FormGenerationModel
    {
        private IList<Product> _products = new List<Product>();
        private IList<ShippingMethod> _shippingMethods = new List<ShippingMethod>();
        private IList<FormFieldInfo> _formFields = new List<FormFieldInfo>();
       private IList<formname> _formnames = new List<formname>();

        private Customerinfo _custInfo = new Customerinfo();
        /// <summary>
        /// Defines the page the shipping method data will be displayed
        /// </summary>
        public int ShippingMethodPage { get; set; }
        /// <summary>
        /// Defines where in the page the shipping method will be displayed
        /// </summary>
        public int ShippingMethodOrder { get; set; }
        /// <summary>
        /// Defines the page the product information will be displayed
        /// </summary>
        public int ProductPage { get; set; }
        /// <summary>
        /// Defines where in the page the product will be displayed
        /// </summary>
        public int ProductOrder { get; set; }

        /// <summary>
        /// Defines if the generated HTML should include POST requests to the API
        /// If false, no POST code should be generated
        /// </summary>
        public bool IsPreview { get; set; }
        public IList<int> ProductId { get; set; }
        public IList<int> ShippingMethodId { get; set; }

        [DisplayName("User prospect")]
        public bool UseProspect { get; set; }
        public bool BillingAddressDifferent { get; set; }
        public int TotalPages { get; set; }

        /// <summary>
        /// List of products associated to the form generation process
        /// </summary>
        public IList<Product> Products
        {
            get { return _products; }
            set { _products = value; }
        }

        public IList<ShippingMethod> ShippingMethods
        {
            get { return _shippingMethods; }
            set { _shippingMethods = value; }
        }

        public IList<FormFieldInfo> FormFields
        {
            get { return _formFields; }
            set { _formFields = value; }
        }

        public IList<formname> FormNames
        {
            get { return _formnames; }
            set { _formnames = value; }
        }

        public Customerinfo CustomerInfo
        {
            get { return _custInfo; }
            set { _custInfo = value; }
        }

        public List<FormFieldInfo> GetPropertiesForPage(int page)
        {
            return FormFields.Where(f => f.PageIndex == page + 1 && f.Visible).OrderBy(f => f.Order).ToList();
        }

        public bool HasBillingFields()
        {
            return FormFields.Any(f => f.Name.StartsWith("Billing") && f.Visible);
        }



    }

    /// <summary>
    /// Information required to render the HTML component
    /// </summary>
    public class FormGenerationInfo
    {
        /// <summary>
        /// Label for the component
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Name of the component
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// HTML input type
        /// </summary>
        public HtmlInputType Type { get; set; }
        /// <summary>
        /// Required field
        /// </summary>
        public bool Required { get; set; }
        /// <summary>
        /// Regular expression to validate against. If no regular expression is required, this field has an empty string value
        /// </summary>
        public string RegularExpression { get; set; }
    }
}