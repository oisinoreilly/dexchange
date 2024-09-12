import * as React from "react";
import { FormGroup, FormControl, Button, InputGroup, Glyphicon } from 'react-bootstrap';

export interface IChatFooterProps {

    documentId : string
    fromUser : string
    addChatMessage: ( selectedDocument : string, fromUser : string, chatMessage: string) => void;
}

interface IChatFooterState {
    message: string;
}

export class Footer extends React.Component<IChatFooterProps, IChatFooterState> {
    constructor(props) {
        super(props);
        this.state = { message: "" };
    }

    postMsg = () => {
        if (this.props != null && this.state.message !== "")
        {
            this.props.addChatMessage(this.props.documentId, this.props.fromUser, this.state.message);
            this.setState({ message: "" });
        }
    }

    handleKeyDown = (e) => {
        enum Keys {
            tab = 9,
            enter = 13,
            space = 32,
            left = 37,
            up = 38,
            right = 39,
            down = 40,
        }

        const keycode = e.keyCode;     
        if (keycode === Keys.enter)
        {
            this.postMsg();
        }
    }

    handleMsgChange = (e) => {
        this.setState({
            message: e.target.value
        });
    }

    render() {
        return (
            <div id="bottom">
                <FormGroup>
                    <InputGroup>
                        <FormControl
                            autoFocus 
                            type="text" 
                            value={this.state.message}
                            onChange={this.handleMsgChange}
                            onKeyDown={this.handleKeyDown} 
                            id="txtMsg"
                            placeholder="Type a message" />
                        <InputGroup.Addon className="__chatbutton">
                            <Glyphicon className="__chatbutton_icon" glyph='comment' id="btnSend" onClick={this.postMsg}></Glyphicon>
                        </InputGroup.Addon>
                    </InputGroup>
                </FormGroup>
            </div>
        );
    }
}


