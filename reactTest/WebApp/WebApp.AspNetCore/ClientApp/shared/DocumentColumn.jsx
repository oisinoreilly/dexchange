import * as React from 'react';
import DocumentColumnItem from './DocumentColumnItem';
import { Glyphicon, FormGroup, FormControl, Button, Modal, ControlLabel } from 'react-bootstrap';
import { DocumentUi } from '../store/documentState';
import { StatusEx } from '../backendTypes';
import { SortByStatus } from '../typeMappings';
export class DocumentColumn extends React.Component {
    constructor(props) {
        super(props);
        this.getValidationState = () => {
            const length = this.state.title.length;
            if (length > 2)
                return 'success';
            else if (length > 1)
                return 'warning';
            else if (length >= 0)
                return 'error';
        };
        ///Process any changes to the bank name from the modal dialog
        this.handleDocTitleChange = (e) => {
            this.setState({
                title: e.target.value,
            });
        };
        ///Process any changes to the bank name from the modal dialog
        this.handleDocTypeChange = (e) => {
            this.setState({
                type: e.target.value,
            });
        };
        this.handleQueryTypeChange = (e) => {
            this.setState({
                queryType: e.target.value,
            });
        };
        this.handleDocOwnerChange = (e) => {
            this.setState({
                owner: e.target.value,
            });
        };
        this.handleDocDaysOverdueChange = (e) => {
            this.setState({
                daysOverdue: e.target.value,
            });
        };
        //Handle the opening of the modal dialog
        this.modalOpen = () => {
            this.setState({
                showNewDocumentModal: true,
                showUploadModal: this.state.showUploadModal,
                showDeleteModal: this.state.showDeleteModal,
                title: "",
                owner: "",
                type: "",
                daysOverdue: 0,
                queryType: "",
                documentBase64: "",
                id: this.state.id
            });
        };
        this.modalDeleteClose = () => {
            this.props.deleteDocument(this.state.id);
            this.setState({ showNewDocumentModal: false,
                showUploadModal: false,
                showDeleteModal: false,
                title: "",
                owner: "",
                type: "",
                daysOverdue: 0,
                queryType: "",
                documentBase64: this.state.documentBase64,
                id: "" });
        };
        //Handle validation and closing of the dialog
        this.closeNewDocumentModal = () => {
            if (this.getValidationState() === 'success') {
                // Set status to rejected if there is no document set.
                const newStatus = new StatusEx();
                if ((null == this.state.documentBase64) || ("" == this.state.documentBase64)) {
                    newStatus.Status = "Rejected_e";
                }
                else {
                    newStatus.Status = "Pending_e";
                }
                const documentToAdd = new DocumentUi();
                documentToAdd.id = this.getRandomInt(2000, 2999).toString();
                documentToAdd.accountId = this.props.selectedAccount.id;
                documentToAdd.icon = "";
                documentToAdd.title = this.state.title;
                //documentToAdd.owner = this.state.owner ? this.state.owner : "Me";
                documentToAdd.type = this.state.type ? this.state.type : "Report";
                documentToAdd.daysOverdue = this.state.daysOverdue;
                documentToAdd.documentBase64 = this.state.documentBase64;
                documentToAdd.status = newStatus;
                this.props.addDocument(this.props.selectedAccount.id, documentToAdd);
            }
            //Regardless of the user selection, close the modal
            this.setState({
                showNewDocumentModal: false,
                title: "",
                owner: "",
                type: "",
                daysOverdue: 0,
                queryType: "",
                documentBase64: this.state.documentBase64,
                id: ""
            });
        };
        this.modalCloseNoAction = () => {
            // Close the modal       
            this.setState({ showNewDocumentModal: false,
                showUploadModal: false,
                showDeleteModal: false,
                title: "",
                owner: "",
                type: "",
                daysOverdue: 0,
                queryType: "",
                documentBase64: "",
                id: "" });
        };
        //Read the file as base64 and save it against the file contents
        this.handleChangeDocumentUpload = (evt) => {
            let reader = new FileReader();
            let file = evt.target.files[0];
            reader.onloadend = () => {
                let result = reader.result;
                this.setState({
                    documentBase64: result,
                });
            };
            reader.readAsDataURL(file);
        };
        this.onDocumentClick = (index) => {
            this.setState({
                id: index,
            });
            this.props.selectDocument(index);
        };
        this.onUploadDocumentClick = (document) => {
            //Update with the current state of the doc to be updated
            this.setState({
                showUploadModal: true,
                showDeleteModal: false,
                title: document.title,
                owner: document.owner,
                type: document.type,
                daysOverdue: document.daysOverdue,
                documentBase64: document.documentBase64,
                id: this.state.id
            });
        };
        this.handleUploadDocument = () => {
            this.props.uploadDocument(this.props.selectedDocument.id, this.state.documentBase64);
            this.setState({
                showUploadModal: false,
                showDeleteModal: false
            });
        };
        this.handleModalClose = () => {
            this.setState({
                showUploadModal: false,
                showDeleteModal: false
            });
        };
        this.state = {
            showNewDocumentModal: false,
            showUploadModal: false,
            showDeleteModal: false,
            title: "",
            owner: "Me",
            type: "Report",
            daysOverdue: 0,
            queryType: "DocumentUi Missing",
            documentBase64: "",
            id: ""
        };
    }
    getRandomInt(min, max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }
    modalDeleteSelectedOpen() {
        this.setState({ showDeleteModal: true });
    }
    render() {
        const documentTypes = [
            'Certificate of Incorporation',
            'Memorandum and Articles of Association',
            'CRS Form',
            'Audited financial accounts',
            'Shareholders list',
            'Directors list',
            'Authorised signatories list',
            'Corporate account opening form',
            'W9 Form',
            'Proof of identity',
            'Proof of address',
        ];
        const docTypes = documentTypes.map((docType) => <option value="{docType}">{docType}</option>);
        const newDocumentModal = <Modal show={this.state.showNewDocumentModal} onHide={this.closeNewDocumentModal}>
            <Modal.Header closeButton>
                <Modal.Title>Add a document</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <form>
                    <FormGroup controlId="formBasicText" validationState={this.getValidationState()}>
                        <ControlLabel>Please enter an document name</ControlLabel>
                        <FormControl type="text" value={this.state.title} placeholder="Enter text" onChange={this.handleDocTitleChange}/>
                        <FormControl.Feedback />
                    </FormGroup>
                    <FormGroup controlId="formControlsSelectDocType">
                        <ControlLabel>Document Type</ControlLabel>
                        <FormControl componentClass="select" placeholder="Choose Type" onChange={this.handleDocTypeChange}>
                            {docTypes}
                        </FormControl>
                    </FormGroup>
                    <FormGroup controlId="formBasicFile">
                        <ControlLabel>Please upload a document</ControlLabel>
                        <FormControl type="file" placeholder="Please upload a document" accept="application/pdf" onChange={this.handleChangeDocumentUpload}/>
                        <FormControl.Feedback />
                    </FormGroup>
                </form>
            </Modal.Body>
            <Modal.Footer>
                <Button onClick={this.closeNewDocumentModal}>Add</Button>
            </Modal.Footer>
        </Modal>;
        const uploadDocumentModal = <Modal show={this.state.showUploadModal} onHide={this.handleModalClose}>
            <Modal.Header closeButton>
                <Modal.Title>Upload a document</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <form>
                    <FormGroup controlId="formBasicFile">
                        <ControlLabel>Please upload a document</ControlLabel>
                        <FormControl type="file" placeholder="please upload a document" accept="application/pdf" onChange={this.handleChangeDocumentUpload}/>
                        <FormControl.Feedback />
                    </FormGroup>
                </form>
            </Modal.Body>
            <Modal.Footer>
                <Button onClick={this.handleModalClose}>Close</Button>
                <Button bsStyle="primary" onClick={this.handleUploadDocument}>
                    <Glyphicon glyph="cloud-upload"/> Upload
                </Button>
            </Modal.Footer>
        </Modal>;
        const deleteSelectedModal = <Modal show={this.state.showDeleteModal} onHide={this.modalCloseNoAction}>
            <Modal.Header closeButton>
                <Modal.Title>Delete Selected Document</Modal.Title>
            </Modal.Header>
            <Modal.Footer>
                <Button onClick={this.modalDeleteClose}>Delete</Button>
            </Modal.Footer>
        </Modal>;
        //TODO: Link to doc type
        const docIcon = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB4AAAAcCAIAAAD5mpj+AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAK0SURBVEhLxZTtT1JRHMf7V2rTN71wvaq2amtrvrAHV2tSc7Rp080eXkiFa7ZqrVWrLNMUFNIgKwmnrhKDhiCtLNt6UBwpJAEX4ulykcGAeLj92LkBXc69ZqP13ffFefj9Pvfsd879raP/mf4HWmai9na5ajqd/ D4oIa5OBP2RNJNWJE40cA / 3uc+PB3gM6Nq7uc / Xy9zOUIrJ / CVONCRAMjPhEIrRW2J77rhK6WVAwwBLLw8aVEovGxqE6A0DnkQqC9O / QVOxDBpATJva7wmn8pYYKVjUmqOwu2a0g0wJpO5vwR8wFsrdEFbq / mkKdteM / uxKHH / otfpy6EVvEp6 / 1Fhwh5aERBjALh49a/FsbB6uFPZvO6Fsk06RkTizQdNL3iQU1EmyXzES1IQPPTFj23Cod72gJ+8tLffz9GQ6C39Tvtws8aFT6UxVo7yYi3y2z4AC4PYP9BD2QK4gpeJDL9gDLCjyLtEjFPByISqQErd0JJqyxId2eFdYUOT97SMoQKTyzdrjcJMfHQm0UqxVal0jVrG4YMn4B9iCOjQpvsMArrFx0PN2uXC9SKugrURoa4uimNt0Q5PJ5n6wy8+Dk/PRlXjG+ CV2Ru2Dp1InIWq7XceGvNc0wUyWG20jQs03NXDqp6 + tm8VTO8XPzsmnx14tdqjeVZ96LJKYdnc6uvUh5ZvwzNc4QcGV02Z38raObB32tY / 6YYpHhyLxTUfv5U9acWRge+tY3YXRqobCgxFemWSiOYRHd428zyO4DO992RNmEnDCo + GALBDWQzozk4ATHl1cDR5fHDQxCTjh0ZX1UhYF69O9eiYBJzx6x8kHLArW + y4ZoCNyGdo3oKEdArCAVhstrK5U6op6WfX1JUjmMTRxaLYALKBBn2w + pXZe8WIO6ycGi5 + KMaF / oN / Q5RRN / wQZ3gAbW20ohwAAAABJRU5ErkJggg ==";
        const sortedDocs = [...this.props.documents].sort(SortByStatus);
        const docColumnItems = sortedDocs.map((document, i) => {
            const selectedId = this.props.selectedDocument ? this.props.selectedDocument.id : "";
            return <DocumentColumnItem key={i} onClick={() => this.onDocumentClick(document.id)} onDocumentClick={() => this.onUploadDocumentClick(document)} selected={document.id == selectedId} document={document} icon={docIcon} onDelete={() => this.modalDeleteSelectedOpen()} requestContent={() => this.props.requestContent(document.id, document.documentContentId)}>
            </DocumentColumnItem>;
        });
        return <div className="__column">
            <div className="__column_header">
                <h3>Related Documents</h3>
                <Glyphicon glyph='plus' onClick={this.modalOpen}/>
            </div>
            {docColumnItems}
            {newDocumentModal}
            {uploadDocumentModal}
            {deleteSelectedModal}
        </div>;
    }
}
//# sourceMappingURL=DocumentColumn.jsx.map