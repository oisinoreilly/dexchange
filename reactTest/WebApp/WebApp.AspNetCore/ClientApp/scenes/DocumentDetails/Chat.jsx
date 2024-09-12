import * as React from 'react';
import { withRedux } from '../../shared/withReduxStore';
import { Tab, Tabs, Table } from 'react-bootstrap';
import ChatColumn from '../Chat/ChatColumn';
import { Footer } from '../Chat/ChatFooter';
class DocumentDetails extends React.Component {
    renderVersionHistory() {
        const { selectedDocument } = this.props.documents;
        return <Table striped>
            <tbody>
                {selectedDocument.Versions.map(version => {
            return <tr>
                        <td>{version.Creation}</td>
                        <td>View</td>
                        <td>Download</td>
                    </tr>;
        })}
            </tbody>
        </Table>;
    }
    render() {
        const { selectedDocument } = this.props.documents;
        return <Tabs className="__main_panel_NavBar_Docs" defaultActiveKey={1} id="uncontrolled-tab-example">
            <Tab className="__main_panel_Tab" eventKey={1} title="Chat">
                <ChatColumn></ChatColumn>
                <Footer documentId={selectedDocument.id} fromUser={this.props.auth.displayName} addChatMessage={this.props.actions.addChatMessage}>
                </Footer>
            </Tab>
            <Tab className="__main_panel_Tab" eventKey={2} title="History">
                {selectedDocument.Versions && this.renderVersionHistory()}
            </Tab>
            <Tab eventKey={3} title="Details" disabled>
            </Tab>
        </Tabs>;
    }
}
export default withRedux(DocumentDetails);
//# sourceMappingURL=Chat.jsx.map