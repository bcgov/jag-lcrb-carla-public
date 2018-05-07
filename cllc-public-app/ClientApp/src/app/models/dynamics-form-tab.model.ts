import { DynamicsFormSection } from "./dynamics-form-section.model";
export class DynamicsFormTab {
  id: string;
  name: string;
  visible: boolean;
  showlabel: boolean;
  sections: DynamicsFormSection[];   
  constructor() { }
}
