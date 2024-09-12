import * as React from 'react';
import Groups from './Groups';
import UserList from './UserList';
import RoleList from './RoleList';
import './GroupsAdmin.scss';
export default class GroupsAdmin extends React.Component {
    render() {
        return <div className='groups-admin'>
            <div className='group-container'>
                <Groups />
            </div>
            <div className='details-container'>
                <UserList />
                <RoleList />
            </div>
        </div>;
    }
}
//# sourceMappingURL=GroupsAdmin.jsx.map