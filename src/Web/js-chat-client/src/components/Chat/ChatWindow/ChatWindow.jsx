import ChatMessage from "./ChatMessage";

export default function ChatWindow(props) {
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