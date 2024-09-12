import * as React from 'react';
import { Checkbox, Glyphicon } from 'react-bootstrap';
import { AccessRightPicker } from './AccessRights';
import { Resource, Permission, Access } from '../../backendTypes';
import  NodeList from './NodeList';
import './Node.scss';


interface NodeState {
    checked: boolean;
    loaded: boolean;
    children: IChild[];
}

interface IChild {
    title: string;
    id: string;
    parentId?: string;
}

interface NodeProps {
    title: string;
    type: Resource;
    getChildren?: Function;
    id: string;
    onSelect: Function;
    onUnSelect: Function;
    onChangeAccessRights: Function;
}

export class Node extends React.Component<NodeProps, NodeState>{

    state = {
        checked: false,
        loaded: false,
        children: [],
    }

    handleChange = (e) => {
        const { id, type, onSelect, onUnSelect } = this.props;

        const children = this.props.getChildren
            ? this.props.getChildren(this.props.id)
            : [];

        this.setState({
            checked: e.target.checked,
            children
        });

        const permission: Permission = {
            Id: "",
            ResourceId: id,
            ResourceType: type,
            Access: []
        };
        e.target.checked ? onSelect(permission) : onUnSelect(permission);
    }

    render() {
        return (
            <li>
                <div className='node-item'>
                    <Checkbox className='checkbox' inline onChange={this.handleChange}>
                        {this.props.title}
                    </Checkbox>
                    {this.state.checked &&
                        <AccessRightPicker onChange={this.props.onChangeAccessRights} id={this.props.id}/>
                    }
                </div>
                {this.state.checked &&
                        <NodeList
                            type="Account"
                            addPermission={this.props.onSelect}
                            removePermission={this.props.onUnSelect}
                            changeAccessRights={this.props.onChangeAccessRights}
                            topLevelItems={this.state.children}
                            getChildren={this.props.getChildren}
                        />}
            </li>
        );
    }
}

