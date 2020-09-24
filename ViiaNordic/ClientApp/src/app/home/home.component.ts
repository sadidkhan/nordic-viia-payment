import { HomeService } from './home.service';
import { Component, OnInit } from '@angular/core';
import { combineLatest } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  accounts: any[] = [];
  accessToken: string;
  isIceBroken = false;
  constructor(private homeService: HomeService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.queryParamMap
      .subscribe((params: any) => {
        const code = params.get('code');
        const consentId = params.get('consentId');
        console.log(code, consentId);
        const paymentId = params.get('paymentId');
        if (code && consentId) {
          this.isIceBroken = true;
          this.homeService.getBankList(code, consentId).subscribe((response: any) => {
            this.accounts = response;
            console.log('response bank', response);
          });
        }
        if (paymentId) {
          console.log(paymentId);
        }
      });
  }

  connectWithVia() {
    this.homeService.connect();
  }

  connectWithBackend() {
    this.homeService.connectWithBackend();
  }

  // validateAccount(account){
  //   this.homeService.validateAccount(account);
  //  // this.createOutboundPayment(account);
  // }

  createOutboundPayment(account) {
    this.homeService.createOutboundPayment(account);
  }

  refreshToken() {
    this.homeService.refreshToken()
      .subscribe({
        next: response => {
          this.accessToken = response['accessToken'];
        }
      });
  }

  getAccounts() {
    this.homeService.getAccounts().subscribe((response: any) => {
      this.accounts = response;
      console.log('response bank', response);
    });
  }
}
