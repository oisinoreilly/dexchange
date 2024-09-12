import { Navbar, Form, Nav, NavItem, NavDropdown, MenuItem, Breadcrumb, Glyphicon } from 'react-bootstrap';
import * as React from 'react';
import { connect } from 'react-redux';
import { ApplicationState } from '../../store/index';
import * as Auth from '../../store/authStore';


// At runtime, Redux will merge together...
type LoginProps =
    Auth.AuthState                 // ... state we've requested from the Redux store
    & typeof Auth.actionCreators   // ... plus action creators we've ted  
    & {                                 // ... plus incoming routing parameters
        history: any
    }

class loginState {
    username: string;
    password: string;
}
export class Login extends React.Component<LoginProps, loginState> {
    constructor(props) {
        super(props);
        this.state = new loginState();
        //TODO: Reroute if we're not logged in.
    }

    componentWillMount() {
    }

    onLoginClick = (e) => {
        e.preventDefault();
        this.props.requestLogin(this.state.username, this.state.password);
    }

    onRegisterClick = (e) => {
        e.preventDefault();
        this.props.requestRegisterUser(this.state.username, this.state.password);
    }

    handleUsernameChange = (event) => {
        this.setState({ username: event.target.value });
    }
    handlePasswordChange = (event) => {
        this.setState({ password: event.target.value });
    }

    public render() {
       
        return <div>
            <div className="card card-container">
            <img id="profile-img" className="profile-img-card" src="//ssl.gstatic.com/accounts/ui/avatar_2x.png" />
            <p id="profile-name" className="profile-name-card"></p>
            <form className="form-signin" >
                    <input type="username" id="inputEmail" className="form-control" placeholder="Username" value={this.state.username} onChange={this.handleUsernameChange} required/>
                    <input type="password" id="inputPassword" className="form-control" placeholder="Password" value={this.state.password} onChange={this.handlePasswordChange} required />
                    <button type="submit" className="btn btn-lg btn-primary btn-block btn-signin" onClick={this.onLoginClick}>Log On</button>
                    <button type="submit" className="btn btn-lg btn-primary btn-block btn-signin" onClick={this.onRegisterClick}>Register</button>
            </form>
        </div>
    </div>;
    }
}

//Extend the login screen with the redux state
export default connect(
    (state: ApplicationState) => state.auth,            // Selects which state properties are merged into the component's props
    Auth.actionCreators                 // Selects which action creators are merged into the component's props
)(Login);
