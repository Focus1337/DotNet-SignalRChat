import ChatMessage from "./ChatMessage";
import IMessage from "../../../models/IMessage";

interface ChatWindowProps {
    chat: IMessage[];
}

export default function ChatWindow(props: ChatWindowProps) {
    let messages = props.chat.map(message => <ChatMessage
        key={Date.now() * Math.random()}
        name={message.name}
        text={message.text}
        sentTime={message.sentTime}
        sameUser={message.sameUser}/>);

    return (
        <div id="chat">
            {messages}
        </div>
    );
}