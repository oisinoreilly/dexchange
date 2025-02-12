
import * as React from 'react';
import './RightArrowButton.scss';

interface Props {
    onSelect: Function;
}


export class RightArrowButton extends React.Component<Props, null>{

    handleSelect = (e) => {
        e.stopPropagation();
        this.props.onSelect();
    }

    render() {
        return (
            <div className="right-arrow" onClick={this.handleSelect} >
                <i className="fa fa-caret-right" aria-hidden="true"></i>
            </div>
        )
    }
}