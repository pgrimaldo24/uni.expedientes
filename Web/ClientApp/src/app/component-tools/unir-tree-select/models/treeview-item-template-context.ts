import { UnirTreeViewItem } from './unir-tree-view-item';

export interface TreeviewItemTemplateContext {
  item: UnirTreeViewItem;
  onCollapseExpand: () => void;
}
