import { Component, OnInit } from '@angular/core';
import { TiedHouseViewMode, TiedHouseDeclaration } from '@models/tied-house-relationships.model';
import { faPlus } from "@fortawesome/free-solid-svg-icons";


@Component({
  selector: 'app-tied-house-declaration',
  templateUrl: './tied-house-declaration.component.html',
  styleUrls: ['./tied-house-declaration.component.scss']
})
export class TiedHouseDeclarationComponent implements OnInit {
  tiedHouseTypes = [{ name: "Indivual", id: 1 }, { name: "Legal Entity", id: 2 }]
  tiedHouseRelationships = [{ name: "Father", id: 1 }, { name: "Other", id: 2 }]
  tiedHouseDeclarationstoAdd: TiedHouseDeclaration[] = [];
  tiedHouseDeclarations: [string, TiedHouseDeclaration[]][] = [];
  TiedHouseViewMode = TiedHouseViewMode;
  faPlus = faPlus;


  constructor() { }

  ngOnInit(): void {

  }
  addNewTiedHouse(viewMode: TiedHouseViewMode, declaration?: TiedHouseDeclaration ) {
    var newTiedHouseDeclaration = new TiedHouseDeclaration();
    if(declaration){
      newTiedHouseDeclaration.firstName = declaration.firstName;
      newTiedHouseDeclaration.lastName = declaration.lastName;
      newTiedHouseDeclaration.middleName = declaration.middleName;
      newTiedHouseDeclaration.dateOfBirth = declaration.dateOfBirth;
      newTiedHouseDeclaration.tiedHouseType = declaration.tiedHouseType;
    }
    newTiedHouseDeclaration.viewMode = viewMode;
    this.tiedHouseDeclarationstoAdd.push(newTiedHouseDeclaration);
    this.updateTiedHouseDeclarations()
  }

  changeDeclarationViewMode(viewMode: TiedHouseViewMode, declaration: TiedHouseDeclaration ){
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

  private updateTiedHouseDeclarations() {
    const grouped = this.tiedHouseDeclarationstoAdd?.reduce((acc, declaration) => {
      const key = `${declaration.firstName} -${declaration.dateOfBirth}`;

      if (!acc[key]) {
        acc[key] = [];
      }

      acc[key].push(declaration);
      return acc;
    }, {} as Record<string, TiedHouseDeclaration[]>);

    this.tiedHouseDeclarations = Object.entries(grouped);
  }
}