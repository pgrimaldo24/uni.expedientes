import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UnirTreeViewComponent } from '@tools/unir-tree-select/components/treeview/unir-tree-view.component';
import { UnirTreeViewItemComponent } from '@tools/unir-tree-select/components/treeview-item/unir-tree-view-item.component';
import { ComboboxTreeComponent } from './components/combobox-tree/combobox-tree.component';
import { HighlightTextPipe } from '@tools/unir-tree-select/pipes/highlight-text.pipe';

@NgModule({
  imports: [FormsModule, CommonModule, ReactiveFormsModule],
  declarations: [
    UnirTreeViewComponent,
    UnirTreeViewItemComponent,
    ComboboxTreeComponent,
    HighlightTextPipe
  ],
  exports: [UnirTreeViewComponent, ComboboxTreeComponent]
})
export class UnirTreeSelectModule {}
