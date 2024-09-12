import * as React from 'react';
import CorporateColumn from './CorporateColumn';
import AccountColumn from '../../shared/AccountColumn';
import { DocumentColumn } from '../../shared/DocumentColumn';
import Accordion from './Accordian';
import { AccountUi } from '../../store/accountState';
import MainPanel from '../../shared/MainPanel';
import DocumentDetails from '../DocumentDetails/DocumentDetailsColumn';

import { Status } from '../../backendTypes';
import { SortByStatus } from '../../typeMappings';
import { ReduxProps, withRedux } from '../../shared/withReduxStore';
import { ContextMenu, IMenuOption } from '../../shared/ContextMenu';

interface ICorporatesState {
    accordionEnabled: boolean;
}

class CorporatesOverview extends React.Component<ReduxProps, ICorporatesState> {
    state = {
        accordionEnabled: false
    }

    componentDidMount() {
        // This method runs when the component is first added to the page
        this.props.actions.listCorporates();
    }

    hideAccordian = (parentId) => {
        this.selectCorporate(parentId);
        this.setState({ accordionEnabled: false });
    }

    selectCorporate = (id) => {
        const { EntityID: bankId } = this.props.auth.userConfig;

        this.props.actions.selectCorporate(id);
        this.props.actions.listAccounts(bankId, id);
    }

    viewSubsids = (parentId) => {
        this.props.actions.clearAccounts();
        this.props.actions.selectCorporate(parentId);
        this.setState({ accordionEnabled: true });
    }

    selectSubsid = (id) => {
        this.props.actions.selectSubsid(id);
    }

    addAccount = (account: AccountUi, filldocs: boolean) => {
        //if there's a selected subsid use this, otherwise its a top level corporate
        const { EntityID: bankId } = this.props.auth.userConfig;

        if (this.props.corporates.selectedSubsid.id) {
            this.props.actions.addAccount(bankId, this.props.corporates.selectedSubsid.id, account, filldocs);
        }
        else
            this.props.actions.addAccount(bankId, this.props.corporates.selectedCorporate.id, account, filldocs);
    }

    public render() {
        const parentId = this.props.corporates.selectedCorporate.parentId;
        const subsids = this.props.corporates.selectedCorporate.subsids

        const isTopLevel = !parentId ? true : false;
        const hasSubsids = typeof subsids !== 'undefined' && subsids.length > 0;

        const firstColumn = this.state.accordionEnabled
            ? <Accordion
                topLevelCorporate={this.props.corporates.selectedCorporate}
                hide={this.hideAccordian} />
            : <CorporateColumn
                selectCorporate={this.selectCorporate}
                selectSubsid={this.selectSubsid}
                viewSubsids={this.viewSubsids} />

        const panelData = <div className="__columns">
            {firstColumn}
            <AccountColumn addAccount={this.addAccount} ></AccountColumn>
            <DocumentColumn isAdmin={this.props.auth.role == "admin"}
                selectedAccount={this.props.accounts.selectedAccount}
                documents={this.props.documents.documentList}
                selectedDocument={this.props.documents.selectedDocument}
                addDocument={this.props.actions.addDocument}
                selectDocument={this.props.actions.selectDocument}
                uploadDocument={this.props.actions.uploadDocument}
                deleteDocument={this.props.actions.deleteDocument}
                requestContent={this.props.actions.requestContent}>
            </DocumentColumn>
            {this.props.documents.selectedDocument && <DocumentDetails />}
        </div>;

        return <MainPanel>
            {panelData}
        </MainPanel>
    };
}


export default withRedux(CorporatesOverview);
