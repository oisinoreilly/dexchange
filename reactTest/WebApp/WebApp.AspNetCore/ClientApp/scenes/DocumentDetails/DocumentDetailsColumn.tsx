import * as React from 'react';
import { withRedux, ReduxProps } from '../../shared/withReduxStore';
import { Tab, Tabs, Button, Table, Modal } from 'react-bootstrap';
import { withRouter } from 'react-router-dom';

import ChatColumn from './ChatColumn';
import { Footer } from './ChatFooter';
import { toLocalDateStringUS } from '../../Utils';
 
type Props = ReduxProps & {
    name: string;
    type: string;
    match: any;
    history: any;
    router: any
};

type State = {
    showDownloadModal: boolean;
    contentIdForDownload: string;
    docNameForDownload: string;
}

class DocumentDetailsColumn extends React.Component<Props, State> {

    state = {
        showDownloadModal: false,
        contentIdForDownload: "",
        docNameForDownload: ""
    }

    downloadFile(contentId) {
        const data = this.props.documents.downloadedDocuments[contentId];
        const blob: Blob = new Blob([data]);
        const fileName = this.props.documents.selectedDocument.title;
        const objectUrl = URL.createObjectURL(blob);
        const a: HTMLAnchorElement = document.createElement('a') as HTMLAnchorElement;

        a.href = data;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();

        document.body.removeChild(a);
        URL.revokeObjectURL(objectUrl);
    }

    viewDoc = (contentId: string) => {
        this.props.history.push('/viewDoc', { contentId });
    }

    downloadDoc = (contentIdForDownload: string) => {
        const { downloadedDocuments, selectedDocument } = this.props.documents;
        const documentDownloaded = downloadedDocuments[contentIdForDownload];

        if (!documentDownloaded) {
            this.props.actions.requestContent(selectedDocument.id, contentIdForDownload);
            this.setState({
                showDownloadModal: true,
                contentIdForDownload,
                docNameForDownload: selectedDocument.title
            })
        }
        else {
            this.downloadFile(contentIdForDownload);
        }
    }

    closeDownloadModal = () => {
        this.setState({
            showDownloadModal: false,
            contentIdForDownload: "",
            docNameForDownload: ""
        })
    }

    renderVersionHistory() {
        const { selectedDocument } = this.props.documents;
        const hasVerions = selectedDocument.Versions && selectedDocument.Versions.length > 0; 

        if (!hasVerions)
            return <p>No versions found</p>

        return <Table striped >
            <tbody>
                {selectedDocument.Versions.map(version => {
                    return <tr>
                        <td>{version.Creation ? toLocalDateStringUS(version.Creation) : "No creation date found" }</td>
                        <td>
                            <a className="is-link" onClick={() => this.viewDoc(version.DocumentContentId)}>
                                View
                            </a>
                        </td>
                        <td>
                            <a className="is-link" onClick={() => this.downloadDoc(version.DocumentContentId)}>
                                Download
                        </a>
                        </td>
                    </tr>
                })}
            </tbody>
        </Table>
    }

    render() {
        const { selectedDocument } = this.props.documents;
        const { downloadedDocuments } = this.props.documents;
        const { contentIdForDownload } = this.state;

        const downloadModal = <Modal show={this.state.showDownloadModal} onHide={this.closeDownloadModal}>
            <Modal.Header closeButton>
                <Modal.Title>Download</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                {downloadedDocuments[this.state.contentIdForDownload]
                    ? <a className="is-link" onClick={() => this.downloadFile(contentIdForDownload)}>
                        <i className="fa fa-file fa-4x"></i>
                        {" " + this.state.docNameForDownload}
                    </a>
                    : <span><i className="fa fa-4x fa-spinner fa-pulse"></i></span>}
            </Modal.Body>
            <Modal.Footer>
                <Button onClick={this.closeDownloadModal}>Close</Button>
            </Modal.Footer>
        </Modal>;

        return <Tabs className="__main_panel_NavBar_Docs" defaultActiveKey={1} id="uncontrolled-tab-example">
            <Tab className="__main_panel_Tab" eventKey={1} title="Chat">
                <ChatColumn></ChatColumn>
                <Footer
                    documentId={selectedDocument.id}
                    fromUser={this.props.auth.displayName}
                    addChatMessage={this.props.actions.addChatMessage}>
                </Footer>
            </Tab>
            <Tab className="__main_panel_Tab" eventKey={2} title="History">
                {downloadModal}
                {this.renderVersionHistory()}
            </Tab>
            <Tab eventKey={3} title="Details" disabled>
            </Tab>
        </Tabs>
    }
}

export default withRouter(withRedux(DocumentDetailsColumn));