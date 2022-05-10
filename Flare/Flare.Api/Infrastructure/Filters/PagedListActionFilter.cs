using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Flare.Api.Infrastructure
{
    public class AddPaginationHeader : ResultFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result as ObjectResult;

            base.OnResultExecuting(context);
        }
    }
}
