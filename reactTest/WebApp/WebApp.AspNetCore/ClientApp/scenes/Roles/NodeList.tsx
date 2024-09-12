import * as React from 'react';
import { Node } from './Node';
import { AccountUi } from '../../store/accountState';
import { Entity } from '../../frontendTypes';
import { Resource } from '../../backendTypes';
import './NodeList.scss';

interface Props {
    addPermission: Function;
    removePermission: Function;
    changeAccessRights: Function;
    getChildren: Function;
    topLevelItems: Entity[];
    type: Resource;
}
   
export class NodeList extends React.Component<Props, null> {

    public render() {
        return <div className='node-list'>
            {
                this.props.topLevelItems.map(item => {
                    return <Node title={item.title}
                        getChildren={this.props.getChildren}
                        id={item.id}
                        type={this.props.type}
                        onSelect={this.props.addPermission}
                        onUnSelect={this.props.removePermission}
                        onChangeAccessRights={this.props.changeAccessRights}
                    />
                })
            }
        </div>
    }
}

export default NodeList;

