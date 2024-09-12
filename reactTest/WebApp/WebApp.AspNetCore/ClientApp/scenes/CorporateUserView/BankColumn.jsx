import * as React from 'react';
import { ColumnItemWithStatus, ColumnItemTypeEnum } from '../../shared/ColumnItemWithStatus';
import { Glyphicon, FormGroup, FormControl, Button, Modal, ControlLabel } from 'react-bootstrap';
import { StatusEx } from '../../backendTypes';
import { SortByStatus } from '../../typeMappings';
import { ContextMenu } from '../../shared/ContextMenu';
import { withRedux } from '../../shared/withReduxStore';
export class BankColumn extends React.Component {
    constructor(props) {
        super(props);
        this.getValidationState = () => {
            const length = this.state.bankName.length;
            if (length > 2)
                return 'success';
            else if (length > 1)
                return 'warning';
            else if (length >= 0)
                return 'error';
        };
        ///Process any changes to the bank name from the modal dialog
        this.handleBankNameChange = (e) => {
            this.setState({ bankName: e.target.value });
        };
        //Read the file as base64 and save it against the file contents
        this.handleChangeImage = (evt) => {
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
            };
            reader.readAsDataURL(file);
        };
        this.handleOpenAddModal = () => {
            this.setState({
                showAddBankModal: true,
                showDeleteSelectedModal: false,
                bankName: "",
                bankId: "",
                bankIcon: ""
            });
        };
        this.handleOpenDeleteModal = () => {
            this.setState({
                showAddBankModal: false,
                showDeleteSelectedModal: true,
            });
        };
        this.closeDeleteModal = () => {
            this.setState({
                showDeleteSelectedModal: false,
            });
        };
        this.closeAddModal = () => {
            this.setState({
                showAddBankModal: false,
                bankName: "",
                bankId: "",
                bankIcon: ""
            });
        };
        this.handleAddBank = () => {
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
        this.handleDeleteBank = () => {
            //If the inputs were validated, store the data
            //if (this.getValidationState() === 'success') {
            //OOR TODO: Some sort of confirmation?          
            this.props.actions.deleteBank(this.state.bankId);
            //}
            this.closeDeleteModal();
        };
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
    onBankClick(bankId) {
        this.state = { showAddBankModal: false, showDeleteSelectedModal: false, bankName: "", status: null, bankId: bankId, bankIcon: "" };
        this.props.actions.selectBank(bankId);
        this.props.actions.retrieveAccountTypes(bankId);
        this.props.actions.listAccounts(bankId, this.props.auth.userConfig.EntityID);
    }
    render() {
        const menuOptions = [
            { label: "Delete", handler: this.handleOpenDeleteModal }
        ];
        const contextMenu = <ContextMenu options={menuOptions} suppressEventPropagation={false}/>;
        const sortedBanks = [...this.props.banks.bankList].sort(SortByStatus);
        const columnItems = sortedBanks.map((entry, i) => <ColumnItemWithStatus key={i} itemType={ColumnItemTypeEnum.Bank} onClick={() => this.onBankClick(entry.id)} selected={entry.id === this.props.banks.selectedBank.id} icon={entry.icon} status={entry.status} title={entry.title} rightContent={contextMenu}>
        </ColumnItemWithStatus>);
        const addBankModal = <Modal show={this.state.showAddBankModal} onHide={this.closeAddModal}>
            <Modal.Header closeButton>
                <Modal.Title>Add a bank</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <form>
                    <FormGroup controlId="formBasicText" validationState={this.getValidationState()}>
                        <ControlLabel>Please enter a bank name</ControlLabel>
                        <FormControl type="text" value={this.state.bankName} placeholder="Enter text" onChange={this.handleBankNameChange}/>
                        <FormControl.Feedback />
                    </FormGroup>
                    <FormGroup controlId="formBasicFile">
                        <ControlLabel>Please choose an icon</ControlLabel>
                        <FormControl type="file" placeholder="please select an image" onChange={this.handleChangeImage}/>
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
               {this.props.auth.role == "admin" && <Glyphicon glyph='plus' onClick={this.handleOpenAddModal}/>}
           </div>
            {columnItems}
            {addBankModal}
            {deleteSelectedBankModal}
        </div>;
    }
}
export default withRedux(BankColumn);
//# sourceMappingURL=BankColumn.jsx.map