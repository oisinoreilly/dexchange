import * as React from 'react';
import { ColumnItemWithStatus, ColumnItemTypeEnum } from '../../shared/ColumnItemWithStatus';
import { Navbar, Nav, NavItem, NavDropdown, MenuItem, Glyphicon, FormGroup, FormControl, Button, ButtonGroup, Modal, OverlayTrigger, Form, Col, ControlLabel, HelpBlock } from 'react-bootstrap';
import { BankUi } from '../../store/bankState';
import { StatusEx } from '../../backendTypes';
import { SortByStatus } from '../../typeMappings';
import { ContextMenu, IMenuOption } from '../../shared/ContextMenu';
import { ReduxProps, withRedux } from '../../shared/withReduxStore';


interface IBankColumnState {
    showAddBankModal: boolean;
    showDeleteSelectedModal: boolean;
    bankName: string;
    bankId: string;
    bankIcon: string;
    status: StatusEx;
}

export class BankColumn extends React.Component<ReduxProps, IBankColumnState> {
    constructor(props) {
        super(props);
        this.state = {
            showAddBankModal: false,
            showDeleteSelectedModal: false,
            status: null, bankName: "",
            bankId: "",
            bankIcon: ""
        };
    }

    getRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }

    onBankClick(bankId: string) {
        this.state = { showAddBankModal: false, showDeleteSelectedModal: false, bankName: "", status: null, bankId: bankId, bankIcon: "" };
        this.props.actions.selectBank(bankId);
        this.props.actions.retrieveAccountTypes(bankId);
        this.props.actions.listAccounts(bankId, this.props.auth.userConfig.EntityID);
    }

    getValidationState = () => {
        const length = this.state.bankName.length;
        if (length > 2) return 'success';
        else if (length > 1) return 'warning';
        else if (length >= 0) return 'error';
    }
    ///Process any changes to the bank name from the modal dialog
    handleBankNameChange = (e) => {
        this.setState( {bankName: e.target.value} );
    };

    //Read the file as base64 and save it against the file contents
    handleChangeImage = (evt) => {
        let reader = new FileReader();
        let file = evt.target.files[0];

        reader.onloadend = () => {
            let result = reader.result;
            this.setState({
                showAddBankModal: this.state.showAddBankModal,
                showDeleteSelectedModal: this.state.showDeleteSelectedModal,
                bankName: this.state.bankName,
                bankId: this.state.bankId,
                bankIcon: result
            });
        }
        reader.readAsDataURL(file)
    };

    handleOpenAddModal = () => {
        this.setState({
            showAddBankModal: true,
            showDeleteSelectedModal: false,
            bankName: "",
            bankId: "",
            bankIcon: ""
        });
    };

    handleOpenDeleteModal = () => {
        this.setState({
            showAddBankModal: false,
            showDeleteSelectedModal: true,
        });
    };

    closeDeleteModal = () => {
        this.setState({
            showDeleteSelectedModal: false,
        });
    }

    closeAddModal = () => {
        this.setState({
            showAddBankModal: false,
            bankName: "",
            bankId: "",
            bankIcon: ""
        });
    }

    handleAddBank = () => {
        //If the inputs were validated, store the data
        const approvedStatus = new StatusEx();
        approvedStatus.Status = 'Approved_e';
        if (this.getValidationState() === 'success') {
            this.props.actions.addBank({
                id: this.getRandomInt(1, 999),
                title: this.state.bankName,
                icon: this.state.bankIcon,
                status: approvedStatus,
                accounts: [],
                accountTypes: []
            });
        }
        //Regardless of the user seleciton, close the modal
        this.closeAddModal();
    };

   handleDeleteBank = () => {
        //If the inputs were validated, store the data
        //if (this.getValidationState() === 'success') {
            //OOR TODO: Some sort of confirmation?          
            this.props.actions.deleteBank(
                this.state.bankId
            );
        //}
        this.closeDeleteModal();
    };

   public render() {

       const menuOptions: IMenuOption[] = [
           { label: "Delete", handler: this.handleOpenDeleteModal }
       ];
       const contextMenu = <ContextMenu options={menuOptions} suppressEventPropagation={false} />

       const sortedBanks = [...this.props.banks.bankList].sort(SortByStatus);
       const columnItems = sortedBanks.map((entry, i) => <ColumnItemWithStatus
           key={i}
           itemType={ColumnItemTypeEnum.Bank}
           onClick={() => this.onBankClick(entry.id)}
           selected={entry.id === this.props.banks.selectedBank.id}
           icon={entry.icon}
           status={entry.status}
           title={entry.title}
           rightContent={contextMenu}>
        </ColumnItemWithStatus>);

        const addBankModal = <Modal show={this.state.showAddBankModal} onHide={this.closeAddModal}>
            <Modal.Header closeButton>
                <Modal.Title>Add a bank</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <form>
                    <FormGroup controlId="formBasicText" validationState={this.getValidationState()}>
                        <ControlLabel>Please enter a bank name</ControlLabel>
                        <FormControl
                            type="text"
                            value={this.state.bankName}
                            placeholder="Enter text"
                            onChange={this.handleBankNameChange}
                        />
                        <FormControl.Feedback />
                    </FormGroup>
                    <FormGroup controlId="formBasicFile">
                        <ControlLabel>Please choose an icon</ControlLabel>
                        <FormControl
                            type="file"
                            placeholder="please select an image"
                            onChange={this.handleChangeImage}
                        />
                        <FormControl.Feedback />
                    </FormGroup>
                </form>
            </Modal.Body>
            <Modal.Footer>
                <Button onClick={this.handleAddBank}>Add</Button>
            </Modal.Footer>
        </Modal>;


        const deleteSelectedBankModal = <Modal show={this.state.showDeleteSelectedModal} onHide={this.closeDeleteModal}>
            <Modal.Header closeButton>
                <Modal.Title>Delete Selected Bank</Modal.Title>
            </Modal.Header>
            <Modal.Footer>
                <Button onClick={this.handleDeleteBank}>Delete</Button>
            </Modal.Footer>
        </Modal>;

       return <div className="__column">
           <div className="__column_header">
               <h3>Filter by Bank</h3>
               {this.props.auth.role == "admin" && <Glyphicon glyph='plus' onClick={this.handleOpenAddModal} />}
           </div>
            {columnItems}
            {addBankModal}
            {deleteSelectedBankModal}
        </div>;
    }
}

export default withRedux(BankColumn);
