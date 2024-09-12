import * as React from 'react';
import './RightArrowButton.scss';
export class RightArrowButton extends React.Component {
    constructor() {
        super(...arguments);
        this.handleSelect = (e) => {
            e.stopPropagation();
            this.props.onSelect();
        };
    }
    render() {
        return (<div className="right-arrow" onClick={this.handleSelect}>
                <i className="fa fa-caret-right" aria-hidden="true"></i>
            </div>);
    }
}
//# sourceMappingURL=RightArrowButton.jsx.map