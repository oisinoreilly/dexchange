import * as React from 'react';
import { Navbar, Nav, NavItem, NavDropdown, MenuItem, Glyphicon, FormGroup, FormControl, Button, Media, Label } from 'react-bootstrap';
import { StatusEx, Status } from '../backendTypes';
import StatusMapping, { StatusColour } from '../typeMappings';
import { ColumnItem } from './ColumnItem';

export enum ColumnItemTypeEnum {
    Bank = 1,
    Corporate,
    Account,
    Document
}
export interface Props{
    itemType: ColumnItemTypeEnum;
    icon: string;
    title: string;
    subtitle?: string;
    status: StatusEx;
    selected: boolean;
    onClick: () => void;
    rightContent?: JSX.Element;
}
export class ColumnItemWithStatus extends React.Component<Props, null> {

    public render() {

        const customIconExists = this.props.icon && this.props.icon != ""; 
        const itemIcon = customIconExists
            ? <div className="column_item_icon"><img alt="Image" src={this.props.icon} /></div>
            : <Glyphicon glyph="list-alt" className='column_item_icon' />
       
        const getCssColour = () => {
            if (this.props.status == null || this.props.status.Status == null)
                return "";

            return StatusMapping.get(this.props.status.Status).statusColour;
        }

        return <ColumnItem {...this.props}
            icon={itemIcon}
            customClassName={"status " + getCssColour()} />
    }
}
