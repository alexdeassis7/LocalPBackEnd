using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;

using System.Web.Http.Cors;
using SharedBusiness.Log;
using System.Web;
using System.Web.Http.Description;

namespace WA_LP.Controllers.WA.Services
{
    [Authorize]
    [RoutePrefix("v2/banking_solutions/autm_deb")]
    [ApiExplorerSettings(IgnoreApi = true)]


    public class DebAutController : BaseApiController
    {
        [HttpPost]
        [Route("create")]
        public HttpResponseMessage AutmDeb()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                if (data != null && data.Length > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "OK");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "BAD");
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "ERROR");
            }            
        }

        [HttpPost]
        [Route("download")]
        public HttpResponseMessage Download()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                if (data != null && data.Length > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "OK");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "BAD");
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "ERROR");
            }
        }

        [HttpPost]
        [Route("upload")]
        public HttpResponseMessage Upload()
        {
            try
            {
                HttpContent requestContent = Request.Content;
                string data = requestContent.ReadAsStringAsync().Result;

                if (data != null && data.Length > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "OK");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "BAD");
                }
            }
            catch (Exception ex)
            {
                LogService.LogError(ex, HttpContext.Current.User.Identity.Name,SerializeRequest());
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "ERROR");
            }
        }
    }
}
