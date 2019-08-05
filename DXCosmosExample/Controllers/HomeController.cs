using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevExpress.XtraReports.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DXCosmosExample.Models;

namespace DXCosmosExample.Controllers {
    public class HomeController : Controller {
        private readonly ReportStorageWebExtension reportStorageExtension;
        public HomeController(ReportStorageWebExtension reportStorageExtension) {
            this.reportStorageExtension = reportStorageExtension;
        }
        public IActionResult Index() {
            var reports = reportStorageExtension.GetUrls().Select(x => x.Key);
            return View(new HomeModel() {
                Reports = reports
            });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Designer(string id) {
            return View(new ReportModel() {
                ReportUrl = id,
                Report = new DevExpress.XtraReports.UI.XtraReport()
            });
        }

        [Authorize]
        public IActionResult Viewer(string id) {
            return View(new ReportModel() {
                ReportUrl = id
            });
        }
    }
}
