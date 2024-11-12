﻿using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using SimpleResults;

namespace TransactionProcessorACL.Controllers;

public static class ResultExtensions
{
    public static IActionResult ToActionResultX(this Result result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result);

        return result.Status switch
        {
            ResultStatus.Invalid => new BadRequestObjectResult(result),
            ResultStatus.NotFound => new NotFoundObjectResult(result),
            ResultStatus.Unauthorized => new UnauthorizedObjectResult(result),
            ResultStatus.Conflict => new ConflictObjectResult(result),
            ResultStatus.Failure => CreateObjectResult(result, HttpStatusCode.InternalServerError),
            ResultStatus.CriticalError => CreateObjectResult(result, HttpStatusCode.InternalServerError),
            ResultStatus.Forbidden => new ForbidResult(),
            _ => CreateObjectResult(result, HttpStatusCode.NotImplemented)

        };
    }

    internal static IActionResult ToActionResultX<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result);

        return result.Status switch
        {
            ResultStatus.Invalid => new BadRequestObjectResult(result),
            ResultStatus.NotFound => new NotFoundObjectResult(result),
            ResultStatus.Unauthorized => new UnauthorizedObjectResult(result),
            ResultStatus.Conflict => new ConflictObjectResult(result),
            ResultStatus.Failure => CreateObjectResult(result, HttpStatusCode.InternalServerError),
            ResultStatus.CriticalError => CreateObjectResult(result, HttpStatusCode.InternalServerError),
            ResultStatus.Forbidden => new ForbidResult(),
            _ => CreateObjectResult(result, HttpStatusCode.NotImplemented)

        };
    }

    internal static ObjectResult CreateObjectResult(Result result,
                                                    HttpStatusCode statusCode)
    {
        ObjectResult or = new ObjectResult(result);
        or.StatusCode = (Int32)statusCode;
        return or;
    }

    internal static ObjectResult CreateObjectResult<T>(Result<T> result,
                                                       HttpStatusCode statusCode)
    {
        ObjectResult or = new ObjectResult(result);
        or.StatusCode = (Int32)statusCode;
        return or;
    }
}