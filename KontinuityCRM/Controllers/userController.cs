using KontinuityCRM.Filters;
using KontinuityCRM.Helpers;
using KontinuityCRM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using TrackerEnabledDbContext.Common.Models;
using WebMatrix.WebData;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("Users")]
    public class userController : BaseController
    {
        public userController(IUnitOfWork uow, IWebSecurityWrapper wsw)
            : base(uow, wsw)
        {

        }
        /// <summary>
        /// Redirects to userProfile Listing View
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="searchString"></param>
        /// <param name="page"></param>
        /// <param name="display"></param>
        /// <returns></returns>
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.UserDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.UserDisplay = pageSize;

                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewBag.UserNameSortParm = sortOrder == "username" ? "username_desc" : "username";
            ViewBag.APIKeySortParm = sortOrder == "apikey" ? "apikey_desc" : "apikey";

            ViewBag.usernameOrderIcon = "sort";
            ViewBag.apikeyOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";

            Func<IQueryable<UserProfile>, IOrderedQueryable<UserProfile>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.UserId);

                    case "username":
                        ViewBag.usernameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.UserName).ThenByDescending(c => c.UserId);
                    case "username_desc":
                        ViewBag.usernameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.UserName).ThenByDescending(c => c.UserId);

                    case "apikey":
                        ViewBag.apikeyOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.APIKey).ThenByDescending(c => c.UserId);
                    case "apikey_desc":
                        ViewBag.apikeyOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.APIKey).ThenByDescending(c => c.UserId);

                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.UserId);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<UserProfile, bool>> filter = f => true;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.UserName.Contains(searchString)
                    //|| f.LastName.Contains(searchString)
                    //|| f.Address1.Contains(searchString)
                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    //|| f.Email.Contains(searchString)
                    //|| f.Phone.Contains(searchString)
                    ;
            }

            Expression<Func<UserProfile, bool>> filter2 = f => f.UserId != wsw.CurrentUserId;

            Expression<Func<UserProfile, bool>> filter3 = f => f.Status == null || f.Status == true;

            var lambda = filter2.AndAlso(filter).AndAlso(filter3);


            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.UserProfileRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: lambda,
                orderBy: orderBy
                );

            return View(model);


            //return View(uow.UserProfileRepository.Get(u => u.UserId != wsw.CurrentUserId));
        }
        /// <summary>
        /// Redirect to create UserProfile View
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            CreateViewBag();
            return View();
        }
        private void CreateViewBag()
        {
            var assembly = System.Reflection.Assembly.Load("EnumAssembly");

            ViewBag.GroupId = uow.UserGroupRepository.Get(u => u.Status == true);
            ViewBag.TimeZones = uow.TimeZoneRepository.Get().ToList();
        }
        /// <summary>
        /// Post Action to create UserProfile
        /// </summary>
        /// <param name="model">RegisterModel</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(RegisterModel model)
        {
            if (ModelState.IsValid)
            {


                WebSecurity.CreateUserAndAccount(model.UserName, model.Password, propertyValues: new
                {
                    Email = model.Email,
                    UserGroupId = model.GroupId,
                    APIKey = Guid.NewGuid(),
                    Status = true,
                    TimeZoneId = model.TimeZoneId
                });

                var roleProvider = (SimpleRoleProvider)System.Web.Security.Roles.Provider;
                var group = uow.UserGroupRepository.Find(model.GroupId);
                if (group != null && group.Roles != null && group.Roles.Count > 0)
                    roleProvider.AddUsersToRoles(new[] { model.UserName }, group.Roles.Select(r => r.Role).ToArray());

                return RedirectToAction("index");

            }

            return View(model);
        }
        /// <summary>
        /// Redirect to Edit UserProfile View
        /// </summary>
        /// <param name="id">UsserProfile id</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            //WebSecurity.
            var user = uow.UserProfileRepository.Find(id); // repo.FindUserProfile(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            CreateViewBag();
            return View(user);
        }
        /// <summary>
        /// Post Action to update userProfile
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(UserProfile user)
        {
            if (ModelState.IsValid)
            {
                var iscurrentuser = user.UserId == wsw.CurrentUserId; //WebSecurity.CurrentUserId;
                var up = uow.UserProfileRepository.Find(user.UserId); //repo.FindUserProfile(user.UserId); // need to find to avoid exception about already attached entity  

                if (!iscurrentuser) // means that the permissions is not disabled
                {
                    up.UserGroupId = user.UserGroupId;                    
                    up.UserName = user.UserName;
                }

                up.Email = user.Email;
                up.Status = true;
                up.TimeZoneId = user.TimeZoneId;
                //repo.EditUserProfile(up);
                uow.UserProfileRepository.Update(up);
                uow.Save(wsw.CurrentUserName);

                var roleProvider = (SimpleRoleProvider)System.Web.Security.Roles.Provider;
                var rolesForUser = roleProvider.GetRolesForUser(user.UserName);
                roleProvider.RemoveUsersFromRoles(new[] { user.UserName }, rolesForUser);
                var group = uow.UserGroupRepository.Find(user.UserGroupId);
                if (group != null && group.Roles != null && group.Roles.Count > 0)
                    roleProvider.AddUsersToRoles(new[] { user.UserName }, group.Roles.Select(r => r.Role).ToArray());                

                return RedirectToAction("index");
            }
            return View(user);
        }

    
        //public ActionResult Logs(int id, string sortOrder, string currentFilter, string searchString, int? page, int? display)
        //{
        //    var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
        //    int pageSize = user.AuditLogDisplay;

        //    if (display.HasValue && display != pageSize)
        //    {
        //        page = 1;
        //        pageSize = display.Value;

        //        user.AuditLogDisplay = pageSize;

        //        uow.UserProfileRepository.Update(user);
        //        uow.Save(wsw.CurrentUserName);
        //    }
        //    if (searchString != null)
        //        page = 1;
        //    else
        //        searchString = currentFilter;

        //    ViewBag.CurrentSort = sortOrder;
        //    ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
        //    ViewBag.TableNameSortParm = sortOrder == "tablename" ? "tablename_desc" : "tablename";
        //    ViewBag.TimeSortParm = sortOrder == "time" ? "time_desc" : "time";
        //    ViewBag.RecordIdSortParm = sortOrder == "recordid" ? "recordid_desc" : "recordid";
        //    ViewBag.ActionSortParm = sortOrder == "action" ? "action_desc" : "action";

        //    ViewBag.actionOrderIcon = "sort";
        //    ViewBag.recordidOrderIcon = "sort";
        //    ViewBag.tablenameOrderIcon = "sort";
        //    ViewBag.timeOrderIcon = "sort";
        //    ViewBag.idOrderIcon = "sort";

        //    Func<IQueryable<AuditLog>, IOrderedQueryable<AuditLog>> orderBy = o =>
        //    {
        //        switch (sortOrder)
        //        {
        //            case "id_asc":
        //                ViewBag.idOrderIcon = "sorting_asc";
        //                return o.OrderBy(c => c.AuditLogId);

        //            case "action":
        //                ViewBag.actionOrderIcon = "sorting_asc";
        //                return o.OrderBy(c => c.EventType).ThenByDescending(c => c.AuditLogId);
        //            case "action_desc":
        //                ViewBag.actionOrderIcon = "sorting_desc";
        //                return o.OrderByDescending(c => c.EventType).ThenByDescending(c => c.AuditLogId);

        //            case "time":
        //                ViewBag.timeOrderIcon = "sorting_asc";
        //                return o.OrderBy(c => c.EventDateUTC).ThenByDescending(c => c.AuditLogId);
        //            case "time_desc":
        //                ViewBag.timeOrderIcon = "sorting_desc";
        //                return o.OrderByDescending(c => c.EventDateUTC).ThenByDescending(c => c.AuditLogId);

        //            case "recordid":
        //                ViewBag.recordidOrderIcon = "sorting_asc";
        //                return o.OrderBy(c => c.RecordId).ThenByDescending(c => c.AuditLogId);
        //            case "recordid_desc":
        //                ViewBag.recordidOrderIcon = "sorting_desc";
        //                return o.OrderByDescending(c => c.RecordId).ThenByDescending(c => c.AuditLogId);

        //            case "tablename":
        //                ViewBag.tablenameOrderIcon = "sorting_asc";
        //                return o.OrderBy(c => c.TableName).ThenByDescending(c => c.AuditLogId);
        //            case "tablename_desc":
        //                ViewBag.tablenameOrderIcon = "sorting_desc";
        //                return o.OrderByDescending(c => c.TableName).ThenByDescending(c => c.AuditLogId);

        //            default:
        //                ViewBag.idOrderIcon = "sorting_desc";
        //                return o.OrderByDescending(c => c.AuditLogId);
        //        }
        //    };


        //    ViewBag.CurrentFilter = searchString;

        //    Expression<Func<AuditLog, bool>> filter = f => true;

        //    if (!String.IsNullOrEmpty(searchString))
        //    {
        //        filter = f => f.TableName.Contains(searchString)
        //            //|| f.LastName.Contains(searchString)
        //            //|| f.Address1.Contains(searchString)
        //            //|| f.Address2.Contains(searchString)
        //            //|| f.Country.Contains(searchString)
        //            //|| f.IPAddress.Contains(searchString)
        //            //|| f.Email.Contains(searchString)
        //            //|| f.Phone.Contains(searchString)
        //            ;
        //    }
        //    if (wsw.CurrentUserId != id)
        //        user = uow.UserProfileRepository.Find(id);

        //    Expression<Func<AuditLog, bool>> filter2 = f => f.LogDetails.Any() && f.UserName == user.UserName;

        //    var lambda = filter2.AndAlso(filter);

        //    ViewBag.Display = pageSize;

        //    int pageNumber = (page ?? 1);

        //    var model = uow.AuditLogRepository
        //        .GetPage(pageSize, pageNumber,
        //        //out count,
        //        filter: lambda,
        //        orderBy: orderBy
        //        );

        //    return View(model);
        //}

        public ActionResult Views(int id, string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.KLogDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.KLogDisplay = pageSize;

                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.TimeSortParm = string.IsNullOrEmpty(sortOrder) ? "time_asc" : "";
            ViewBag.UrlSortParm = sortOrder == "url" ? "url_desc" : "url";
            ViewBag.IpSortParm = sortOrder == "ip" ? "ip_desc" : "ip";

            ViewBag.ipOrderIcon = "sort";
            ViewBag.urlOrderIcon = "sort";
            ViewBag.timeOrderIcon = "sort";

            Func<IQueryable<KLog>, IOrderedQueryable<KLog>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "time_asc":
                        ViewBag.timeOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Timestamp).ThenBy(c => c.Id);

                    case "ip":
                        ViewBag.ipOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.IPAddress).ThenByDescending(c => c.Id);
                    case "ip_desc":
                        ViewBag.ipOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.IPAddress).ThenByDescending(c => c.Id);

                    case "url":
                        ViewBag.urlOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Url).ThenByDescending(c => c.Id);
                    case "url_desc":
                        ViewBag.urlOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Url).ThenByDescending(c => c.Id);

                    default:
                        ViewBag.timeOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Timestamp).ThenBy(c => c.Id);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<KLog, bool>> filter = f => true;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Url.Contains(searchString)
                    //|| f.LastName.Contains(searchString)
                    //|| f.Address1.Contains(searchString)
                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    //|| f.Email.Contains(searchString)
                    //|| f.Phone.Contains(searchString)
                    ;
            }

            if (wsw.CurrentUserId != id)
                user = uow.UserProfileRepository.Find(id);

            Expression<Func<KLog, bool>> filter2 = f => f.UserName == user.UserName;

            var lambda = filter2.AndAlso(filter);

            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.KLogRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: lambda,
                orderBy: orderBy
                );

            return View(model);



            //return View(uow.KLogRepository.Get().OrderByDescending(k => k.Timestamp));
        }
        /// <summary>
        /// Redirects to Password View
        /// </summary>
        /// <returns></returns>
        public ActionResult Password()
        {
            return View();
        }
        /// <summary>
        /// Change Passowrd Action for UserProfile
        /// </summary>
        /// <param name="model">Change Password Model</param>
        /// <param name="id">UserProfile Id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Password(ChangePasswordModel model, int id)
        {
            if (ModelState.IsValid)
            {
                var user = uow.UserProfileRepository.Find(id); //repo.FindUserProfile(id);
                string token = WebSecurity.GeneratePasswordResetToken(user.UserName);
                WebSecurity.ResetPassword(token, model.Password);

                return RedirectToAction("Index");
            }

            return View(model);
        }
        /// <summary>
        /// Delete UserProfile by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var user = uow.UserProfileRepository.Find(id); //repo.FindUserProfile(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            if (id != WebSecurity.CurrentUserId) // if we are not removing the currentuser
            {
                user.Status = false;
                uow.UserProfileRepository.Update(user);
                //repo.RemoveUserProfile(user);
                //uow.UserProfileRepository.Delete(user);
                //uow.Save(wsw.CurrentUserName);
            }
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("index");

        }
           
    }
}