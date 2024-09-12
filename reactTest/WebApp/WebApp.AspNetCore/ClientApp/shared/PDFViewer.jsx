import * as React from 'react';
import { Button } from 'react-bootstrap';
import { withRouter } from 'react-router-dom';
import DocumentDetailsColumn from '../scenes/DocumentDetails/DocumentDetailsColumn';
import MainPanel from './MainPanel';
import './PDFViewer.scss';
import { withRedux } from './withReduxStore';
export class PDFViewer extends React.Component {
    constructor() {
        super(...arguments);
        this.handleClose = () => {
            this.props.history.goBack();
        };
    }
    componentDidMount() {
        const { downloadedDocuments, selectedDocument } = this.props.documents;
        const { contentId } = this.props.location.state;
        const documentDownloaded = downloadedDocuments[contentId];
        if (!documentDownloaded) {
            this.props.actions.requestContent(selectedDocument.id, contentId);
        }
    }
    getDoc(contentId, downloadedDocuments) {
        return <iframe className="pdf-viewer" src={downloadedDocuments[contentId]} height="600px"/>;
    }
    render() {
        const { downloadedDocuments } = this.props.documents;
        const { contentId } = this.props.location.state;
        const viewerContent = downloadedDocuments[contentId]
            ? this.getDoc(contentId, downloadedDocuments)
            : <span className="icon-container"><i className="fa fa-4x fa-spinner fa-pulse"></i></span>;
        return (<MainPanel>
                <div className="__columns">
                    <div className="pdf-container">
                        <div className="left-absolute"><Button onClick={this.handleClose}>Close</Button></div>
                        {viewerContent}
                    </div>
                    <DocumentDetailsColumn />
                </div>
            </MainPanel>);
    }
}
export default withRouter(withRedux(PDFViewer));
//# sourceMappingURL=PdfViewer.jsx.map