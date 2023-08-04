import { Injectable } from '@angular/core';
import {
  TreeNode,
  TreeNodeOptionSelected
} from '@tools/tree-view/model/tree-node';
import { Observable, Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TreeHandleService {
  private node: Subject<TreeNode>;
  private selectedOption: Subject<TreeNodeOptionSelected>;

  constructor() {
    this.node = new Subject<TreeNode>();
    this.selectedOption = new Subject<TreeNodeOptionSelected>();
  }

  onSelectNode(node: TreeNode): void {
    this.node.next(node);
  }

  selectedNode(): Observable<TreeNode> {
    return this.node.asObservable();
  }

  emitOption(option: TreeNodeOptionSelected): void {
    this.selectedOption.next(option);
  }

  getOptions(): Observable<TreeNodeOptionSelected> {
    return this.selectedOption.asObservable();
  }
}
