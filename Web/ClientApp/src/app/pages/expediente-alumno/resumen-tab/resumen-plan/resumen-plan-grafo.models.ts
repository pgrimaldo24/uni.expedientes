import { ArcoDto, TipoNodoDto } from '../resumen.models';

export interface IECONode {
  id?: number;
  title?: string;
  data?: IECONodeData;
  linkColor?: string;
  background?: string;
  color?: string;
  width?: number;
  height?: number;
  children?: IECONode[];
}

export interface IECONodeData {
  id: number;
  nombre: string;
  tipo: TipoNodoDto;
  arcosSalientes?: ArcoDto[];
  seleccionado?: boolean;
  superado?: boolean;
  esNodoFinal?: boolean;
  anyAsignaturaMatriculada?: boolean;
}

export class ECONode {
  id: number;
  pid: number;
  width: number;
  height: number;
  color: string;
  background: string;
  linkColor: string;
  data: IECONodeData;
  siblingIndex = 0;
  dbIndex = 0;
  XPosition = 0;
  YPosition = 0;
  prelim = 0;
  modifier = 0;
  leftNeighbor = null;
  rightNeighbor = null;
  nodeParent = null;
  nodeChildren = [];
  constructor(
    id: number,
    pid: number,
    width: number,
    height: number,
    color: string,
    background: string,
    linkColor: string,
    meta: IECONodeData
  ) {
    this.id = id;
    this.pid = pid;
    this.width = width;
    this.height = height;
    this.color = color;
    this.background = background;
    this.linkColor = linkColor;
    this.data = meta;
    this.siblingIndex = 0;
    this.dbIndex = 0;
    this.XPosition = 0;
    this.YPosition = 0;
    this.prelim = 0;
    this.modifier = 0;
    this.leftNeighbor = null;
    this.rightNeighbor = null;
    this.nodeParent = null;
    this.nodeChildren = [];
  }

  _getLevel(): number {
    return this.nodeParent.id == -1 ? 0 : this.nodeParent._getLevel() + 1;
  }

  _getChildrenCount(): number {
    return this.nodeChildren == null ? 0 : this.nodeChildren.length;
  }

  _getLeftSibling(): ECONode {
    return this.leftNeighbor != null &&
      this.leftNeighbor.nodeParent == this.nodeParent
      ? this.leftNeighbor
      : null;
  }

  _getRightSibling(): ECONode {
    return this.rightNeighbor != null &&
      this.rightNeighbor.nodeParent == this.nodeParent
      ? this.rightNeighbor
      : null;
  }

  _getChildAt(i: number): ECONode {
    return this.nodeChildren[i];
  }

  _getChildrenCenter(tree: ECOTree): number {
    const nodeFirst = this._getFirstChild();
    const nodeLast = this._getLastChild();
    return (
      nodeFirst.prelim +
      (nodeLast.prelim - nodeFirst.prelim + tree._getNodeSize(nodeLast)) / 2
    );
  }

  _getFirstChild(): ECONode {
    return this._getChildAt(0);
  }

  _getLastChild(): ECONode {
    return this._getChildAt(this._getChildrenCount() - 1);
  }

  _drawPath(node: ECONode, subNode: ECONode): string {
    let xa = 0,
      ya = 0,
      xb = 0,
      yb = 0,
      xc = 0,
      yc = 0,
      xd = 0,
      yd = 0;
    xa = node.XPosition + node.width / 2;
    ya = node.YPosition + node.height;
    xd = xc = subNode.XPosition + subNode.width / 2;
    yd = subNode.YPosition;
    xb = xa;
    yb = yc = ya + (yd - ya) / 2;
    const path = `M ${xa} ${ya - 4} C ${xb} ${yb} ${xc} ${yc} ${xd} ${yd}`;
    return path;
  }
}

export class ECOTree {
  config: any;
  canvasoffsetTop = 0;
  canvasoffsetLeft = 0;
  maxLevelHeight = [];
  maxLevelWidth = [];
  previousLevelNode = [];
  rootYOffset = 0;
  rootXOffset = 0;
  nDatabaseNodes = [];
  nDatabaseNodesPath = [];
  mapIDs = {};
  root;
  iSelectedNode = -1;
  iLastSearch = 0;
  width = 0;
  height = 0;

  constructor() {
    this.config = {
      iMaxDepth: 100,
      iLevelSeparation: 60,
      iSiblingSeparation: 40,
      iSubtreeSeparation: 80,
      topXAdjustment: 0,
      topYAdjustment: 0,
      nodeColor: '#333',
      nodeBorderColor: 'white',
      nodeSelColor: '#FFFFCC',
      defaultNodeWidth: 180,
      defaultNodeHeight: 40
    };

    this.canvasoffsetTop = 0;
    this.canvasoffsetLeft = 0;
    this.maxLevelHeight = [];
    this.maxLevelWidth = [];
    this.previousLevelNode = [];
    this.rootYOffset = 0;
    this.rootXOffset = 0;
    this.nDatabaseNodes = [];
    this.mapIDs = {};
    this.root = new ECONode(-1, null, 2, 2, null, null, null, null);
    this.iSelectedNode = -1;
    this.iLastSearch = 0;
  }

  _firstWalk(tree: ECOTree, node: ECONode, level: number): void {
    let leftSibling = null;
    node.XPosition = 0;
    node.YPosition = 0;
    node.prelim = 0;
    node.modifier = 0;
    node.leftNeighbor = null;
    node.rightNeighbor = null;
    tree._setLevelHeight(node, level);
    tree._setLevelWidth(node, level);
    tree._setNeighbors(node, level);
    if (node._getChildrenCount() == 0 || level == tree.config.iMaxDepth) {
      leftSibling = node._getLeftSibling();
      if (leftSibling != null)
        node.prelim =
          leftSibling.prelim +
          tree._getNodeSize(leftSibling) +
          tree.config.iSiblingSeparation;
      else node.prelim = 0;
    } else {
      const n = node._getChildrenCount();
      for (let i = 0; i < n; i++) {
        const iChild = node._getChildAt(i);
        this._firstWalk(tree, iChild, level + 1);
      }

      let midPoint = node._getChildrenCenter(tree);
      midPoint -= tree._getNodeSize(node) / 2;
      leftSibling = node._getLeftSibling();
      if (leftSibling != null) {
        node.prelim =
          leftSibling.prelim +
          tree._getNodeSize(leftSibling) +
          tree.config.iSiblingSeparation;
        node.modifier = node.prelim - midPoint;
        this._apportion(tree, node, level);
      } else {
        node.prelim = midPoint;
      }
    }
  }

  _apportion(tree: ECOTree, node: ECONode, level: number): void {
    let firstChild = node._getFirstChild();
    let firstChildLeftNeighbor = firstChild.leftNeighbor;
    let j = 1;
    for (
      let k = tree.config.iMaxDepth - level;
      firstChild != null && firstChildLeftNeighbor != null && j <= k;

    ) {
      let modifierSumRight = 0;
      let modifierSumLeft = 0;
      let rightAncestor = firstChild;
      let leftAncestor = firstChildLeftNeighbor;
      for (let l = 0; l < j; l++) {
        rightAncestor = rightAncestor.nodeParent;
        leftAncestor = leftAncestor.nodeParent;
        modifierSumRight += rightAncestor.modifier;
        modifierSumLeft += leftAncestor.modifier;
      }

      let totalGap =
        firstChildLeftNeighbor.prelim +
        modifierSumLeft +
        tree._getNodeSize(firstChildLeftNeighbor) +
        tree.config.iSubtreeSeparation -
        (firstChild.prelim + modifierSumRight);
      if (totalGap > 0) {
        let subtreeAux = node;
        let numSubtrees = 0;
        for (
          ;
          subtreeAux != null && subtreeAux != leftAncestor;
          subtreeAux = subtreeAux._getLeftSibling()
        )
          numSubtrees++;

        if (subtreeAux != null) {
          let subtreeMoveAux = node;
          const singleGap = totalGap / numSubtrees;
          for (
            ;
            subtreeMoveAux != leftAncestor;
            subtreeMoveAux = subtreeMoveAux._getLeftSibling()
          ) {
            subtreeMoveAux.prelim += totalGap;
            subtreeMoveAux.modifier += totalGap;
            totalGap -= singleGap;
          }
        }
      }
      j++;
      if (firstChild._getChildrenCount() == 0)
        firstChild = tree._getLeftmost(node, 0, j);
      else firstChild = firstChild._getFirstChild();
      if (firstChild != null) firstChildLeftNeighbor = firstChild.leftNeighbor;
    }
  }

  _secondWalk(
    tree: ECOTree,
    node: ECONode,
    level: number,
    numberX: number,
    numberY: number
  ): void {
    if (level <= tree.config.iMaxDepth) {
      const xTmp = tree.rootXOffset + node.prelim + numberX;
      const yTmp = tree.rootYOffset + numberY;
      let maxsizeTmp = 0;
      let nodesizeTmp = 0;
      maxsizeTmp = tree.maxLevelHeight[level];
      nodesizeTmp = node.height;
      node.XPosition = xTmp;
      node.YPosition = yTmp + (maxsizeTmp - nodesizeTmp) / 2;
      if (node._getChildrenCount() != 0) {
        const separacion = level == 0 ? 5 : tree.config.iLevelSeparation;
        this._secondWalk(
          tree,
          node._getFirstChild(),
          level + 1,
          numberX + node.modifier,
          numberY + maxsizeTmp + separacion
        );
      }
      const rightSibling = node._getRightSibling();
      if (rightSibling != null)
        this._secondWalk(tree, rightSibling, level, numberX, numberY);
    }
  }

  _positionTree(): void {
    this.maxLevelHeight = [];
    this.maxLevelWidth = [];
    this.previousLevelNode = [];
    this._firstWalk(this, this.root, 0);
    this.rootXOffset = this.config.topXAdjustment + this.root.XPosition;
    this.rootYOffset = this.config.topYAdjustment + this.root.YPosition;
    this._secondWalk(this, this.root, 0, 0, 0);
  }

  _setLevelHeight(node: ECONode, level: number): void {
    if (this.maxLevelHeight[level] == null) this.maxLevelHeight[level] = 0;
    if (this.maxLevelHeight[level] < node.height)
      this.maxLevelHeight[level] = node.height;
  }

  _setLevelWidth(node: ECONode, level: number): void {
    if (this.maxLevelWidth[level] == null) this.maxLevelWidth[level] = 0;
    if (this.maxLevelWidth[level] < node.width)
      this.maxLevelWidth[level] = node.width;
  }

  _setNeighbors(node: ECONode, level: number): void {
    node.leftNeighbor = this.previousLevelNode[level];
    if (node.leftNeighbor != null) node.leftNeighbor.rightNeighbor = node;
    this.previousLevelNode[level] = node;
  }

  _getNodeSize(node: ECONode): number {
    return node.width;
  }

  _getLeftmost(node: ECONode, level: number, maxlevel: number): ECONode {
    if (level >= maxlevel) return node;
    if (node._getChildrenCount() == 0) return null;

    const n = node._getChildrenCount();
    for (let i = 0; i < n; i++) {
      const iChild = node._getChildAt(i);
      const leftmostDescendant = this._getLeftmost(iChild, level + 1, maxlevel);
      if (leftmostDescendant != null) return leftmostDescendant;
    }

    return null;
  }

  UpdateTree(): void {
    this._positionTree();
    this.width = Math.max(
      ...this.nDatabaseNodes.map((x) => x.XPosition + x.width)
    );
    this.height = Math.max(
      ...this.nDatabaseNodes.map((x) => x.YPosition + x.height)
    );
  }

  add = function (
    id: number,
    pid: number,
    width: number,
    height: number,
    color: string,
    background: string,
    linkColor: string,
    data: IECONodeData
  ): void {
    const nwidth = width || this.config.defaultNodeWidth;
    const nheight = height || this.config.defaultNodeHeight;
    const ncolor = color || this.config.nodeColor;
    const nbackground = background || this.config.nodeBorderColor;
    let pnode = null;
    if (pid == -1) {
      pnode = this.root;
    } else {
      for (let k = 0; k < this.nDatabaseNodes.length; k++) {
        if (this.nDatabaseNodes[k].id == pid) {
          pnode = this.nDatabaseNodes[k];
          break;
        }
      }
    }

    const node = new ECONode(
      id,
      pid,
      nwidth,
      nheight,
      ncolor,
      nbackground,
      linkColor,
      data
    );
    node.nodeParent = pnode;
    pnode.canCollapse = true;
    const i = this.nDatabaseNodes.length;
    node.dbIndex = this.mapIDs[id] = i;
    this.nDatabaseNodes[i] = node;
    height = pnode.nodeChildren.length;
    node.siblingIndex = height;
    pnode.nodeChildren[height] = node;
  };
}
