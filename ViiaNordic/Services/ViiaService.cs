using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ViiaNordic.Config;
using ViiaNordic.Handler;
using ViiaNordic.Models.Cache;
using ViiaNordic.Models.View;
using ViiaNordic.Models.Viia;
using ViiaNordic.Services.Interfaces;

namespace ViiaNordic.Services
{
    public class ViiaService: IViiaService
    {
        private readonly IOptionsMonitor<SiteOptions> _options;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ViiaService(IOptionsMonitor<SiteOptions> options, HttpClient httpClient, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            _options = options;
            _httpClient = httpClient;
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public Uri GetAuthUri(bool oneTime = false)
        {
            var connectUrl =
                $"{_options.CurrentValue.Viia.BaseApiUrl}/v1/oauth/connect" +
                $"?client_id={_options.CurrentValue.Viia.ClientId}" +
                "&response_type=code" +
                //$"&redirect_uri={_options.CurrentValue.Viia.LoginCallbackUrl}" +
                $"&redirect_uri=http://localhost:61658/home" +
                $"&flow={(oneTime ? "OneTimeUser" : "PersistentUser")}";

            return new Uri(connectUrl);
        }


        private string GenerateBasicAuthorizationHeaderValue()
        {
            var credentials = $"{_options.CurrentValue.Viia.ClientId}:{_options.CurrentValue.Viia.ClientSecret}";
            var credentialsByteData = Encoding.GetEncoding("iso-8859-1").GetBytes(credentials);
            var base64Credentials = Convert.ToBase64String(credentialsByteData);
            return $"{base64Credentials}";
        }

        private ChachedViiaInfo GetStoredToken()
        {
            var tokenInfo = _memoryCache.Get<ChachedViiaInfo>(_options.CurrentValue.FakeUserEmail);
            return tokenInfo;
        }

        private string GetPaymentRedirectUrl()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}{request.PathBase}/home";
        }

        public async Task<CodeExchangeResponse> ExchangeCodeForAccessToken(string code)
        {
            //using (var httpClient = _httpClient)
            //{
                var requestUrl = "v1/oauth/token";

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", GenerateBasicAuthorizationHeaderValue());

                var tokenBody = new
                {
                    grant_type = "authorization_code",
                    code,
                    scope = "read",
                    redirect_uri = "http://localhost:61658/home"
                };

                var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(tokenBody),
                        Encoding.UTF8,
                        "application/json")
                };

                var response = await _httpClient.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Failed to exchange code for tokens");
                }

                var tokenResponse =
                    JsonConvert.DeserializeObject<CodeExchangeResponse>(content);
                return tokenResponse;
            //}
        }

        public async Task<ValidateAccountResponse> ValidateAccount(string id, CodeExchangeResponse tokenResponse = null)
        {
            if (tokenResponse == null)
            {
                tokenResponse = await RefreshAccessTokenAndSaveToUser();
            }

            var result = await HttpGet<ValidateAccountResponse>($"/v1/accounts/{id}", tokenResponse.TokenType, tokenResponse.AccessToken);

            return result;
        }


        public async Task<CodeExchangeResponse> RefreshAccessToken(string refreshToken)
        {

            var requestUrl = "v1/oauth/token";

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", GenerateBasicAuthorizationHeaderValue());

            var tokenBody = new
            {
                grant_type = "refresh_token",
                refresh_token = refreshToken,
                scope = "read",
                redirect_uri = "http://localhost:61658/home"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl)
            {
                Content = new StringContent(JsonConvert.SerializeObject(tokenBody),
                    Encoding.UTF8,
                    "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to exchange code for tokens");
            }

            var tokenResponse =
                JsonConvert.DeserializeObject<CodeExchangeResponse>(content);
            return tokenResponse;

        }

        public async Task<CodeExchangeResponse> RefreshAccessTokenAndSaveToUser()
        {
            //var currentUserId = principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var user = _dbContext.Users.FirstOrDefault(x => x.Id == currentUserId);
            //if (user == null)
            //{
            //    return null;
            //}
            if (!_memoryCache.TryGetValue(_options.CurrentValue.FakeUserEmail, out ChachedViiaInfo cachedViiaInfo))
            {
                return null;
            }

            var result = await RefreshAccessToken(cachedViiaInfo.ViiaRefreshToken);
            cachedViiaInfo.ViiaAccessToken = result.AccessToken;
            cachedViiaInfo.ViiaRefreshToken = result.RefreshToken;
            cachedViiaInfo.ViiaTokenType = result.TokenType;

            _memoryCache.Set(_options.CurrentValue.FakeUserEmail, cachedViiaInfo, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(6),
                Priority = CacheItemPriority.Normal
            });
            
            return result;
        }

        public async Task<IImmutableList<Account>> GetUserAccounts(CodeExchangeResponse tokenResponse = null)
        {
            if (tokenResponse == null)
            {
                tokenResponse = await RefreshAccessTokenAndSaveToUser();
            }

            var result = await HttpGet<AccountsResponse>("/v1/accounts", tokenResponse.TokenType, tokenResponse.AccessToken);
            return result?.Accounts.ToImmutableList();
        }

        public async Task<CreatePaymentResponse> CreateOutboundPayment(CreatePaymentRequestViewModel request)
        {
            //var currentUserId = principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var user = _dbContext.Users.FirstOrDefault(x => x.Id == currentUserId);
            //if (user == null)
            //{
            //    throw new UserNotFoundException();
            //}
            var token = GetStoredToken();
            var paymentRequest = new CreateOutboundPaymentRequest
            {
                Culture = request.Culture,
                RedirectUrl = GetPaymentRedirectUrl(),
                Payment = new PaymentRequest
                {
                    Message = request.message,
                    TransactionText = request.TransactionText,
                    Amount = new PaymentAmountRequest
                    {
                        Value = request.Amount
                    },
                    Destination = new PaymentDestinationRequest()
                },
            };

            paymentRequest.Payment.Destination.RecipientFullname = request.RecipientFullname;

            if (!string.IsNullOrWhiteSpace(request.Iban))
            {
                paymentRequest.Payment.Destination.IBan = request.Iban;
            }
            else
            {
                paymentRequest.Payment.Destination.BBan = new PaymentBBanRequest
                {
                    BankCode = request.BbanBankCode,
                    AccountNumber = request.BbanAccountNumber
                };
            }

            return await CallApi<CreatePaymentResponse>($"v1/accounts/{request.SourceAccountId}/payments/outbound",
                                                        paymentRequest,
                                                        HttpMethod.Post,
                                                        token.ViiaTokenType,
                                                        token.ViiaAccessToken);
        }
        private async Task<T> CallApi<T>(string url,
                                         object body,
                                         HttpMethod method,
                                         string accessTokenType = null,
                                         string accessToken = null)
        {
            HttpResponseMessage result = null;
            string responseContent = null;
            try
            {
                var httpRequestMessage = new HttpRequestMessage(method, url)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json")
                };

                if (accessTokenType != null && accessToken != null)
                {
                    httpRequestMessage.Headers.Authorization =
                        new AuthenticationHeaderValue(accessTokenType, accessToken);
                }

                // var sw = Stopwatch.StartNew();
                result = await _httpClient.SendAsync(httpRequestMessage);
                // var duration = sw.Elapsed;

                if (!result.IsSuccessStatusCode)
                {
                    responseContent = await result.Content.ReadAsStringAsync();
                    throw new ViiaClientException(url, result.StatusCode, responseContent);
                }

                responseContent = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseContent);
            }
            catch (ViiaClientException)
            {
                throw;
            }
            catch (JsonSerializationException e)
            {
                throw new ViiaClientException(url, method, responseContent, e);
            }
            catch (Exception e)
            {
                throw new ViiaClientException(url, method, result, e);
            }
        }

        private async Task<T> HttpGet<T>(string url,
            string accessTokenType = null,
            string accessToken = null,
            ClaimsPrincipal principal = null,
            bool isRetry = false)
        {
            try
            {
                return await CallApi<T>(url, null, HttpMethod.Get, accessTokenType, accessToken);
            }
            catch (ViiaClientException e) when (e.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(accessToken) &&
                                                !isRetry)
            {
                var updatedTokens = await RefreshAccessTokenAndSaveToUser();
                return await HttpGet<T>(url, updatedTokens.TokenType, updatedTokens.AccessToken, principal, true);
            }
        }

        
    }
}
