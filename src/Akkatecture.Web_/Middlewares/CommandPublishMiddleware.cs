// Decompiled with JetBrains decompiler
// Type: Akkatecture.Web.Middlewares.CommandPublishMiddleware
// Assembly: Akkatecture.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 043A546D-C869-4CCE-8BB0-8D9655232ADF
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.Web.dll

using Akkatecture.Commands;
using Akkatecture.Commands.ExecutionResults;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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
      this._next = next;
      this._log = (ILogger) log;
      this._serializedCommandPublisher = serializedCommandPublisher;
    }

    public async Task Invoke(HttpContext context)
    {
      PathString path = context.Request.Path;
      if (context.Request.Method == "PUT" && path.HasValue)
      {
        Match match = CommandPublishMiddleware.CommandPath.Match(path.Value);
        if (match.Success)
        {
          await this.PublishCommandAsync(match.Groups["name"].Value, int.Parse(match.Groups["version"].Value), context).ConfigureAwait(false);
          return;
        }
        match = (Match) null;
      }
      await this._next(context);
    }

    private async Task PublishCommandAsync(string name, int version, HttpContext context)
    {
      this._log.LogTrace(string.Format("Publishing command '{0}' v{1} from OWIN middleware", (object) name, (object) version));
      string requestJson;
      using (StreamReader streamReader = new StreamReader(context.Request.Body))
        requestJson = await streamReader.ReadToEndAsync().ConfigureAwait(false);
      try
      {
        IExecutionResult result = await this._serializedCommandPublisher.PublishSerilizedCommandAsync(name, version, requestJson, CancellationToken.None).ConfigureAwait(false);
        await this.WriteAsync((object) result, HttpStatusCode.OK, context).ConfigureAwait(false);
        result = (IExecutionResult) null;
      }
      catch (ArgumentException ex)
      {
        this._log.LogDebug((Exception) ex, string.Format("Failed to publish serialized command '{0}' v{1} due to: {2}", (object) name, (object) version, (object) ex.Message));
        await this.WriteErrorAsync(ex.Message, HttpStatusCode.BadRequest, context).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        this._log.LogError(ex, string.Format("Unexpected exception when executing '{0}' v{1}", (object) name, (object) version));
        await this.WriteErrorAsync("Internal server error!", HttpStatusCode.InternalServerError, context).ConfigureAwait(false);
      }
    }

    private async Task WriteAsync(object obj, HttpStatusCode statusCode, HttpContext context)
    {
      string json = JsonConvert.SerializeObject(obj);
      context.Response.StatusCode = (int) statusCode;
      await context.Response.WriteAsync(json, new CancellationToken()).ConfigureAwait(false);
    }

    private Task WriteErrorAsync(
      string errorMessage,
      HttpStatusCode statusCode,
      HttpContext context)
    {
      return this.WriteAsync((object) new{ ErrorMessage = errorMessage }, statusCode, context);
    }
  }
}
