import { createStore, applyMiddleware, compose, combineReducers } from 'redux';
import thunk from 'redux-thunk';
import * as Store from './store';
import { sseEventsMiddleware } from './store/sseEventsMiddleware';
import authMiddleware from './store/authMiddleware';
import { autoRehydrate } from 'redux-persist';
import { routerMiddleware } from 'react-router-redux';
import createHistory from 'history/createBrowserHistory';
export const history = createHistory();
export default function configureStore(initialState) {
    // Build middleware. These are functions that can process the actions before they reach the store.
    const windowIfDefined = typeof window === 'undefined' ? null : window;
    // If devTools is installed, connect to it
    const devToolsExtension = windowIfDefined && windowIfDefined.devToolsExtension;
    // Create a history (we're using a browser history)
    // Build the middleware for intercepting and dispatching navigation actions
    const navigateMiddleware = routerMiddleware(history);
    const createStoreWithMiddleware = compose(applyMiddleware(thunk, authMiddleware, sseEventsMiddleware, navigateMiddleware), autoRehydrate(), devToolsExtension ? devToolsExtension() : f => f)(createStore);
    // Combine all reducers and instantiate the app-wide store instance
    const allReducers = buildRootReducer(Store.reducers);
    const store = createStoreWithMiddleware(allReducers, initialState);
    //// Enable Webpack hot module replacement for reducers
    //if (module.hot) {
    //    module.hot.accept('./store', () => {
    //        const nextRootReducer = require<typeof Store>('./store');
    //        store.replaceReducer(buildRootReducer(nextRootReducer.reducers));
    //    });
    //}
    //Add a reference to the store to the window, so we can access it from other components.
    window.store = store;
    return store;
}
function buildRootReducer(allReducers) {
    return combineReducers(Object.assign({}, allReducers));
}
//# sourceMappingURL=configureStore.js.map