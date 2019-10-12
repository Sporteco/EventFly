﻿// Decompiled with JetBrains decompiler
// Type: Akkatecture.Web.Swagger.CommandsApiDescriptionGroupCollectionProvider
// Assembly: Akkatecture.Web, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 043A546D-C869-4CCE-8BB0-8D9655232ADF
// Assembly location: C:\Users\naych\source\repos\!!!!!\netcoreapp2.2\Akkatecture.Web.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using Akkatecture.Commands;
using Akkatecture.Definitions;
using Akkatecture.Queries;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Akkatecture.Web.Swagger
{
  public class CommandsApiDescriptionGroupCollectionProvider : IApiDescriptionGroupCollectionProvider
  {
    private readonly IApplicationDefinition _metadata;
    private readonly ApiDescriptionGroupCollectionProvider _internal;

    public CommandsApiDescriptionGroupCollectionProvider(
      IApplicationDefinition metadata,
      IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
      IEnumerable<IApiDescriptionProvider> apiDescriptionProviders)
    {
      _metadata = metadata;
      _internal = new ApiDescriptionGroupCollectionProvider(actionDescriptorCollectionProvider, apiDescriptionProviders);
    }

    public ApiDescriptionGroupCollection ApiDescriptionGroups
    {
      get
      {
        ApiDescriptionGroupCollection descriptionGroups = _internal.ApiDescriptionGroups;
        List<ApiDescription> apis = new List<ApiDescription>();
        PrepareCommands(apis);
        return new ApiDescriptionGroupCollection(PrepareQueries(apis, descriptionGroups), 1);
      }
    }

    private List<ApiDescriptionGroup> PrepareQueries(
      List<ApiDescription> apis,
      ApiDescriptionGroupCollection data)
    {
      foreach (IDomainDefinition domain in _metadata.Domains)
      {
        foreach (IQueryDefinition query in domain.Queries)
        {
          Type type = query.Type;
          string name = type.Name.ToLowerInvariant().Replace("query", "");
          string str = "api/" + query.Name;
          Type genericInterface = ReflectionExtensions.GetSubclassOfRawGenericInterface(typeof (IQuery<>), type);
          if (!(genericInterface == null))
          {
            Type genericArgument = genericInterface.GetGenericArguments()[0];
            ApiDescription apiDescription1 = new ApiDescription();
            ApiDescription apiDescription2 = apiDescription1;
            ControllerActionDescriptor actionDescriptor1 = new ControllerActionDescriptor();
            actionDescriptor1.ActionConstraints = new List<IActionConstraintMetadata>
            {
                new HttpMethodActionConstraint(new[]
                {
                    "POST"
                })
            };
            actionDescriptor1.ActionName = name;
            actionDescriptor1.ControllerName = "context";
            actionDescriptor1.DisplayName = name;
            actionDescriptor1.Parameters = new List<ParameterDescriptor>
            {
                new ParameterDescriptor
                {
                    Name = "query",
                    ParameterType = type
                }
            };
            actionDescriptor1.MethodInfo = new CustomMethodInfo(name, type);
            actionDescriptor1.ControllerTypeInfo = type.GetTypeInfo();
            actionDescriptor1.RouteValues = new Dictionary<string, string>
            {
                {
                    "controller",
                    domain.Name
                }
            };
            ControllerActionDescriptor actionDescriptor2 = actionDescriptor1;
            apiDescription2.ActionDescriptor = actionDescriptor2;
            apiDescription1.HttpMethod = "POST";
            apiDescription1.RelativePath = str;
            apiDescription1.SupportedResponseTypes.Add(new ApiResponseType
            {
              StatusCode = 200,
              Type = genericArgument
            });
            ApiDescription apiDescription3 = apiDescription1;
            ((List<ApiParameterDescription>) apiDescription3.ParameterDescriptions).Add(new ApiParameterDescription
            {
              Name = "query",
              Type = type,
              Source = BindingSource.Body
            });
            apis.Add(apiDescription3);
          }
        }
      }
      List<ApiDescriptionGroup> descriptionGroupList = new List<ApiDescriptionGroup> { new ApiDescriptionGroup("R3", apis) };
      descriptionGroupList.AddRange(data.Items);
      return descriptionGroupList;
    }

    private void PrepareCommands(List<ApiDescription> apis)
    {
      foreach (IDomainDefinition domain in _metadata.Domains)
      {
        foreach (CommandDefinition allDefinition in domain.Commands.GetAllDefinitions())
        {
          Type type = allDefinition.Type;
          string name = type.Name.ToLowerInvariant().Replace("command", "");
          string str = "api/" + allDefinition.Name + "/" + allDefinition.Version;
          Type subclassOfRawGeneric = ReflectionExtensions.GetSubclassOfRawGeneric(typeof (Command<,>), type);
          if (!(subclassOfRawGeneric == null))
          {
            Type genericArgument = subclassOfRawGeneric.GetGenericArguments()[1];
            ApiDescription apiDescription1 = new ApiDescription();
            ApiDescription apiDescription2 = apiDescription1;
            ControllerActionDescriptor actionDescriptor1 = new ControllerActionDescriptor();
            actionDescriptor1.ActionConstraints = new List<IActionConstraintMetadata>
            {
                new HttpMethodActionConstraint(new[]
                {
                    "POST"
                })
            };
            actionDescriptor1.ActionName = name;
            actionDescriptor1.ControllerName = domain.Name;
            actionDescriptor1.DisplayName = allDefinition.Name;
            actionDescriptor1.Parameters = new List<ParameterDescriptor>
            {
                new ParameterDescriptor
                {
                    Name = "request",
                    ParameterType = type
                }
            };
            actionDescriptor1.MethodInfo = new CustomMethodInfo(name, type);
            actionDescriptor1.ControllerTypeInfo = type.GetTypeInfo();
            actionDescriptor1.RouteValues = new Dictionary<string, string>
            {
                {
                    "controller",
                    domain.Name
                }
            };
            ControllerActionDescriptor actionDescriptor2 = actionDescriptor1;
            apiDescription2.ActionDescriptor = actionDescriptor2;
            apiDescription1.HttpMethod = "PUT";
            apiDescription1.RelativePath = str;
            apiDescription1.SupportedResponseTypes.Add(new ApiResponseType
            {
              StatusCode = 200,
              Type = genericArgument
            });
            ApiDescription apiDescription3 = apiDescription1;
            ((List<ApiParameterDescription>) apiDescription3.ParameterDescriptions).Add(new ApiParameterDescription
            {
              Name = "request",
              Type = type,
              Source = BindingSource.Body
            });
            apis.Add(apiDescription3);
          }
        }
      }
    }
  }
}