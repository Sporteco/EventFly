﻿// The MIT License (MIT)
//
// Copyright (c) 2018 - 2019 Lutando Ngqakaza
// https://github.com/Lutando/EventFly 
// 
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Akka.Actor;
using EventFly.Commands;
using EventFly.DependencyInjection;
using EventFly.Examples.Api.Controllers.Models;
using EventFly.Examples.Api.Domain.Aggregates.Resource;
using EventFly.Examples.Api.Domain.Aggregates.Resource.Commands;
using EventFly.Examples.Api.Domain.Repositories.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace EventFly.Examples.Api.Controllers
{
    public class ResourceController : BaseController
    {


        private readonly IQueryResources _resourceQuery;
        public ResourceController(ActorSystem system, IQueryResources resourceQuery) : base(system)
        {
            _resourceQuery = resourceQuery;
        }

        [HttpPost("resources")]
        public async Task<IActionResult> PostResource()
        {
            var resourceId = ResourceId.New;
            var id = resourceId.GetGuid();
            var command = new CreateResourceCommand(resourceId);

            var result = await System.GetExtension<ServiceProviderHolder>().ServiceProvider.GetRequiredService<ICommandBus>().Publish(command);

            if (result.IsSuccess)
            {
                var location = new Uri($"{GetAbsoluteUri(HttpContext)}/api/operations/{id}");
                var contentLocation = new Uri($"{GetAbsoluteUri(HttpContext)}/api/resources/{id}");
                HttpContext.Response.Headers.Add("Location", location.ToString());
                HttpContext.Response.Headers.Add("Content-Location", contentLocation.ToString());

                return Accepted(new ResourceResponseModel(id, id));
            }

            return BadRequest();
        }

        [HttpGet("resources")]
        public async Task<IActionResult> GetResources()
        {
            var resources = await _resourceQuery.FindAll();

            return Ok(resources);
        }

        [HttpGet("resources/{id:Guid}")]
        public async Task<IActionResult> GetResources([FromRoute]Guid id)
        {
            var resources = await _resourceQuery.Find(id);

            return Ok(resources);
        }


    }
}