import * as React from 'react';
import { FieldMapping } from '../../frontendTypes';
import { FieldDefinition } from '../../backendTypes';
import { Button, FormGroup, ControlLabel, FormControl, Modal } from 'react-bootstrap';
import './corporateConfig.scss';
import { ReduxProps, withRedux } from '../../shared/withReduxStore';

type state = {
    fields: FieldDefinition[];
    lei: string;
    showUpdateModal: boolean;
}


const fieldMapping: FieldMapping[] = [
    { propertyName: 'LegalName', displayName: 'Legal Name' },
    { propertyName: 'LegalJurisdiction', displayName: 'Legal Juristiction' },
    { propertyName: 'LegalAddress', displayName: 'Legal Address', isLargeField: true },
    { propertyName: 'HeadquartersAddress', displayName: 'Headquarters Address', isLargeField: true },
];


class CorporateConfig extends React.Component<ReduxProps, state> {
    state = {
        fields: [],
        lei: "",
        showUpdateModal: false
    }

    componentDidMount() {
        this.props.actions.getCorporate(this.props.auth.userConfig.EntityID);
    }

    componentWillReceiveProps(nextProps: ReduxProps) {
        const defaults = [
            'LegalName',
            'LegalJurisdiction',
            'LegalAddress',
            'HeadquartersAddress'];

        const fields = nextProps.corporates.selectedCorporate.fields;

         //check for field values in corporate that match our default list
        const fieldsThatMatchDefaults = defaults.map(name => {

            let value = "";
            if (fields) {
                const fieldInData = fields.find(field => field.Name === name);
                value = fieldInData ? fieldInData.DefaultValue : "";
            }
           
            const def = new FieldDefinition();
            def.DataType = "String_e";
            def.DefaultValue = value;
            def.Name = name;
            return def;
        });

        //filter out the default fields - we already have values for these above
        if (fields) {
            const nonDefaults = fields.filter(field => defaults.indexOf(field.Name) === -1)
            const fieldsToRender = fieldsThatMatchDefaults.concat(nonDefaults);

            this.setState({
                fields: fieldsToRender
            });
        }
        else {
            this.setState({
                fields: fieldsThatMatchDefaults
            });
        }
      
    }

    handleInputChange = (e) => {
        const { fields } = this.state;
        const index = fields.findIndex(field => field.Name === e.target.name);
        fields[index].DefaultValue = e.target.value;

        this.setState({
           fields
        });
    }

    handleLeiChange = (e) => {
        this.setState({
            lei: e.target.value
        });
    }

    showUpdateModal = () => {
        this.setState({
            showUpdateModal: true
        });
    }

    hideUpdateModal = () => {
        this.setState({
            showUpdateModal: false
        });
    }

    handleUpdateCorporate = () => {
        this.props.actions.updateFieldDefinitions(this.props.auth.userConfig.EntityID, this.state.fields);
        this.setState({
            showUpdateModal: false
        })
    }

    handleLEILookup = () => {

        if (this.state.lei) {
            const { lei } = this.state;
            const url = `https://leilookup.gleif.org/api/v1/leirecords?lei=${lei}`;
            fetch(url).then(res => {
                return res.json();
            }).then(data => {

                const fields = [ ...this.state.fields ];
                const result = data[0].Entity;
                for (let outerkey in result) {
                    if (Object.keys(result[outerkey]).length > 1) {
                        let multiple = '';
                        for (let innerKey in result[outerkey]) {
                            multiple += result[outerkey][innerKey].$ + '\n';
                        }
                        const index = fields.findIndex(field => field.Name === outerkey);
                        if(index !== -1)
                            fields[index].DefaultValue = multiple;
                    }
                    else {
                        const index = fields.findIndex(field => field.Name === outerkey);
                        if(index !== -1)
                            fields[index].DefaultValue = result[outerkey].$;
                    }
                }

                this.setState({
                    fields
                });

            }).catch(error => {
                //report error
                console.log(error);
            });
        }
    }

    render() {

    const updateModal = <Modal show={this.state.showUpdateModal} onHide={this.hideUpdateModal}>
            <Modal.Header closeButton>
                <Modal.Title>Are you sure you want to update all fields?</Modal.Title>
            </Modal.Header>
            <Modal.Footer>
                <Button onClick={this.handleUpdateCorporate}>Yes</Button>
            </Modal.Footer>
        </Modal>;


        const textFields = this.state.fields.map(field => {
            const { DefaultValue, Name } = field;

            return <FormGroup controlId={Name + 'Input'}>
                <ControlLabel>{Name}</ControlLabel>
                <FormControl name={Name}
                    type="text" value={DefaultValue}
                    onChange={this.handleInputChange}
                    componentClass={DefaultValue.length > 20 ? 'textarea' : undefined}/>
                <FormControl.Feedback />
            </FormGroup>
        });


        return <div className="corporate-config-container">
            {updateModal}
            <div>
                <FormGroup controlId='LEIInput'>
                    <ControlLabel>LEI</ControlLabel>
                    <FormControl name='lei'
                        type="text" value={this.state.lei}
                        onChange={this.handleLeiChange}
                       />
                    <FormControl.Feedback />
                </FormGroup>
                <Button type='button' bsStyle='info' className='button' onClick={this.handleLEILookup}>Perform LEI Lookup</Button>
                <Button type='button' bsStyle='primary' className='button' onClick={this.showUpdateModal}>Update Details</Button>
                {textFields}
            </div>
        </div>
    }
}

export default withRedux(CorporateConfig);