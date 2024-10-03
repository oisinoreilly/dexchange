var __rest = (this && this.__rest) || function (s, e) {
    var t = {};
    for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p) && e.indexOf(p) < 0)
        t[p] = s[p];
    if (s != null && typeof Object.getOwnPropertySymbols === "function")
        for (var i = 0, p = Object.getOwnPropertySymbols(s); i < p.length; i++) if (e.indexOf(p[i]) < 0)
            t[p[i]] = s[p[i]];
    return t;
};
import { Button, ButtonGroup, Table, FormGroup, FormControl, Modal, ControlLabel, Radio } from 'react-bootstrap';
import * as React from 'react';
import { UserAuth, UserConfig } from '../../backendTypes';
import { withRedux } from '../../shared/withReduxStore';
import './Users.scss';
class Users extends React.Component {
    constructor(props) {
        super(props);
        this.closeModalsAndClearUser = () => {
            this.setState({
                showEditUserModal: false,
                showNewUserModal: false,
                showDeleteSelectedModal: false,
                user: new UserAuth(),
                validationState: new Map(),
                Password: ""
            });
        };
        this.allfieldsValid = () => {
            for (var [key, value] of this.state.validationState) {
                if (value === 'error')
                    return false;
            }
            return true;
        };
        this.getValidationStatus = (fieldName, value) => {
            let validationStatus = 'error';
            if (value.length > 4)
                validationStatus = 'success';
            else if (value.length > 2)
                validationStatus = 'warning';
            else if (value.length >= 1)
                validationStatus = 'error';
            let currentValidation = this.state.validationState;
            currentValidation.set(fieldName, validationStatus);
            return currentValidation;
        };
        this.handleUserDetailsChange = (e) => {
            this.setState({
                user: Object.assign({}, this.state.user, { [e.target.name]: e.target.value }),
                validationState: this.getValidationStatus(e.target.name, e.target.value)
            });
            ;
        };
        this.handleInputChange = (e) => {
            this.setState({
                [e.target.name]: e.target.value,
                validationState: this.getValidationStatus(e.target.name, e.target.value)
            });
        };
        this.handleSave = () => {
            const { Password, user, userPrivilege, userConfig } = this.state;
            if (this.allfieldsValid()) {
                //config is the same entity details 
                //TODO: this logic should be in the back end, only pass in an entityId from UI 
                let newConfig = new UserConfig;
                if (userConfig) {
                    newConfig = userConfig;
                    newConfig.UserPrivilege = userPrivilege;
                }
                else {
                    const _a = this.props.auth.userConfig, { UserId, Id, UserPrivilege } = _a, currentUserConfig = __rest(_a, ["UserId", "Id", "UserPrivilege"]);
                    newConfig = Object.assign({ Id: null, UserId: null, UserPrivilege: userPrivilege }, currentUserConfig);
                }
                this.props.actions.createUser(user, Password, newConfig);
                this.closeModalsAndClearUser();
            }
        };
        this.handleEdit = () => {
            if (this.allfieldsValid()) {
                this.props.actions.editUser(this.state.user);
                this.closeModalsAndClearUser();
            }
        };
        this.handleDelete = () => {
            this.props.actions.deleteUser(this.state.user.UserName);
            this.closeModalsAndClearUser();
        };
        this.modalEditUserOpen = (e, user) => {
            this.setState({
                showEditUserModal: true,
                user
            });
        };
        this.modalAddUserOpen = () => {
            this.setState({ showNewUserModal: true });
        };
        this.modalDeleteOpen = (e, user) => {
            this.setState({
                showDeleteSelectedModal: true,
                user
            });
        };
        this.handleEntityChange = (e, userConfigs) => {
            const userConfig = userConfigs.find(config => config.EntityID === e.target.value);
            if (userConfig) {
                this.setState({
                    userConfig
                });
            }
        };
        this.state = {
            showEditUserModal: false,
            showNewUserModal: false,
            showDeleteSelectedModal: false,
            user: new UserAuth(),
            userPrivilege: null,
            entityType:null,
            userConfig: null,
            validationState: new Map(),
            Password: ""
        };
    }
    componentWillMount() {
        this.props.actions.listUsers();
        this.props.actions.listBanks();
        this.props.actions.listCorporates();
    }
    renderUserRow(user) {
        return (<tr>
                <td>{user.UserName}</td>
                <td>{user.FirstName}</td>
                <td>{user.LastName}</td>
                <td>{user.DisplayName}</td>
                <td>{user.Email}</td>
                <td>
                    <ButtonGroup>
                        <i className="fa fa-pencil context-icon" aria-hidden="true" onClick={(e) => this.modalEditUserOpen(e, user)}></i>
                        <i className="fa fa-trash context-icon" aria-hidden="true" onClick={(e) => this.modalDeleteOpen(e, user)}></i>
                    </ButtonGroup>
                </td>
            </tr>);
    }
    entityToConfig(entity) {
        const config = {
            EntityDisplayName: entity.title,
            EntityIcon: entity.icon,
            EntityID: entity.id,
            EntityName: entity.title,
        };
        return config;
    }
    render() {
        const deleteSelectedUserModal = <Modal show={this.state.showDeleteSelectedModal} onHide={this.closeModalsAndClearUser}>
                <Modal.Header closeButton>
                    <Modal.Title>Delete {this.state.user ? this.state.user.UserName : ""}</Modal.Title>
                </Modal.Header>
                <Modal.Footer>
                    <Button bsStyle='default' onClick={this.closeModalsAndClearUser}>Cancel</Button>
                    <Button bsStyle='primary' onClick={this.handleDelete}>OK</Button>
                </Modal.Footer>
            </Modal>;
        const addLabel = <Modal.Title>Add a User</Modal.Title>;
        const editLabel = <Modal.Title>Edit {this.state.user.UserName}</Modal.Title>;
        const addButton = <Button bsStyle='primary' onClick={this.handleSave}>Create User</Button>;
        const editButton = <Button bsStyle='primary' onClick={this.handleEdit}>Save</Button>;
        const fieldMapping = [
            { propertyName: 'UserName', displayName: 'User Name' },
            { propertyName: 'FirstName', displayName: 'First Name' },
            { propertyName: 'LastName', displayName: 'Last Name' },
            { propertyName: 'Email', displayName: 'Email Name' },
            { propertyName: 'DisplayName', displayName: 'Display Name' },
        ];
        const textFields = fieldMapping.map(field => {
            const { propertyName, displayName } = field;
            return <FormGroup controlId={propertyName + 'Input'} validationState={this.state.validationState.get(propertyName)}>
                <ControlLabel>{displayName}</ControlLabel>
                <FormControl name={propertyName} type="text" value={this.state.user[propertyName]} placeholder={'Enter ' + displayName} onChange={this.handleUserDetailsChange}/>
                <FormControl.Feedback />
            </FormGroup>;
        });
        const banks = this.props.banks.bankList.map(this.entityToConfig);
        const corporates = this.props.corporates.corporateList.map(this.entityToConfig);
        const configForEntities = corporates.concat(banks);
        //list of banks and corps so we can set up admin users
        const banksAndCorps = <FormGroup controlId="formUserType">
            <ControlLabel>Select a Corporate or Bank</ControlLabel>
            <FormControl componentClass="select" placeholder="Choose Type" onChange={(e) => this.handleEntityChange(e, configForEntities)}>
                <option value="">Please select an option</option>
                {configForEntities.map(userConfig => <option value={userConfig.EntityID}>{userConfig.EntityName}</option>)}
            </FormControl>
        </FormGroup>;
        const modal = <Modal show={this.state.showEditUserModal || this.state.showNewUserModal} onHide={this.closeModalsAndClearUser}>
            <Modal.Header closeButton>
                {this.state.showEditUserModal ? editLabel : addLabel}
            </Modal.Header>
            <Modal.Body>
                <form>
                    {textFields}
                    {this.state.showNewUserModal && <FormGroup controlId="passwordInput" validationState={this.state.validationState.get('Password')}>
                        <ControlLabel>Password</ControlLabel>
                        <FormControl type="password" name='Password' value={this.state.Password} placeholder="Enter Password" onChange={this.handleInputChange}/>
                        <FormControl.Feedback />
                    </FormGroup>}
                    <FormGroup>
                        <ControlLabel>Select Entity Type  </ControlLabel>
                        <Radio onChange={this.handleInputChange} name="entityType" value='Bank' checked={this.state.entityType === 'Bank'} inline>
                            Standard 
                        </Radio>
                        <Radio onChange={this.handleInputChange} name="entityType" value='Corporate' checked={this.state.entityType === 'Corporate'} inline>
                            Admin
                        </Radio>
                    </FormGroup>

                    <FormGroup>
                        <ControlLabel>Select User Type  </ControlLabel>
                        <Radio onChange={this.handleInputChange} name="userPrivilege" value='User' checked={this.state.userPrivilege === 'User'} inline>
                            Standard 
                        </Radio>
                        <Radio onChange={this.handleInputChange} name="userPrivilege" value='Admin' checked={this.state.userPrivilege === 'Admin'} inline>
                            Admin
                        </Radio>
                    </FormGroup>

                    {this.props.auth.userConfig.UserPrivilege === 'SuperAdmin' && (this.state.entityType === 'Bank')?banks:corporates}
                </form>
            </Modal.Body>
            <Modal.Footer>
                <Button bsStyle='default' onClick={this.closeModalsAndClearUser}>Cancel</Button>
                {this.state.showEditUserModal ? editButton : addButton}
            </Modal.Footer>
        </Modal>;
        return (<div className="users-container">
                <Table striped>
                    <thead>
                        <tr>
                            <th>Username</th>
                            <th>First Name</th>
                            <th>Last Name</th>
                            <th>Display Name</th>
                            <th>Email</th>
                            <th>
                                <i className="fa fa-2x fa-plus-circle context-icon primary" aria-hidden="true" onClick={this.modalAddUserOpen}></i>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        {this.props.users.userList.map((user, i) => this.renderUserRow(user))}
                    </tbody>
                </Table>
                {modal}
                {deleteSelectedUserModal}
            </div>);
    }
}
//Extend the login screen with the redux state
export default withRedux(Users);
//# sourceMappingURL=Users.jsx.map