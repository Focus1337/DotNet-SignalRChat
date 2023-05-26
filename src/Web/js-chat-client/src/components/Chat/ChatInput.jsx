import {useState} from "react";
import MessageRequest from "../../models/MessageRequest";

export default function ChatInput(props) {
    const [text, setText] = useState('')

    let submitHandler = function (event) {
        event.preventDefault();

        if (!(text && text !== '')) {
            document.getElementById('text').focus();
            return;
        }

        props.sendMessage(new MessageRequest(text));
    }

    let inputChangeHandler = function (event) {
        switch (event.target.id) {
            case "text":
                setText(event.target.value);
                break;
            default:
                return;
        }
    }

    return (
        <div id="inputs">
            <form onSubmit={submitHandler}>
                <input id="text" type="text" placeholder="Text" className="form-input" value={text}
                       onChange={inputChangeHandler}/>
                <input type="submit" value="Send" className="form-submit"/>
            </form>
        </div>
    );
}