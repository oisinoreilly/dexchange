import * as React from 'react';
import { Node } from './Node';
import './NodeList.scss';
export class NodeList extends React.Component {
    render() {
        return <div className='node-list'>
            {this.props.topLevelItems.map(item => {
            return <Node title={item.title} getChildren={this.props.getChildren} id={item.id} type={this.props.type} onSelect={this.props.addPermission} onUnSelect={this.props.removePermission} onChangeAccessRights={this.props.changeAccessRights}/>;
        })}
        </div>;
    }
}
export default NodeList;
//# sourceMappingURL=NodeList.jsx.map