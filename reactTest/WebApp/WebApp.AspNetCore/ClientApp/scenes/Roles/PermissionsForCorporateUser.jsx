import * as React from 'react';
//import NodeList from './NodeList';
import PermissionPicker from './PermissionPicker';
import { withRedux } from '../../shared/withReduxStore';
export class PermissionsForCorporateUser extends React.Component {
    constructor() {
        super(...arguments);
        this.handleGetAccounts = (bankId) => {
            return this.props.accounts.accountList
                .filter(acc => acc.bankId === bankId);
        };
    }
    render() {
        return <PermissionPicker {...this.props} topLevelItems={this.props.banks.bankList} type="Bank" getAccounts={this.handleGetAccounts}/>;
    }
}
export default withRedux(PermissionsForCorporateUser);
//# sourceMappingURL=PermissionsForCorporateUser.jsx.map