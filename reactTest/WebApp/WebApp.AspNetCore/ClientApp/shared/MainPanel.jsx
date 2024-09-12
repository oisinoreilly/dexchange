import * as React from 'react';
import { ControlLabel, FormControl, Form, FormGroup, Button, ButtonGroup, Badge } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import Notifications from './Notifications';
import { withRedux } from './withReduxStore';
export class MainPanel extends React.Component {
    constructor() {
        super(...arguments);
        this.state = {
            showNotificationsModal: false,
        };
        this.handleShowNotificationsModal = () => {
            this.setState({
                showNotificationsModal: true
            });
        };
        this.handleHideNotificationsModal = () => {
            this.setState({
                showNotificationsModal: false
            });
        };
    }
    render() {
        const notificationsModal = <Notifications showModal={this.state.showNotificationsModal} hideModal={this.handleHideNotificationsModal} entity="bank" updateDocumentStatus={this.props.actions.updateDocumentStatus}/>;
        const documentTitle = this.props.documents.selectedDocument && this.props.documents.selectedDocument.title
            ? this.props.documents.selectedDocument.title
            : null;
        const numberOfNotifications = this.props.notifications.notificationsList.length;
        const { EntityIcon } = this.props.auth.userConfig;
        const icon = EntityIcon
            ? <img src={EntityIcon} alt={this.props.auth.userConfig.EntityDisplayName + ' icon'}/>
            : <i className="fa fa-3x fa-globe"></i>;
        return <div className="__panel_holder">
            <div className="__main_panel">
                <div className="__main_panel_menues">
                    <div className="__main_panel__controls_container">
                        <div className="__main_panel_NavBar">
                            <div className="__main_panel_Brand" onClick={this.handleShowNotificationsModal}>
                                {icon}
                                <Badge className='__main_panel_notification_badge'>
                                    {numberOfNotifications >= 1 ? numberOfNotifications : ""}
                                </Badge>
                            </div>
                            <Form inline className="__searchForm">
                                <FormGroup controlId="formControlsSelect">
                                    <ControlLabel>Filter</ControlLabel>
                                    <FormControl componentClass="select" placeholder="showAll">
                                        <option value="showAll">Show All</option>
                                    </FormControl>
                                </FormGroup>
                                <FormGroup>
                                    <FormControl type="text" placeholder="Search"/>
                                </FormGroup>
                                {' '}
                                <Button type="submit">Submit</Button>
                            </Form>
                        </div>
                    </div>
                    <ButtonGroup className="__main_panel_document_link">
                        {this.props.documents.selectedDocument &&
            <Link to={`/viewdoc`}>
                                <div className='glyphicon glyphicon-th-list' style={{ fontSize: '24px', margin: '14px' }}></div>
                                {documentTitle}
                            </Link>}
                    </ButtonGroup>
                </div>
                {this.props.children}
                {notificationsModal}
            </div>
        </div>;
    }
    ;
}
export default withRedux(MainPanel);
//# sourceMappingURL=MainPanel.jsx.map