import * as backendTypes from './backendTypes';

export interface Entity {
    id: string;
    title: string;
    icon: string;
    status: backendTypes.StatusEx;
    accounts: string[];
}

export interface Item {
    Name: string;
    Id: any;
}

export interface FieldMapping {
    propertyName: string;
    displayName: string;
    isLargeField?: boolean;
}
