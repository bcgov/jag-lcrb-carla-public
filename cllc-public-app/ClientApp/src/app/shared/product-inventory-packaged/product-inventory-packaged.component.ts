import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-product-inventory-packaged',
  templateUrl: './product-inventory-packaged.component.html',
  styleUrls: ['./product-inventory-packaged.component.scss']
})
export class ProductInventoryPackagedComponent implements OnInit {
  @Input() displayedColumns = [ 'rowLabel', 'seeds', 'vegetativeCannabisPlants', 'freshCannabis', 'driedCannabis', 'ediblesSolids',
    'ediblesNonSolids', 'inhaledExtracts', 'ingestedExtracts', 'otherExtracts', 'tropicals', 'other'];
  @Input() dataSource = [{rowLabel: 'Quantity received - domestic', seeds: 2},
  {rowLabel: 'Quantity received - returns', seeds: 1},
  {rowLabel: 'Other', seeds: 20},
];
  @Input() showTotals = false;
  @Input() totalsLabel = 'Total';

  constructor() { }

  ngOnInit() {
  }

  getRowTotal(colName: string) {
    return this.dataSource.map(t => (t[colName] || 0)).reduce((acc, value) => acc + value, 0);
  }

}
