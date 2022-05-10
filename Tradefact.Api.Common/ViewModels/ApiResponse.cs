using System.Collections.Generic;

namespace Tradefact.Api.Common.ViewModels
{
    public class ApiResponse
    {
        public object Data { get; set; }

        public IEnumerable<ApiError> Errors { get; set; }
    }
}
