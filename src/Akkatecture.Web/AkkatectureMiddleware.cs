using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Akkatecture.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Akkatecture.Web
{
  public class AkkatectureMiddleware
  {
    private static readonly Regex CommandPath = new Regex("/*(?<name>[a-z]+)/(?<version>\\d+)/{0,1}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private readonly RequestDelegate _next;
    private readonly ILogger _log;
    private readonly ISerializedCommandPublisher _serializedCommandPublisher;

    public AkkatectureMiddleware(
      RequestDelegate next,
      ILogger<AkkatectureMiddleware> log,
      ISerializedCommandPublisher serializedCommandPublisher)
    {
      _next = next;
      _log = log;
      _serializedCommandPublisher = serializedCommandPublisher;
    }

    public async Task Invoke(HttpContext context)
    {
      var path = context.Request.Path;
      if (context.Request.Method == "PUT" && path.HasValue)
      {
        var match = CommandPath.Match(path.Value);
        if (match.Success)
        {
          await PublishCommandAsync(match.Groups["name"].Value, int.Parse(match.Groups["version"].Value), context).ConfigureAwait(false);
          return;
        }
      }
      await _next(context);
    }

    private async Task PublishCommandAsync(string name, int version, HttpContext context)
    {
      _log.LogTrace($"Publishing command '{name}' v{version} from OWIN middleware");
      string requestJson;
      using (var streamReader = new StreamReader(context.Request.Body))
        requestJson = await streamReader.ReadToEndAsync().ConfigureAwait(false);
      try
      {
        var result = await _serializedCommandPublisher.PublishSerilizedCommandAsync(name, version, requestJson, CancellationToken.None).ConfigureAwait(false);
        await WriteAsync(result, HttpStatusCode.OK, context).ConfigureAwait(false);
      }
      catch (ArgumentException ex)
      {
        _log.LogDebug(ex, $"Failed to publish serialized command '{name}' v{version} due to: {ex.Message}");
        await WriteErrorAsync(ex.Message, HttpStatusCode.BadRequest, context).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        _log.LogError(ex, $"Unexpected exception when executing '{name}' v{version}");
        await WriteErrorAsync("Internal server error!", HttpStatusCode.InternalServerError, context).ConfigureAwait(false);
      }
    }

    private async Task WriteAsync(object obj, HttpStatusCode statusCode, HttpContext context)
    {
      var json = JsonConvert.SerializeObject(obj);
      context.Response.StatusCode = (int) statusCode;
      await context.Response.WriteAsync(json).ConfigureAwait(false);
    }

    private Task WriteErrorAsync(
      string errorMessage,
      HttpStatusCode statusCode,
      HttpContext context)
    {
      return WriteAsync(new{ ErrorMessage = errorMessage }, statusCode, context);
    }
  }
}
