import { createStore, applyMiddleware, compose, combineReducers, GenericStoreEnhancer } from 'redux';
import thunk from 'redux-thunk';
import * as Store from './store';
import { sseEventsMiddleware } from './store/sseEventsMiddleware';
import authMiddleware from './store/authMiddleware';
import { persistStore, autoRehydrate } from 'redux-persist';
import { routerReducer, routerMiddleware } from 'react-router-redux'
import createHistory from 'history/createBrowserHistory';


export const history = createHistory();

export default function configureStore(initialState?: Store.ApplicationState) {
    // Build middleware. These are functions that can process the actions before they reach the store.
    const windowIfDefined = typeof window === 'undefined' ? null : window as any;
    // If devTools is installed, connect to it
    const devToolsExtension = windowIfDefined && windowIfDefined.devToolsExtension as () => GenericStoreEnhancer;

    // Create a history (we're using a browser history)
   

    // Build the middleware for intercepting and dispatching navigation actions
    const navigateMiddleware = routerMiddleware(history);

    const createStoreWithMiddleware = compose(
        applyMiddleware(thunk, authMiddleware, sseEventsMiddleware, navigateMiddleware),
        autoRehydrate(),
        devToolsExtension ? devToolsExtension() : f => f
    )(createStore);

    // Combine all reducers and instantiate the app-wide store instance
    const allReducers = buildRootReducer(Store.reducers);
    const store = createStoreWithMiddleware(allReducers, initialState) as Redux.Store<Store.ApplicationState>;

    //// Enable Webpack hot module replacement for reducers
    //if (module.hot) {
    //    module.hot.accept('./store', () => {
    //        const nextRootReducer = require<typeof Store>('./store');
    //        store.replaceReducer(buildRootReducer(nextRootReducer.reducers));
    //    });
    //}
    //Add a reference to the store to the window, so we can access it from other components.
    (window as any).store = store;
    return store;
}

function buildRootReducer(allReducers) {
    return combineReducers<Store.ApplicationState>(Object.assign({}, allReducers));
}
