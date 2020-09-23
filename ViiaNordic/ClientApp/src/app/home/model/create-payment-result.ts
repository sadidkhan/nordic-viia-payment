export interface CreatePaymentResult {
    errorDescription: string;
    paymentId: string;
    authorizationUrl: string;
}
