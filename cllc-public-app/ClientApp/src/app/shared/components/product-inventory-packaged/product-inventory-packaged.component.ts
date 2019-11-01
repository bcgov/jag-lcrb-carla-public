import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-product-inventory-packaged',
  templateUrl: './product-inventory-packaged.component.html',
  styleUrls: ['./product-inventory-packaged.component.scss']
})
export class ProductInventoryPackagedComponent implements OnInit {
  @Input() displayedColumns = [
    'rowLabel', 'seeds', 'vegetativeCannabisPlants', 'freshCannabis',
    'driedCannabis', 'ediblesSolids', 'ediblesNonSolids', 'inhaledExtracts',
    'ingestedExtracts', 'otherExtracts', 'topicals', 'other'
  ];
  @Input() columns = [
    'product',
    'openingInventory',
    'domesticAdditions',
    'returnsAdditions',
    'otherAdditions',
    'domesticReductions',
    'returnsReductions',
    'destroyedReductions',
    'lostReductions',
    'otherReductions',
    'closingValue',
    'closingWeight'
  ];
  @Input() dataSource;
    // {
    //   rowLabel: 'Seeds (Packaged units)',
    //   openingInventory: 0,
    //   domesticAdditions: 2,
    //   returnsAdditions: 3,
    //   otherAdditions: 4,
    //   domesticReductions: 1,
    //   returnsReductions: 2,
    //   destroyedReductions: 3,
    //   lostReductions: 0,
    //   otherReductions: 1,
    //   closingValue: 100,
    //   closingWeight: 0.45
    // },
    // {rowLabel: 'Quantity received - domestic', value: 2},
    // {rowLabel: 'Quantity received - returns', value: 1},
    // {rowLabel: 'Other additions to inventory', value: 1},
    // {rowLabel: 'Quantity shipped - domestic', value: 2},
    // {rowLabel: 'Quantity shipped - returned', value: 1},
    // {rowLabel: 'Quantity destroyed', value: 1},
    // {rowLabel: 'Quantity lost/stolen', value: 2},
    // {rowLabel: 'Other reductions to inventory', value: 1},
    // {rowLabel: 'Value of closing inventory ($)', value: 1},
    // {rowLabel: 'Weight of closing inventory (kg)', value: 1}
  // ];
  @Input() showTotals = false;
  @Input() totalsLabel = 'Total';

  constructor() { }

  ngOnInit() {
    console.log(this.dataSource);
  }

  getRowTotal(colName: string) {
    return this.dataSource.map(t => (t[colName] || 0)).reduce((acc, value) => acc + value, 0);
  }

}
