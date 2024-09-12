import { Panel, ControlLabel, FormControl, FormGroup, PageHeader } from 'react-bootstrap';
import * as React from 'react';
import { RoleInfo } from '../../backendTypes';
import PermissionsForBankUser from './PermissionsForBankUser';
import PermissionsForCorporateUser from './PermissionsForCorporateUser';
import { withRedux } from '../../shared/withReduxStore';
import { arrayUnique } from '../../Utils';
export class AddRole extends React.Component {
    constructor() {
        super(...arguments);
        this.state = {
            roleName: '',
            currentUserType: null,
            permissions: new Array()
        };
        this.handleAddPermissions = (permissionToAdd, access) => {
            const newPermissions = [];
            const existingPermissions = [];
            //loop through the existing permissions in state to see if the permissions to add exist
            permissionToAdd.forEach(perm => {
                if (this.state.permissions.findIndex(p => p.ResourceId === perm.ResourceId) !== -1) {
                    perm.Access = [access];
                    existingPermissions.push(perm);
                }
                else {
                    perm.Access = [access];
                    newPermissions.push(perm);
                }
            });
            //if a permissions exists ensure the new access is added
            //for new permissions add them to the permissions list in state
            const permissions = this.state.permissions.map(perm => {
                const permissionNeedsUpdating = existingPermissions.findIndex(p => perm.ResourceId === perm.ResourceId) !== -1;
                if (permissionNeedsUpdating && perm.Access.indexOf(access) === -1) {
                    perm.Access = arrayUnique(perm.Access.concat([access]));
                    return perm;
                }
                return perm;
            }).concat(newPermissions);
            this.setState({ permissions });
        };
        this.handleCreateRole = () => {
            const newRole = new RoleInfo();
            newRole.Name = this.state.roleName;
            newRole.Permissions = this.state.permissions;
            //Add the role to the store
            this.props.actions.requestAddRole(newRole);
            this.props.history.push('/admin/roles');
        };
        this.handleCancel = () => {
            this.props.history.goBack();
        };
        this.handleCheckboxChange = (e) => {
            this.setState({
                [e.target.name]: e.target.checked
            });
        };
        this.handleRoleNameChange = (e) => {
            this.setState({
                roleName: e.target.value
            });
        };
        this.handleUserTypeChange = (e) => {
            this.setState({
                currentUserType: e.target.value
            });
        };
    }
    render() {
        return <div>
            <PageHeader>Add a Role</PageHeader>
            <form>
                <FormGroup controlId="formBasicText">
                    <ControlLabel>Please enter a role name</ControlLabel>
                    <FormControl type="text" value={this.state.roleName} placeholder="Enter name" onChange={this.handleRoleNameChange}/>
                    <FormControl.Feedback />
                </FormGroup>
                <FormGroup controlId="formUserType">
                    <ControlLabel>User Type</ControlLabel>
                    <FormControl componentClass="select" placeholder="Choose Type" onChange={this.handleUserTypeChange}>
                        <option value="">Please select an option</option>
                        <option value="Corporate">Corporate</option>
                        <option value="Bank">Bank</option>
                    </FormControl>
                </FormGroup>
            </form>
            <Panel>
                <div className='--align-in-column'>
                   
                    {this.state.currentUserType === "Bank" &&
            <PermissionsForBankUser addPermissions={this.handleAddPermissions} permissionsToAdd={this.state.permissions} createRole={this.handleCreateRole} cancel={this.handleCancel}/>}
                    {this.state.currentUserType === "Corporate" &&
            <PermissionsForCorporateUser addPermissions={this.handleAddPermissions} permissionsToAdd={this.state.permissions} createRole={this.handleCreateRole} cancel={this.handleCancel}/>}
                </div>
            </Panel>
        </div>;
    }
}
export default withRedux(AddRole);
//# sourceMappingURL=AddRole.jsx.map