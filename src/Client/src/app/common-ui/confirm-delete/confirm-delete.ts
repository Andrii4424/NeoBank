import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-confirm-delete',
  imports: [TranslateModule],
  templateUrl: './confirm-delete.html',
  styleUrl: './confirm-delete.scss'
})
export class ConfirmDelete {
  @Input() itemName: string = 'item';

  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();
  
  onConfirm() {
    this.confirm.emit();
  }

  onCancel() {
    this.cancel.emit();
  }
}
