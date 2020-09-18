import { HomeService } from './home.service';
import { Component, OnInit } from '@angular/core';
import { combineLatest } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  constructor(private homeService: HomeService,
    private route: ActivatedRoute){
  }

  ngOnInit() {
    this.route.queryParamMap
      .subscribe((params: any) => {
        const code = params.get('code');
        const consentId = params.get('consentId');
        console.log(code, consentId);

        if(code && consentId) {
          this.homeService.getBankList(code, consentId);
        }
      });
  }

  connectWithVia() {
    this.homeService.connect();
  }


  connectWithBackend() {
    this.homeService.connectWithBackend();
  }

  createOutboundPayment() {
    this.homeService.createOutboundPayment();
  }
}
