import { Component, OnInit, Input, TemplateRef } from '@angular/core';
import { TreeNode, TreeNodeOption } from './model/tree-node';
import { TreeHandleService } from '@tools/tree-view/tree-handle.service';

@Component({
  selector: 'app-tree-view',
  templateUrl: './tree-view.component.html',
  styleUrls: ['./tree-view.component.scss']
})
export class TreeViewComponent implements OnInit {
  @Input() treeData: TreeNode[];
  @Input() isRoot: boolean;
  @Input() rootText: string;
  @Input() collapseAll: TemplateRef<unknown>;
  @Input() collapseIcon: TemplateRef<unknown>;
  @Input() allCollapsable = true;

  classText = 'col tree-node-text';
  classSelectedText = 'bg-secondary text-white';
  expandRoot: boolean;
  expandRootAll: boolean;
  constructor(private treeHandle: TreeHandleService) {
    this.isRoot = true;
    this.rootText = 'Root';
    this.expandRoot = true;
    this.expandRootAll = false;
  }

  ngOnInit(): void {}

  public getShowNode(node: TreeNode): boolean {
    if (node.showNodes == null) {
      return false;
    }
    return node.showNodes;
  }

  toggleChild(node: TreeNode): boolean {
    this.treeHandle.onSelectNode(null);
    node.showNodes = !this.getShowNode(node);
    this.checkSelectedNode();
    return false;
  }

  private checkSelectedNode() {
    const selected = document.querySelector('.bg-secondary.text-white');
    const root = document.querySelector('.rootNodeTag');
    if (selected) {
      selected.className = this.classText;
    }
    if (root) {
      root.className = 'tree-node-text rootNodeTag';
    }
  }

  onClick(node: TreeNode): void {
    this.checkSelectedNode();
    const setClass = this.classText;
    if (!node.id) return;
    const currentNode = document.getElementById(`${node.name}-${node.id}`);
    const span = currentNode.querySelector('.tree-node-text');
    if (span.className !== `${this.classText} ${this.classSelectedText}`) {
      const elements = document.getElementsByClassName(setClass);
      Array.from(elements).forEach((elem) => {
        elem.className = setClass;
      });
      span.className = `col ${this.classText} ${this.classSelectedText}`;
    } else {
      span.className = setClass;
    }
    this.treeHandle.onSelectNode(node);
  }

  onClickOption(option: TreeNodeOption, id: number): void {
    this.treeHandle.emitOption({
      id,
      option
    });
  }

  clearSelected(): void {
    const setClass = this.classText;
    const elements = document.getElementsByClassName(setClass);
    Array.from(elements).forEach((elem) => {
      elem.className = setClass;
    });
  }

  onClickRoot(event): boolean {
    this.checkSelectedNode();
    this.treeHandle.onSelectNode(null);
    this.expandRoot = !this.expandRoot;
    let target: Element = event.target;
    while (target.tagName.toUpperCase() !== 'DIV') {
      target = target.parentNode as Element;
      if (!target) {
        break;
      }
    }
    if (target != null) {
      const next = target.nextSibling as HTMLElement;
      if (this.expandRoot) {
        next.style.display = 'block';
      } else {
        next.style.display = 'none';
      }
    }
    return false;
  }

  onClickRootTag(): void {
    this.checkSelectedNode();
    const rootNode = new TreeNode('root');
    rootNode.entity = 'root';
    this.treeHandle.onSelectNode(rootNode);
    const root = document.querySelector('.rootNodeTag');
    if (root) {
      root.className = 'tree-node-text rootNodeTag bg-secondary text-white';
    }
  }

  collapseExpandAll(): boolean {
    this.expandRootAll = !this.expandRootAll;
    for (const node of this.treeData) {
      this.collapseExpando(node, this.expandRootAll);
    }
    return false;
  }

  collapseExpand(node: TreeNode): boolean {
    if (this.getShowNode(node)) {
      this.collapseExpando(node, false);
    } else {
      this.collapseExpando(node, true);
    }
    return false;
  }

  collapseExpando(node: TreeNode, expando: boolean): void {
    node.showNodes = expando;
    if (node.nodes.length === 0) {
      return;
    }
    for (const sub of node.nodes) {
      sub.showNodes = expando;
      this.collapseExpando(sub, expando);
    }
  }
}
