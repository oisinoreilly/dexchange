import * as React from 'react';
import { ColumnItemWithStatus } from '../../shared/ColumnItemWithStatus';
import { ContextMenu } from '../../shared/ContextMenu';
import { CorporateUi } from '../../store/corporateState';
import { Button, Modal, FormGroup, ControlLabel, FormControl } from 'react-bootstrap';
import { SortByStatus } from '../../typeMappings';
export class AccordionColumn extends React.Component {
    constructor(props) {
        super(props);
        this.closeModal = () => {
            this.setState({
                showAddSubsidModal: false,
                subsidIcon: "",
                subsidName: "",
                subsidContextForMenu: null
            });
        };
        this.showAddModal = (activeSubsid) => {
            this.setState({
                subsidContextForMenu: activeSubsid,
                showAddSubsidModal: true
            });
        };
        this.handleAdd = (e) => {
            e.preventDefault();
            const corpToAdd = new CorporateUi();
            corpToAdd.icon = this.state.subsidIcon;
            corpToAdd.parentId = this.state.subsidContextForMenu.id;
            corpToAdd.title = this.state.subsidName;
            corpToAdd.id = this.getRandomInt(0, 999);
            this.props.addSubsid(corpToAdd);
            this.closeModal();
            this.props.onClick(this.state.subsidContextForMenu);
        };
        this.handleSubsidNameChange = (e) => {
            this.setState({ subsidName: e.target.value });
        };
        this.handleChangeImage = (evt) => {
            let reader = new FileReader();
            let file = evt.target.files[0];
            reader.onloadend = () => {
                let result = reader.result;
                this.setState({
                    subsidIcon: result
                });
            };
            reader.readAsDataURL(file);
        };
        this.state = {
            showAddSubsidModal: false,
            subsidIcon: "",
            subsidName: "",
            subsidContextForMenu: null
        };
    }
    getRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }
    render() {
        const addModal = <Modal show={this.state.showAddSubsidModal} onHide={this.closeModal}>
            <Modal.Header closeButton>
                <Modal.Title>Add a subsidiary</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <form>
                    <FormGroup controlId="formBasicText">
                        <ControlLabel>Please enter the subsidiary name</ControlLabel>
                        <FormControl type="text" value={this.state.subsidName} placeholder="Subsidiary Name" onChange={this.handleSubsidNameChange}/>
                        <FormControl.Feedback />
                    </FormGroup>
                    <FormGroup controlId="formBasicFile">
                        <ControlLabel>Please choose an icon</ControlLabel>
                        <FormControl type="file" placeholder="Select image" onChange={this.handleChangeImage}/>
                        <FormControl.Feedback />
                    </FormGroup>
                </form>
            </Modal.Body>
            <Modal.Footer>
                <Button bsStyle='primary' onClick={this.handleAdd}>Add</Button>
            </Modal.Footer>
        </Modal>;
        const columnItems = this.props.corporates
            .filter(corp => corp.parentId === this.props.id)
            .sort(SortByStatus)
            .map((entry, i) => {
            const subsidOptions = [
                {
                    label: "Add Subsidiary",
                    handler: () => { this.showAddModal(entry); }
                },
            ];
            const contextMenu = this.props.isAdmin
                ? <ContextMenu options={subsidOptions} suppressEventPropagation={true}/>
                : null;
            return <ColumnItemWithStatus key={i} itemType={null} onClick={() => this.props.onClick(entry)} selected={entry.id === this.props.selectedId} icon={entry.icon} status={entry.status} title={entry.title} rightContent={contextMenu}>
                </ColumnItemWithStatus>;
        });
        return <div className="accordion_column">
            {columnItems}
            {addModal}
        </div>;
    }
}
export default AccordionColumn;
//# sourceMappingURL=AccordionColumn.jsx.map