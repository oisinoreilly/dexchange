import * as React from 'react';
import { Image, Navbar, Nav, NavDropdown, MenuItem } from 'react-bootstrap';
import * as AuthState from '../../store/authStore';
import { connect } from 'react-redux';
import Login from '../login/Login';
import BanksOverview from '../CorporateUserView/BanksOverview';
import CorporatesOverview from '../BankUserView/CorporatesOverview';
import AddRole from '../Roles/AddRole';
import Admin from '../Admin/Admin';
import Signed from '../DocusSign/Signed';
import PDFViewer from '../../shared/PdfViewer';
import { Route, Link } from 'react-router-dom';
import Toasts from './Toasts';
// export interface LayoutProps {
//     body: React.ReactElement<any>;
// }
export class Layout extends React.Component {
    render() {
        //Get the authstate from the session storage
        let authStateJson = sessionStorage.getItem("AuthDetails");
        let authState = null;
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
                        <a href="/"><Image src="/dist/logo-gold.png" responsive/> DeXchange
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
                            {(isBankUser || isSuperAdmin) && <MenuItem eventKey={3.2}>
                                <Link to='/corporates'>Bank View</Link>
                            </MenuItem>}
                            {isAdmin && <MenuItem eventKey={3.3}>
                                <Link to='/admin'>Admin</Link>
                            </MenuItem>}
                            <MenuItem divider/>
                            <MenuItem eventKey={3.3} onClick={this.props.requestLogoff}>Logout</MenuItem>
                        </NavDropdown>
                    </Nav>
                </Navbar.Collapse>}
            </Navbar>
            <div className="__mainApp">
                <Route path='/login' component={Login}/>
                <Route path='/addRole' component={AddRole}/>
                <Route path='/viewdoc' component={PDFViewer}/>
                <Route path='/signed/:docId/:envelopeId' component={Signed}/>

                {(isCorporateUser || isSuperAdmin) && <Route path='/banks' component={BanksOverview}/>}
                {(isBankUser || isSuperAdmin) && <Route path='/corporates' component={CorporatesOverview}/>}
                {isAdmin && <Route path='/admin' component={Admin}/>}
            </div>
        </div>;
    }
}
export default connect((state) => state.auth, // Selects which state properties are merged into the component's props
AuthState.actionCreators // Selects which action creators are merged into the component's props
)(Layout);
//# sourceMappingURL=Layout.jsx.map