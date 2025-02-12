import * as React from 'react';

import { Modal, Button } from 'react-bootstrap';


interface Props {
    confirmHandler: (id) => void;
    modalTitle: string;
    closeModal: ()=> void;
    showModal: boolean;
}

export const ConfirmModal: React.SFC<Props> = ({ confirmHandler, modalTitle, closeModal, showModal }) => {
    return (
         <Modal show={showModal} onHide={closeModal}>
            <Modal.Header closeButton>
                <Modal.Title>{modalTitle}</Modal.Title>
        </Modal.Header>
            <Modal.Footer>
                <Button onClick={closeModal}>Cancel</Button>
                <Button bsStyle="primary" onClick={confirmHandler}>OK</Button>
        </Modal.Footer>
    </Modal>
    )
}
