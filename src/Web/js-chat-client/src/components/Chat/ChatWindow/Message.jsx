export default function Message(props) {
    return (
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
    );
}