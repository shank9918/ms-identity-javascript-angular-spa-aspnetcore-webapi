﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using TodoListAPI.Models;

namespace TodoListAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TodoListController : Controller
    {
        static readonly ConcurrentBag<TodoItem> TodoStore = new ConcurrentBag<TodoItem>();

        /// <summary>
        /// The Web API will only accept tokens 1) for users, and 
        /// 2) having the access_as_user scope for this API
        /// </summary>
        static readonly string[] scopeRequiredByApi = new string[] { "access_as_user" };

        // GET: api/values
        [HttpGet]
        public IEnumerable<TodoItem> Get()
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            string owner = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return TodoStore.Where(t => t.Owner == owner).ToList();
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]TodoItem todo)
        {
            HttpContext.VerifyUserHasAnyAcceptedScope(scopeRequiredByApi);
            string owner = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            TodoStore.Add(new TodoItem { Owner = owner, Title = todo.Title });
        }
    }
}