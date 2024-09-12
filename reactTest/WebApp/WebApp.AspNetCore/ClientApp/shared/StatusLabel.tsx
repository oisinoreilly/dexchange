
import * as React from 'react';
import { Label } from 'react-bootstrap';
import StatusMapping, { StatusColour } from '../typeMappings';
import { StatusEx } from '../backendTypes';

interface StatusLabelProps {
    status: StatusEx;
    contentExists: boolean;
}

export const StatusLabel: React.SFC<StatusLabelProps> = ({ status, contentExists }) => {
    const colourToLabelType =
        new Map<StatusColour, string>([
            ['green', 'success'],
            ['amber', 'warning'],
            ['red', 'danger']
        ]);

    const statusColour: StatusColour = StatusMapping.get(status.Status).statusColour;
    const displayTextForStatus = StatusMapping.get(status.Status).displayText;

    const statusLabel = contentExists
        ? <Label className='__statusLabel' bsStyle={colourToLabelType.get(statusColour)}>{displayTextForStatus}</Label >
        : <Label className='__statusLabel' bsStyle="danger">Document Missing</Label >;
    return statusLabel;
}

