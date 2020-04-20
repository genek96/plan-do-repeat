using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PlanDoRepeatWeb.Implementations.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next.Invoke(context).ConfigureAwait(false);
            }
            catch (UnauthorizedAccessException e)
            {
                context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync(e.Message).ConfigureAwait(false);
            }
            catch (ArgumentException e)
            {
                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(e.Message).ConfigureAwait(false);
            }
        }
    }
}