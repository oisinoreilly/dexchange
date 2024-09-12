import * as React from 'react';
import { Button } from 'react-bootstrap';
import './RaisedButton.scss';
const RaisedButton = ({ handler, children }) => {
    return (<Button bsStyle='primary' className='raised-button' onClick={() => handler}>{children}</Button>);
};
export default RaisedButton;
//# sourceMappingURL=RaisedButton.jsx.map