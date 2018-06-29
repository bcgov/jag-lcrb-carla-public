import { Component, OnInit, Input } from '@angular/core';
import { UserDataService } from '../../../services/user-data.service';
import { User } from '../../../models/user.model';

@Component({
  selector: 'app-organization-structure',
  templateUrl: './organization-structure.component.html',
  styleUrls: ['./organization-structure.component.scss']
})
export class OrganizationStructureComponent implements OnInit {
  @Input() accountId: string;
  @Input() businessType: string;

  constructor() { }

  ngOnInit() {

  }

}
