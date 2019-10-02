import { Component, OnInit, Injectable, ViewChild, OnDestroy } from '@angular/core';
import { NestedTreeControl } from '@angular/cdk/tree';
import { MatTreeNestedDataSource, MatTree } from '@angular/material/tree';
import { Store } from '@ngrx/store';
import { AppState } from '@app/app-state/models/app-state';
import { takeWhile } from 'rxjs/operators';
import { Account } from '@models/account.model';



/**
 * Food; data; with nested structure.
 * Each; node; has; a; name; and; an; optiona; list; of; children.
 */
interface FoodNode {
  name: string;
  newName?: string;
  children?: FoodNode[];
  isNew?: boolean;
  deleted?: boolean;
  edited?: boolean;
}

const TREE_DATA: FoodNode[] = [
  {
    name: 'Parent Company',
    children: [
      {
        name: 'Shareholder Company Alpha',
        children: [
          {
            name: 'Shareholder Company Beta',
            children: [
              { name: 'Shareholder George' },
              { name: 'Shareholder George\'s Wife' },
            ]
          },
          {
            name: 'Shareholder Company Gamma',
            children: [
              { name: 'Shareholder Carlos' },
              { name: 'Shareholder Carlos¿s Wife' },
            ]
          },
        ]
      },
    ]
  }
];

// TREE_DATA[0].children[0].children[0] = TREE_DATA[0];

@Component({
  selector: 'app-licensee-tree',
  templateUrl: './licensee-tree.component.html',
  styleUrls: ['./licensee-tree.component.scss'],
})
export class LicenseeTreeComponent implements OnInit {
  treeControl = new NestedTreeControl<FoodNode>(node => node.children);
  dataSource = new MatTreeNestedDataSource<FoodNode>();
  @ViewChild('tree') tree: MatTree<any>;
  componentActive = true;
  account: Account;

  constructor(private store: Store<AppState>) {
    this.dataSource.data = TREE_DATA;
  }

  hasChild = (_: number, node: FoodNode) => !!node.children && node.children.length > 0;

  ngOnInit() {
    this.store.select(state => state.currentAccountState.currentAccount)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(account => {
        this.account = account;
      });
  }

  editAssociate(node) {
    node.name = node.name.toUpperCase();
    node.edited = true;
  }

  addAssociate(node) {
    node.children = node.children || [];
    node.children.push({
      name: node.name + ' - child - ' + node.children.length,
      isNew: true
    });
    this.refreshTree();
  }

  deleteAssociate(node) {
    node.deleted = true;
    const children = node.children || [];
    children.forEach(element => {
      this.deleteAssociate(element);
    });
    this.refreshTree();
  }

  refreshTree() {
    const data = [...this.dataSource.data];
    this.dataSource.data = [];
    this.dataSource.data = data;
  }

  OnDestroy() {
    this.componentActive = false;
  }
}
