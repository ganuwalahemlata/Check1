using System;
using System.ComponentModel.DataAnnotations;
using KontinuityCRM.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace KontinuityCRM.Models
{
    [TrackChanges]
    public class FormGeneration
    {
        /// <summary>
        /// Form generation Id, Indicated as primary key
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        /// <summary>
        /// Indicates the Content related t formGeneration
        /// </summary>
        [Required] 
        public string Content { get; set; }
        /// <summary>
        /// Indicates Create Date of FormGeneration
        /// </summary>
        public DateTime GenerationDate { get; set; }



        public string Name { get; set; }

        /// <summary>
        /// Add FormGeneration to Db
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="save"></param>
        public void Create(IUnitOfWork uow, bool save = true)
        {
            uow.FormGenerationRepository.Add(this);
            this.GenerationDate = DateTime.Now; 
            
            if (save)
            {
                uow.Save();
            }
        }
    }

}