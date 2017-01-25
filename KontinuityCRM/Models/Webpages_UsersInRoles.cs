using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace KontinuityCRM.Models
{
    /// <summary>
    /// Webpages UsersInRoles.
    /// </summary>
    [Table("webpages_UsersInRoles")]
    public class Webpages_UsersInRoles
    {
        /// <summary>
        /// User Id.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Role Id.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RoleId { get; set; }

    }

}