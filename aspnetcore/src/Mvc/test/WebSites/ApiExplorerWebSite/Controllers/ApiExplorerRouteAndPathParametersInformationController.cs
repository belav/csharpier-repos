// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Mvc;

namespace ApiExplorerWebSite;

[Route("ApiExplorerRouteAndPathParametersInformation")]
public class ApiExplorerRouteAndPathParametersInformationController
{
    [HttpGet]
    public void Get() { }

    [HttpGet("{id}")]
    public void Get(int id) { }

    [HttpGet("Optional/{id?}")]
    public void GetOptional(int id = 0) { }

    [HttpGet("Constraint/{integer:int}")]
    public void GetInteger(int integer) { }

    [HttpGet("CatchAll/{*parameter}")]
    public void GetCatchAll(string parameter) { }

    [HttpGet("MultipleParametersInSegment/{month:range(1,12)}-{day:int}-{year:int}")]
    public void GetMultipleParametersInSegment(string month, string day, string year) { }

    [HttpGet("MultipleParametersInMultipleSegments/{month:range(1,12)}/{day:int?}/{year:int?}")]
    public void GetMultipleParametersInMultipleSegments(
        string month,
        string day,
        string year = ""
    ) { }

    [HttpGet("MultipleTypesOfParameters/{path}/{pathAndQuery}/{pathAndFromBody}")]
    public void MultipleTypesOfParameters(
        string query,
        string pathAndQuery,
        [FromBody] string pathAndFromBody
    ) { }

    [HttpGet("CatchAllAndConstraint/{*integer:int}")]
    public void GetIntegers(string integer) { }
}
