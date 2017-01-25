using System;
using System.Web.Http;
using System.Web.Mvc;
using KontinuityCRM.Areas.HelpPage.ModelDescriptions;
using KontinuityCRM.Areas.HelpPage.Models;

namespace KontinuityCRM.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    [System.Web.Mvc.Authorize]
    public class HelpController : Controller
    {
        private const string ErrorViewName = "Error";

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpController"/> class.
        /// </summary>
        public HelpController()
            : this(GlobalConfiguration.Configuration)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpController" /> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public HelpController(HttpConfiguration config)
        {
            Configuration = config;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public HttpConfiguration Configuration { get; private set; }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Index()
        {
            ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
            return View(Configuration.Services.GetApiExplorer().ApiDescriptions);
        }

        /// <summary>
        /// APIs the specified API identifier.
        /// </summary>
        /// <param name="apiId">The API identifier.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Api(string apiId)
        {
            if (!String.IsNullOrEmpty(apiId))
            {
                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                if (apiModel != null)
                {
                    return View(apiModel);
                }
            }

            return View(ErrorViewName);
        }

        /// <summary>
        /// Resources the model.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult ResourceModel(string modelName)
        {
            if (!String.IsNullOrEmpty(modelName))
            {
                ModelDescriptionGenerator modelDescriptionGenerator = Configuration.GetModelDescriptionGenerator();
                ModelDescription modelDescription;
                if (modelDescriptionGenerator.GeneratedModels.TryGetValue(modelName, out modelDescription))
                {
                    return View(modelDescription);
                }
            }

            return View(ErrorViewName);
        }

        /// <summary>
        /// Authentications this instance.
        /// </summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Authentication()
        {
            return View();
        }

    }
}