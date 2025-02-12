import './styles/site.scss';
import 'bootstrap';
import * as React from 'react';
import { Store } from 'redux';
import * as ReactDOM from 'react-dom';
import { Route, BrowserRouter as Router } from 'react-router-dom';
import { ConnectedRouter } from 'react-router-redux';
import { Provider } from 'react-redux';
import configureStore, { history } from './configureStore';
import { ApplicationState } from './store';
import { persistStore } from 'redux-persist';
import { asyncSessionStorage } from 'redux-persist/storages';
import Layout from './scenes/Layout/Layout';
import 'react-checkbox-tree/lib/react-checkbox-tree.css';
import 'font-awesome/css/font-awesome.min.css';


export default class Preloader extends React.Component<{ store: any }, { rehydrated: boolean }> {

    state = { rehydrated: false }

    // Before the app loads we need to ensure pervious state is loaded before other
    // actions are dispatched.
    componentWillMount() {
    persistStore(this.props.store, { storage: asyncSessionStorage}, () => {
            this.setState({ rehydrated: true });
        })
    }

    render() {
        if (!this.state.rehydrated) {
            return <div id="react-app">Loading...</div>;
        }
        return (
            <Provider store={store}>
                <ConnectedRouter history={history}>
                    <Route path='/' component={Layout}></Route>
                </ConnectedRouter>
            </Provider>
        );
    }
}

