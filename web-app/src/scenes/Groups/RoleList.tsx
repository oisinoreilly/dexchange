import * as React from 'react';
import { Button, Panel, Glyphicon } from 'react-bootstrap';

import { Item } from '../../frontendTypes';
import { ReduxProps, withRedux } from '../../shared/withReduxStore';
import AddToGroupModal from './AddToGroupModal';
import { ColumnItem } from '../../shared/ColumnItem';
import { ContextMenu } from '../../shared/ContextMenu';
import { RoleInfo } from '../../backendTypes';

type Props = ReduxProps;

interface State {
    showAddModal: boolean;
}

export class RoleList extends React.Component<Props, State>{
    state = { showAddModal: false }

    handleRemoveRoleFromGroup = (roleId) => {
        this.props.actions.removeRoleFromGroup(roleId, this.props.groups.selectedGroup.Id);
    }

    handleOpenAddModal = () => {
        this.setState({
            showAddModal: true
        })
    }

    handleHide = () => {
        this.setState({ showAddModal: false });
    }

    renderRoleItems(rolesInGroup: RoleInfo[]) {
        return rolesInGroup.map(role => {
            const options = [{ label: "Remove from Group", handler: () => this.handleRemoveRoleFromGroup(role.Id) }];
            const optionsMenu = <ContextMenu options={options} suppressEventPropagation={true} />

            return <ColumnItem title={role.Name}
                rightContent={optionsMenu}
                icon={<i className="fa fa-2x fa-key" aria-hidden="true"></i>} />
        });
    }

    render() {
        const selected = this.props.groups.selectedGroup.Roles;
        const roleInSelectedGroup = this.props.roles.rolesList
            .filter(role => selected && selected.indexOf(role.Id) !== -1);

        return <div className='admin-column --border-top' style={{ flexBasis: "33.333333%" }} >
            <div className="__column_header">
                <h3>Roles in this Group</h3>
                <Glyphicon glyph="plus" onClick={this.handleOpenAddModal}></Glyphicon>
            </div>
            <div className="list-container">
                {this.renderRoleItems(roleInSelectedGroup)}
            </div>
            {this.state.showAddModal &&
                <AddToGroupModal
                    onHide={this.handleHide}
                    onAdd={this.props.actions.addRolesToGroup}
                    items={this.props.roles.rolesList.map(role => { return { Name: role.Name, ...role } })}
                    selectedGroupId={this.props.groups.selectedGroup.Id}
                    alreadyInGroup={this.props.groups.selectedGroup.Roles}
                />
            }
        </div>
    }
}

export default withRedux(RoleList)