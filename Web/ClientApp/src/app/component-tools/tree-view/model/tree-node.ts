export class TreeNode {
  id?: string;
  name: string;
  value?: string;
  showNodes?: boolean;
  entity?: string;
  opciones?: TreeNodeOption[];
  nodes?: TreeNode[];
  data?: any;
  constructor(
    name: string,
    value?: string,
    nodes?: TreeNode[],
    entity?: string,
    data?: any
  ) {
    this.name = name;
    this.value = value;
    this.entity = entity;
    this.data = data;
    if (nodes == null) {
      this.nodes = [];
    } else {
      this.nodes = nodes;
    }
  }
}

export interface TreeNodeOption {
  entity: string;
  type: string;
  value?: any;
}

export interface TreeNodeOptionSelected {
  id: number;
  option: TreeNodeOption;
}
