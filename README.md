# Nordic VIiA Payment

A .NET + TypeScript project for integrating a Nordic payment flow (checkout, capture/refund, and webhooks).  
This repository contains a Visual Studio solution and an application under `ViiaNordic/`.

---

## ✨ Features

- Secure payment initialization and redirect/return flows  
- Capture / refund / void operations  
- Idempotent server APIs for payment updates  
- Webhook receiver for asynchronous state changes  
- Minimal frontend to kick off a payment session  

---

## 🛠️ Used Technologies

- **C# / .NET 7.0** — backend APIs and payment logic  
- **ASP.NET Core Web API** — REST endpoints  
- **TypeScript / JavaScript** — frontend logic  
- **HTML / CSS** — UI for payment initiation  
- **Visual Studio 2022** — development environment  

---

## 🚀 Quick Start

### Prerequisites
- .NET SDK 7.0+  
- Node.js 18+ (if using frontend)  
- Visual Studio 2022 or `dotnet` CLI  

### Setup
```bash
git clone https://github.com/sadidkhan/nordic-viia-payment.git
cd nordic-viia-payment
dotnet restore
dotnet run --project ViiaNordic

---

## ⚙️ Configuration

Add your credentials in appsettings.json (for local dev) or as environment variables (for production):

```json
{
  "Payment": {
    "ApiBaseUrl": "https://sandbox.example.com",
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET",
    "MerchantId": "YOUR_MERCHANT_ID",
    "Callback": {
      "ReturnUrl": "https://localhost:5001/payment/return",
      "WebhookUrl": "https://localhost:5001/payment/webhook"
    }
  }
}
