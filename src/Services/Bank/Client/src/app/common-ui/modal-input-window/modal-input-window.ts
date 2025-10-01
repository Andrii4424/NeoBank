import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-modal-input-window',
  imports: [FormsModule],
  templateUrl: './modal-input-window.html',
  styleUrl: './modal-input-window.scss'
})
export class ModalInputWindow {
  @Input() headerMessage: string ="";
  @Input() subTittleMessage: string ="";
  @Output() submitValue = new EventEmitter<number>();
  @Output() closeWindow = new EventEmitter<void>();
  amount : number = 0;

  onModalInputChange(value: number){
    if (value < 0) {
      this.amount = 0;
    } 
    else if(value > 200000000){
      setTimeout(() => this.amount = 200000000);
    }
    else {
      this.amount = value;
    }
  }

  closeModalWindow(){
    this.closeWindow.emit();
  }

  onSubmit(){
    this.submitValue.emit(this.amount);
  }
}
