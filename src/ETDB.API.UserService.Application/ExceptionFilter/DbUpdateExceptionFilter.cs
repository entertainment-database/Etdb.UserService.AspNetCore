﻿using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETDB.API.UserService.Application.ExceptionFilter
{
    public class DbUpdateExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            //if (!(context.Exception is DbUpdateException))
            //{
            //    return;
            //}

            //context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            //context.Result = new ObjectResult(context.Exception);
            //context.ExceptionHandled = true;
        }
    }
}
