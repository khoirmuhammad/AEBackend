using AEBackendProject.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AEBackendProject.Common
{
    public class ResponseHelper : IResponseHelper
    {
        public IActionResult CreateOkResponse<T>(T data)
        {
            return new OkObjectResult(new ApiResponse<T>
            {
                StatusCode = (int)HttpStatusCode.OK,
                Data = data
            });
        }

        public IActionResult CreateNotFoundResponse(string message)
        {
            return new NotFoundObjectResult(new ApiResponse<object>
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = message
            });
        }

        public IActionResult CreateBadRequestResponse(IEnumerable<string> errors)
        {
            return new BadRequestObjectResult(new ApiResponse<object>
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "Validation failed. Please see the details",
                Errors = errors
            });
        }

        public IActionResult CreateCreatedResponse<T>(T data, Guid id, string controllerName, string actionName)
        {
            return new CreatedAtActionResult(actionName, controllerName, new { id }, new ApiResponse<T>
            {
                StatusCode = (int)HttpStatusCode.Created,
                Message = "Item created successfully.",
                Data = data
            });
        }

        public IActionResult CreateNoContentResponse()
        {
            return new NoContentResult();
        }

        public IActionResult CreateCustomResponse(int statusCode, string message)
        {
            return new ObjectResult(new ApiResponse<object>
            {
                StatusCode = statusCode,
                Message = message
            });
        }
    }
}
