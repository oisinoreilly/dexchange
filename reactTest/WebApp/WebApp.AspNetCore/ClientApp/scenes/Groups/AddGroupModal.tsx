import * as React from 'react';
import {
    ListGroup, ListGroupItem, Panel, ControlLabel, FormControl,
    Glyphicon, Button, FormGroup, Checkbox, Modal
} from 'react-bootstrap';

interface State {
    name: string;
}

interface Props {
    onAddGroup: Function;
    onHide: Function;
}

export class AddGroupModal extends React.Component<Props, State>{
    constructor(props) {
        super(props);
        this.state= {
            name: ""
        }
    }

    handleChangeName = (e) => {
        this.setState({name: e.currentTarget.value})
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
                        <FormControl
                            type="text"
                            value={this.state.name}
                            placeholder="Enter group name"
                            onChange={this.handleChangeName}
                        />
                        <FormControl.Feedback />
                    </FormGroup>
                </form>
            </Modal.Body>
            <Modal.Footer>
                <Button onClick={() => this.props.onAddGroup(this.state.name)}>Add</Button>
            </Modal.Footer>
        </Modal>
    }
}