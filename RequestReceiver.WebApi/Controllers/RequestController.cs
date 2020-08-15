using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RequestReceiver.Common;
using RequestReceiver.Common.Models;
using RequestReceiver.Messaging.Send;

namespace RequestReceiver.WebApi.Controllers
{
    [Route("api/v1/request")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IRequestSenderRpc _requestSender;
        private readonly IActionContextAccessor _accessor;

        public RequestController(IRequestSenderRpc requestSender, IActionContextAccessor accessor)
        {
            _requestSender = requestSender;
            _accessor = accessor;
        }

        [HttpPost]
        public ActionResult<string> CreateRequest([FromBody] Request request)
        {
            if (ModelState.IsValid)
            {
                request.IpAddress = _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
            
                return Ok(_requestSender.CallAsync(request).Result);
            }

            return BadRequest();
        }
        
        [HttpGet("{id}")]
        public ActionResult<RequestDto> GetRequestById(int id)
        {
            return Ok(_requestSender.CallAsync(id).Result);
        }
        
        [HttpGet("{clientId}/{departmentAddress}")]
        public ActionResult<RequestDto> GetRequestByClient(int clientId, string departmentAddress)
        {
            RequestByClient requestByClient = new RequestByClient()
            {
                Id = clientId,
                DepartmentAddress = departmentAddress
            };
            
            return Ok(_requestSender.CallAsync(requestByClient).Result);
        }
        
    }
}