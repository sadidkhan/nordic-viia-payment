import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { combineLatest } from 'rxjs/internal/observable/combineLatest';
import { CreatePaymentResult } from './model/create-payment-result';

@Injectable({
    providedIn: 'root'
})
export class HomeService  {

    accounts: any[];
    baseUri = 'http://localhost:61658/api/payment';
    viiaBaseUri = 'https://api-sandbox.getviia.com/v1';
    constructor(private http: HttpClient
        ) {
    }

    connect(): any {
        const param = {
            client_id: 'fftest-3e2d3cce-a215-4c00-96b5-c3f3611d5eb3',
            redirect_uri: `${this.baseUri}/connectionSuccess`,
            response_type: 'code'
        };

        // this.http.get(`${this.viiaBaseUri}/oauth/connect`, { params, observe: 'response' })
        //     // .pipe(map(response => response))
        //     .subscribe(resposne => {
        //         console.log(resposne);
        //     });
        const a = `${this.viiaBaseUri}/oauth/connect?client_id=${param.client_id}&redirect_uri=${param.redirect_uri}&response_type=${param.response_type}`;
        window.location.href = encodeURI(a);
    }

    connectWithBackend(): any {
        this.http.get(this.baseUri + '/connect')
            .subscribe((resposne: string) => {
                console.log(resposne);
                window.location.href = resposne;
            });
    }

    getBankList(code: string, consentId: string): any {
        console.log(code);
        // tslint:disable-next-line: max-line-length
        let url = 'http://localhost:61658/api/payment/connectionSuccess' + `?code=${encodeURIComponent(code)}` + '&&consentId=' + consentId;
        console.log(url);
        return this.http.get(url);
    }

    createOutboundPayment(account): any {
        const body = {
            amount: 100,
            sourceAccountId: account.id,
            bbanAccountNumber: account.number.bbanParsed.accountNumber,
            bbanBankCode: account.number.bbanParsed.bankCode,
            iban: account.number.iban,
            recipientFullname: account.owner,
            message: `amount 100`
        };
        this.http.post(this.baseUri + '/createoutboundpayment', body)
            .subscribe((response: CreatePaymentResult) => {
                window.location.href = encodeURI(response.authorizationUrl);
            });
    }

    refreshToken(): Observable<any> {
        return this.http.get(this.baseUri + '/RefreshToken');
    }

    getAccounts(): any {
        // tslint:disable-next-line: max-line-length
        let url = 'http://localhost:61658/api/payment/GetAccounts';
        console.log(url);
        return this.http.get(url);
    }
}
