import * as React from 'react';
import { ColumnItemWithStatus, ColumnItemTypeEnum } from '../../shared/ColumnItemWithStatus';
import { Navbar, Nav, NavItem, NavDropdown, MenuItem, Glyphicon, FormGroup, FormControl, Button, ButtonGroup, Modal, OverlayTrigger, Form, Col, ControlLabel, HelpBlock } from 'react-bootstrap';
import { CorporateUi } from '../../store/corporateState';
import { StatusEx } from '../../backendTypes';
import { IMenuOption, ContextMenu } from '../../shared/ContextMenu';
import { ConfirmModal } from '../../shared/ConfirmModal';
import { RightArrowButton } from '../../shared/RightArrowButton';
import { generateId } from '../../Utils';

import { SortByStatus } from '../../typeMappings';
import { withRedux, ReduxProps } from '../../shared/withReduxStore';


interface ICorporateColumnState {
    showAddCorporateModal: boolean;
    showDeleteSelectedModal: boolean;
    corporateName: string;
    corporateId: string;
    corporateIcon: string;
    status: StatusEx;
    corporateContextForMenu: CorporateUi;
}

interface OwnProps {
    selectCorporate: (corporate: string) => void;
    viewSubsids: Function;
}

type Props = ReduxProps & OwnProps;

export class CorporateColumn extends React.Component<Props, ICorporateColumnState> {

    state = {
        showAddCorporateModal: false,
        showDeleteSelectedModal: false,
        status: null,
        corporateName: "",
        corporateId: "",
        corporateIcon: "",
        corporateContextForMenu: null
    }

    onCorporateClick(corporateId: string) {
        this.setState({
            showAddCorporateModal: false,
            showDeleteSelectedModal: false,
            status: null,
            corporateName: "",
            corporateId: corporateId,
            corporateIcon: ""
        });
        this.props.selectCorporate(corporateId);
    }

    getValidationState = () => {
        const length = this.state.corporateName.length;
        if (length > 2) return 'success';
        else if (length > 1) return 'warning';
        else if (length >= 0) return 'error';
    }

    ///Process any changes to the corporate name from the modal dialog
    handleCorporateNameChange = (e) => {
        this.setState({
            corporateName: e.target.value,
        });
    };

    //Read the file as base64 and save it against the file contents
    handleChangeImage = (evt) => {
        let reader = new FileReader();
        let file = evt.target.files[0];

        reader.onloadend = () => {
            let result = reader.result;
            this.setState({
                corporateIcon: result
            });
        }

        reader.readAsDataURL(file)
    };

    handleOpenAddModal = (corporate) => {
        const context = corporate ? corporate : null;

        this.setState({
            showAddCorporateModal: true,
            showDeleteSelectedModal: false,
            corporateContextForMenu: context
        });
    };

    handleOpenDeleteModal = (id) => {
        this.setState({
            corporateId: id,
            showDeleteSelectedModal: true
        });
    };

    handleAddCorporate = () => {
        const approvedStatus = new StatusEx();
        approvedStatus.Status = 'Pending_e';
        if (this.getValidationState() === 'success') {
            const corpToAdd = new CorporateUi();
            corpToAdd.title = this.state.corporateName;
            corpToAdd.icon = this.state.corporateIcon;
            corpToAdd.id = generateId();
            corpToAdd.status = approvedStatus;
            if (this.state.corporateContextForMenu)
                corpToAdd.parentId = this.state.corporateContextForMenu.id;
            this.props.actions.addCorporate(corpToAdd);
        }
        this.closeModal();
    };

    closeModal = () => {
        this.setState({
            showAddCorporateModal: false,
            showDeleteSelectedModal: false,
            corporateName: "",
            corporateId: "",
            corporateIcon: "",
            corporateContextForMenu: null
        });
    };

    handleDeleteCorporate = () => {
        //If the inputs were validated, store the data
        //if (this.getValidationState() === 'success') {
        //OOR TODO: Some sort of confirmation?          
        this.props.actions.deleteCorporate(
            this.state.corporateId
        );
        //}
        //Regardless of the user seleciton, close the modal
        this.setState({ showAddCorporateModal: false, showDeleteSelectedModal: false, corporateName: "", corporateId: "", corporateIcon: "" });
    };

    public render() {
        const columnItems = this.props.corporates.corporateList
            .filter(corp => !corp.parentId)
            .sort(SortByStatus)
            .map((entry, i) => {

                const subsidOptions: IMenuOption[] = [
                    {
                        label: "Add Subsidiary",
                        handler: () => { this.handleOpenAddModal(entry) }
                    },
                    { label: "Delete", handler: () => this.handleOpenDeleteModal(entry.id) }
                ];
                const contextMenu = this.props.auth.role == 'admin'
                    ? <ContextMenu options={subsidOptions} suppressEventPropagation={true} />
                    : null;

                const hasSubsidiaries = entry.subsids && entry.subsids.length >= 1;
                const menus = <div>
                    {contextMenu}
                    {hasSubsidiaries && <RightArrowButton onSelect={() => this.props.viewSubsids(entry.id)} />}
                </div>

                return <ColumnItemWithStatus
                    key={i}
                    itemType={ColumnItemTypeEnum.Corporate}
                    onClick={() => this.onCorporateClick(entry.id)}
                    selected={entry.id === this.props.corporates.selectedCorporate.id}
                    icon={entry.icon}
                    status={entry.status}
                    title={entry.title}
                    rightContent={menus} >
                </ColumnItemWithStatus>
            });

        const addCorporateModal = <Modal show={this.state.showAddCorporateModal} onHide={this.closeModal}>
            <Modal.Header closeButton>
                <Modal.Title>Add a corporate</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <form>
                    <FormGroup controlId="formBasicText" validationState={this.getValidationState()}>
                        <ControlLabel>Please enter a corporate name</ControlLabel>
                        <FormControl
                            type="text"
                            value={this.state.corporateName}
                            placeholder="Enter text"
                            onChange={this.handleCorporateNameChange}
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
                <Button onClick={this.handleAddCorporate}>Add</Button>
            </Modal.Footer>
        </Modal>;

        return <div className="__column">
            <div className="__column_header">
                <h3>Filter by Company</h3>
                {this.props.auth.role == 'admin' && <Glyphicon glyph='plus' onClick={this.handleOpenAddModal} />}
            </div>
            {columnItems}
            {addCorporateModal}
            <ConfirmModal
                showModal={this.state.showDeleteSelectedModal}
                closeModal={this.closeModal}
                confirmHandler={this.handleDeleteCorporate}
                modalTitle="Are you sure you want to delete this corporate?" />
        </div>;
    }
}
export default withRedux(CorporateColumn);
