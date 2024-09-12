import * as React from 'react';
//import { Document } from 'react-pdf/build/entry.webpack';
import { connect } from 'react-redux';
import { Panel, Button } from 'react-bootstrap';
import { withRouter } from 'react-router-dom';

import * as DocumentState from '../store/documentState';
import { ApplicationState } from '../store/index';
import DocumentDetailsColumn from '../scenes/DocumentDetails/DocumentDetailsColumn';
import { StatusLabel } from './StatusLabel';
import MainPanel from './MainPanel';
import './PDFViewer.scss';
import { withRedux, ReduxProps } from './withReduxStore';


type PDFViewerProps =
    ReduxProps
    & {
        name: string;
        type: string;
        match: any;
        history: any;
        location: any;
    };   

export class PDFViewer extends React.Component<PDFViewerProps, null> {

    handleClose = () => {
        this.props.history.goBack();
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
        return <iframe className="pdf-viewer" src={downloadedDocuments[contentId]}
            height="600px" />;
    }

    render() {
        const { downloadedDocuments } = this.props.documents;
        const { contentId } = this.props.location.state;


        const viewerContent = downloadedDocuments[contentId]
            ? this.getDoc(contentId, downloadedDocuments)
            : <span className="icon-container"><i className="fa fa-4x fa-spinner fa-pulse"></i></span>

        return (
            <MainPanel>
                <div className="__columns">
                    <div className="pdf-container">
                        <div className="left-absolute"><Button onClick={this.handleClose}>Close</Button></div>
                        {viewerContent}
                    </div>
                    <DocumentDetailsColumn />
                </div>
            </MainPanel>
        );
    }
}

export default withRouter(withRedux(PDFViewer));
