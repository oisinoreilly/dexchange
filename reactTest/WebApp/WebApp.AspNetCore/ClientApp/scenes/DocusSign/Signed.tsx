import * as React from 'react';
import { ReduxProps, withRedux } from '../../shared/withReduxStore';
import { withRouter } from 'react-router-dom';

type props = ReduxProps & {                                
    name: string,
    type: string;
    match: any;
    history: any;
};   

export class Signed extends React.Component<props, null>{
    componentDidMount() {
        const { actions, match, history } = this.props;
        //gets doc from docusign and uploads it to our system, on server side
        actions.uploadSignedDocument(match.params.docId, match.params.envelopeId);
        history.push('/banks');
    }

    render() {
        return <div>
            redirecting
            </div>
    }
}

export default withRouter(withRedux(Signed));