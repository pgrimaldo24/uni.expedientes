import {
  Component,
  EventEmitter,
  Input,
  Output,
  TemplateRef
} from '@angular/core';
import { TreeviewItemTemplateContext } from '@tools/unir-tree-select/models/treeview-item-template-context';
import { UnirTreeViewItem } from '@tools/unir-tree-select/models/unir-tree-view-item';

@Component({
  selector: 'unir-tree-view-item',
  templateUrl: './unir-tree-view-item.component.html',
  styleUrls: ['./unir-tree-view-item.component.scss']
})
export class UnirTreeViewItemComponent {
  @Input() template: TemplateRef<TreeviewItemTemplateContext>;
  @Input() item: UnirTreeViewItem;
  @Output() checkedChange = new EventEmitter<boolean>();

  constructor() {}

  onCollapseExpand = (): void => {
    this.item.collapsed = !this.item.collapsed;
  };
}
