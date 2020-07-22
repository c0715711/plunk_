using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace TestPunkApi.Models
{
    public class ValidateEmailFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var beerRating = (BeerRating)actionContext.ActionArguments.First().Value;            
            if (string.IsNullOrWhiteSpace(beerRating.UserName))
            {
                var httpResponseMessage = new HttpResponseMessage()
                {
                    Content = new ObjectContent<object>(new { message = "Please enter email address .", error = "1" }, new JsonMediaTypeFormatter()),
                    ReasonPhrase = "Invalid request",
                    StatusCode = HttpStatusCode.BadRequest
                };
                actionContext.Response = httpResponseMessage;
            }
            else
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(beerRating.UserName);
                if (!match.Success)
                {
                    var httpResponseMessage = new HttpResponseMessage()
                    {
                        Content = new ObjectContent<object>(new { message = "Please enter valid email address .", error = "1" }, new JsonMediaTypeFormatter()),
                        ReasonPhrase = "Invalid request",
                        StatusCode = HttpStatusCode.BadRequest
                    };
                    actionContext.Response = httpResponseMessage;
                }
            }
            
        }
    }
    
}