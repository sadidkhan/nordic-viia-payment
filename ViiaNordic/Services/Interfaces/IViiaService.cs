using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ViiaNordic.Models.Cache;
using ViiaNordic.Models.View;
using ViiaNordic.Models.Viia;

namespace ViiaNordic.Services.Interfaces
{
    public interface IViiaService
    {
        Uri GetAuthUri(bool oneTime = false);
        Task<CodeExchangeResponse> ExchangeCodeForAccessToken(string code);
        Task<IImmutableList<Account>> GetUserAccounts(CodeExchangeResponse tokenResponse);
        Task<CreatePaymentResponse> CreateOutboundPayment(CreatePaymentRequestViewModel request);
    }
}
