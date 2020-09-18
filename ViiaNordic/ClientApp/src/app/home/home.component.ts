import { HomeService } from './home.service';
import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  constructor(private homeService: HomeService){}
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
