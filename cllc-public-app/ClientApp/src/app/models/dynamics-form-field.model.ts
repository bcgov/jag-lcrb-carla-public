import { DynamicsFormFieldOption } from "./dynamics-form-field-option.model";

export class DynamicsFormField {
  classid: string;
  controltype: string;
  datafieldname: string;
  name: string;
  required: boolean;
  showlabel: boolean;
  visible: boolean;
  options: DynamicsFormFieldOption[];
  constructor() { }
}
