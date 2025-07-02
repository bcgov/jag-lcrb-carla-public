import { Component, OnInit } from '@angular/core';
import { TiedHouseViewMode, TiedHouseDeclaration, TiedHouseTypeEnum } from '@models/tied-house-relationships.model';
import { faPlus } from "@fortawesome/free-solid-svg-icons";


@Component({
  selector: 'app-tied-house-declaration',
  templateUrl: './tied-house-declaration.component.html',
  styleUrls: ['./tied-house-declaration.component.scss']
})
export class TiedHouseDeclarationComponent implements OnInit {
  tiedHouseRelationships = [{ name: "Father", id: 1 }, { name: "Other", id: 2 }]
  tiedHouseDeclarationstoAdd: TiedHouseDeclaration[] = [];
  tiedHouseDeclarations: [string, TiedHouseDeclaration[]][] = [];
  TiedHouseViewMode = TiedHouseViewMode;
  openedPanelIndex: number | null = null;
  faPlus = faPlus;


  constructor() { }

  ngOnInit(): void {

  }
  addNewTiedHouse(viewMode: TiedHouseViewMode, declaration?: TiedHouseDeclaration, index?: number) {
    var newTiedHouseDeclaration = new TiedHouseDeclaration();
    if (declaration) {
      newTiedHouseDeclaration.firstName = declaration.firstName;
      newTiedHouseDeclaration.lastName = declaration.lastName;
      newTiedHouseDeclaration.middleName = declaration.middleName;
      newTiedHouseDeclaration.dateOfBirth = declaration.dateOfBirth;
      newTiedHouseDeclaration.tiedHouseType = declaration.tiedHouseType;
    }
    newTiedHouseDeclaration.viewMode = viewMode;
    this.tiedHouseDeclarationstoAdd.push(newTiedHouseDeclaration);
    this.updateTiedHouseDeclarations();

    this.openPanel(index);
  }

  changeDeclarationViewMode(viewMode: TiedHouseViewMode, declaration: TiedHouseDeclaration) {
    declaration.viewMode = viewMode;
  }

  saveTiedHouseDeclaration(updated: TiedHouseDeclaration, original: TiedHouseDeclaration) {
    updated.viewMode = TiedHouseViewMode.disabled;
    Object.assign(original, updated);
    this.updateTiedHouseDeclarations();
  }

  removeTiedHouseDeclaration(declaration: TiedHouseDeclaration) {
    var index = this.tiedHouseDeclarationstoAdd.indexOf(declaration);
    if (index !== -1) {
      this.tiedHouseDeclarationstoAdd.splice(index, 1);
    }
    this.updateTiedHouseDeclarations();
  }

  removeExistingTiedHouseDeclaration(declaration: TiedHouseDeclaration, undo: boolean) {
    declaration.removeExistingLicense = undo;
  }

  hasExistingDeclarations(group: TiedHouseDeclaration[]): boolean {
    return group.find(d => d.viewMode == TiedHouseViewMode.table) != undefined;
  }

  openPanel(index?: number) {
    this.openedPanelIndex = index ?? this.tiedHouseDeclarations.length - 1;
  }


  private updateTiedHouseDeclarations() {
    const grouped = this.tiedHouseDeclarationstoAdd?.reduce((acc, declaration) => {
      var key = '';
      if (declaration.tiedHouseType == TiedHouseTypeEnum.Individual) {
        if (!declaration.firstName || !declaration.dateOfBirth) {
          key = "New Declaration"
        }
        else {
          key = `${declaration.firstName} -${declaration.dateOfBirth}`;
        }
      }

      if (!acc[key]) {
        acc[key] = [];
      }

      acc[key].push(declaration);
      return acc;
    }, {} as Record<string, TiedHouseDeclaration[]>);

    this.tiedHouseDeclarations = Object.entries(grouped);
  }
}