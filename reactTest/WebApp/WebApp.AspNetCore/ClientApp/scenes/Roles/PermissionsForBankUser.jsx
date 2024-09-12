import * as React from 'react';
//import NodeList from './NodeList';
import PermissionPicker from './PermissionPicker';
import { withRedux } from '../../shared/withReduxStore';
export class PermissionsForBankUser extends React.Component {
    constructor() {
        super(...arguments);
        this.handleGetAccounts = (corporateId) => {
            return this.props.accounts.accountList
                .filter(acc => acc.corporateId === corporateId);
        };
    }
    render() {
        const { accounts, corporates } = this.props;
        return <PermissionPicker {...this.props} topLevelItems={corporates.corporateList} type="Corporate" getAccounts={this.handleGetAccounts}/>;
    }
}
export default withRedux(PermissionsForBankUser);
//# sourceMappingURL=PermissionsForBankUser.jsx.map