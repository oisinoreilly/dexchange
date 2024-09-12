import * as React from 'react';
import { Label } from 'react-bootstrap';
import StatusMapping from '../typeMappings';
export const StatusLabel = ({ status, contentExists }) => {
    const colourToLabelType = new Map([
        ['green', 'success'],
        ['amber', 'warning'],
        ['red', 'danger']
    ]);
    const statusColour = StatusMapping.get(status.Status).statusColour;
    const displayTextForStatus = StatusMapping.get(status.Status).displayText;
    const statusLabel = contentExists
        ? <Label className='__statusLabel' bsStyle={colourToLabelType.get(statusColour)}>{displayTextForStatus}</Label>
        : <Label className='__statusLabel' bsStyle="danger">Document Missing</Label>;
    return statusLabel;
};
//# sourceMappingURL=StatusLabel.jsx.map