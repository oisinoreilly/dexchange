import * as React from 'react';
import { ListGroup, ListGroupItem, Panel, ControlLabel, FormControl, Glyphicon, Button, FormGroup, Checkbox, Modal } from 'react-bootstrap';
import { Item } from '../../frontendTypes';

interface State {
    selectedIds: number[]
}

interface ModalProps {
    onHide: Function;
    onAdd: Function;
    alreadyInGroup: any[];
    selectedGroupId: string;
    items: Item[];
}

class AddToGroupModal extends React.Component<ModalProps, State>{
    constructor(props) {
        super(props);
        this.state = {
            selectedIds: []
        }
    }

    handleAdd = (res) => {
        this.props.onAdd(this.state.selectedIds, this.props.selectedGroupId);
        this.props.onHide();
    }

    handleChange = (event, id) => {
        const inArray = this.state.selectedIds.indexOf(id) !== -1
        if (event.target.checked && !inArray) {
            this.setState({
                selectedIds: this.state.selectedIds.concat([id])
            })
        }
        else {
            this.setState({
                selectedIds: this.state.selectedIds.filter(item => item !== id)
            })
        }
    }

    render() {
        const inGroup = this.props.alreadyInGroup;
        const notIngroup = this.props.items
            .filter(item => inGroup.indexOf(item) === -1);

        return <Modal show={true} onHide={this.props.onHide}>
            <Modal.Header closeButton>
                <Modal.Title>Select items to add to group</Modal.Title>
            </Modal.Header>
            <Modal.Body >
                <form>
                    {notIngroup.map(item =>
                        <Checkbox onChange={e => this.handleChange(e, item.Id)}> {item.Name}</Checkbox>)
                    }
                </form>
            </Modal.Body>
            <Modal.Footer >
                <Button onClick={this.handleAdd}>Add</Button>
            </Modal.Footer>
        </Modal>
    }
}

export default AddToGroupModal;
