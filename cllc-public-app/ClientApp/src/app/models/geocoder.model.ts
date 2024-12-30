// Define the Fault interface
export interface Fault {
    value: string;
    element: string;
    fault: string;
    penalty: number;
}

// Define the Geometry interface
export interface Geometry {
    type: string; // e.g., "Point"
    crs: {
        type: string; // e.g., "EPSG"
        properties: {
            code: number; // e.g., 4326
        };
    };
    coordinates: number[]; // e.g., [-116.9299298, 50.3558064]
}

// Define the Properties interface
export interface Properties {
    fullAddress: string;
    score: number;
    matchPrecision: string;
    precisionPoints: number;
    faults: Fault[];
    siteName: string;
    unitDesignator: string;
    unitNumber: string;
    unitNumberSuffix: string;
    civicNumber: string;
    civicNumberSuffix: string;
    streetName: string;
    streetType: string;
    isStreetTypePrefix: string;
    streetDirection: string;
    isStreetDirectionPrefix: string;
    streetQualifier: string;
    localityName: string;
    localityType: string;
    electoralArea: string;
    provinceCode: string;
    locationPositionalAccuracy: string;
    locationDescriptor: string;
    siteID: string;
    blockID: string;
    fullSiteDescriptor: string;
    accessNotes: string;
    siteStatus: string;
    siteRetireDate: string;
    changeDate: string;
    isOfficial: string;
}

// Define the Feature interface
export interface Feature {
    type: string; // e.g., "Feature"
    geometry: Geometry;
    properties: Properties;
}

// Define the CRS interface
export interface CRS {
    type: string; // e.g., "EPSG"
    properties: {
        code: number; // e.g., 4326
    };
}

// Define the main Response interface
export interface GeocoderModel {
    type: string; // e.g., "FeatureCollection"
    queryAddress: string;
    searchTimestamp: string; // ISO 8601 format
    executionTime: number; // in seconds
    version: string;
    baseDataDate: string; // ISO 8601 format
    crs: CRS;
    interpolation: string;
    echo: string;
    locationDescriptor: string;
    setBack: number;
    minScore: number;
    maxResults: number;
    disclaimer: string;
    privacyStatement: string;
    copyrightNotice: string;
    copyrightLicense: string;
    features: Feature[];
}