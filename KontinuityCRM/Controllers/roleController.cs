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
    [System.ComponentModel.DisplayName("Role")]
    public class roleController : BaseController
    {
        public roleController(IUnitOfWork uow, IWebSecurityWrapper wsw)
            : base(uow, wsw)
        {

        }
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = 10;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.TaxProfileDisplay = pageSize;

                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.UserGroupIdSortParm = String.IsNullOrEmpty(sortOrder) ? "UserGroupId_asc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.DescriptionSortParm = sortOrder == "Description" ? "Description_desc" : "Description";

            ViewBag.NameOrderIcon = "sort";
            ViewBag.DescriptionOrderIcon = "sort";
            ViewBag.UserGroupIdOrderIcon = "sort";


            Func<IQueryable<UserGroup>, IOrderedQueryable<UserGroup>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "UserGroupId_asc":
                        ViewBag.UserGroupIdOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.UserGroupId);

                    case "Name":
                        ViewBag.NameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.Name).ThenByDescending(c => c.UserGroupId);
                    case "Name_desc":
                        ViewBag.NameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.Description).ThenByDescending(c => c.UserGroupId);


                    default:
                        ViewBag.UserGroupIdOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.UserGroupId);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<UserGroup, bool>> filter = f => f.Status == null || f.Status == true;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.Name.Contains(searchString)
                    //|| f.Country.Name.Contains(searchString)
                    ;
            }



            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.UserGroupRepository
                .GetPage(pageSize, pageNumber,
                filter: filter,
                orderBy: orderBy
                );

            return View(model);

        }

        private void CreateViewBag()
        {
            var assembly = System.Reflection.Assembly.Load("EnumAssembly");            

            ViewBag.ViewPermissiond = KontinuityCRMHelper.CalculateAllPermissions(assembly, "ViewPermissiond");
            ViewBag.ViewPermission = KontinuityCRMHelper.CalculateAllPermissions(assembly, "ViewPermission");
            ViewBag.ViewPermission2 = KontinuityCRMHelper.CalculateAllPermissions(assembly, "ViewPermission2");
        }     
        /// <summary>
        /// Return View to Create New User Group
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            CreateViewBag();
            return View();
        }
        /// <summary>
        /// Post Action for Create UserGroup
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ViewPermissiond"></param>
        /// <param name="ViewPermission"></param>
        /// <param name="ViewPermission2"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(UserGroup model, long[] ViewPermissiond, long[] ViewPermission, long[] ViewPermission2, string[] role)
        {
            if (ModelState.IsValid)
            {
                long? p = null, p1 = null, p2 = null;

                if (ViewPermissiond != null)
                {
                    p = 0;
                    foreach (var item in ViewPermissiond)
                    {
                        p += item;
                    }
                }

                if (ViewPermission != null)
                {
                    p1 = 0;
                    foreach (var item in ViewPermission)
                    {
                        p1 += item;
                    }
                }

                if (ViewPermission2 != null)
                {
                    p2 = 0;
                    foreach (var item in ViewPermission2)
                    {
                        p2 += item;
                    }
                }

                model.Permissions = p;
                model.Permissions1 = p1;
                model.Permissions2 = p2;
                model.Status = true;
                model.Roles = new List<UserGroupsInRoles>();
                
                if(role != null)
                { 
                    foreach (var r in role)
                    {
                        UserGroupsInRoles ugr = new UserGroupsInRoles();
                        ugr.UserGroup = model;
                        ugr.Role = r;
                        model.Roles.Add(ugr);
                    }
                }

                uow.UserGroupRepository.Add(model);
                uow.Save(wsw.CurrentUserName);

                return RedirectToAction("index");
            }

            ViewBag.ViewPermissiond = ViewPermissiond.Sum();
            ViewBag.ViewPermission = ViewPermission.Sum();
            ViewBag.ViewPermission2 = ViewPermission2.Sum();

            return View(model);
        }
        /// <summary>
        /// Return Edit page for UserGroup having UserGroupId as id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            //WebSecurity.
            var user = uow.UserGroupRepository.Find(id); // repo.FindUserProfile(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.ViewPermissiond = user.Permissions;
            ViewBag.ViewPermission = user.Permissions1;
            ViewBag.ViewPermission2 = user.Permissions2;
            return View(user);
        }
        /// <summary>
        /// Post updated UserGroup Action
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ViewPermissiond"></param>
        /// <param name="ViewPermission"></param>
        /// <param name="ViewPermission2"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(UserGroup user, long[] ViewPermissiond, long[] ViewPermission, long[] ViewPermission2, string[] role)
        {
            if (ModelState.IsValid)
            {
                var up = uow.UserGroupRepository.Find(user.UserGroupId); //repo.FindUserProfile(user.UserId); // need to find to avoid exception about already attached entity  

                long? p = up.Permissions, p1 = up.Permissions1, p2 = up.Permissions2;
                if (ViewPermissiond != null)
                {
                    p = 0;
                    foreach (var item in ViewPermissiond)
                    {
                        p += item;
                    }
                }
                if (ViewPermission != null)
                {
                    p1 = 0;
                    foreach (var item in ViewPermission)
                    {
                        p1 += item;
                    }
                }
                if (ViewPermission2 != null)
                {
                    p2 = 0;
                    foreach (var item in ViewPermission2)
                    {
                        p2 += item;
                    }
                }

                up.Name = user.Name;
                up.Description = user.Description;
                up.Permissions = p;
                up.Permissions1 = p1;
                up.Permissions2 = p2;
               
                up.Roles.Clear();

                if (role != null)
                {
                    foreach (var r in role)
                    {
                        UserGroupsInRoles ugr = new UserGroupsInRoles();
                        ugr.UserGroup = up;
                        ugr.Role = r;
                        up.Roles.Add(ugr);
                    }
                }
                uow.UserGroupRepository.Update(up);

                foreach (var u in uow.UserProfileRepository.Get(u => u.UserGroupId == up.UserGroupId))
                {
                    var roleProvider = (SimpleRoleProvider)System.Web.Security.Roles.Provider;
                    var rolesForUser = roleProvider.GetRolesForUser(u.UserName);
                    roleProvider.RemoveUsersFromRoles(new[] { u.UserName }, rolesForUser);                    
                    if (role != null)
                        roleProvider.AddUsersToRoles(new[] { u.UserName }, role);
                }
                
                uow.Save(wsw.CurrentUserName);


                return RedirectToAction("index");
            }

            return View(user);
        }
        /// <summary>
        /// Remove a UserGroup with UserGroupId as id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
  
        public ActionResult Delete(int id)
        {
            var user = uow.UserGroupRepository.Find(id); //repo.FindUserProfile(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            if (id != WebSecurity.CurrentUserId) // if we are not removing the currentuser
            {
                user.Status = false;
                uow.UserGroupRepository.Update(user);
                //repo.RemoveUserProfile(user);
                //uow.UserProfileRepository.Delete(user);
                //uow.Save(wsw.CurrentUserName);
            }
            uow.Save(wsw.CurrentUserName);
            return RedirectToAction("index");

        }
    }
}
