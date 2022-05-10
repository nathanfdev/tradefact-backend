using System;
using Microsoft.AspNetCore.Http;

namespace Tradefact.Api.Common.Middleware.Correlation
{
    public interface ICorrelationIdResponseWriter
    {
        void Write(HttpContext context, Guid correlationId);
    }
}
