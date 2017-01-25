using System.ComponentModel;

namespace KontinuityCRM.Models.Enums
{
    /// <summary>
    /// billtype for declineSalvage, product, orderProduct
    /// </summary>
    public enum BillType
    {
        /// <summary>
        /// Bills every n days
        /// </summary>
        [Description("By Cycle")]
        ByCycle = 1,

        /// <summary>
        /// Bills on the nth day of every month
        /// </summary>
        [Description("By Date")]
        ByDate,

        /// <summary>
        /// Bills the first Sunday of every month
        /// </summary>
        [Description("By Day")]
        ByDay,
    }
}