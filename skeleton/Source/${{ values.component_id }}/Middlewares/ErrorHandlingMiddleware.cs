using Elements.Logging.Tracing;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.ServiceModel;
using System.Text.Json;
using System.Threading.Tasks;

namespace ${{ values.component_id }}.Middlewares
{
	public class ErrorHandlingMiddleware
	{
		private readonly RequestDelegate _next;

		public ErrorHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var code = GetStatusCode(exception);

			Tracer.TraceErrorMessage(exception, $"Request failed with exception. StatusCode: {code}");

			var jsonOutput = JsonSerializer.Serialize(
				new
				{
					exception.Message,
					ExceptionType = exception.GetType().ToString(),
					Tracer.CorrelationId
				});

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)code;
			return context.Response.WriteAsync(jsonOutput);
		}

		private static HttpStatusCode GetStatusCode(Exception exception)
		{
			return exception switch
			{
				// TODO: Update implementation with new exceptions
				ArgumentException _ => HttpStatusCode.BadRequest,
				InvalidCredentialException _ => HttpStatusCode.Unauthorized,
				NotSupportedException _ => HttpStatusCode.BadRequest,
				TimeoutException _ => HttpStatusCode.GatewayTimeout,
				CommunicationException _ => HttpStatusCode.BadGateway,
				UnauthorizedAccessException => HttpStatusCode.Unauthorized,
				HttpRequestException _ => HttpStatusCode.BadGateway,
				_ => HttpStatusCode.InternalServerError,
			};
		}
	}
}
