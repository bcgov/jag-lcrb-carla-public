import { DynamicsFormField } from "./dynamics-form-field.model";

export class DynamicsFormSection {
  id: string;
  name: string;
  label: string;
  visible: boolean;
  showlabel: boolean;
  fields: DynamicsFormField[];

  constructor() { }
}
