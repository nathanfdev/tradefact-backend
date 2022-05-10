using System.Linq;
using Core.Models;
using Infrastructure.Functions.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Functions.Extensions
{
    public static class ValidationExtensions
    {
        public static BadRequestObjectResult ToBadRequest<T>(this ValidatableRequest<T> request) => new BadRequestObjectResult(request.Errors
            .Select(e => new Error { Title = e.PropertyName, Description = e.ErrorMessage }));
    }
}