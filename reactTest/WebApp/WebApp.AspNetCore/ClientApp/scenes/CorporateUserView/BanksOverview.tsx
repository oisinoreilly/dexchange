import * as React from 'react';
import BankColumn from './BankColumn';
import AccountColumn from '../../shared/AccountColumn';
import { DocumentColumn } from '../../shared/DocumentColumn';
import { AccountUi } from '../../store/accountState';
import MainPanel from '../../shared/MainPanel';
import DocumentDetails from '../DocumentDetails/DocumentDetailsColumn';

import { Status, NotificationType, Resource } from '../../backendTypes';
import { SortByStatus } from '../../typeMappings';
import { ReduxProps, withRedux } from '../../shared/withReduxStore';


class BanksOverview extends React.Component<ReduxProps, null> {

    componentDidMount() {
        this.props.actions.listBanks();
    }

    addAccount = (account: AccountUi, fillDocs: boolean) => {
        this.props.actions.addAccount(
            this.props.banks.selectedBank.id,
            this.props.auth.userConfig.EntityID,
            account,
            fillDocs);
    }

    public render() {
        const panelData = <div className="__columns">
            <BankColumn />
            <AccountColumn addAccount={this.addAccount} >
            </AccountColumn>
            <DocumentColumn isAdmin={this.props.auth.role == "admin"}
                selectedAccount={this.props.accounts.selectedAccount}
                documents={this.props.documents.documentList}
                selectedDocument={this.props.documents.selectedDocument}
                addDocument={this.props.actions.addDocument}
                selectDocument={this.props.actions.selectDocument}
                uploadDocument={this.props.actions.uploadDocument}
                deleteDocument={this.props.actions.deleteDocument}
                requestContent={this.props.actions.requestContent}></DocumentColumn>
            {this.props.documents.selectedDocument && <DocumentDetails /> }
        </div>;

        return <MainPanel>
            {panelData}
        </MainPanel>
    };
}

export default withRedux(BanksOverview);


