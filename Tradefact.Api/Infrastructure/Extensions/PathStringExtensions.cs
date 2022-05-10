using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Tradefact.Api.Infrastructure.Extensions
{
    public static class PathStringExtensions
    {
        private static Regex _storeLangeExpr = new Regex(@"^/\b\S+\b/[a-zA-Z]{2}-[a-zA-Z]{2}/");
        public static PathString GetStoreAndLangSegment(this PathString path)
        {
            var matches = _storeLangeExpr.Match(path);
            return matches.Success ? matches.Value : "/";
        }

        public static bool IsApi(this PathString path)
        {
            return path.ToString().Contains("/api/");
        }

    }
}
