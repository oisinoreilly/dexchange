import { Button, ButtonGroup, Glyphicon, MediaList, Media, Modal } from 'react-bootstrap';
import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../store/index';
import * as NotificationsStore from '../store/notificationsStore';
import { NotificationType, Status, StatusEx } from '../backendTypes';
import StatusMapping from '../typeMappings';
import configureStore from '../configureStore';


interface INotificationSate {
    showModal: boolean;
}

interface OwnProps {
    showModal: boolean;
    hideModal: Function;
    entity: string;
    updateDocumentStatus: Function;
}

type NotificationsProps =
    NotificationsStore.NotificationsState  // ... state we've requested from the Redux store
    & typeof NotificationsStore.actionCreators   // ... plus action creators we've ted  
    & {                                 // ... plus incoming routing parameters
        name: string,
        type: string,
        location: any,
        params: any;
        showModal: boolean;
        hideModal: Function;
        entity: string;
        updateDocumentStatus: Function;
    }

export class Notifications extends React.Component<NotificationsProps, INotificationSate> {
    state = { showModal: false }

    componentWillMount() {
        // TODO: Obtain corporate ID to filter notifications.
        // TODO: show correct notifications based on existence of bank/corporate id
        if (this.props.entity === "corporate") this.props.requestNotificationsForCorporate("");
        if (this.props.entity === "bank") this.props.requestNotificationsForBank("");
    }

    handleApprove = (notification: NotificationsStore.Notification) => {
        const status = new StatusEx();
        status.Status = "Approved_e";
        this.props.updateDocumentStatus(notification.document, status);
    }

    handleReject = (notification: NotificationsStore.Notification) => {
        const status = new StatusEx();
        status.Status = "Rejected_e";
        this.props.updateDocumentStatus(notification.document, status);
    }

    getIcon(status) {
        const statusStyling = StatusMapping.get(status);
        const iconCSSClassName = "notifications__icon--" + statusStyling.statusColour;
        return <i className={`fa fa-2x fa-${statusStyling.iconType} ${iconCSSClassName} notifications__icon`} aria-hidden="false"></i>;
    }

    renderNotification = (notification: NotificationsStore.Notification) => {
        let status = "Approved_e" as Status;
        let message = "";
        let dateCreated = "";
        let processed = false;

        if (notification != null) {
            status = notification.status;
            message = notification.message;
            dateCreated = notification.dateCreated;
            processed = notification.processed;
        }

        //notification requires action if it has not been processed and it is in a pending state
        const notificationIsActionable = status === "Pending_e";
        const notificationRequiresAction = !processed && notificationIsActionable;
        const needActionClass = notificationRequiresAction ? "notification--requires_action" : "";

        return (
            <div className={needActionClass}>
                <Media.ListItem>
                    <Media.Left align="middle">
                        {this.getIcon(status)}
                    </Media.Left>
                    <Media.Body>
                        <Media.Heading>{message}</Media.Heading>
                        <p>{dateCreated}</p>
                    </Media.Body>
                    <Media.Right>
                        {notificationRequiresAction &&
                            <ButtonGroup className='notifications__buttongroup--horizontal'>
                                <Button bsStyle="success" onClick={() => this.handleApprove(notification)}><Glyphicon glyph='ok' /></Button>
                                <Button bsStyle="danger" onClick={() => this.handleReject(notification)}><Glyphicon glyph='remove' /></Button>
                            </ButtonGroup>
                        }
                    </Media.Right>
                </Media.ListItem>
            </div>
        )
    }

    public render() {
        return (
            <Modal className="notifications" show={this.props.showModal} onHide={this.props.hideModal}>
                <Modal.Header>
                    <Modal.Title>
                        Notifications
                        <span className="pull-right">
                            <Glyphicon glyph="remove" onClick={() => this.props.hideModal()} />
                        </span>
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Media.List>
                        {this.props.notificationsList.map(this.renderNotification)}
                    </Media.List>
                </Modal.Body>
                <Modal.Footer>
                    <Button bsStyle='primary' onClick={() => this.props.hideModal()}>Close</Button>
                </Modal.Footer>
            </Modal>
        )
    }
}

const mergeProps = (state, actions, ownProps) => ({
    ...state,
    ...actions,
    ...ownProps
});

//Extend the login screen with the redux state
export default connect(
    (state: ApplicationState) => state.notifications,            // Selects which state properties are merged into the component's props
    NotificationsStore.actionCreators,               // Selects which action creators are merged into the component's props
    mergeProps
)(Notifications);

