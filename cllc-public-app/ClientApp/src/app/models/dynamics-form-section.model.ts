import { DynamicsFormField } from "./dynamics-form-field.model";
export class DynamicsFormSection {
  id: string;
  name: string;
  visible: boolean;
  showlabel: boolean;
  fields: DynamicsFormField[];   
  constructor() { }
}
