import {useState} from "react";

export default function ChatInput(props) {
    const [name, setName] = useState('')
    const [text, setText] = useState('')

    let submitHandler = function (event) {
        event.preventDefault();

        if (name && name !== '' && text && text !== '') {
            props.sendMessage(name, text);
        } else {
            alert('Please, insert an name and a text.');
        }
    }

    let inputChangeHandler = function (event) {
        switch (event.target.id) {
            case "name":
                setName(event.target.value);
                break;
            case "text":
                setText(event.target.value);
                break;
            default:
                throw new Error("Invalid input");
        }
    }

    return (
        <div id="inputs">
            <form onSubmit={submitHandler}>
                <input id="name" type="text" placeholder="Name" className="form-input" value={name}
                       onChange={inputChangeHandler}/>
                <input id="text" type="text" placeholder="Text" className="form-input" value={text}
                       onChange={inputChangeHandler}/>
                <input type="submit" value="Send" className="form-submit"/>
            </form>
        </div>
    );
}