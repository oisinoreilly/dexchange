import * as React from 'react';
import { withRedux } from '../../shared/withReduxStore';
import { withRouter } from 'react-router-dom';
export class Signed extends React.Component {
    componentDidMount() {
        const { actions, match, history } = this.props;
        //gets doc from docusign and uploads it to our system, on server side
        actions.uploadSignedDocument(match.params.docId, match.params.envelopeId);
        history.push('/banks');
    }
    render() {
        return <div>
            redirecting
            </div>;
    }
}
export default withRouter(withRedux(Signed));
//# sourceMappingURL=Signed.jsx.map