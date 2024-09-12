import { Button, ControlLabel, Form, Table, FormControl, FormGroup, Modal, Col, Checkbox, Panel, PageHeader, Glyphicon, ListGroup, ListGroupItem } from 'react-bootstrap';
import * as React from 'react';
import { Link, withRouter, Route } from 'react-router-dom';
import { Access, Permission, RoleInfo } from '../../backendTypes';
import RoleViewer from './RoleViewer';
import { withRedux, ReduxProps } from '../../shared/withReduxStore';
import AddRole from './AddRole';
import { ContextMenu } from '../../shared/ContextMenu';
import { ColumnItem } from '../../shared/ColumnItem';
import { ConfirmModal } from '../../shared/ConfirmModal';
import './Roles.scss';


interface OwnProps {          // ... plus incoming routing parameters
    name: string;
    type: string;
    location: any;
    params: any;
    history: any;
}

type Props = OwnProps & ReduxProps;

type State = {
    showDeleteModal: boolean;
    roleId: string;
}

export class Roles extends React.Component<Props, State> {

    state = {
        showDeleteModal: false,
        roleId: ""
    }

    componentWillMount() {
        this.props.actions.requestRolesList();
        this.props.actions.listBanks();
        this.props.actions.listCorporates();
        this.props.actions.listAllAccounts();
    }

    goToAddRole = () => {
        this.props.history.push('/admin/roles/add');
    };

    handleDeleteRole = () => {
        this.props.actions.deleteRole(this.state.roleId);
        this.setState({
            showDeleteModal: false,
        })
    }

    showDeleteModal = (roleId) => {
        this.setState({
            showDeleteModal: true,
            roleId
        })
    }

    closeDeleteModal = () => {
        this.setState({
            showDeleteModal: false,
        })
    }

    public render() {

        const { selectedRole } = this.props.roles;

        return (
            <div className="role-columns">
                <div className={"admin-column role-column"}>
                    <div className="__column_header">
                        <h3>Roles</h3>
                        <Glyphicon glyph="plus" onClick={this.goToAddRole}></Glyphicon>
                    </div>
                    <div className="column-items">
                        {this.props.roles.rolesList.map(role => {
                            const options = [{ label: "Delete", handler: () => this.showDeleteModal(role.Id) }];
                            const optionsMenu = <ContextMenu options={options} suppressEventPropagation={true} />

                            return <ColumnItem title={role.Name}
                                selected={role.Id === selectedRole.Id}
                                rightContent={optionsMenu}
                                onClick={() => this.props.actions.selectRole(role.Id)}
                                icon={<i className="fa fa-2x fa-key" aria-hidden="true"></i>} />
                             })
                        }
                    </div>
                </div>
                <div className={"admin-column permission-column"}>
                    <div className="__column_header">
                        <h3>Permissions</h3>
                    </div>
                    <Route exact path='/admin/roles' component={RoleViewer} />
                    <Route path='/admin/roles/add' component={AddRole} />
                </div>

                <ConfirmModal closeModal={this.closeDeleteModal}
                    confirmHandler={this.handleDeleteRole}
                    modalTitle="Are you sure you want to delete this role?"
                    showModal={this.state.showDeleteModal} />
            </div>
        )
    }
}

export default withRouter(withRedux(Roles));
