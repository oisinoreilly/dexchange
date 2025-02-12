import * as React from 'react';
import { connect } from 'react-redux';
import { Breadcrumb, Button } from 'react-bootstrap';

import { CorporateUi } from '../../store/corporateState';
import { ApplicationState } from '../../store/index';
import AccordionColumn from './AccordionColumn';
import { withRedux, ReduxProps } from '../../shared/withReduxStore';

interface OwnProps {
    topLevelCorporate: CorporateUi;
    hide: (parentId: string) => void;
}

type Props = OwnProps & ReduxProps;


interface IAccordianState {
    columns: ISubsid[];
    displayedColumns: ISubsid[];
    breadCrumbColumns: ISubsid[];
}
export interface ISubsid {
    parentId: string;
    id: string;
    selectedId: string;
    title: string;
}

class Accordion extends React.Component<Props, IAccordianState> {
    constructor(props) {
        super(props);
        this.state = {
            columns: [{
                parentId: "",
                id: this.props.topLevelCorporate.id,
                selectedId: "",
                title: this.props.topLevelCorporate.title
            }],
            breadCrumbColumns: [],
            displayedColumns: [{parentId: "",
                id: this.props.topLevelCorporate.id,
                selectedId: "",
                title: this.props.topLevelCorporate.title
            }]
        }
    }

    insertSubsidInAccordion = (array, obj, location) => {
        const newarray = array.slice(0, location);
        newarray.push(obj);
        return newarray;
    }

    handleSubsidClick = (subsid) => {
        let accordianColumns = [...this.state.columns];
        const subsidColumnEntry = {
            parentId: subsid.parentId,
            id: subsid.id,
            selectedId: "",
            title: subsid.title
        };

        const location = accordianColumns.findIndex(col => col.parentId === subsid.parentId);
        const levelExistsInAccordion = location !== -1;

        const subsidAlreadyExists = accordianColumns.findIndex(col => col.id === subsid.id) !== -1;
        
        if (levelExistsInAccordion) { //re render from this location
            const columnToInsert = subsidColumnEntry
            accordianColumns = this.insertSubsidInAccordion(accordianColumns, columnToInsert, location)
        }
        else { //append to end
            accordianColumns.push(subsidColumnEntry);
        }

        //set the selected item in the column
        const currentColIndex = accordianColumns.findIndex(col => col.id === subsid.parentId);
        accordianColumns[currentColIndex].selectedId = subsid.id;

        this.setAccordionState(subsid.parentId, subsid.id, accordianColumns, -2);
        this.props.actions.selectSubsid(subsid.id);
        this.props.actions.listAccounts(this.props.auth.userConfig.EntityID, subsid.id);   
    }

    renderSubsidColumns = (subsidInfo) => {
        return subsidInfo.map((parent, index) => {
            return (<AccordionColumn
                corporates={this.props.corporates.corporateList}
            id={parent.id}
            onClick={this.handleSubsidClick}
            selectedId={parent.selectedId}
            addSubsid={this.props.actions.addCorporate}
            isAdmin={this.props.auth.role == "admin"} />
            )
        });
    }

    setAccordionState(parentId, id, columns, offset) {
        const locInArray = columns.findIndex(corp => corp.id === id);
        const moreToDisplay = locInArray !== -1;

        let displayed = [...this.state.displayedColumns];
        if (moreToDisplay) {
            if (columns.length > 3) {
                displayed = columns.slice(locInArray + offset, locInArray + offset + 3);
            }
            else {
                displayed = columns;
            }
        }

        const breadCrumb = columns.slice(1, locInArray - 1);
        this.setState({
            columns: columns,
            displayedColumns: displayed,
            breadCrumbColumns: breadCrumb
        });
    }

    navigate = (parentId, id) => {
        this.setAccordionState(parentId, id, [...this.state.columns], -1);
    }

    public render() {
        return (
            <div className="__column __subsid_column">
                <div className="__column_header">
                    <Breadcrumb>
                        <Breadcrumb.Item>
                            <span onClick={() => this.props.hide(this.props.topLevelCorporate.id)}>{this.props.topLevelCorporate.title}</span>
                        </Breadcrumb.Item>
                        {this.state.breadCrumbColumns.map(item => {
                            return (
                                <Breadcrumb.Item >
                                    <span onClick={() => this.navigate(item.parentId, item.id)}>{item.title}</span>
                                </Breadcrumb.Item>
                            );
                    })}
                    </Breadcrumb>
                </div>
                <div className="__accordian_columns">
                    {this.renderSubsidColumns(this.state.displayedColumns)}
                </div>
            </div>)
    }
}
export default withRedux(Accordion);

