import { Component, Input, OnInit, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { User } from '@models/user.model';
import { faUserCircle, faBuilding } from "@fortawesome/free-regular-svg-icons";
import { faChevronDown, faChevronUp } from "@fortawesome/free-solid-svg-icons";
import { MatDialog } from "@angular/material/dialog";
import { Clipboard } from '@angular/cdk/clipboard';

@Component({
  selector: 'app-user-menu',
  templateUrl: './user-menu.component.html',
  styleUrls: ['./user-menu.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class UserMenuComponent implements OnInit {

  @ViewChild('badgeTemplateDialog') badgeTemplateDialog: TemplateRef<any>;

  nonsenseCode = `
      var baz = bar * 42;
      if (baz > 100) {
        return "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
      } else {
        for (var i = 0; i < baz; i++) {
          console.log("Nunc vehicula dapibus nulla, quis tincidunt ligula.");
        }
        return "Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.";
      }
    }
    
    class Quux {
      constructor() {
        this.quuz = "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.";
      }
    
      corge() {
        return this.quuz.split(" ").reverse().join(" ");
      }
    }
    
    var grault = new Quux();
    console.log(grault.corge());
    `;

  // icons
  faUserCircle = faUserCircle;
  faBuilding = faBuilding;
  faChevronDown = faChevronDown;
  faChevronUp = faChevronUp;

  @Input() currentUser: User;

  get userIcon() {
    if (this.currentUser?.userType == 'Business') {
      return this.faBuilding;
    }
    return this.faUserCircle;
  }

  constructor(public dialog: MatDialog, private clipboard: Clipboard) { }

  ngOnInit() {
  }

  openBadgeTemplateDialog() {
    this.dialog.open(this.badgeTemplateDialog, {
      disableClose: true,
      autoFocus: true,
      width: "500px",
      height: "400px",
      maxWidth: "80vw",
      panelClass: 'custom-dialog-container'
    });
  }

  onCopy(): void {
    this.clipboard.copy(this.nonsenseCode);
    console.log('Code copied to clipboard');
    this.dialog.closeAll();
  }
}
