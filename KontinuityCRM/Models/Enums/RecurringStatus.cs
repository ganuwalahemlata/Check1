using System.ComponentModel.DataAnnotations;

namespace KontinuityCRM.Models.Enums
{
    /// <summary>
    /// entity status for Order Products Recurring
    /// </summary>

    public enum RecurringStatus
    {
        [Display(Name = "Not Active")]
        NotActive = 0,
        Active = 1

    }
}