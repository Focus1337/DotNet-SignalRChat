import Message from "./Message";

export default function ChatWindow(props) {
    let messages = props.chat.map(message => <Message
        key={Date.now() * Math.random()}
        name={message.name}
        text={message.text}
        sameUser={message.sameUser}/>);

    return (
        <div id="chat">
            {messages}
        </div>
    );
}