import './App.css'
import Preloader from './boot-client'
import configureStore from './configureStore'


// Get the application-wide store instance
const initialState = (window as any).initialReduxState as ApplicationState;
const store = configureStore(initialState);


function App() {

  return (
      <Preloader store={store} />
  )
}

export default App
