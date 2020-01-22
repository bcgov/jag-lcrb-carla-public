import { Account } from './account.model';
import { Worker } from './worker.model';
import { Contact } from './contact.model';

export interface Alias {
    id: string;
    firstname: string;
    middlename: string;
    lastname: string;
    worker: Worker;
    contact:  Contact;
}
