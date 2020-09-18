using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ViiaNordic.Handler;
using ViiaNordic.Models.Cache;
using ViiaNordic.Models.View;
using ViiaNordic.Services;
using ViiaNordic.Services.Interfaces;

namespace ViiaNordic.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PaymentController : ControllerBase
    {
        private readonly IHttpClientHandler _httpHandler;
        private readonly IViiaService _viiaService;
        private readonly IMemoryCache _memoryCache;
        private readonly IOptionsMonitor<SiteOptions> _options;
        private readonly string _clientId = "fftest-3e2d3cce-a215-4c00-96b5-c3f3611d5eb3";
        private readonly string _clientSecret = "cdc480746835d74c331d8d13630d98ff384ccfe0fe61274d5525fed524b3bcf4";

        private readonly string _baseUri = "https://api-sandbox.getviia.com/v1";


        public PaymentController(IHttpClientHandler httpHandler, IViiaService viiaService, IMemoryCache memoryCache, IOptionsMonitor<SiteOptions> options)
        {
            _httpHandler = httpHandler;
            _viiaService = viiaService;
            _memoryCache = memoryCache;
            _options = options;
        }

        [HttpGet]
        public JsonResult Connect()
        {
            var uri = _viiaService.GetAuthUri();
            
            return new JsonResult(uri); // can be redirected from here
           
        }

        [HttpGet]
        public async Task<JsonResult> ConnectionSuccess(string code, string consentId)
        {
            var tokenResponse = await _viiaService.ExchangeCodeForAccessToken(code);
            
            _memoryCache.Set(_options.CurrentValue.FakeUserEmail,
                new ChachedViiaInfo
                {
                    ViiaAccessToken = tokenResponse.AccessToken,
                    ViiaAccessTokenExpires = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                    ViiaConsentId = consentId,
                    ViiaRefreshToken = tokenResponse.RefreshToken,
                    ViiaTokenType = tokenResponse.TokenType
                },
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(6),
                    Priority = CacheItemPriority.Normal
                });

            var accounts = await _viiaService.GetUserAccounts(tokenResponse);
            return new JsonResult(accounts);
        }



        //[HttpPost("payments/create/outbound")]
        //public async Task<ActionResult<CreatePaymentResultViewModel>> CreateOutboundPayment(
        //    [FromBody] CreatePaymentRequestViewModel body)
        //{
        //    var result = new CreatePaymentResultViewModel();
        //    try
        //    {
        //        var createPaymentResult = await _viiaService.CreateOutboundPayment(User, body);
        //        result.PaymentId = createPaymentResult.PaymentId;
        //        result.AuthorizationUrl = createPaymentResult.AuthorizationUrl;
        //    }
        //    catch (ViiaClientException e)
        //    {
        //        result.ErrorDescription = e.Message;
        //    }

        //    return Ok(result);
        //}
    }
}
