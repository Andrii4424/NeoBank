import { Component, Input, signal } from '@angular/core';

@Component({
  selector: 'app-success-message',
  imports: [],
  templateUrl: './success-message.html',
  styleUrl: './success-message.scss'
})
export class SuccessMessage {
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
