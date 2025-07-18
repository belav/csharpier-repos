﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.AspNetCore.Mvc.Api.Analyzers.TestFiles.ApiActionsDoNotRequireExplicitModelValidationCheckAnalyzerIntegrationTest
{
    [ApiController]
    [Route("/api/[controller]")]
    public class DiagnosticsAreReturned_ForApiActionsWithModelStateChecksWithoutBracing
        : ControllerBase
    {
        public IActionResult Method(int id)
        {
            if (id == 0)
                return BadRequest();

            /*MM*/if (!ModelState.IsValid)
                return BadRequest();

            return Ok();
        }
    }
}
