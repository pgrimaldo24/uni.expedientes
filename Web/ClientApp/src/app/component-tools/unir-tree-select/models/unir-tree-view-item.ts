import { isBoolean, isNil, isString } from 'lodash';

export interface TreeItem {
  id: number;
  name: string;
  selectable?: boolean;
  entity?: string;
  collapsed?: boolean;
  nodes?: TreeItem[];
}

export class UnirTreeViewItem {
  private internalCollapsed = false;
  private internalChildren: UnirTreeViewItem[];
  id: number;
  name: string;
  selectable?: boolean;
  entity?: string;

  constructor(item: TreeItem) {
    if (isNil(item)) {
      throw new Error('Item must be defined');
    }
    if (isString(item.name)) {
      this.name = item.name;
    } else {
      throw new Error('A text of item must be string object');
    }
    this.id = item.id;
    if (isBoolean(item.collapsed)) {
      this.collapsed = item.collapsed;
    } else {
      this.collapsed = true;
    }
    if (!isNil(item.nodes) && item.nodes.length > 0) {
      this.nodes = item.nodes.map((child) => {
        return new UnirTreeViewItem(child);
      });
    }
    this.entity = item.entity;
    this.selectable = true;
    if (item.selectable !== null) {
      this.selectable = item.selectable;
    }
  }

  get collapsed(): boolean {
    return this.internalCollapsed;
  }

  set collapsed(value: boolean) {
    if (this.internalCollapsed !== value) {
      this.internalCollapsed = value;
    }
  }

  setCollapsedRecursive(value: boolean): void {
    this.internalCollapsed = value;
    if (!isNil(this.internalChildren)) {
      this.internalChildren.forEach((child) =>
        child.setCollapsedRecursive(value)
      );
    }
  }

  get nodes(): UnirTreeViewItem[] {
    return this.internalChildren;
  }

  set nodes(value: UnirTreeViewItem[]) {
    if (this.internalChildren !== value) {
      if (!isNil(value) && value.length === 0) {
        throw new Error('Children must be not an empty array');
      }
      this.internalChildren = value;
    }
  }
}
