import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { formatDate } from '@components/applications/tied-house-decleration/tide-house-utils';
import { faPlus } from '@fortawesome/free-solid-svg-icons';
import {
  RelationshipTypes,
  TiedHouseConnection,
  TiedHouseStatusCode,
  TiedHouseViewMode
} from '@models/tied-house-connection.model';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { GenericMessageDialogComponent } from '@shared/components/dialog/generic-message-dialog/generic-message-dialog.component';
import { FormBase } from '@shared/form-base';

const NEW_DECLARATION_KEY = 'New Declaration';

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

  constructor(
    private tiedHouseService: TiedHouseConnectionsDataService,
    private matDialog: MatDialog
  ) {
    super();
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.tiedHouseService.getAllTiedHouses(this.applicationId).subscribe({
      next: (data) => {
        this.tiedHouseDeclarations = data;
        this.tiedHouseDeclarations.forEach((item) => {
          if (item.statusCode == TiedHouseStatusCode.new && !item.markedForRemoval) {
            item.viewMode = TiedHouseViewMode.disabled;
          } else {
            item.viewMode = TiedHouseViewMode.existing;
          }
        });
        this.updateGroupedTiedHouseDeclarations();
      },
      error: (error) => {
        console.error('Error loading tied house data', error);
        this.matDialog.open(GenericMessageDialogComponent, {
          data: {
            title: 'Error Loading Tied House Form Data',
            message:
              'Failed to load tied house form data. Please try again. If the problem persists, please contact support.',
            closeButtonText: 'Close'
          }
        });
      }
    });
  }

  get flatDeclarations(): TiedHouseConnection[] {
    return this.groupedTiedHouseDeclarations.reduce((acc, [_, declarations]) => acc.concat(declarations), []);
  }

  /**
   * Add a new Tied House declaration.
   *
   */
  addNewTiedHouse() {
    var newTiedHouseDeclaration = new TiedHouseConnection();
    newTiedHouseDeclaration.isLegalEntity = false;
    newTiedHouseDeclaration.viewMode = TiedHouseViewMode.new;

    this.tiedHouseDeclarations.push(newTiedHouseDeclaration);
    this.updateGroupedTiedHouseDeclarations();

    this.openPanel();
  }

  /**
   * Add a new Tied House declaration, which is related to an existing declaration.
   *
   * @param {TiedHouseConnection} relatedDeclaration
   * @param {number} groupedTiedHouseDeclarationsIndex
   */
  addNewTiedHouseRelationship(relatedDeclaration: TiedHouseConnection, groupedTiedHouseDeclarationsIndex: number) {
    var newTiedHouseDeclaration = new TiedHouseConnection();
    newTiedHouseDeclaration.isLegalEntity = false;
    newTiedHouseDeclaration.viewMode = TiedHouseViewMode.addNewRelationship;

    if (relatedDeclaration) {
      // Update the new declaration with the related declaration's details
      newTiedHouseDeclaration.firstName = relatedDeclaration.firstName;
      newTiedHouseDeclaration.lastName = relatedDeclaration.lastName;
      newTiedHouseDeclaration.middleName = relatedDeclaration.middleName;
      newTiedHouseDeclaration.dateOfBirth = relatedDeclaration.dateOfBirth;
      newTiedHouseDeclaration.isLegalEntity = relatedDeclaration.isLegalEntity;
    }

    this.tiedHouseDeclarations.push(newTiedHouseDeclaration);
    this.updateGroupedTiedHouseDeclarations();

    this.openPanel(groupedTiedHouseDeclarationsIndex);
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

  /**
   * Saves/updates a Tied House declaration.
   *
   * @param {TiedHouseConnection} updated
   * @param {TiedHouseConnection} original
   */
  saveTiedHouseDeclaration(updated: TiedHouseConnection, original: TiedHouseConnection) {
    updated.viewMode = TiedHouseViewMode.disabled;
    Object.assign(original, updated);
    this.updateGroupedTiedHouseDeclarations();
  }

  /**
   * Removes a new (non-existing) Tied House declaration.
   *
   * @param {TiedHouseConnection} declaration
   * @param {boolean} keepAccordionOpen
   * @param {number} accordionIndex
   */
  removeNewTiedHouseDeclaration(declaration: TiedHouseConnection, keepAccordionOpen: boolean, accordionIndex: number) {
    declaration.viewMode = TiedHouseViewMode.hidden;
    declaration.markedForRemoval = true;

    if (keepAccordionOpen) {
      this.openPanel(accordionIndex);
    }

    this.updateGroupedTiedHouseDeclarations();
  }

  /**
   * Marks an existing Tied House declaration for removal.
   * Does nothing if the declaration is already marked for removal.
   *
   * @param {TiedHouseConnection} declaration
   * @return {*}
   */
  removeExistingTiedHouseDeclaration(declaration: TiedHouseConnection) {
    if (declaration.markedForRemoval === true) {
      // Already marked for removal, nothing to do
      return;
    }

    declaration.statusCode = TiedHouseStatusCode.new;
    declaration.markedForRemoval = true;
  }

  /**
   * Unmarks an existing Tied House declaration for removal.
   * Does nothing if the declaration is not marked for removal.
   *
   * @param {TiedHouseConnection} declaration
   * @return {*}
   */
  undoRemoveExistingTiedHouseDeclaration(declaration: TiedHouseConnection) {
    if (declaration.markedForRemoval === false) {
      // Already not marked for removal, nothing to do
      return;
    }

    declaration.statusCode = TiedHouseStatusCode.new;
    declaration.markedForRemoval = false;
  }

  hasExistingDeclarations(group: TiedHouseConnection[]): boolean {
    return group.find((item) => item.viewMode == TiedHouseViewMode.existing) != undefined;
  }

  /**
   * Open the panel for a specific index or the last index of `groupedTiedHouseDeclarations` if no index is provided.
   *
   * @param {number} [index]
   */
  openPanel(index?: number) {
    this.openedPanelIndex = index ?? this.groupedTiedHouseDeclarations.length - 1;
  }

  getRelationshipName(value: number): string | undefined {
    return RelationshipTypes.find((item) => item.value == value)?.name;
  }

  doesGroupHaveAtLeastOneDeclaration(groupIndex: number): boolean {
    const group = this.groupedTiedHouseDeclarations[groupIndex];

    if (!group) {
      return false;
    }

    return group[1].some((declaration) => declaration.viewMode != TiedHouseViewMode.hidden);
  }

  private getLegalEntityKey(declaration: TiedHouseConnection): string {
    if (!declaration.legalEntityName) {
      return NEW_DECLARATION_KEY;
    }

    return declaration.legalEntityName;
  }

  private getIndividualKey(declaration: TiedHouseConnection): string {
    if (!declaration.firstName || !declaration.dateOfBirth) {
      return NEW_DECLARATION_KEY;
    }

    return `${declaration.firstName} ${declaration.middleName || ''} ${declaration.lastName} - ${formatDate(declaration.dateOfBirth)}`;
  }

  private getGroupedTiedHouseKey(declaration: TiedHouseConnection): string {
    if (declaration.isLegalEntity) {
      return this.getLegalEntityKey(declaration);
    }

    return this.getIndividualKey(declaration);
  }

  private updateGroupedTiedHouseDeclarations() {
    const grouped = this.tiedHouseDeclarations
      .filter((item) => item.viewMode != TiedHouseViewMode.hidden)
      ?.reduce(
        (acc, declaration) => {
          var key = this.getGroupedTiedHouseKey(declaration);

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
}
