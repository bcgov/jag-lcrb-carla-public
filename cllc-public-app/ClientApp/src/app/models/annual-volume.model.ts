export interface AnnualVolume {
    id?: string;
    applicationId?: string;
    licenceId?: string;
    calendarYear: string;
    volumeProduced?: number;
    volumeDestroyed?: number;
}