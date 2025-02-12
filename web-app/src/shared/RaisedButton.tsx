import * as React from 'react';
import { Button } from 'react-bootstrap';
import './RaisedButton.scss';


interface Props {
    handler: Function;
}

const RaisedButton: React.SFC<Props> = ({ handler, children }) => {
    return (
        <Button bsStyle='primary' className='raised-button' onClick={() => handler}>{children}</Button>
    )
}

export default RaisedButton;

