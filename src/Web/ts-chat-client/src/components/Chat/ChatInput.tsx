import {ChangeEvent, FormEvent, useState} from "react";

interface ChatInputProps {
    sendMessage(): Promise<void>;
}

export default function ChatInput(props: ChatInputProps) {
    const [text, setText] = useState('')

    let submitHandler = function (event: FormEvent) {
        event.preventDefault();

        if (!(text && text !== '')) {
            let elem = document.getElementById('text');
            if (elem === null) return;

            elem.focus();
        }

        props.sendMessage();//(new MessageRequest(text));
    }

    let inputChangeHandler = function (event: ChangeEvent<HTMLInputElement>) {
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