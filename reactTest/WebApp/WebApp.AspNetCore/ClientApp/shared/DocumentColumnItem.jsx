import * as React from 'react';
import { Glyphicon } from 'react-bootstrap';
import StatusMapping from '../typeMappings';
import { ContextMenu } from './ContextMenu';
import { Link } from 'react-router-dom';
import { StatusLabel } from './StatusLabel';
import { DaysOverdue } from './DaysOverdue';
import { withRedux } from './withReduxStore';
import { ColumnItem } from './ColumnItem';
class DocumentColumnItem extends React.Component {
    constructor() {
        super(...arguments);
        this.handleUploadDocumentClick = () => {
            if (this.props.onDocumentClick) {
                this.props.onDocumentClick();
            }
        };
        this.handleDeleteDocumentClick = () => {
            if (this.props.onDelete) {
                this.props.onDelete();
            }
        };
        this.handleSignDocumentClick = () => {
            this.props.actions.signDocument(this.props.document.id);
        };
        this.onSelectAlert = (eventKey) => {
            alert(`Alert from menu item.\neventKey: ${eventKey}`);
        };
        this.viewDoc = (contentId) => {
            this.props.history.push('/viewDoc', { contentId });
        };
    }
    getDays(titleLength) {
        return Math.ceil(titleLength / 3);
    }
    render() {
        const { documentBase64, documentContentId, status, owner, title } = this.props.document;
        const contentExistsForDoc = documentContentId != "" && documentContentId != null;
        const itemIcon = contentExistsForDoc
            ? <Link to={{ pathname: '/viewdoc', state: { contentId: documentContentId } }}>
                <Glyphicon glyph='eye-open' className='column_item_icon'/>
            </Link>
            : <Glyphicon glyph='question-sign' className='column_item_icon'/>;
        const menuOptions = [
            { label: "Delete", handler: this.handleDeleteDocumentClick },
            { label: "Upload New Version", handler: this.handleUploadDocumentClick },
        ];
        //only allow user to sign document if one exists
        if (contentExistsForDoc) {
            menuOptions.push({ label: "Digitally Sign", handler: this.handleSignDocumentClick });
        }
        const statusColour = StatusMapping.get(this.props.document.status.Status).statusColour;
        const dummyDays = this.getDays(title.length); //use title length to generate a number of days
        const overdueDays = status.Status !== 'Approved_e' ? <DaysOverdue statusColor={statusColour} days={dummyDays}/> : null;
        const ownerAndStatusLabel = [<p>{owner}</p>,
            <StatusLabel contentExists={contentExistsForDoc} status={status}/>];
        return (<ColumnItem {...this.props.document} leftContent={overdueDays} bodyContent={ownerAndStatusLabel} icon={itemIcon} customClassName={"status " + statusColour} rightContent={<ContextMenu options={menuOptions} suppressEventPropagation={false}/>} onClick={() => this.props.onClick()} selected={this.props.selected}/>);
    }
}
export default withRedux(DocumentColumnItem);
//# sourceMappingURL=DocumentColumnItem.jsx.map