import { Component } from '@angular/core';
import { BankInfo } from './bank-info/bank-info';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [BankInfo, CommonModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = 'NeoBank';
}
