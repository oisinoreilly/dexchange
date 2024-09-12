import { Breadcrumb, Glyphicon } from 'react-bootstrap';
import * as React from 'react';
import * as AuthState from '../../store/authStore';
import { connect } from 'react-redux';
export class Home extends React.Component {
    render() {
        //Create test data. TODO: Fetch this from the database
        const user = this.props.displayName;
        return <div>
            <Breadcrumb>
                <Breadcrumb.Item active href="#">
                    <Glyphicon glyph="home"/>
                </Breadcrumb.Item>
            </Breadcrumb>
        </div>;
    }
}
export default connect((state) => state.auth, // Selects which state properties are merged into the component's props
AuthState.actionCreators // Selects which action creators are merged into the component's props
)(Home);
//# sourceMappingURL=Home.jsx.map