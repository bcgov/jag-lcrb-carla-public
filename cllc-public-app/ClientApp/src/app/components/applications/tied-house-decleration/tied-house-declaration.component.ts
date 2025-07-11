import { Component, Input, OnInit } from '@angular/core';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import {
  RelationshipTypes,
  TiedHouseConnection,
  TiedHouseStatusCode,
  TiedHouseViewMode
} from '@models/tied-house-connection.model';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { FormBase } from '@shared/form-base';

@Component({
  selector: 'app-tied-house-declaration',
  templateUrl: './tied-house-declaration.component.html',
  styleUrls: ['./tied-house-declaration.component.scss']
})
export class TiedHouseDeclarationComponent extends FormBase implements OnInit {
  @Input() applicationId: string;
  tiedHouseConnections: TiedHouseConnection[];

  tiedHouseDeclarations: TiedHouseConnection[] = [];
  groupedTiedHouseDeclarations: [string, TiedHouseConnection[]][] = [];
  TiedHouseViewMode = TiedHouseViewMode;
  openedPanelIndex: number | null = null;
  faPlus = faPlus;

  get flatDeclarations(): TiedHouseConnection[] {
    return this.groupedTiedHouseDeclarations.reduce((acc, [_, declarations]) => acc.concat(declarations), []);
  }

  constructor(private tiedHouseService: TiedHouseConnectionsDataService) {
    super();
  }

  ngOnInit(): void {
    this.loadData();
  }

  addNewTiedHouse(viewMode: TiedHouseViewMode, declaration?: TiedHouseConnection, index?: number) {
    var newTiedHouseDeclaration = new TiedHouseConnection();
    newTiedHouseDeclaration.isLegalEntity = false;
    if (declaration) {
      newTiedHouseDeclaration.firstName = declaration.firstName;
      newTiedHouseDeclaration.lastName = declaration.lastName;
      newTiedHouseDeclaration.middleName = declaration.middleName;
      newTiedHouseDeclaration.dateOfBirth = declaration.dateOfBirth;
      newTiedHouseDeclaration.isLegalEntity = declaration.isLegalEntity;
    }
    newTiedHouseDeclaration.viewMode = viewMode;
    this.tiedHouseDeclarations.push(newTiedHouseDeclaration);
    this.updateTiedHouseDeclarations();

    this.openPanel(index);
  }

  changeDeclarationViewMode(viewMode: TiedHouseViewMode, declaration: TiedHouseConnection, isNew?: boolean) {
    declaration.viewMode = viewMode;
    if (isNew === true) {
      declaration.statusCode = TiedHouseStatusCode.new;
    }
    if (isNew === false) {
      declaration.statusCode = TiedHouseStatusCode.existing;
    }
  }

  saveTiedHouseDeclaration(updated: TiedHouseConnection, original: TiedHouseConnection) {
    updated.viewMode = TiedHouseViewMode.disabled;
    Object.assign(original, updated);
    this.updateTiedHouseDeclarations();
  }

  removeTiedHouseDeclaration(declaration: TiedHouseConnection, keepAccordionOpen: boolean, accordionIndex: number) {
    declaration.viewMode = TiedHouseViewMode.hidden;
    declaration.markedForRemoval = true;
    if(keepAccordionOpen){
      this.openPanel(accordionIndex);
    }
    this.updateTiedHouseDeclarations();
  }

  removeExistingTiedHouseDeclaration(declaration: TiedHouseConnection, remove: boolean) {
    declaration.statusCode = TiedHouseStatusCode.new;
    declaration.markedForRemoval = remove;
  }

  hasExistingDeclarations(group: TiedHouseConnection[]): boolean {
    return group.find((d) => d.viewMode == TiedHouseViewMode.existing) != undefined;
  }

  openPanel(index?: number) {
    this.openedPanelIndex = index ?? this.groupedTiedHouseDeclarations.length - 1;
  }

  loadData() {
    this.tiedHouseService.getAllTiedHouses(this.applicationId).subscribe((data) => {
      this.tiedHouseDeclarations = data;
      this.tiedHouseDeclarations.forEach((th) => {
        if (th.statusCode == TiedHouseStatusCode.new && !th.markedForRemoval ) {
          th.viewMode = TiedHouseViewMode.disabled;
        } else {
          th.viewMode = TiedHouseViewMode.existing;
        }
      });
      this.updateTiedHouseDeclarations();
    });
  }

  getRelationshipName(value: number) {
    return RelationshipTypes.find((o) => o.value == value).name;
  }

  private updateTiedHouseDeclarations() {
    const grouped = this.tiedHouseDeclarations.filter(th=> th.viewMode != TiedHouseViewMode.hidden)?.reduce(
      (acc, declaration) => {
        var key = '';
        if (declaration.isLegalEntity == false) {
          if (!declaration.firstName || !declaration.dateOfBirth) {
            key = 'New Declaration';
          } else {
            key = `${declaration.firstName} ${declaration.middleName} ${declaration.lastName} -${this.formatDate(declaration.dateOfBirth)}`;
          }
        } else {
          if (!declaration.legalEntityName) {
            key = 'New Declaration';
          } else {
            key = `${declaration.legalEntityName}`;
          }
        }

        if (!acc[key]) {
          acc[key] = [];
        }

        acc[key].push(declaration);
        return acc;
      },
      {} as Record<string, TiedHouseConnection[]>
    );

    this.groupedTiedHouseDeclarations = Object.entries(grouped);
  }

  private formatDate(dateString: string | null | undefined): string | null {
    if (!dateString?.trim()) return null;

    const date = new Date(dateString);
    return isNaN(date.getTime()) ? null : date.toISOString().split('T')[0];
  }
}
