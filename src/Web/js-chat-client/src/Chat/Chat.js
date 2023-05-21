import ChatWindow from "./ChatWindow/ChatWindow";
import ChatInput from "./ChatInput";
import {HubConnectionBuilder} from "@microsoft/signalr";
import {useEffect, useState} from "react";

export default function Chat() {
    const [connection, setConnection] = useState(null);
    const [chat, setChat] = useState([]);

    useEffect(() => {
        const connect = new HubConnectionBuilder()
            .withUrl('http://localhost:5058/chat')
            .build();

        setConnection(connect);
    }, []);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => {
                    console.log('Connected');

                    connection.on('ReceiveMessage', (req) => {
                        let sameUser = connection.connectionId === req.connectionId;
                        setChat(prevState => [...prevState, {name: req.name, text: req.text, sameUser}]);
                    });

                    connection.on('GetCurrentTime', (time) => {
                        setChat(prevState => [...prevState, {name: "Current time is:", text: time, sameUser: false}]);
                    });
                })
                .catch(reason => {
                    console.log(reason);
                });
        }
    }, [connection]);

    let sendMessage = async function (name, text) {
        if (connection) {
            try {
                await connection.send('SendMessage', name, text, connection.connectionId);
                console.log(await connection.invoke('GetTotalLength', {param1: text, param2: name}));
            } catch (e) {
                console.log(e);
            }
        } else
            throw new Error('Lost connection');
    }

    let getCurrentTime = async function () {
        if (connection) {
            let response = await fetch(`http://localhost:5058/home/${connection.connectionId}`);

            console.log(await response.text());
        }
    }

    return (
        <div id="chat-container">
            <ChatWindow chat={chat}></ChatWindow>

            <ChatInput sendMessage={sendMessage}></ChatInput>
            <button onClick={getCurrentTime} className="form-submit">Get current time!</button>
        </div>
    );
}