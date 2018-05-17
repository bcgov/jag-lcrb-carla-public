import { DynamicsContact } from "./dynamics-contact.model";

export class DynamicsAccount {
  id: string;
  name: string;
  description: string;
  primarycontact: DynamicsContact;
  
  constructor() { }
}
