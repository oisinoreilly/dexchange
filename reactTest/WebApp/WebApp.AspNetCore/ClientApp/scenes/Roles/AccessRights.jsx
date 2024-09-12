import * as React from 'react';
import { Glyphicon } from 'react-bootstrap';
import './AccessRights.scss';
export const AccessRightPicker = ({ onChange, id }) => {
    return <span>
        <AccessIcon icon='eye-open' access='Read' onChange={onChange} resourceId={id}/>
        <AccessIcon icon='pencil' access='Write' onChange={onChange} resourceId={id}/>
        <AccessIcon icon='trash ' access='Delete' onChange={onChange} resourceId={id}/>
    </span>;
};
export class AccessIcon extends React.Component {
    constructor(props) {
        super(props);
        this.handleClick = () => {
            this.props.onChange(this.props.access, !this.state.selected, this.props.resourceId);
            this.setState({
                selected: !this.state.selected
            });
        };
        this.state = {
            selected: false
        };
    }
    render() {
        const { icon } = this.props;
        const { selected } = this.state;
        const className = selected ? 'access-right selected' : 'access-right';
        return <span onClick={this.handleClick} className={className}>
            <Glyphicon glyph={icon}/>
        </span>;
    }
}
//# sourceMappingURL=AccessRights.jsx.map