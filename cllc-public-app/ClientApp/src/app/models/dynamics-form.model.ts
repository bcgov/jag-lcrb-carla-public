import { DynamicsFormSection } from './dynamics-form-section.model';
import { DynamicsFormTab } from './dynamics-form-tab.model';

export class DynamicsForm {
  id: string;
  name: string;
  label: string;

  displayname: string;
  entity: string;
  tabs: DynamicsFormTab[];
  sections: DynamicsFormSection[];

  constructor() {}
}
