import { Component, Input, signal } from '@angular/core';

@Component({
  selector: 'app-error-message',
  imports: [],
  templateUrl: './error-message.html',
  styleUrl: './error-message.scss'
})
export class ErrorMessage {
  @Input() message: string = '';
  @Input() secondMessage: string = '';
  
  visible = signal<boolean>(true);

  ngOnInit() {
    setTimeout(() => this.visible.set(false), 3000);
  }

  close(){
    this.visible.set(false);
  }
}
