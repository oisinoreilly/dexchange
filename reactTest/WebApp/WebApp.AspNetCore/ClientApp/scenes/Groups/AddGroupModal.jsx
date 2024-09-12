import * as React from 'react';
import { ControlLabel, FormControl, Button, FormGroup, Modal } from 'react-bootstrap';
export class AddGroupModal extends React.Component {
    constructor(props) {
        super(props);
        this.handleChangeName = (e) => {
            this.setState({ name: e.currentTarget.value });
        };
        this.state = {
            name: ""
        };
    }
    render() {
        return <Modal show={true} onHide={this.props.onHide}>
            <Modal.Header closeButton>
                <Modal.Title>Add a group</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <form>
                    <FormGroup controlId="formBasicText">
                        <ControlLabel>Please enter a Group name</ControlLabel>
                        <FormControl type="text" value={this.state.name} placeholder="Enter group name" onChange={this.handleChangeName}/>
                        <FormControl.Feedback />
                    </FormGroup>
                </form>
            </Modal.Body>
            <Modal.Footer>
                <Button onClick={() => this.props.onAddGroup(this.state.name)}>Add</Button>
            </Modal.Footer>
        </Modal>;
    }
}
//# sourceMappingURL=AddGroupModal.jsx.map