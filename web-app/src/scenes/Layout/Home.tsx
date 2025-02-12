import { Breadcrumb, Glyphicon } from 'react-bootstrap';
import * as React from 'react';
import * as AuthState from '../../store/authStore';
import { ApplicationState } from '../../store/index';
import { connect } from 'react-redux';

// At runtime, Redux will merge together...
type HomeProps =
    AuthState.AuthState                // ... state we've requested from the Redux store
    & typeof AuthState.actionCreators   // ... plus action creators we've ted  
    & {                                 // ... plus incoming routing parameters
        name: string,
        type: string;
        params: any;
    };

interface IHomeState {

}

export class Home extends React.Component<HomeProps, IHomeState> {
    public render() {
        //Create test data. TODO: Fetch this from the database
        const user = this.props.displayName;

        return <div>
            <Breadcrumb>
                <Breadcrumb.Item active href="#">
                    <Glyphicon glyph="home" />
                </Breadcrumb.Item>
            </Breadcrumb>
        </div>;
    }
}

export default connect(
    (state: ApplicationState) => state.auth,            // Selects which state properties are merged into the component's props
    AuthState.actionCreators                // Selects which action creators are merged into the component's props
)(Home);
