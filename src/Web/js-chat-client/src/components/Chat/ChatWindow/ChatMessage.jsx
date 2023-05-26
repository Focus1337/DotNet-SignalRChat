export default function ChatMessage(props) {
    return (
        <div id="outer-bubble" className={props.sameUser ? "own-message" : "other-message"}>
            <div id="bubble" className={props.sameUser ? "own-message" : "other-message"}>
                <img id="bubble-image" alt="avatar"
                     src="https://www.vhv.rs/dpng/d/119-1199788_user-vector-icon-png-clipart-png-download-icon.png"/>
                <div id="bubble-content" className={props.sameUser ? "own-message" : "other-message"}>
                    <div id="bubble-name" className={props.sameUser ? "own-message" : "other-message"}>
                        {props.name}
                    </div>
                    <div id="bubble-text" className={props.sameUser ? "own-message" : "other-message"}>
                        {props.text}
                    </div>
                </div>
            </div>
            <div id="sent-time" className={props.sameUser ? "own-message tooltip" : "other-message tooltip"}>
                {new Date(Date.parse(props.sentTime)).toLocaleTimeString()}
                <span className="tooltip-text">{new Date(Date.parse(props.sentTime)).toLocaleString()}</span>
            </div>
        </div>
    );
}