import { DynamicsFormTab } from "./dynamics-form-tab.model";
import { DynamicsFormSection } from "./dynamics-form-section.model";
export class DynamicsForm {
  id: string;
  name: string;
  displayname: string;
  entity: string;
  tabs: DynamicsFormTab[];
  sections: DynamicsFormSection[];
  constructor() { }
}
