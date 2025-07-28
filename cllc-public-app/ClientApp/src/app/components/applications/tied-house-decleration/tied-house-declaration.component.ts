import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { formatDate } from '@components/applications/tied-house-decleration/tide-house-utils';
import { faPlusSquare } from '@fortawesome/free-regular-svg-icons';
import {
  RelationshipTypes,
  TiedHouseConnection,
  TiedHouseStatusCode,
  TiedHouseViewMode
} from '@models/tied-house-connection.model';
import { TiedHouseConnectionsDataService } from '@services/tied-house-connections-data.service';
import { GenericConfirmationDialogComponent } from '@shared/components/dialog/generic-confirmation-dialog/generic-confirmation-dialog.component';
import { GenericMessageDialogComponent } from '@shared/components/dialog/generic-message-dialog/generic-message-dialog.component';
import { FormBase } from '@shared/form-base';
import { Observable, of } from 'rxjs';
import { catchError, mergeMap, takeWhile } from 'rxjs/operators';

const NEW_DECLARATION_KEY = 'New Declaration';

@Component({
  selector: 'app-tied-house-declaration',
  templateUrl: './tied-house-declaration.component.html',
  styleUrls: ['./tied-house-declaration.component.scss']
})
export class TiedHouseDeclarationComponent extends FormBase implements OnInit {
  /**
   * Optional application ID used to fetch any existing tied house connections to initialize the component with.
   *
   * @type {string}
   */
  @Input() applicationId?: string;
  /**
   * Optional account ID used to fetch any existing tied house connections to initialize the component with.
   *
   * @type {string}
   */
  @Input() accountId?: string;
  /**
   * Optional set of initial tied house connections data to initialize the component with.
   * If provided, no API call will be made to fetch the tied house connections.
   *
   * @type {TiedHouseConnection[]}
   */
  @Input() initialTiedHouseConnections?: TiedHouseConnection[];
  /**
   * Indicates whether the tied house component is in read-only mode. Default is false.
   *
   * @type {boolean}
   */
  @Input() isReadOnly?: boolean = false;

  tiedHouseDeclarations: TiedHouseConnection[] = [];
  groupedTiedHouseDeclarations: [string, TiedHouseConnection[]][] = [];

  TiedHouseViewMode = TiedHouseViewMode;

  openedPanelIndex: number | null = null;

  faPlusSquare = faPlusSquare;

  constructor(
    private tiedHouseService: TiedHouseConnectionsDataService,
    private snackBar: MatSnackBar,
    private matDialog: MatDialog
  ) {
    super();
  }

  ngOnInit(): void {
    if (this.initialTiedHouseConnections) {
      // Use the provided tied house connections
      this.initTiedHouseDeclarations(this.initialTiedHouseConnections);
    } else {
      // Fetch the tied house connections
      this.loadData();
    }
  }

  loadData() {
    // If an application ID is provided, fetch tied house connections for that application.
    // Otherwise, fetch tied house connections for the current user.
    let request$ = null;

    if (this.applicationId) {
      request$ = this.tiedHouseService.GetAllLiquorTiedHouseConnectionsForApplication(this.applicationId);
    } else if (this.accountId) {
      request$ = this.tiedHouseService.GetAllLiquorTiedHouseConnectionsForUser(this.accountId);
    } else {
      this.matDialog.open(GenericMessageDialogComponent, {
        data: {
          title: 'Error Loading Tied House Form Data',
          message: 'No application ID or account ID provided to load Tied House data.',
          closeButtonText: 'Close'
        }
      });

      return;
    }

    request$.subscribe({
      next: (data) => {
        this.initTiedHouseDeclarations(data);
      },
      error: (error) => {
        console.error('Error loading Tied House data', error);
        this.matDialog.open(GenericMessageDialogComponent, {
          data: {
            title: 'Error Loading Tied House Form Data',
            message:
              'Failed to load Tied House form data. Please try again. If the problem persists, please contact support.',
            closeButtonText: 'Close'
          }
        });
      }
    });
  }

  initTiedHouseDeclarations(tiedHouseConnections: TiedHouseConnection[]) {
    this.tiedHouseDeclarations = tiedHouseConnections.map((item) => {
      if (item.statusCode === TiedHouseStatusCode.new && !item.markedForRemoval) {
        item.viewMode = TiedHouseViewMode.disabled;
      } else {
        item.viewMode = TiedHouseViewMode.existing;
      }
      return item;
    });

    this.updateGroupedTiedHouseDeclarations();
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
   * Add a new Tied House declaration, which is related to an existing declaration. A related declaration will
   * automatically inherit some of the details from the related declaration.
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
      newTiedHouseDeclaration.legalEntityName = relatedDeclaration.legalEntityName;
      newTiedHouseDeclaration.relationshipToLicence = relatedDeclaration.relationshipToLicence;
      newTiedHouseDeclaration.businessType = relatedDeclaration.businessType;
    }

    this.tiedHouseDeclarations.push(newTiedHouseDeclaration);
    this.updateGroupedTiedHouseDeclarations();

    this.openPanel(groupedTiedHouseDeclarationsIndex);
  }

  /**
   * Checks if any Tied House declarations are in the process of being added or edited.
   *
   * We want to limit the number of in-progress declarations to one at a time.
   *
   * This is to prevent too many in-progress form elements from being created at once, which is particularly important
   * for new related declarations, as they need to inherit some details from a completed declaration.
   *
   * @return {*}  {boolean}
   */
  hasInProgressDeclarations(): boolean {
    // Return true if there are any declarations that are in the process of being added or edited.
    return this.tiedHouseDeclarations.some((declaration) =>
      [TiedHouseViewMode.new, TiedHouseViewMode.addNewRelationship, TiedHouseViewMode.editExistingRecord].includes(
        declaration.viewMode
      )
    );
  }

  /**
   * Checks if a related group of Tied House declarations has been saved.
   *
   * This is necessary when determining if a new related declaration can be added, as a new related declaration
   * requires a completed declaration in the same group to inherit some details from.
   *
   * @param {[string, TiedHouseConnection[]]} groupedTiedHouseDeclarations
   * @return {*}  {boolean}
   */
  hasSavedRelatedDeclaration(groupedTiedHouseDeclarations: [string, TiedHouseConnection[]]): boolean {
    // Only new (unsaved) declarations will have the NEW_DECLARATION_KEY as the name of the group.
    return groupedTiedHouseDeclarations[0] !== NEW_DECLARATION_KEY;
  }

  /**
   * Changes the view mode of a Tied House declaration.
   * Additionally updates the status code, if `isNew` is provided.
   *
   * @param {TiedHouseViewMode} viewMode
   * @param {TiedHouseConnection} declaration
   * @param {boolean} [isNew] If provided, and set to `true`, the status code will be set to `TiedHouseStatusCode.new`.
   * If set to `false`, the status code will be set to `TiedHouseStatusCode.existing`. If not provided, the status code
   * will not be changed.
   */
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
  saveTiedHouseDeclaration(updated: TiedHouseConnection, original: TiedHouseConnection, groupIndex?: number) {
    updated.viewMode = TiedHouseViewMode.disabled;
    Object.assign(original, updated);

    this.submitTiedHouseDeclarationChange(updated);
    this.updateGroupedTiedHouseDeclarations();

    this.openPanel(groupIndex);
  }

  submitTiedHouseDeclarationChange(declaration: TiedHouseConnection) {
    this.save(declaration)
      .pipe(takeWhile(() => this.componentActive))
      .subscribe(([saveSucceeded]) => {
        if (!saveSucceeded) {
          this.snackBar.open('Error saving Application', 'Fail', { duration: 3500, panelClass: ['red-snackbar'] });
        }
      });
  }

  /**
   * Removes a new (non-existing) Tied House declaration.
   *
   * @param {TiedHouseConnection} declaration
   * @param {boolean} keepAccordionOpen
   * @param {number} accordionIndex
   */
  removeNewTiedHouseDeclaration(declaration: TiedHouseConnection, keepAccordionOpen: boolean, accordionIndex: number) {
    this.matDialog.open(GenericConfirmationDialogComponent, {
      disableClose: true,
      autoFocus: true,
      data: {
        title: 'Remove Tied House Connection',
        message: `Are you sure you want to remove this tied house declaration?`,
        confirmButtonText: 'Yes, Remove',
        cancelButtonText: 'No, Go Back',
        onConfirm: () => {
          //if removing existing declaration mark as removed
          if (declaration.supersededById) {
            declaration.viewMode = TiedHouseViewMode.existing;
            declaration.markedForRemoval = true;
            this.submitTiedHouseDeclarationChange(declaration);
          }
          //if declaration has not been saved to dynamics remove from declaration list
          else if (!declaration.applicationId) {
            this.tiedHouseDeclarations = this.tiedHouseDeclarations.filter((th) => th != declaration);
            this.updateGroupedTiedHouseDeclarations();
          }
          //else declaration is not an existing declaration but has been saved to dynamics so hide and call api to remove declaration from dynamics
          else {
            declaration.viewMode = TiedHouseViewMode.hidden;
            declaration.markedForRemoval = true;
            this.submitTiedHouseDeclarationChange(declaration);
            this.updateGroupedTiedHouseDeclarations();
          }

          if (keepAccordionOpen) {
            this.openPanel(accordionIndex);
          }
        }
      }
    });
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
    this.matDialog.open(GenericConfirmationDialogComponent, {
      disableClose: true,
      autoFocus: true,
      data: {
        title: 'Remove Tied House Connection',
        message: `Are you sure you want to remove this tied house declaration?`,
        confirmButtonText: 'Yes, Remove',
        cancelButtonText: 'No, Go Back',
        onConfirm: () => {
          declaration.statusCode = TiedHouseStatusCode.new;
          declaration.markedForRemoval = true;
          this.submitTiedHouseDeclarationChange(declaration);
        }
      }
    });
  }

  /**
   * Restores an existing Tied House declaration that was marked for removal.
   * Does nothing if the declaration is not marked for removal.
   *
   * @param {TiedHouseConnection} declaration
   * @return {*}
   */
  restoreExistingTiedHouseDeclaration(declaration: TiedHouseConnection) {
    if (declaration.markedForRemoval === false) {
      // Already not marked for removal, nothing to do
      return;
    }

    declaration.statusCode = TiedHouseStatusCode.existing;
    declaration.markedForRemoval = false;
    this.submitTiedHouseDeclarationChange(declaration);
  }

  /**
   * Given a group of Tied House declarations, checks if there are any declarations with
   * `viewMode === TiedHouseViewMode.existing`.
   *
   * @param {TiedHouseConnection[]} group
   * @return {*}  {boolean}
   */
  hasExistingDeclarations(group: TiedHouseConnection[]): boolean {
    return group.find((item) => item.viewMode === TiedHouseViewMode.existing) != undefined;
  }

  /**
   * Open the panel for a specific index or the last index of `groupedTiedHouseDeclarations` if no index is provided.
   *
   * @param {number} [index]
   */
  openPanel(index?: number) {
    this.openedPanelIndex = index ?? this.groupedTiedHouseDeclarations.length - 1;
  }

  /**
   * Give a `relationshipToLicence` value, returns the corresponding `RelationshipTypes` enum name.
   *
   * @param {number} value
   * @return {*}  {(string | undefined)}
   */
  getRelationshipName(value: number): string | undefined {
    return RelationshipTypes.find((item) => item.value === value)?.name;
  }

  /**
   * Given a group index, checks if that group has at least one declaration, that is not hidden.
   *
   * @param {number} groupIndex
   * @return {*}  {boolean}
   */
  doesGroupHaveAtLeastOneDeclaration(groupIndex: number): boolean {
    const group = this.groupedTiedHouseDeclarations[groupIndex];

    if (!group) {
      return false;
    }

    return group[1].some((declaration) => declaration.viewMode != TiedHouseViewMode.hidden);
  }

  /**
   * Given a Tied House declaration for a Legal Entity, returns a unique key.
   *
   * @param {TiedHouseConnection} declaration
   * @return {*}  {string}
   */
  private getLegalEntityKey(declaration: TiedHouseConnection): string {
    if (!declaration.legalEntityName) {
      return NEW_DECLARATION_KEY;
    }

    return declaration.legalEntityName;
  }

  /**
   * Given an Tied House declaration for an Individual, returns a unique key.
   *
   * @param {TiedHouseConnection} declaration
   * @return {*}  {string}
   */
  private getIndividualKey(declaration: TiedHouseConnection): string {
    if (!declaration.firstName || !declaration.dateOfBirth) {
      return NEW_DECLARATION_KEY;
    }

    return `${declaration.firstName} ${declaration.middleName || ''} ${declaration.lastName} - ${formatDate(declaration.dateOfBirth)}`;
  }

  /**
   * Given a Tied House declaration, returns a unique key.
   *
   * @param {TiedHouseConnection} declaration
   * @return {*}  {string}
   */
  getGroupedTiedHouseKey(declaration: TiedHouseConnection): string {
    if (declaration.isLegalEntity) {
      return this.getLegalEntityKey(declaration);
    }

    return this.getIndividualKey(declaration);
  }

  /**
   * Updates the `groupedTiedHouseDeclarations` property based on the current `tiedHouseDeclarations`.
   *
   */
  updateGroupedTiedHouseDeclarations() {
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

  private save(declaration: TiedHouseConnection): Observable<[boolean, TiedHouseConnection]> {
    let request$;
    if (this.applicationId) {
      request$ = this.tiedHouseService.createLiquorTiedHouseConnection(declaration, this.applicationId);
    } else if (this.accountId) {
      request$ = this.tiedHouseService.createLiquorTiedHouseConnectionForUser(declaration, this.accountId);
    } else {
      this.matDialog.open(GenericMessageDialogComponent, {
        data: {
          title: 'Error Saving Tied House Form Data',
          message: 'No application ID or account ID provided to save Tied House data.',
          closeButtonText: 'Close'
        }
      });

      return;
    }

    return request$
      .pipe(takeWhile(() => this.componentActive))
      .pipe(
        catchError(() => {
          this.snackBar.open('Error saving Tied House Declaration', 'Fail', {
            duration: 3500,
            panelClass: ['red-snackbar']
          });

          const res: [boolean, TiedHouseConnection] = [false, null];

          return of(res);
        })
      )
      .pipe(
        mergeMap((data) => {
          this.snackBar.open('Tied House Declaration has been saved', 'Success', {
            duration: 3500,
            panelClass: ['green-snackbar']
          });

          const res: [boolean, TiedHouseConnection] = [true, data as TiedHouseConnection];

          return of(res);
        })
      );
  }
}
