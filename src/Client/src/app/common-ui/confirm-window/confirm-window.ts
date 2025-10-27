import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-confirm-window',
  imports: [TranslateModule],
  templateUrl: './confirm-window.html',
  styleUrl: './confirm-window.scss'
})
export class ConfirmWindow {
  @Input() title: string="";
  @Input() subtitle: string="";
  @Input() action: string="";

  @Output() confirmAction = new EventEmitter<string>();
  @Output() cancelAction = new EventEmitter<void>();

  onConfirm(){
    this.confirmAction.emit(this.action);
  }

  onCancel(){
    this.cancelAction.emit();
  }
  
}
