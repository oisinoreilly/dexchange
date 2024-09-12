import * as React from 'react';
import { withRedux } from '../../shared/withReduxStore';
import { Alert } from 'react-bootstrap';
import './toasts.scss';
class Toasts extends React.Component {
    constructor() {
        super(...arguments);
        this.handleDismiss = (id) => {
            this.props.actions.removeToast(id);
        };
    }
    render() {
        return <div className='toast-container'>
            {this.props.toasts.toastList.map(toast => {
            return <div className="animated-alert"><Alert bsStyle="info" onDismiss={() => this.handleDismiss(toast.id)}>
                    {toast.messsage}
                </Alert></div>;
        })}
        </div>;
    }
}
export default withRedux(Toasts);
//# sourceMappingURL=Toasts.jsx.map