﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Mvc.Api.Analyzers._OUTPUT_
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CodeFixAddsStatusCodesFromObjectInitializerController : ControllerBase
    {
        private const int FieldStatusCode = 201;

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesDefaultResponseType]
        public IActionResult GetItem(int id)
        {
            if (id == 0)
            {
                return new ObjectResult(new object()) { StatusCode = 422 };
            }

            if (id == 1)
            {
                return new ObjectResult(new object())
                {
                    StatusCode = StatusCodes.Status202Accepted,
                };
            }

            if (id == 2)
            {
                const int localStatusCode = 204;

                return new ObjectResult(new object()) { StatusCode = localStatusCode };
            }

            if (id == 3)
            {
                return new ObjectResult(new object())
                {
                    ContentTypes = { "application/json" },
                    StatusCode = FieldStatusCode,
                };
            }

            return Ok(new object());
        }
    }
}
