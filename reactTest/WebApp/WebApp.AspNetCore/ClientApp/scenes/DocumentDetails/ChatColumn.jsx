import * as React from 'react';
import { Timeline, TimelineEvent } from 'react-event-timeline';
import { withRedux } from '../../shared/withReduxStore';
import { toLocalDateStringUS } from '../../Utils';
import './chatColumn.scss';
class ChatColumn extends React.Component {
    constructor() {
        super(...arguments);
        this.scrollToBottom = () => {
            const panel = document.querySelector('.tab-column');
            panel.scrollTop = panel.scrollHeight;
        };
    }
    componentDidUpdate() {
        this.scrollToBottom();
    }
    componentDidMount() {
        this.scrollToBottom();
    }
    renderChats() {
        const allMessages = this.props.documents.selectedDocument.chatHistory.map(chatUi => {
            return chatUi.messages;
        }).reduce((a, b) => a.concat(b));
        const usersInChat = allMessages
            .map(msg => msg.caller)
            .filter((msg, index, array) => {
            if (array.indexOf(msg) === index) {
                return msg;
            }
        });
        const colors = [
            'indigo',
            'red',
            'cyan',
            'pink',
            'blue',
            'purple',
            'teal',
            'deep-purple',
        ];
        //const iconColor = colors[Math.floor((msg.caller.toUpperCase().charCodeAt(0) - 65) / 3.25)];
        return allMessages.map(msg => {
            const userIndex = usersInChat.indexOf(msg.caller);
            const iconColor = userIndex < colors.length ? colors[userIndex] : colors[userIndex % colors.length];
            return <TimelineEvent createdAt={toLocalDateStringUS(msg.time)} title={<span><b>{msg.caller}</b></span>} icon={<div className={'text-icon ' + iconColor}>{msg.caller.charAt(0)}</div>} contentStyle={{ backgroundColor: 'white', borderRadius: '5px' }} cardHeaderStyle={{ background: '#e4effb', color: 'inherit' }} container="card">
                {msg.message}
            </TimelineEvent>;
        });
    }
    render() {
        const { id, chatHistory } = this.props.documents.selectedDocument;
        const documentSelectedHasChats = id && chatHistory;
        return <div className="tab-column">
            <Timeline className="chat-panel">
                {documentSelectedHasChats && this.renderChats()}
            </Timeline>
        </div>;
    }
}
export default withRedux(ChatColumn);
//# sourceMappingURL=ChatColumn.jsx.map