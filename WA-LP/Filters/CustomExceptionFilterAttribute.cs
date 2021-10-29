using SharedBusiness.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using WA_LP.Serializer.Extensions;
namespace WA_LP.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        //Resolve logservice y logear
        public override Task OnExceptionAsync(HttpActionExecutedContext actionExecutedContext, System.Threading.CancellationToken cancellationToken)
        {
            var customerId = HttpContext.Current.User.Identity.Name;
            return LogService.LogErrorAsync(actionExecutedContext.Exception, customerId, actionExecutedContext.Request.SerializeRequest());
            /*
           return await new Task(()=> {
               logService.LogError(actionExecutedContext.Exception);
           });*/
        }
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var customerId = HttpContext.Current.User.Identity.Name;
            LogService.LogError(actionExecutedContext.Exception, customerId, actionExecutedContext.Request.SerializeRequest());
        }
    }
}