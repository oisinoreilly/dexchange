import * as React from 'react';
import { ColumnItemWithStatus, ColumnItemTypeEnum } from './ColumnItemWithStatus';
import { Navbar, Nav, NavItem, NavDropdown, MenuItem, Glyphicon, FormGroup, FormControl, Button, ButtonGroup, Modal, ControlLabel, Radio } from 'react-bootstrap';
import { AccountUi } from '../store/accountState';
import { StatusEx, Resource } from '../backendTypes';
import { SortByStatus } from '../typeMappings';
import { ContextMenu, IMenuOption } from './ContextMenu';
import { withRedux, ReduxProps } from './withReduxStore';


interface IAccountColumnState {
    showAddModal: boolean;
    showDeleteModal: boolean;
    showEditModal: boolean;
    title: string;
    description: string;
    Id: string;
    icon: string;
    status: StatusEx;
    loadingDocs: boolean;
    accountTypeId: string;
    fillDocs: boolean;
}

interface IAccountColumnProps {
    addAccount: (account: AccountUi, fillDocs: boolean) => void;
}

type Props = IAccountColumnProps & ReduxProps;

class AccountColumn extends React.Component<Props, IAccountColumnState> {
    state = {
        showAddModal: false,
        showDeleteModal: false,
        showEditModal: false,
        title: "",
        description: "",
        Id: "",
        icon: "",
        status: new StatusEx(),
        loadingDocs: false,
        accountTypeId: "",
        fillDocs: false
    }

    componentDidMount() {
        const { selectedBank } = this.props.banks;

        if (!selectedBank.id)
            this.props.actions.retrieveAccountTypes(this.props.auth.userConfig.EntityID);
    }

    getRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }

    makeid(length: number) {
        var text = "";
        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        for (var i = 0; i < length; i++)
            text += possible.charAt(Math.floor(Math.random() * possible.length));

        return text;
    }

    onAccountClick(account: AccountUi) {
        this.setState({
            title: account.title,
            description: account.description,
            Id: account.id,
            icon: account.icon,
            status: account.status
        });

        this.props.actions.selectAccount(account.id);
        this.props.actions.listDocuments(account.id)
    }

    getValidationState = () => {
        const { fillDocs, title, accountTypeId } = this.state;

        if (title.length > 2 && accountTypeId ) return 'success';
        else if (title.length > 1 && accountTypeId) return 'warning';
        else if (title.length  >= 0) return 'error';
    }
    ///Process any changes to the bank name from the modal dialog
    handleAccountNameChange = (e) => { this.setState({ title: e.target.value }); };

    //Read the file as base64 and save it against the file contents
    handleChangeImage = (evt) => {
        let reader = new FileReader();
        let file = evt.target.files[0];

        reader.onloadend = () => {
            let result = reader.result;
            this.setState({
                showAddModal: this.state.showAddModal,
                showEditModal: this.state.showEditModal,
                showDeleteModal: this.state.showDeleteModal,
                title: this.state.title,
                description: this.state.description,
                Id: this.state.Id,
            });
        }
        reader.readAsDataURL(file)
    };

    handleInputChange = (e) => {
        this.setState({ [e.target.name]: e.target.value });
    }

    handleAccountTypeChange = (e) => {
        this.setState({
            [e.target.name]: e.target.value
        });
    }

    handleDocumentOptionsChange = (e) => {
        this.setState({
            fillDocs: e.target.value
        });
    }

    //Handle the opening of the modal dialog
    openAddModal = () => {
        this.setState({
            showAddModal: true,
            showEditModal: false,
            showDeleteModal: false,
            title: ""
        });
    };

    //Handle the opening of the modal dialog
    openEditModal = () => {
        this.setState({
            showAddModal: false,
            showEditModal: true,
            showDeleteModal: false,
        });
    };

    openDeleteModal = () => {
        this.setState({
            showDeleteModal: true
        });
    };

    addAccount = (e) => {
        e.preventDefault();
        if (this.getValidationState() === 'success') {
            //Create an account.
            const status = new StatusEx();
            status.Status = "Pending_e";
            let account = {
                id: this.getRandomInt(1000, 1999),
                title: this.state.title,
                description: this.state.description,
                status: status,
                icon: "",
                selected: true,
                documents: [],
                accountType: this.state.accountTypeId
            };
            this.props.addAccount(account, this.state.fillDocs);
        }
        //Regardless of the user seleciton, close the modal and clear the fields
        this.closeModalAndClearFields();
    };


    //Handle validation and closing of the dialog
    updateAccount = () => {
        //If the inputs were validated, store the data
        if (this.getValidationState() === 'success') {
            //Create an account.
            //TODO: Handle this on the backend
            const status = new StatusEx();
            status.Status = "Pending_e";

            let account = {
                id: this.getRandomInt(1000, 1999),
                title: this.state.title,
                description: this.state.description,
                status: status,
                icon: "",
                selected: true,
                documents: [],
            };
            //Call the callback function to add the account.
            //this.props.updateAccount(account);
        }
        //Regardless of the user seleciton, close the modal and clear the fields
        this.closeModalAndClearFields();
    };


    handleDeleteAccount = () => {
        //If the inputs were validated, store the data
        //if (this.getValidationState() === 'success') {
        //OOR TODO: Some sort of confirmation?          
        this.props.actions.deleteAccount(
            this.state.Id
        );
        //}
        //Regardless of the user seleciton, close the modal
        this.props.actions.clearDocuments();
        this.closeModalAndClearFields();

    };

    closeModalAndClearFields = () => {
        this.setState({
            showAddModal: false,
            showEditModal: false,
            showDeleteModal: false,
            title: "",
            description: "",
            Id: "",
            icon: "",
        });
    }

    public render() {

        const { selectedBank } = this.props.banks;

        const accountOptions = this.props.accounts.accountTypeList.map((acc) =>
            <option value={acc.Id}>{acc.Name}</option >);

        const menuOptions: IMenuOption[] = [
            { label: "Delete", handler: () => { this.openDeleteModal() }},
            { label: "Edit", handler: () => this.openEditModal() }
        ];

        const contextMenu = <ContextMenu options={menuOptions} suppressEventPropagation={false} />

        //Only render accounts for the current bank
        const sortedAccounts = [...this.props.accounts.accountList].sort(SortByStatus);
        const columnItems = sortedAccounts.map((account, i) => {
            return <ColumnItemWithStatus
                key={i}
                itemType={ColumnItemTypeEnum.Account}
                onClick={() => this.onAccountClick(account)}
                selected={account.id === this.props.accounts.selectedAccount.id}
                icon={account.icon}
                status={account.status}
                title={account.title}
                subtitle={account.description}
                rightContent={contextMenu}>
            </ColumnItemWithStatus>
        });

        const createButton = this.state.loadingDocs
            ? <Button bsStyle='primary' disabled><i className="fa fa-spinner fa-pulse fa-fw"></i>Retrieving Documents</Button>
            : <Button bsStyle='primary' onClick={this.addAccount}>Create Account</Button>

        const modalTitle = this.state.showAddModal && !this.state.showEditModal ? "Create an Account" : "Edit Account: " + this.state.title;
        const modalButton = this.state.showAddModal && !this.state.showEditModal
            ? createButton
            : <Button onClick={this.updateAccount}>Save</Button>

        const addEditAccountModal = <Modal show={this.state.showAddModal || this.state.showEditModal} onHide={this.closeModalAndClearFields}>
            <Modal.Header closeButton>
                <Modal.Title>{modalTitle}</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <form>
                    <FormGroup controlId="formBasicText" validationState={this.getValidationState()}>
                        <ControlLabel>Account name</ControlLabel>
                        <FormControl
                            type="text"
                            value={this.state.title}
                            placeholder="Enter account Name"
                            onChange={this.handleInputChange}
                            name='title'
                        />
                        <FormControl.Feedback />
                    </FormGroup>
                    <FormGroup controlId="formControlsSelectAccType" validationState={this.getValidationState()}>
                        <ControlLabel>Select Account Type</ControlLabel>
                        <FormControl componentClass="select"
                            placeholder="Choose Account Type"
                            onChange={this.handleAccountTypeChange}
                            name='accountTypeId'>
                            <option value="" disabled selected>Select an Account Type</option>
                            {accountOptions}
                        </FormControl>
                    </FormGroup>
                    <FormGroup validationState={this.getValidationState()}>
                        <ControlLabel>Account Opening Documents</ControlLabel>
                        <Radio name="radioGroup" value='true' onChange={this.handleDocumentOptionsChange}>Pre-populated documents</Radio>
                        <Radio name="radioGroup" value='false' onChange={this.handleDocumentOptionsChange} inline>Blank Documents</Radio>                        
                    </FormGroup>
                  
                </form>
            </Modal.Body>
            <Modal.Footer>
                {modalButton}
            </Modal.Footer>
        </Modal>;

        const deleteSelectedModal = <Modal show={this.state.showDeleteModal} onHide={this.closeModalAndClearFields}>
            <Modal.Header closeButton>
                <Modal.Title>Delete Selected Account</Modal.Title>
            </Modal.Header>
            <Modal.Footer>
                <Button onClick={this.handleDeleteAccount}>Delete</Button>
            </Modal.Footer>
        </Modal>;

        return <div className="__column">
            <div className="__column_header">
                <h3>Accounts</h3>
                {this.props.auth.role == 'admin' && <Glyphicon glyph='plus' onClick={this.openAddModal} />}
            </div>
            {columnItems}
            {addEditAccountModal}
            {deleteSelectedModal}
        </div>;

    }
}

export default withRedux(AccountColumn);
