import * as React from "react";
import { FormGroup, FormControl, InputGroup, Glyphicon } from 'react-bootstrap';
export class Footer extends React.Component {
    constructor(props) {
        super(props);
        this.postMsg = () => {
            if (this.props != null && this.state.message !== "") {
                this.props.addChatMessage(this.props.documentId, this.props.fromUser, this.state.message);
                this.setState({ message: "" });
            }
        };
        this.handleKeyDown = (e) => {
            let Keys;
            (function (Keys) {
                Keys[Keys["tab"] = 9] = "tab";
                Keys[Keys["enter"] = 13] = "enter";
                Keys[Keys["space"] = 32] = "space";
                Keys[Keys["left"] = 37] = "left";
                Keys[Keys["up"] = 38] = "up";
                Keys[Keys["right"] = 39] = "right";
                Keys[Keys["down"] = 40] = "down";
            })(Keys || (Keys = {}));
            const keycode = e.keyCode;
            if (keycode === Keys.enter) {
                this.postMsg();
            }
        };
        this.handleMsgChange = (e) => {
            this.setState({
                message: e.target.value
            });
        };
        this.state = { message: "" };
    }
    render() {
        return (<div id="bottom">
                <FormGroup>
                    <InputGroup>
                        <FormControl autoFocus type="text" value={this.state.message} onChange={this.handleMsgChange} onKeyDown={this.handleKeyDown} id="txtMsg" placeholder="Type a message"/>
                        <InputGroup.Addon className="__chatbutton">
                            <Glyphicon className="__chatbutton_icon" glyph='comment' id="btnSend" onClick={this.postMsg}></Glyphicon>
                        </InputGroup.Addon>
                    </InputGroup>
                </FormGroup>
            </div>);
    }
}
//# sourceMappingURL=ChatFooter.jsx.map