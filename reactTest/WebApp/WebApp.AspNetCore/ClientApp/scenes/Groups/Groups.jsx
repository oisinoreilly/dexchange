import * as React from 'react';
import { Glyphicon } from 'react-bootstrap';
import { Group } from '../../backendTypes';
import { withRedux } from '../../shared/withReduxStore';
import { ConfirmModal } from '../../shared/ConfirmModal';
import { AddGroupModal } from './AddGroupModal';
import { ContextMenu } from '../../shared/ContextMenu';
import { ColumnItem } from '../../shared/ColumnItem';
import { generateId } from '../../Utils';
import './Groups.scss';
export class Groups extends React.Component {
    constructor(props) {
        super(props);
        this.handleRemovegroup = (id) => {
            this.props.actions.deleteGroup(id);
        };
        this.handleOpenAddModal = () => {
            this.setState({
                showAddModal: true
            });
        };
        this.closeAddModal = () => {
            this.setState({
                showAddModal: false
            });
        };
        this.handleAddGroup = (groupName) => {
            const newGroup = new Group();
            newGroup.Name = groupName;
            newGroup.UserAuthIds = [];
            newGroup.Roles = [];
            newGroup.Subgroups = [];
            newGroup.Id = generateId();
            this.props.actions.createGroup(newGroup);
            this.closeAddModal();
        };
        this.handleDeleteGroup = () => {
            this.props.actions.deleteGroup(this.state.groupId);
            this.setState({
                showDeleteModal: false,
            });
        };
        this.showDeleteModal = (groupId) => {
            this.setState({
                showDeleteModal: true,
                groupId
            });
        };
        this.closeDeleteModal = () => {
            this.setState({
                showDeleteModal: false,
            });
        };
        this.state = {
            showAddModal: false,
            showDeleteModal: false,
            groupId: ""
        };
    }
    componentWillMount() {
        this.props.actions.listAllGroups();
        this.props.actions.listUsers();
        this.props.actions.requestRolesList();
    }
    handleItemClick(group) {
        this.props.actions.selectGroup(group);
    }
    renderGroupRow(group) {
        return (<tr>
                <td>{group.name}</td>
            </tr>);
    }
    renderGroupItems() {
        const { selectedGroup } = this.props.groups;
        return this.props.groups.groupsList.map(group => {
            const options = [{ label: "Delete", handler: () => this.showDeleteModal(group.Id) }];
            const optionsMenu = <ContextMenu options={options} suppressEventPropagation={true}/>;
            return <ColumnItem title={group.Name} selected={group.Id === selectedGroup.Id} rightContent={optionsMenu} onClick={() => this.handleItemClick(group)} icon={<i className="fa fa-2x fa-users" aria-hidden="true"></i>}/>;
        });
    }
    render() {
        return (<div className='admin-column groups'>
                <div className="__column_header">
                    <h3>Groups</h3>
                    <Glyphicon glyph="plus" onClick={this.handleOpenAddModal}></Glyphicon>
                </div>
                {this.renderGroupItems()}
                {this.state.showAddModal && <AddGroupModal onAddGroup={this.handleAddGroup} onHide={this.closeAddModal}/>}

                <ConfirmModal closeModal={this.closeDeleteModal} confirmHandler={this.handleDeleteGroup} modalTitle="Are you sure you want to delete this group?" showModal={this.state.showDeleteModal}/>
            </div>);
    }
}
export default withRedux(Groups);
//# sourceMappingURL=Groups.jsx.map