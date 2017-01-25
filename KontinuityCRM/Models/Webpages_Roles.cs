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
    /// Webpages Roles.
    /// </summary>
    [Table("webpages_Roles")]
    public class webpages_Roles
    {
        /// <summary>
        /// Role Id.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RoleId { get; set; }

        /// <summary>
        /// Role Name.
        /// </summary>
        public String RoleName { get; set; }
    }

}