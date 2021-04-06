import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { StarterChecklistComponent } from '@components/sep/starter-checklist/starter-checklist.component';
import { faCocktail, faCalculator, faCheck, faQuestion, faShoppingCart, faPencilAlt, faUserCircle } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-resources',
  templateUrl: './resources.component.html',
  styleUrls: ['./resources.component.scss']
})
export class ResourcesComponent implements OnInit {
  faCocktail = faCocktail;
  faCalculator = faCalculator;
  faCheck = faCheck;
  faQuestion = faQuestion;
  faShoppingCart = faShoppingCart;
  faUserCircle = faUserCircle;
  faPencilAlt = faPencilAlt;

  constructor(private dialog: MatDialog) { }

  ngOnInit(): void {
  }

  openChecklist() {
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: "500px",
      data: {
        showStartApp: false
      }
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(StarterChecklistComponent, dialogConfig);
    dialogRef.afterClosed()
      .subscribe(cancelApplication => {

      });
  }

}
