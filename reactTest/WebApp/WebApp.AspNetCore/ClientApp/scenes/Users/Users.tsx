import { Breadcrumb, Button, ButtonGroup, Glyphicon, Table, FormGroup, FormControl, Modal, ControlLabel, Checkbox, ButtonToolbar, Row, Col, Radio } from 'react-bootstrap';
import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../../store/index';
import * as Auth from '../../store/authStore';
import * as UsersStore from '../../store/usersStore';
import { UserAuth, UserConfig, Privilege } from '../../backendTypes';
import { FieldMapping } from '../../frontendTypes';
import { withRedux, ReduxProps } from '../../shared/withReduxStore';
import './Users.scss';


type ValidationStatus = "success" | "warning" | "error";

interface IUserState {
    showEditUserModal: boolean;
    showDeleteSelectedModal: boolean;
    showNewUserModal: boolean;
    user: UserAuth;
    userPrivilege: Privilege;
    userConfig: UserConfig;
    Password: string;
    validationState: Map<string, ValidationStatus>;
}

class Users extends React.Component<ReduxProps, IUserState> {
    constructor(props) {
        super(props);
        this.state = {
            showEditUserModal: false,
            showNewUserModal: false,
            showDeleteSelectedModal: false,
            user: new UserAuth(),
            userPrivilege: null,
            userConfig: null,
            validationState: new Map<string, ValidationStatus>(),
            Password: ""
        }
    }

    componentWillMount() {
        this.props.actions.listUsers();
        this.props.actions.listBanks();
        this.props.actions.listCorporates();
    }
    closeModalsAndClearUser = () => {
        this.setState({
            showEditUserModal: false,
            showNewUserModal: false,
            showDeleteSelectedModal: false,
            user: new UserAuth(),
            validationState: new Map<string, ValidationStatus>(),
            Password: ""
        });
    }

    allfieldsValid = () => {
        for (var [key, value] of this.state.validationState) {
            if (value === 'error')
                return false;
        }
        return true;
    }

    getValidationStatus = (fieldName, value) => {
        let validationStatus = 'error' as ValidationStatus;
        if (value.length > 4) validationStatus = 'success' as ValidationStatus;
        else if (value.length > 2) validationStatus = 'warning' as ValidationStatus;
        else if (value.length >= 1) validationStatus = 'error' as ValidationStatus;

        let currentValidation = this.state.validationState;
        currentValidation.set(fieldName, validationStatus);
        return currentValidation;
    }

    handleUserDetailsChange = (e) => {
        this.setState({
            user: {
                ...this.state.user,
                [e.target.name]: e.target.value
            },
            validationState: this.getValidationStatus(e.target.name, e.target.value)
        });;
    }

    handleInputChange = (e) => {
        this.setState({
            [e.target.name]: e.target.value,
            validationState: this.getValidationStatus(e.target.name, e.target.value)
        });
    }

    handleSave = () => {

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
                const { UserId, Id, UserPrivilege, ...currentUserConfig } = this.props.auth.userConfig;
                newConfig = {
                    Id: null,
                    UserId: null,
                    UserPrivilege: userPrivilege,
                    ...currentUserConfig
                } as UserConfig;
            }

            this.props.actions.createUser(user, Password, newConfig);
            this.closeModalsAndClearUser();
        }
    }

    handleEdit = () => {
        if (this.allfieldsValid()) {
            this.props.actions.editUser(this.state.user);
            this.closeModalsAndClearUser();
        }
    }

    handleDelete = () => {
        this.props.actions.deleteUser(this.state.user.UserName);
        this.closeModalsAndClearUser();
    }

    modalEditUserOpen = (e, user) => {
        this.setState({
            showEditUserModal: true,
            user
        });
    };

    modalAddUserOpen = () => {
        this.setState({ showNewUserModal: true });
    };

    modalDeleteOpen = (e, user) => {
        this.setState({
            showDeleteSelectedModal: true,
            user
        });
    }

    handleEntityChange = (e, userConfigs: UserConfig[]) => {
        const userConfig = userConfigs.find(config => config.EntityID === e.target.value);
        if (userConfig) {
            this.setState({
                userConfig
            })
        }
    }

    renderUserRow(user: UserAuth) {
        return (
            <tr>
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
            </tr>
        )
    }

    entityToConfig(entity){
        const config = {
            EntityDisplayName: entity.title,
            EntityIcon: entity.icon,
            EntityID: entity.id,
            EntityName: entity.title,
        } as UserConfig;
        return config;
    }

    public render() {
        const deleteSelectedUserModal =
            <Modal show={this.state.showDeleteSelectedModal} onHide={this.closeModalsAndClearUser}>
                <Modal.Header closeButton>
                    <Modal.Title>Delete {this.state.user ? this.state.user.UserName : ""}</Modal.Title>
                </Modal.Header>
                <Modal.Footer>
                    <Button bsStyle='default' onClick={this.closeModalsAndClearUser}>Cancel</Button>
                    <Button bsStyle='primary' onClick={this.handleDelete}>OK</Button>
                </Modal.Footer>
            </Modal>;

        const addLabel = <Modal.Title>Add a User</Modal.Title>;
        const editLabel = <Modal.Title>Edit {this.state.user.UserName}</Modal.Title>
        const addButton = <Button bsStyle='primary' onClick={this.handleSave}>Create User</Button>
        const editButton = <Button bsStyle='primary' onClick={this.handleEdit}>Save</Button>

        const fieldMapping: FieldMapping[] = [
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
                <FormControl name={propertyName} type="text" value={this.state.user[propertyName]} placeholder={'Enter ' + displayName} onChange={this.handleUserDetailsChange} />
                <FormControl.Feedback />
            </FormGroup>
        });

        const banks = this.props.banks.bankList.map(this.entityToConfig);
        const corporates = this.props.corporates.corporateList.map(this.entityToConfig);
        const configForEntities = corporates.concat(banks);

        //list of banks and corps so we can set up admin users
        const banksAndCorps =  <FormGroup controlId="formUserType">
            <ControlLabel>Select a Corporate or Bank</ControlLabel>
            <FormControl componentClass="select" placeholder="Choose Type"
                onChange={(e) => this.handleEntityChange(e, configForEntities)}>
                <option value="">Please select an option</option>
                {configForEntities.map(userConfig =>
                    <option value={userConfig.EntityID}>{userConfig.EntityName}</option>)}
            </FormControl>
        </FormGroup>

        const modal = <Modal show={this.state.showEditUserModal || this.state.showNewUserModal} onHide={this.closeModalsAndClearUser} >
            <Modal.Header closeButton>
                {this.state.showEditUserModal ? editLabel : addLabel}
            </Modal.Header>
            <Modal.Body>
                <form>
                    {textFields}
                    {this.state.showNewUserModal && <FormGroup controlId="passwordInput" validationState={this.state.validationState.get('Password')}>
                        <ControlLabel>Password</ControlLabel>
                        <FormControl type="password" name='Password' value={this.state.Password} placeholder="Enter Password" onChange={this.handleInputChange} />
                        <FormControl.Feedback />
                    </FormGroup>}
                    <FormGroup>
                        <ControlLabel>Select User Type  </ControlLabel>
                        <Radio onChange={this.handleInputChange}
                            name="userPrivilege"
                            value='User'
                            checked={this.state.userPrivilege === 'User'}
                            inline>
                            Standard 
                        </Radio>
                        <Radio onChange={this.handleInputChange}
                            name="userPrivilege"
                            value='Admin'
                            checked={this.state.userPrivilege === 'Admin'}
                            inline>
                            Admin
                        </Radio>
                    </FormGroup>
                    {/*if SuperAdmin allow use to create user for any entity. Need so we can create admins for new banks/corps*/}
                    {this.props.auth.userConfig.UserPrivilege === 'SuperAdmin' && banksAndCorps}
                </form>
            </Modal.Body>
            <Modal.Footer>
                <Button bsStyle='default' onClick={this.closeModalsAndClearUser}>Cancel</Button>
                {this.state.showEditUserModal ? editButton : addButton}
            </Modal.Footer>
        </Modal >

        return (
            <div className="users-container">
                <Table striped >
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
            </div>
        )
    }
}

//Extend the login screen with the redux state
export default withRedux(Users);
