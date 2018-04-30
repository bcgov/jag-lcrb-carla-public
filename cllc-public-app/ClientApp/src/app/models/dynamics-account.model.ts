import { DynamicsFormTab } from "./dynamics-form-tab.model";
export class DynamicsForm {
  id: string;
  name: string;
  displayname: string;
  entity: string;
  tabs: DynamicsFormTab[];
  
  constructor() { }
}
