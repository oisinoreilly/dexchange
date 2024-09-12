import * as React from 'react';
import { Button, Checkbox, Modal } from 'react-bootstrap';
class AddToGroupModal extends React.Component {
    constructor(props) {
        super(props);
        this.handleAdd = (res) => {
            this.props.onAdd(this.state.selectedIds, this.props.selectedGroupId);
            this.props.onHide();
        };
        this.handleChange = (event, id) => {
            const inArray = this.state.selectedIds.indexOf(id) !== -1;
            if (event.target.checked && !inArray) {
                this.setState({
                    selectedIds: this.state.selectedIds.concat([id])
                });
            }
            else {
                this.setState({
                    selectedIds: this.state.selectedIds.filter(item => item !== id)
                });
            }
        };
        this.state = {
            selectedIds: []
        };
    }
    render() {
        const inGroup = this.props.alreadyInGroup;
        const notIngroup = this.props.items
            .filter(item => inGroup.indexOf(item) === -1);
        return <Modal show={true} onHide={this.props.onHide}>
            <Modal.Header closeButton>
                <Modal.Title>Select items to add to group</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <form>
                    {notIngroup.map(item => <Checkbox onChange={e => this.handleChange(e, item.Id)}> {item.Name}</Checkbox>)}
                </form>
            </Modal.Body>
            <Modal.Footer>
                <Button onClick={this.handleAdd}>Add</Button>
            </Modal.Footer>
        </Modal>;
    }
}
export default AddToGroupModal;
//# sourceMappingURL=AddToGroupModal.jsx.map