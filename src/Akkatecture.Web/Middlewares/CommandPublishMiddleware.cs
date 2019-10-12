// Decompiled with JetBrains decompiler
// Type: Akkatecture.Web.Middlewares.CommandPublishMiddleware
// Assembly: Akkatecture.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 043A546D-C869-4CCE-8BB0-8D9655232ADF
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.Web.dll

using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Akkatecture.Commands;
using Akkatecture.Commands.ExecutionResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Akkatecture.Web.Middlewares
{
  public class CommandPublishMiddleware
  {
    private static readonly Regex CommandPath = new Regex("/*(?<name>[a-z]+)/(?<version>\\d+)/{0,1}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private readonly RequestDelegate _next;
    private readonly ILogger _log;
    private readonly ISerializedCommandPublisher _serializedCommandPublisher;

    public CommandPublishMiddleware(
      RequestDelegate next,
      ILogger<CommandPublishMiddleware> log,
      ISerializedCommandPublisher serializedCommandPublisher)
    {
      _next = next;
      _log = log;
      _serializedCommandPublisher = serializedCommandPublisher;
    }

    public async Task Invoke(HttpContext context)
    {
      PathString path = context.Request.Path;
      if (context.Request.Method == "PUT" && path.HasValue)
      {
        Match match = CommandPath.Match(path.Value);
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
      using (StreamReader streamReader = new StreamReader(context.Request.Body))
        requestJson = await streamReader.ReadToEndAsync().ConfigureAwait(false);
      try
      {
        IExecutionResult result = await _serializedCommandPublisher.PublishSerilizedCommandAsync(name, version, requestJson, CancellationToken.None).ConfigureAwait(false);
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
      string json = JsonConvert.SerializeObject(obj);
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
