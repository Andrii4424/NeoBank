import { Component } from '@angular/core';
import { BankInfo } from './bank-info/bank-info';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [BankInfo, CommonModule, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = 'NeoBank';
}
