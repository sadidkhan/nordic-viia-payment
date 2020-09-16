using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ViiaNordic.Services;

namespace ViiaNordic.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PaymentController: ControllerBase
    {
        private readonly IHttpHandler _httpHandler;
        private readonly string _clientId = "fftest-3e2d3cce-a215-4c00-96b5-c3f3611d5eb3";
        private readonly string _baseUri = "https://api-sandbox.getviia.com/v1";


        public PaymentController(IHttpHandler httpHandler)
        {
            _httpHandler = httpHandler;
        }

        [HttpGet]
        public JsonResult ConnectionSuccess()
        {
            var a = Request;
            return new JsonResult("ok");
        }

        //[HttpGet]
        //public async Task<JsonResult> Connect()
        //{
        //    var param = new
        //    {
        //        client_id = _clientId,
        //        redirect_uri = "http://localhost:61658/api/payment/GetData",
        //        response_type = "code"
        //    };
        //    var uri = $"{_baseUri}/oauth/connect?client_id={param.client_id}&redirect_uri={param.redirect_uri}&response_type={param.response_type}";
        //    var result = await _httpHandler.GetJsonAsync<object>(uri);
        //    return new JsonResult("ok");
        //}
    }
}
