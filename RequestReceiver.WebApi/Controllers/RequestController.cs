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

                var result = _requestSender.CallAsync(request).Result;
            
                _requestSender.Close();
                return Ok(result);
            }

            return BadRequest();
        }
        
        [HttpGet("{id}")]
        public ActionResult<RequestDto> GetRequestById(int id)
        {
            var result = _requestSender.CallAsync(id).Result;
            
            _requestSender.Close();
            return Ok(result);
        }
        
        [HttpGet("{clientId}/{departmentAddress}")]
        public ActionResult<RequestDto> GetRequestByClient(int clientId, string departmentAddress)
        {
            RequestByClient requestByClient = new RequestByClient()
            {
                Id = clientId,
                DepartmentAddress = departmentAddress
            };

            var result = _requestSender.CallAsync(requestByClient).Result;
            
            _requestSender.Close();
            return Ok(result);
        }
        
    }
}