import * as React from 'react';
import CheckboxTree from 'react-checkbox-tree';
import { Col, Panel, Label, Button, ButtonGroup } from 'react-bootstrap';
import { withRedux, ReduxProps } from '../../shared/withReduxStore';
import { Permission, Access, Resource } from '../../backendTypes';
import { Entity } from '../../frontendTypes';
import { arrayUnique } from '../../Utils';
import './PermissionPicker.scss';


interface State {
    checked: any[];
    expanded: any[];
    nodes: any[];
    inheritPermissions: boolean;
    resourcesById: {};
}

interface OwnProps {
    topLevelItems: Entity[];
    type: Resource;
    getAccounts: Function;
    addPermissions: Function;
    permissionsToAdd: Permission[];
    createRole: () => void;
    cancel: () => void;
}

type Props = OwnProps & ReduxProps

export class PermissionPicker extends React.Component<Props, State>{
    state = {
        checked: [],
        expanded: [],
        nodes: [],
        inheritPermissions: true,
        resourcesById: {}
    };

    componentDidMount() {
        this.setResourcesAndNodes();
    }

    setResourcesAndNodes() {
        // contructor the list of nodes
        // create a keyed object to use to get information on a resource
        const resourcesById = {};
        const nodes = this.props.topLevelItems
            .map(entity => {
                resourcesById[entity.id] = { resourceType: this.props.type, resourceName: entity.title };
                return {
                    value: entity.id,
                    label: entity.title,
                    children: this.props.getAccounts(entity.id)
                        .map(account => {
                            resourcesById[account.id] = { resourceType: 'Account', resourceName: account.title };
                            return {
                                value: account.id,
                                label: account.title,
                            }
                        })
                }
            });

        this.setState({
            nodes,
            resourcesById
        });
    }

    onCheck = (checked: string[]) => {
        if (this.state.inheritPermissions) {
            this.props.topLevelItems.forEach(entity => {
                const parentIsChecked = checked.indexOf(entity.id) !== -1;
                const accountsForEntity = this.props.getAccounts(entity.id).map(account => account.id);
                if (parentIsChecked) { //check if bank/corp is selected
                    //in inherit mode select all accounts when parent is selected
                    checked = arrayUnique(checked.concat(accountsForEntity));
                }

            });
        }
        
        this.setState({ checked });
    }

    onExpand = (expanded) => {
        this.setState({ expanded })
    }

    handleInheritSelect = () => {
        this.setState({ inheritPermissions: !this.state.inheritPermissions })
    }

    addPermissions = (access: Access) => {
        const { checked, resourcesById } = this.state;
        const newPermissions = checked.map(value => {
            const permission = new Permission;
            permission.ResourceId = value;
            permission.ResourceType = resourcesById[value].resourceType;
            return permission;
        })
        //state is held in parent
        //send back the list of permissions on the tree and what access needs to be added
        this.props.addPermissions(newPermissions, access);
    }

    deselectAll = () => {
        this.setState({ checked: [] });
    }

    renderPermission = (p: Permission) => {
        const permission = this.state.resourcesById[p.ResourceId];
        const icons = p.Access.map(access => {
            if (access === 'Read') {
                return <i className="fa fa-eye acess-icon" aria-hidden="true"></i>
            }
            else if (access === 'Write') {
                return <i className="fa fa-pencil-square-o acess-icon" aria-hidden="true"></i>
            }
            else if (access === 'Delete') {
                return <i className="fa fa-trash acess-icon" aria-hidden="true"></i>
            }
            return null;
        });
        return <div>
            {icons}
            {permission.resourceName}
        </div>
    }

    render() {
        const { nodes, checked, expanded, inheritPermissions } = this.state;
        const checkedClassName = inheritPermissions
            ? 'rct-icon rct-icon-check'
            : 'rct-icon rct-icon-uncheck'

        return <div className='permission-picker --align-in-column'>
            <Panel>
                <div className='react-checkbox-tree cascade-permissions' title='Select to cascade permissions downward. When selected all documents on a selected account will be permissionsed'>
                    <input type='checkbox' />
                    <span className='rct-checkbox' onClick={this.handleInheritSelect} >
                        <span className={checkedClassName}>
                        </span>
                    </span>
                    <span className='rct-title'>Cascade Permissions</span>

                </div>
                <Button bsStyle='info' onClick={this.deselectAll}>Deselect All
                </Button>
                <ButtonGroup className='access-button-group'>
                    <Button bsStyle='info' onClick={() => this.addPermissions('Read')}>Add Read Access</Button>
                    <Button bsStyle='info' onClick={() => this.addPermissions('Write')}>Add Write Access</Button>
                    <Button bsStyle='info' onClick={() => this.addPermissions('Delete')}>Add Delete Access</Button>
                </ButtonGroup>
                <ButtonGroup>
                    <Button bsStyle='primary' onClick={this.props.createRole}>Create Role</Button>
                    <Button bsStyle='danger' onClick={this.props.cancel}>Cancel</Button>
                </ButtonGroup>
            </Panel>
            <div className='container'>
                <Panel header={<h2>Select resources to add to role</h2>}>
                    <CheckboxTree
                    nodes={nodes}
                    checked={checked}
                    expanded={expanded}
                    onCheck={this.onCheck}
                    onExpand={this.onExpand}
                    noCascade
                    optimisticToggle={false}
                    />
                </Panel>
                <Panel header={<h2>Resources to be included in new role</h2>}>
                    {this.props.permissionsToAdd.map(p => {
                        return this.renderPermission(p);
                })}
                </Panel>
            </div>
        </div>
    }
}

export default withRedux(PermissionPicker)