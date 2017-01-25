using System.ComponentModel.DataAnnotations;
using KontinuityCRM.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class PrepaidInfo
    {
        /// <summary>
        /// Id of PrepaidInfo as Primary Key
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Indicates the First 6 Digits of prepaidInfo
        /// </summary>
        [Index(IsUnique = true)]
        [MaxLength(6), MinLength(6)] 
        [Required] 
        public string BIN { get; set; }
        /// <summary>
        /// Indicates whether the info is prepaid
        /// </summary>
        [Required] 
        public bool Prepaid { get; set; }


        public void Create(IUnitOfWork uow, bool save = true)
        {
            uow.PrepaidInfoRepository.Add(this);
            
            if (save)
            {
                uow.Save();
            }
        }
    }

}