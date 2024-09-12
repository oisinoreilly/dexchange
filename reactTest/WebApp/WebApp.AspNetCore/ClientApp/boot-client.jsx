import './styles/site.scss';
import 'bootstrap';
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Route } from 'react-router-dom';
import { ConnectedRouter } from 'react-router-redux';
import { Provider } from 'react-redux';
import configureStore, { history } from './configureStore';
import { persistStore } from 'redux-persist';
import { asyncSessionStorage } from 'redux-persist/storages';
import Layout from './scenes/Layout/Layout';
import 'react-checkbox-tree/lib/react-checkbox-tree.css';
import 'font-awesome/css/font-awesome.min.css';
export default class Preloader extends React.Component {
    constructor() {
        super(...arguments);
        this.state = { rehydrated: false };
    }
    // Before the app loads we need to ensure pervious state is loaded before other
    // actions are dispatched.
    componentWillMount() {
        persistStore(this.props.store, { storage: asyncSessionStorage }, () => {
            this.setState({ rehydrated: true });
        });
    }
    render() {
        if (!this.state.rehydrated) {
            return <div id="react-app">Loading...</div>;
        }
        return (<Provider store={store}>
                <ConnectedRouter history={history}>
                    <Route path='/' component={Layout}></Route>
                </ConnectedRouter>
            </Provider>);
    }
}
// Get the application-wide store instance
const initialState = window.initialReduxState;
const store = configureStore(initialState);
// This code starts up the React app when it runs in a browser.
ReactDOM.render(<Preloader store={store}/>, document.getElementById('react-app'));
//# sourceMappingURL=boot-client.jsx.map