import * as React from 'react';
import { Clearfix, ControlLabel, Navbar, Nav, NavItem, NavDropdown, Modal, MenuItem, Glyphicon, FormGroup, FormControl, Button, Media, Label, DropdownButton, ButtonToolbar, Dropdown } from 'react-bootstrap';
import StatusMapping, { StatusColour } from '../typeMappings';
import { StatusEx } from '../backendTypes';
import { ContextMenu, IMenuOption } from './ContextMenu';
import { DocumentUi } from '../store/documentState';
import { Link, withRouter } from 'react-router-dom';
import { StatusLabel } from './StatusLabel';
import { DaysOverdue } from './DaysOverdue';
import { withRedux, ReduxProps } from './withReduxStore';
import { ColumnItem } from './ColumnItem';

type OwnProps = {
    icon: string;
    selected: boolean;
    document: DocumentUi;
    onClick: () => void;
    onDocumentClick: Function;
    onDelete: Function;
    requestContent: Function;
}

type RouterProps = {
    name: string;
    type: string;
    match: any;
    history: any;
    router: any;
    location: any
};

type Props = OwnProps & ReduxProps & RouterProps;

class DocumentColumnItem extends React.Component<Props, void> {

    handleUploadDocumentClick = () => {
        if (this.props.onDocumentClick) {
            this.props.onDocumentClick();
        }
    }

    handleDeleteDocumentClick = () => {
        if (this.props.onDelete) {
            this.props.onDelete();
        }
    }

    handleSignDocumentClick = () => {
        this.props.actions.signDocument(this.props.document.id);
    }

    onSelectAlert = (eventKey) => {
        alert(`Alert from menu item.\neventKey: ${eventKey}`);
    }

    getDays(titleLength) {
        return Math.ceil(titleLength / 3);
    }

    viewDoc = (contentId: string) => {
        this.props.history.push('/viewDoc', { contentId });
    }

    public render() {
        const { documentBase64, documentContentId, status, owner, title } = this.props.document;

        const contentExistsForDoc = documentContentId != "" && documentContentId != null;

        const itemIcon = contentExistsForDoc
            ? <Link to={{ pathname: '/viewdoc', state: { contentId: documentContentId } }}>
                <Glyphicon glyph='eye-open' className='column_item_icon' />
            </Link>
            : <Glyphicon glyph='question-sign' className='column_item_icon' />


        const menuOptions: IMenuOption[] = [
            { label: "Delete", handler: this.handleDeleteDocumentClick },
            { label: "Upload New Version", handler: this.handleUploadDocumentClick },
        ];

        //only allow user to sign document if one exists
        if (contentExistsForDoc) {
            menuOptions.push({ label: "Digitally Sign", handler: this.handleSignDocumentClick })
        }

        const statusColour = StatusMapping.get(this.props.document.status.Status).statusColour;

        const dummyDays = this.getDays(title.length); //use title length to generate a number of days
        const overdueDays = status.Status !== 'Approved_e' ? <DaysOverdue statusColor={statusColour} days={dummyDays} /> : null;
        const ownerAndStatusLabel = [<p>{owner}</p>,
        <StatusLabel contentExists={contentExistsForDoc} status={status} />]

        return (
            <ColumnItem {...this.props.document}
                leftContent={overdueDays}
                bodyContent={ownerAndStatusLabel}
                icon={itemIcon}
                customClassName={"status " + statusColour}
                rightContent={<ContextMenu options={menuOptions} suppressEventPropagation={false} />}
                onClick={() => this.props.onClick()}
                selected={this.props.selected}
            />

        );

    }
}

export default withRedux(DocumentColumnItem);