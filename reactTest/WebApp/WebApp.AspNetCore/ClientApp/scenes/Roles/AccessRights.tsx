import * as React from 'react';
import { Checkbox, Glyphicon } from 'react-bootstrap';
import { Access } from '../../backendTypes';
import './AccessRights.scss';


export const AccessRightPicker: React.SFC<{ onChange: Function, id: string }> = ({ onChange, id }) => {
    return <span>
        <AccessIcon icon='eye-open' access='Read' onChange={onChange} resourceId={id}/>
        <AccessIcon icon='pencil' access='Write' onChange={onChange} resourceId={id} />
        <AccessIcon icon='trash ' access='Delete' onChange={onChange} resourceId={id} />
    </span>
}

interface AccessIconProps {
    icon: string;
    access: Access;
    onChange: Function;
    resourceId: string;
}

export class AccessIcon extends React.Component<AccessIconProps, { selected: boolean }> {
    constructor(props) {
        super(props);
        this.state = {
            selected: false
        };
    }

    handleClick = () => {
        this.props.onChange(this.props.access, !this.state.selected, this.props.resourceId)

        this.setState({
            selected: !this.state.selected
        });
    }

    render() {
        const { icon } = this.props;
        const { selected } = this.state;
        const className = selected ? 'access-right selected' : 'access-right' ;
        
        return <span onClick={this.handleClick}
            className={className}>
            <Glyphicon glyph={icon} />
        </span>;
    }
}