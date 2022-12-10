﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Shared.Enums
{
    public enum FailureHttpStatusCode
    {
        /// <summary>Http status code used when bad input data is received.</summary>
        BadRequest = 400,
        /// <summary>Http status code used when attempting to access information without being logged in.</summary>
        Unauthorized = 401,
        /// <summary>Http status code used when the user tries accessing forbidden resources.</summary>
        Forbidden = 403,
        /// <summary>Http status code used when the server has an internal error.</summary>
        InternalServerError = 500,
    }

    public enum SuccessHttpStatusCode
    {
        /// <summary>Http status code used when a reqest was successful.</summary>
        OK = 200,
        /// <summary>Http status code used when a new resource was created successfully</summary>
        Created = 201,
        /// <summary>Http status code used when a process was executed correctly, but contained no content.</summary>
        NoContent = 204,
    }
}
