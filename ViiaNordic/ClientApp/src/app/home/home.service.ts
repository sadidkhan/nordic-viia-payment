import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { combineLatest } from 'rxjs/internal/observable/combineLatest';

@Injectable({
    providedIn: 'root'
})
export class HomeService  {

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
        this.http.get(url)
        .subscribe((response: any) => {
            console.log('response bank', response);
        });
    }
}
