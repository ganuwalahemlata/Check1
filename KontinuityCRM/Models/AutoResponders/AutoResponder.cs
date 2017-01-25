using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using AutoMapper;

namespace KontinuityCRM.Models.AutoResponders
{
    /// <summary>
    /// Interface for AutoResponder
    /// </summary>
    interface IAutoResponder
    {
        /// <summary>
        /// AddContact
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="campaign"></param>
        /// <returns></returns>
        Task<string> AddContact(Contact contact, string campaign);
        /// <summary>
        /// RemoveContact
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="campaign"></param>
        /// <returns></returns>

        Task<string> RemoveContact(Contact contact, string campaign);
        /// <summary>
        /// MoveContact
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="origincampaign"></param>
        /// <param name="destinationcampaign"></param>
        /// <returns></returns>

        Task<string> MoveContact(Contact contact, string origincampaign, string destinationcampaign);
    }

    public abstract class AutoResponder : IAutoResponder
    {
        /// <summary>
        /// AutoResponder Id as primary key
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Indicates Alias of AutoResponder
        /// </summary>
        [Required]
        public string Alias { get; set; }
        /// <summary>
        /// Indicates CreateDate of AutoResponder
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Indicates AutoResponder Type
        /// </summary>
        public string Type { get { return this.GetType().Name; } }

        /// <summary>
        /// AddContact
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="campaign"></param>
        /// <returns></returns>
        public abstract Task<string> AddContact(Contact contact, string campaign);
        /// <summary>
        /// RemoveContact
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="campaign"></param>
        /// <returns></returns>
        public abstract Task<string> RemoveContact(Contact contact, string campaign);

        /// <summary>
        /// MoveContact
        /// </summary>
        /// <param name="contact"></param>
        /// <param name="origincampaign"></param>
        /// <param name="destinationcampaign"></param>
        /// <returns></returns>
        public abstract Task<string> MoveContact(Contact contact, string origincampaign, string destinationcampaign);
        /// <summary>
        /// Get AutoResponder Provider
        /// </summary>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public AutoResponderProvider Provider(IMappingEngine mapper)
        {
            return mapper.Map(this, this.GetType(), typeof(AutoResponderProvider)) as AutoResponderProvider;
        }


    }
}