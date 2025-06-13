using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;


namespace Application.Extensions
{
    public static class GlobalExceptionHandler
    {
        public static void HandleException(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(o => o.Run(async context =>
            {
                var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = errorFeature.Error;

                if (!(exception is FluentValidation.ValidationException validationException)) 
                    throw exception;

                var errors = validationException.Errors.Select(error => new
                {
                    Property = error.PropertyName,
                    Message = error.ErrorMessage,
                });

                var errorCount = JsonSerializer.Serialize(errors);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(errorCount, System.Text.Encoding.UTF8);
            }));
        }
    }
}
