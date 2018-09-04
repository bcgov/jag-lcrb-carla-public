import { DynamicsAccount } from "./dynamics-account.model";
import { Worker } from "./worker.model";

export interface Alias {
    id: string;
    firstname: string;
    middlename: string;
    lastname: string;
    worker: Worker;
    contact: DynamicsAccount;
}