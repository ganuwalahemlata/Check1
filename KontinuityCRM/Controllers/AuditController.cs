using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KontinuityCRM.Helpers;
using System.Linq.Expressions;
using TrackerEnabledDbContext.Common.Models;
using KontinuityCRM.Filters;

namespace KontinuityCRM.Controllers
{
    [CustomAuthorizeAttribute]
    [System.ComponentModel.DisplayName("Audit Logs")]
    public class AuditController : BaseController
    {

        public AuditController (IUnitOfWork uow, IWebSecurityWrapper wsw) : base(uow, wsw)
	    {
	    }

        public ActionResult Index(int? id, int? oid, string sortOrder, string currentFilter, string searchString, int? page, int? display)
        {
            var user = uow.UserProfileRepository.Find(wsw.CurrentUserId);
            int pageSize = user.AuditLogDisplay;

            if (display.HasValue && display != pageSize)
            {
                page = 1;
                pageSize = display.Value;

                user.AuditLogDisplay = pageSize;

                uow.UserProfileRepository.Update(user);
                uow.Save(wsw.CurrentUserName);
            }
            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder) ? "id_asc" : "";
            ViewBag.TableNameSortParm = sortOrder == "tablename" ? "tablename_desc" : "tablename";
            ViewBag.TimeSortParm = sortOrder == "time" ? "time_desc" : "time";
            ViewBag.RecordIdSortParm = sortOrder == "recordid" ? "recordid_desc" : "recordid";
            ViewBag.ActionSortParm = sortOrder == "action" ? "action_desc" : "action";
            ViewBag.UserNameSortParm = sortOrder == "username" ? "username_desc" : "username";

            ViewBag.actionOrderIcon = "sort";
            ViewBag.recordidOrderIcon = "sort";
            ViewBag.tablenameOrderIcon = "sort";
            ViewBag.timeOrderIcon = "sort";
            ViewBag.idOrderIcon = "sort";
            ViewBag.usernameOrderIcon = "sort";

            Func<IQueryable<AuditLog>, IOrderedQueryable<AuditLog>> orderBy = o =>
            {
                switch (sortOrder)
                {
                    case "id_asc":
                        ViewBag.idOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.AuditLogId);

                    case "action":
                        ViewBag.actionOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.EventType).ThenByDescending(c => c.AuditLogId);
                    case "action_desc":
                        ViewBag.actionOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.EventType).ThenByDescending(c => c.AuditLogId);

                    case "time":
                        ViewBag.timeOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.EventDateUTC).ThenByDescending(c => c.AuditLogId);
                    case "time_desc":
                        ViewBag.timeOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.EventDateUTC).ThenByDescending(c => c.AuditLogId);

                    case "recordid":
                        ViewBag.recordidOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.RecordId).ThenByDescending(c => c.AuditLogId);
                    case "recordid_desc":
                        ViewBag.recordidOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.RecordId).ThenByDescending(c => c.AuditLogId);

                    case "tablename":
                        ViewBag.tablenameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.TableName).ThenByDescending(c => c.AuditLogId);
                    case "tablename_desc":
                        ViewBag.tablenameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.TableName).ThenByDescending(c => c.AuditLogId);

                    case "username":
                        ViewBag.usernameOrderIcon = "sorting_asc";
                        return o.OrderBy(c => c.UserName).ThenByDescending(c => c.AuditLogId);
                    case "username_desc":
                        ViewBag.usernameOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.UserName).ThenByDescending(c => c.AuditLogId);

                    default:
                        ViewBag.idOrderIcon = "sorting_desc";
                        return o.OrderByDescending(c => c.AuditLogId);
                }
            };


            ViewBag.CurrentFilter = searchString;

            Expression<Func<AuditLog, bool>> filter = f => true;

            if (!String.IsNullOrEmpty(searchString))
            {
                filter = f => f.TableName.Contains(searchString)
                    //|| f.LastName.Contains(searchString)
                    //|| f.Address1.Contains(searchString)
                    //|| f.Address2.Contains(searchString)
                    //|| f.Country.Contains(searchString)
                    //|| f.IPAddress.Contains(searchString)
                    //|| f.Email.Contains(searchString)
                    //|| f.Phone.Contains(searchString)
                    ;
            }

            if (id.HasValue && id != wsw.CurrentUserId)
		        user = uow.UserProfileRepository.Find(id);

            Expression<Func<AuditLog, bool>> filter2 = f => f.LogDetails.Any() 
                && (!id.HasValue || f.UserName == user.UserName)
                && (!oid.HasValue || 
                ((f.TableName == "Orders" && f.RecordId == oid.ToString()) || (f.TableName == "OrderProducts" && f.RecordId.StartsWith("[" + oid.ToString() + ","))));
            

            var lambda = filter2.AndAlso(filter);

            ViewBag.Display = pageSize;

            int pageNumber = (page ?? 1);

            var model = uow.AuditLogRepository
                .GetPage(pageSize, pageNumber,
                //out count,
                filter: lambda,
                orderBy: orderBy
                );

            var pageList = new PagedListMapper<AuditLog>(model, model.CloneProps().ConvertUtcTime(TimeZoneUtility.GetTimeZone(uow.TimeZoneRepository.Find(user.TimeZoneId).Name)));
            return View(pageList);


            //return View(uow.AuditLogRepository
            //    .Get(g => g.LogDetails.Any()
            //        , orderBy: l => l.OrderByDescending(g => g.AuditLogId))
            //    );
        }

        public ActionResult Details(int id)
        {
            var audit = uow.AuditLogRepository.Find(id);
            if (audit == null)
            {
                return HttpNotFound();
            }
            return View(audit.LogDetails);
        }

        public ActionResult Delete(int id)
        {
            var audit = uow.AuditLogRepository.Find(id);
            if (audit == null)
            {
                return HttpNotFound();
            }
            uow.AuditLogRepository.Delete(audit);
            uow.Save();
            return RedirectToAction("index");
        }

    }
}
