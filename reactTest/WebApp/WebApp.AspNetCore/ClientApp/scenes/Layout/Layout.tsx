import * as React from 'react';
import { NavMenu } from './NavMenu';
import { Image, Navbar, Nav, NavItem, NavDropdown, MenuItem, Breadcrumb, Glyphicon, Alert } from 'react-bootstrap';
import * as AuthState from '../../store/authStore';
import { ApplicationState } from '../../store/index';
import { connect } from 'react-redux';
import Home from '../Layout/Home';
import Login from '../login/Login';
import BanksOverview from '../CorporateUserView/BanksOverview';
import CorporatesOverview from '../BankUserView/CorporatesOverview';
import AddRole from '../Roles/AddRole';
import Admin from '../Admin/Admin';
import Signed from '../DocusSign/Signed';
import PDFViewerTest from '../PdfViewer/PdfViewer';
import PDFViewer from '../../shared/PdfViewer';
import { Route, Link } from 'react-router-dom';
import Toasts from './Toasts';

// At runtime, Redux will merge together...
type LayoutProps =
    AuthState.AuthState                // ... state we've requested from the Redux store
    & typeof AuthState.actionCreators   // ... plus action creators we've ted  
    & {                                 // ... plus incoming routing parameters
        body: React.ReactElement<any>;
        name: string,
        type: string;
        params: any;
    };

// export interface LayoutProps {
//     body: React.ReactElement<any>;
// }

export class Layout extends React.Component<LayoutProps, null> {
    public render() {
        //Get the authstate from the session storage
        let authStateJson = sessionStorage.getItem("AuthDetails");
        let authState: AuthState.AuthState = null;
        let user = "";
        if (authStateJson) {

            authState = JSON.parse(authStateJson);
            user = "Welcome " + authState.displayName;

        }
        const { UserPrivilege, UserType } = this.props.userConfig;
        const isAdmin = UserPrivilege === "Admin" || UserPrivilege === 'SuperAdmin';
        const isBankUser = UserType === "Bank";
        const isCorporateUser = UserType === "Corporate";
        const isSuperAdmin = UserPrivilege === 'SuperAdmin';

        return <div className='container-fluid'>
            <Toasts />
            <Navbar className="__main_menu_navbar" collapseOnSelect>
                <Navbar.Header>
                    <Navbar.Brand className="__main_menu_logo">
                        <a href="/"><Image src="/dist/logo-gold.png" responsive /> DocumentationHQ
                            </a>
                    </Navbar.Brand>
                    <Navbar.Toggle />
                </Navbar.Header>
                {authState && <Navbar.Collapse>
                    <Nav pullRight>
                        <NavDropdown eventKey={3} title={user} id="basic-nav-dropdown">
                            {(isCorporateUser || isSuperAdmin) && <MenuItem eventKey={3.1}>
                                <Link to='/banks'>Corporate View</Link>
                            </MenuItem>}
                            {(isBankUser || isSuperAdmin) && < MenuItem eventKey={3.2}>
                                <Link to='/corporates'>Bank View</Link>
                            </MenuItem>}
                            {isAdmin && <MenuItem eventKey={3.3}>
                                <Link to='/admin'>Admin</Link>
                            </MenuItem>}
                            <MenuItem divider />
                            <MenuItem eventKey={3.3} onClick={this.props.requestLogoff}>Logout</MenuItem>
                        </NavDropdown>
                    </Nav>
                </Navbar.Collapse>}
            </Navbar>
            <div className="__mainApp">
                <Route path='/login' component={Login} />
                <Route path='/addRole' component={AddRole} />
                <Route path='/viewdoc' component={PDFViewer} />
                <Route path='/signed/:docId/:envelopeId' component={Signed} />

                {(isCorporateUser || isSuperAdmin) && <Route path='/banks' component={BanksOverview} />}
                {(isBankUser || isSuperAdmin) && <Route path='/corporates' component={CorporatesOverview} />}
                {isAdmin && <Route path='/admin' component={Admin} />}
            </div>
        </div>;
    }
}

export default connect(
    (state: ApplicationState) => state.auth,            // Selects which state properties are merged into the component's props
    AuthState.actionCreators                // Selects which action creators are merged into the component's props
)(Layout);
