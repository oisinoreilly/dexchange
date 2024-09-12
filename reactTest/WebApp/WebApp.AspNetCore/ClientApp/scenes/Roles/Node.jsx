import * as React from 'react';
import { Checkbox } from 'react-bootstrap';
import { AccessRightPicker } from './AccessRights';
import NodeList from './NodeList';
import './Node.scss';
export class Node extends React.Component {
    constructor() {
        super(...arguments);
        this.state = {
            checked: false,
            loaded: false,
            children: [],
        };
        this.handleChange = (e) => {
            const { id, type, onSelect, onUnSelect } = this.props;
            const children = this.props.getChildren
                ? this.props.getChildren(this.props.id)
                : [];
            this.setState({
                checked: e.target.checked,
                children
            });
            const permission = {
                Id: "",
                ResourceId: id,
                ResourceType: type,
                Access: []
            };
            e.target.checked ? onSelect(permission) : onUnSelect(permission);
        };
    }
    render() {
        return (<li>
                <div className='node-item'>
                    <Checkbox className='checkbox' inline onChange={this.handleChange}>
                        {this.props.title}
                    </Checkbox>
                    {this.state.checked &&
            <AccessRightPicker onChange={this.props.onChangeAccessRights} id={this.props.id}/>}
                </div>
                {this.state.checked &&
            <NodeList type="Account" addPermission={this.props.onSelect} removePermission={this.props.onUnSelect} changeAccessRights={this.props.onChangeAccessRights} topLevelItems={this.state.children} getChildren={this.props.getChildren}/>}
            </li>);
    }
}
//# sourceMappingURL=Node.jsx.map