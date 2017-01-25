using System.ComponentModel.DataAnnotations;

namespace KontinuityCRM.Models.ViewModels.Enum
{
    public enum RebillSearchOptionEnum : int
    {
        [Display(Name = "Rebill Date")]
        RebillDate = 1,     // search by order's rebill date
        [Display(Name = "Sign Up Date")]
        SignUpDate = 2      // order's create date
    }
}