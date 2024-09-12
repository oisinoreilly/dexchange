import * as React from 'react';
import { Glyphicon } from 'react-bootstrap';
import StatusMapping from '../typeMappings';
import { ColumnItem } from './ColumnItem';
export var ColumnItemTypeEnum;
(function (ColumnItemTypeEnum) {
    ColumnItemTypeEnum[ColumnItemTypeEnum["Bank"] = 1] = "Bank";
    ColumnItemTypeEnum[ColumnItemTypeEnum["Corporate"] = 2] = "Corporate";
    ColumnItemTypeEnum[ColumnItemTypeEnum["Account"] = 3] = "Account";
    ColumnItemTypeEnum[ColumnItemTypeEnum["Document"] = 4] = "Document";
})(ColumnItemTypeEnum || (ColumnItemTypeEnum = {}));
export class ColumnItemWithStatus extends React.Component {
    render() {
        const customIconExists = this.props.icon && this.props.icon != "";
        const itemIcon = customIconExists
            ? <div className="column_item_icon"><img alt="Image" src={this.props.icon}/></div>
            : <Glyphicon glyph="list-alt" className='column_item_icon'/>;
        const getCssColour = () => {
            if (this.props.status == null || this.props.status.Status == null)
                return "";
            return StatusMapping.get(this.props.status.Status).statusColour;
        };
        return <ColumnItem {...this.props} icon={itemIcon} customClassName={"status " + getCssColour()}/>;
    }
}
//# sourceMappingURL=ColumnItemWithStatus.jsx.map