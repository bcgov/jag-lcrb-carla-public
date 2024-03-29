import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { StarterChecklistComponent } from '@components/sep/starter-checklist/starter-checklist.component';
import { faCocktail, faCalculator, faCheck, faQuestion, faShoppingCart, faPencilAlt, faUserCircle } from '@fortawesome/free-solid-svg-icons';
import { User } from '@models/user.model';
import { DrinkPlannerDialog } from '../drink-planner/drink-planner.dialog';
import { LiquorTastingDialog} from '../liquor-tasting/liquor-tasting.dialog';

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

  @Input()
  currentUser: User;

  constructor(private dialog: MatDialog) { }

  ngOnInit(): void {
  }

  openChecklist() {
    const dialogConfig = {
      disableClose: true,
      autoFocus: true,
      width: '600px',
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

  openDrinkPlanner() {
    const dialogConfig = {
      disableClose: false,
      autoFocus: true,
      width: '800px',
      data: {}
    };

    // open dialog, get reference and process returned data from dialog
    const dialogRef = this.dialog.open(DrinkPlannerDialog, dialogConfig);
    dialogRef.afterClosed().subscribe(() => { });
  }

  openTastingPlanner() {
    const dialogConfig = {
      disableClose: false,
      autoFocus: true,
      width: '800px',
      data: {}
    };

    const dialogRef = this.dialog.open(LiquorTastingDialog, dialogConfig);
    dialogRef.afterClosed().subscribe(() => { });

  }
}
