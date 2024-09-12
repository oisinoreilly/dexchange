import * as React from 'react';

//import NodeList from './NodeList';
import PermissionPicker from './PermissionPicker';
import { withRedux, ReduxProps } from '../../shared/withReduxStore';

interface OwnProps {
    addPermissions: Function;
    removePermission: Function;
    changeAccessRights: Function;
}

type Props = ReduxProps & OwnProps;

export class PermissionsForCorporateUser extends React.Component<Props, null> {

    handleGetAccounts = (bankId) => {
        return this.props.accounts.accountList
            .filter(acc => acc.bankId === bankId);
    }

    render() {
        return <PermissionPicker
            {...this.props}
            topLevelItems={this.props.banks.bankList}
            type="Bank"
            getAccounts={this.handleGetAccounts}
        />
    }
}

export default withRedux(PermissionsForCorporateUser)