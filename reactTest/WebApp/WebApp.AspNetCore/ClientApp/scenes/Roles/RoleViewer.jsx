import * as React from 'react';
import CheckboxTree from 'react-checkbox-tree';
import { withRedux } from '../../shared/withReduxStore';
export class RoleViewer extends React.Component {
    constructor() {
        super(...arguments);
        this.state = {
            checked: [],
            expanded: [],
            nodes: []
        };
    }
    componentWillReceiveProps(nextProps) {
        const hasPermissions = nextProps.roles.selectedRole.Permissions;
        if (!hasPermissions) {
            this.setState({
                nodes: [],
                expanded: [],
                checked: [],
            });
        }
        else if (nextProps.roles.selectedRole.Id !== this.props.roles.selectedRole.Id) {
            const permittedBanks = this.getPermitted('Bank', this.props.banks.bankList, nextProps.roles.selectedRole.Permissions);
            const permittedAccounts = this.getPermitted('Account', this.props.accounts.accountList, nextProps.roles.selectedRole.Permissions);
            const permittedCorporates = this.getPermitted('Corporate', this.props.corporates.corporateList, nextProps.roles.selectedRole.Permissions);
            const bankNodes = this.getNodes(permittedBanks, permittedAccounts, 'bankId');
            const corporateNodes = this.getNodes(permittedCorporates, permittedAccounts, 'corporateId');
            const allIds = permittedAccounts.concat(permittedBanks).concat(permittedCorporates).map(item => item.id);
            this.setState({
                nodes: bankNodes.concat(corporateNodes),
                expanded: [...allIds],
                checked: [...allIds]
            });
        }
    }
    getNodes(topLevelEntities, accounts, propertyFilter) {
        return topLevelEntities
            .map(entity => {
            return {
                value: entity.id,
                label: entity.title,
                children: accounts
                    .filter(acc => acc[propertyFilter] === entity.id)
                    .map(account => {
                    return { value: account.id, label: account.title };
                })
            };
        });
    }
    getPermitted(entityType, items, permissions) {
        const permissionsByEntity = permissions
            .filter(permission => permission.ResourceType === entityType);
        return items.filter(item => permissionsByEntity.findIndex(p => p.ResourceId === item.id) !== -1);
    }
    render() {
        if (!this.props.roles.selectedRole.Permissions) {
            return <h3>No permissions found for {this.props.roles.selectedRole.Name}</h3>;
        }
        return <div style={{ padding: "15px" }}>
            <CheckboxTree nodes={this.state.nodes} checked={this.state.checked} expanded={this.state.expanded} onCheck={checked => this.setState({ checked })} onExpand={expanded => this.setState({ expanded })} disabled={true}/></div>;
    }
}
export default withRedux(RoleViewer);
//# sourceMappingURL=RoleViewer.jsx.map