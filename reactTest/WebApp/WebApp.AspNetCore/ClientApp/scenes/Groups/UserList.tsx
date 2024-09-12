import * as React from 'react';
import { connect } from 'react-redux';
import { Panel, Button, Glyphicon } from 'react-bootstrap';

import { Item } from '../../frontendTypes';
import { ReduxProps, withRedux } from '../../shared/withReduxStore';
import AddToGroupModal from './AddToGroupModal';
import { ColumnItem } from '../../shared/ColumnItem';
import { ContextMenu } from '../../shared/ContextMenu';
import { UserAuth } from '../../backendTypes';


type Props = ReduxProps;

interface State {
    showAddModal: boolean;
}

export class UserList extends React.Component<Props, State>{
    state = { showAddModal: false }

    handleRemoveUserFromGroup = (userId) => {
        this.props.actions.removeUserFromGroup(userId, this.props.groups.selectedGroup.Id);
    }

    handleOpenAddModal = () => {
        this.setState({
            showAddModal: true
        })
    }

    handleHide = () => {
        this.setState({ showAddModal: false });
    }

    renderUserItems(usersInGroup: UserAuth[]) {
        return usersInGroup.map(user => {
            const options = [{ label: "Remove from Group", handler: () => this.handleRemoveUserFromGroup(user.Id) }];
            const optionsMenu = <ContextMenu options={options} suppressEventPropagation={true} />

            return <ColumnItem title={user.UserName}
                subtitle={user.Email}
                rightContent={optionsMenu}
                icon={<i className="fa fa-2x fa-user-circle" aria-hidden="true"></i>} />
        });
    }

    render() {
        
        const selected = this.props.groups.selectedGroup.UserAuthIds;
        const usersInSelectedgroup = this.props.users.userList
            .filter(user => selected && selected.indexOf(user.Id) !== -1);

        const usersInGroup = usersInSelectedgroup.map(user => {
            return { Name: user.UserName, ...user }
        });
        

        return <div className='admin-column' style={{ flexBasis: "66.66666%" }}>
            <div className="__column_header">
                <h3>Users in this group ({usersInGroup.length} users)</h3>
                <Glyphicon glyph="plus" onClick={this.handleOpenAddModal}></Glyphicon>
            </div>
            <div className="list-container">
                {this.renderUserItems(usersInSelectedgroup) }
            </div>
            {this.state.showAddModal &&
                <AddToGroupModal
                    onHide={this.handleHide}
                    onAdd={this.props.actions.addUsersToGroup}
                    items={this.props.users.userList.map(user => { return { Name: user.UserName, ...user } })}
                    selectedGroupId={this.props.groups.selectedGroup.Id}
                    alreadyInGroup={this.props.groups.selectedGroup.UserAuthIds}
                />
            }
            </div >
         
    }
}

export default withRedux(UserList)