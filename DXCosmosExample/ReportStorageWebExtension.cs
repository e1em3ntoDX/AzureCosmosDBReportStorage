using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.UI;
using DXCosmosExample.Models;
using DXCosmosExample.Services;
using Microsoft.AspNetCore.Http;

namespace DXCosmosExample
{
    public class CosmosReportStorageWebExtension : ReportStorageWebExtension
    {
        public const string AdminReportName = "AdminReport";
        public static ReportItem CreateAdminReport()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var report = new XtraReport();
                report.Bands.AddRange(new Band[] {
                    new TopMarginBand() { HeightF = 100f },
                    new DetailBand() { HeightF = 100f },
                    new BottomMarginBand() { HeightF = 100f }
                });
                report.Bands[BandKind.Detail].Controls.Add(new XRLabel()
                {
                    Text = "Hello Admin!",
                    SizeF = new System.Drawing.SizeF(200, 50)
                });
                report.SaveLayoutToXml(ms);
                return new ReportItem()
                {
                    Id = AdminReportName,
                    ReportLayout = ms.ToArray()
                };
            }
        }

        readonly ICosmosApiStorage cosmosApiStorage;
        readonly IHttpContextAccessor httpContextAccessor;
        bool isAdmin()
        {
            return httpContextAccessor.HttpContext.User.IsInRole("Admin"); ;
        }
        public CosmosReportStorageWebExtension(IHttpContextAccessor httpContextAccessor, ICosmosApiStorage cosmosApiStorage)
        {
            this.cosmosApiStorage = cosmosApiStorage;
            this.httpContextAccessor = httpContextAccessor;
        }
        public override bool CanSetData(string url)
        {
            return isAdmin();
        }

        public override byte[] GetData(string url)
        {
            if (url == AdminReportName && !isAdmin())
            {
                throw new UnauthorizedAccessException();
            }
            byte[] reportLayout = new byte[0];
            cosmosApiStorage.GetReportItem(url).ContinueWith((task) => {
                reportLayout = task.Result.ReportLayout;
            }).Wait();
            return reportLayout;
        }

        public override Dictionary<string, string> GetUrls()
        {
            Dictionary<string, string> urls = new Dictionary<string, string>();
            cosmosApiStorage.GetReports().ContinueWith((task) => {
                urls = isAdmin() ?
                task.Result :
                task.Result.Where(x => x.Key != AdminReportName).ToDictionary(x => x.Key, x => x.Value);
            }).Wait();
            return urls;
        }

        public override bool IsValidUrl(string url)
        {
            return true;
        }

        public override void SetData(XtraReport report, string url)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                report.SaveLayoutToXml(ms);
                cosmosApiStorage.UpdateReportItem(new ReportItem()
                {
                    Id = url,
                    ReportLayout = ms.ToArray()
                }).Wait();
            }
        }

        public override string SetNewData(XtraReport report, string defaultUrl)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                report.SaveLayoutToXml(ms);
                cosmosApiStorage.CreateReportItem(new ReportItem()
                {
                    Id = defaultUrl,
                    ReportLayout = ms.ToArray()
                }).Wait();
                return defaultUrl;
            }
        }
    }
}