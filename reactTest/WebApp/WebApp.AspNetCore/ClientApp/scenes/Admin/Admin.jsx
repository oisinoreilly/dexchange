import * as React from 'react';
import { Glyphicon } from "react-bootstrap";
import { Link, Route } from 'react-router-dom';
import Roles from '../Roles/Roles';
import Users from '../Users/Users';
import GroupsAdmin from '../Groups/GroupsAdmin';
import CorporateConfig from '../CorporateConfig/CorporateConfig';
import { withRedux } from '../../shared/withReduxStore';
import './Admin.scss';
class Admin extends React.Component {
    getNavItemClass(path) {
        if (this.props.location.pathname === '/admin' && path === '/admin/users')
            return "nav-item selected";
        return this.props.location.pathname.startsWith(path)
            ? "nav-item selected"
            : "nav-item";
    }
    componentWillUnmount() {
        this.props.actions.clearAccounts();
        this.props.actions.clearBanks();
        this.props.actions.clearCorporates();
        this.props.actions.clearDocuments();
    }
    render() {
        const { UserPrivilege, UserType } = this.props.auth.userConfig;
        const isCorporateAdmin = UserPrivilege === "Admin" && UserType === "Corporate";
        return <div className="__panel_holder">
            <div className="__main_panel">
                <div className="__main_panel_menues">
                    <Glyphicon glyph='cog' className='page-icon'>
                    </Glyphicon>
                    <span><h3>Admin</h3></span>
                </div>
                <div className="columns">
                    <div className="admin-column side-nav">
                        <Link to="/admin/users">
                            <div className={this.getNavItemClass("/admin/users")}>
                                <div className="nav-icon">
                                    <i className="fa fa-2x fa-user-circle" aria-hidden="true"></i>
                                </div>
                                <div className="nav-title"><h4>Users</h4></div>
                            </div>
                        </Link>
                        <Link to="/admin/groups">
                            <div className={this.getNavItemClass("/admin/groups")}>
                                <div className="nav-icon">
                                    <i className="fa fa-2x fa-users" aria-hidden="true"></i>
                                </div>
                                <div className="nav-title"><h4>Groups</h4></div>
                            </div>
                        </Link>
                        <Link to="/admin/roles">
                            <div className={this.getNavItemClass("/admin/roles")}>
                                <div className="nav-icon">
                                    <i className="fa fa-2x fa-key" aria-hidden="true"></i>
                                </div>

                                <div className="nav-title"><h4>Roles</h4></div>
                            </div>
                        </Link>
                        {isCorporateAdmin && <Link to="/admin/corporateConfig">
                            <div className={this.getNavItemClass("/admin/corporateConfig")}>
                                <div className="nav-icon">
                                    <i className="fa fa-2x fa-building-o" aria-hidden="true"></i>
                                </div>
                                <div className="nav-title"><h4>Company Details</h4></div>
                            </div>
                        </Link>}
                    </div>
                    <Route exact path='/admin' component={Users}/>
                    <Route path='/admin/users' component={Users}/>
                    <Route path='/admin/roles' component={Roles}/>
                    <Route path='/admin/groups' component={GroupsAdmin}/>
                    {isCorporateAdmin && <Route path='/admin/corporateConfig' component={CorporateConfig}/>}
                </div>
            </div>
        </div>;
    }
}
export default withRedux(Admin);
//# sourceMappingURL=Admin.jsx.map