export interface SepApplication {
    tempJobNumber: string;
    dateCreated: Date;
    lastUpdated: Date;
    eventName: string;
    applicantInfo: any;
    agreeToTnC: boolean;
    dateAgreedToTnC: Date;
    stepCompleted: number;
    status: string;
}