import { Glyphicon } from 'react-bootstrap';
import * as React from 'react';
import { withRouter, Route } from 'react-router-dom';
import RoleViewer from './RoleViewer';
import { withRedux } from '../../shared/withReduxStore';
import AddRole from './AddRole';
import { ContextMenu } from '../../shared/ContextMenu';
import { ColumnItem } from '../../shared/ColumnItem';
import { ConfirmModal } from '../../shared/ConfirmModal';
import './Roles.scss';
export class Roles extends React.Component {
    constructor() {
        super(...arguments);
        this.state = {
            showDeleteModal: false,
            roleId: ""
        };
        this.goToAddRole = () => {
            this.props.history.push('/admin/roles/add');
        };
        this.handleDeleteRole = () => {
            this.props.actions.deleteRole(this.state.roleId);
            this.setState({
                showDeleteModal: false,
            });
        };
        this.showDeleteModal = (roleId) => {
            this.setState({
                showDeleteModal: true,
                roleId
            });
        };
        this.closeDeleteModal = () => {
            this.setState({
                showDeleteModal: false,
            });
        };
    }
    componentWillMount() {
        this.props.actions.requestRolesList();
        this.props.actions.listBanks();
        this.props.actions.listCorporates();
        this.props.actions.listAllAccounts();
    }
    render() {
        const { selectedRole } = this.props.roles;
        return (<div className="role-columns">
                <div className={"admin-column role-column"}>
                    <div className="__column_header">
                        <h3>Roles</h3>
                        <Glyphicon glyph="plus" onClick={this.goToAddRole}></Glyphicon>
                    </div>
                    <div className="column-items">
                        {this.props.roles.rolesList.map(role => {
            const options = [{ label: "Delete", handler: () => this.showDeleteModal(role.Id) }];
            const optionsMenu = <ContextMenu options={options} suppressEventPropagation={true}/>;
            return <ColumnItem title={role.Name} selected={role.Id === selectedRole.Id} rightContent={optionsMenu} onClick={() => this.props.actions.selectRole(role.Id)} icon={<i className="fa fa-2x fa-key" aria-hidden="true"></i>}/>;
        })}
                    </div>
                </div>
                <div className={"admin-column permission-column"}>
                    <div className="__column_header">
                        <h3>Permissions</h3>
                    </div>
                    <Route exact path='/admin/roles' component={RoleViewer}/>
                    <Route path='/admin/roles/add' component={AddRole}/>
                </div>

                <ConfirmModal closeModal={this.closeDeleteModal} confirmHandler={this.handleDeleteRole} modalTitle="Are you sure you want to delete this role?" showModal={this.state.showDeleteModal}/>
            </div>);
    }
}
export default withRouter(withRedux(Roles));
//# sourceMappingURL=Roles.jsx.map