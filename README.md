# Nordic VIiA Payment

A .NET + TypeScript project for integrating a Nordic payment flow (checkout, capture/refund, and webhooks).  
This repository contains a Visual Studio solution and an application under `ViiaNordic/`.

> **Status:** WIP — initial README scaffold. Replace the TODOs below with your actual details.

---

## ✨ Features (high level)

- Secure payment initialization and redirect/return flows
- Capture / refund / void operations
- Idempotent server APIs for payment updates
- Webhook receiver for asynchronous state changes
- Minimal frontend to kick off a payment session

> Tech mix observed: **C#** backend + **TypeScript/HTML/CSS** frontend. :contentReference[oaicite:1]{index=1}

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

## ⚙️ Configuration

Add your credentials in `appsettings.json` (for local dev) or as environment variables (for production):

```json
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


## 🔄 Payment Flow

```mermaid
sequenceDiagram
    participant User
    participant App as Your App
    participant Provider as Payment Provider

    User->>App: Place order
    App->>Provider: Create payment session
    Provider-->>App: Session ID / Redirect URL
    App-->>User: Redirect to checkout
    User->>Provider: Complete payment
    Provider-->>User: Redirect back (ReturnUrl)
    Provider-->>App: Send webhook (payment status)
    App->>App: Update order (Paid / Failed / Refunded)
