import {
  Component,
  Input,
  Output,
  EventEmitter,
  TemplateRef
} from '@angular/core';
import { UnirTreeViewItem } from '../../models/unir-tree-view-item';

@Component({
  selector: 'unir-tree-view',
  templateUrl: './unir-tree-view.component.html',
  styleUrls: ['./unir-tree-view.component.scss']
})
export class UnirTreeViewComponent {
  @Input() items: UnirTreeViewItem[];
  @Input() collapseIcon: TemplateRef<unknown>;
  @Input() searchText = '';
  @Output() selectedChange = new EventEmitter<unknown>();

  constructor() {}

  get maxHeight(): string {
    return '200';
  }

  public selectItem(item: UnirTreeViewItem): void {
    if (item.selectable) {
      this.selectedChange.emit(item);
    }
  }

  public onCollapseExpand(item: UnirTreeViewItem): void {
    item.collapsed = !item.collapsed;
  }
}
