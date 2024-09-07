using Microsoft.AspNetCore.Mvc;

namespace AEBackendProject.Common
{
    public interface IResponseHelper
    {
        IActionResult CreateOkResponse<T>(T data);
        IActionResult CreateNotFoundResponse(string message);
        IActionResult CreateBadRequestResponse(IEnumerable<string> errors);
        IActionResult CreateCreatedResponse<T>(T data, Guid id, string controllerName, string actionName);
        IActionResult CreateNoContentResponse();
        IActionResult CreateCustomResponse(int statusCode, string message);
    }
}
