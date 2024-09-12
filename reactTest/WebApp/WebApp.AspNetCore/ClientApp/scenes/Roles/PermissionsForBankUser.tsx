import * as React from 'react';

//import NodeList from './NodeList';
import PermissionPicker from './PermissionPicker';
import { ReduxProps, withRedux } from '../../shared/withReduxStore';

interface OwnProps {
    addPermissions: Function;
    removePermission: Function;
    changeAccessRights: Function;
}

type Props = ReduxProps & OwnProps; 

export class PermissionsForBankUser extends React.Component<Props, null> {

    handleGetAccounts = (corporateId) => {
        return this.props.accounts.accountList
            .filter(acc => acc.corporateId === corporateId);
    }

    render() {
        const { accounts, corporates } = this.props;
        return <PermissionPicker
            {...this.props}
            topLevelItems={corporates.corporateList}
            type="Corporate"
            getAccounts={this.handleGetAccounts}/>
    }
}

export default withRedux(PermissionsForBankUser);
