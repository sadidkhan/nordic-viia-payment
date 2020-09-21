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
using ViiaNordic.Models.Viia;
using ViiaNordic.Services;
using ViiaNordic.Services.Interfaces;

namespace ViiaNordic.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PaymentController : ControllerBase
    {
        private readonly IViiaService _viiaService;
        private readonly IMemoryCache _memoryCache;
        private readonly IOptionsMonitor<SiteOptions> _options;

        public PaymentController(IHttpClientHandler httpHandler, IViiaService viiaService, IMemoryCache memoryCache, IOptionsMonitor<SiteOptions> options)
        {
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

        [HttpPost]
        public async Task<ActionResult<CreatePaymentResultViewModel>> CreateOutboundPayment(CreatePaymentRequestViewModel body)
        {
            body = new CreatePaymentRequestViewModel
            {
                Amount = 100,
                Iban = "DK5001234567890123",
                SourceAccountId = body.SourceAccountId,
                RecipientFullname = "John Smith",
                message = "this is a message",
                TransactionText = "transaction test"
            };
            var result = new CreatePaymentResultViewModel();
            try
            {
                var createPaymentResult = await _viiaService.CreateOutboundPayment(body);
                result.PaymentId = createPaymentResult.PaymentId;
                result.AuthorizationUrl = createPaymentResult.AuthorizationUrl;
            }
            catch (ViiaClientException e)
            {
                result.ErrorDescription = e.Message;
            }

            return Ok(result);
        }

        [HttpGet]
        public IActionResult PaymentCallback([FromQuery] string paymentId)
        {
            return null;
        }

        [HttpGet]
        public async Task<JsonResult> RefreshToken()
        {
            var updatedTokens = await _viiaService.RefreshAccessTokenAndSaveToUser();
            return new JsonResult(updatedTokens);
        }

        [HttpGet]
        public async Task<JsonResult> GetAccounts()
        {
            var updatedTokens = await _viiaService.GetUserAccounts();
            return new JsonResult(updatedTokens);
        }
    }
}
