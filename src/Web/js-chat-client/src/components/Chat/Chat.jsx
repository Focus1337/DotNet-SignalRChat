import ChatWindow from "./ChatWindow/ChatWindow";
import ChatInput from "./ChatInput";
import {HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import {useEffect, useState} from "react";
import {ACCESS_TOKEN_KEY, API_URL} from "../../utils/env";
import axios, {refreshAccessToken} from "../../utils/axios";

export default function Chat() {
    const [connection, setConnection] = useState(null);
    const [chat, setChat] = useState([]);

    useEffect(() => {
        const connect = new HubConnectionBuilder()
            .withUrl(`${API_URL}/chat`, {
                accessTokenFactory: () => localStorage.getItem(ACCESS_TOKEN_KEY)
            })
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect({
                nextRetryDelayInMilliseconds: retryContext => {
                    if (retryContext.retryReason.message.includes('Unauthorized'))
                        refreshAccessToken();

                    if (retryContext.elapsedMilliseconds < 60000)
                        return Math.random() * 10000;

                    return null;
                }
            })
            .build();

        setConnection(connect);
    }, []);

    useEffect(() => {
        if (!connection)
            return;

        async function start() {
            try {
                await connection.start();
                console.log('Connected');
            } catch (e) {
                console.log(e.message);

                if (e.message.includes('Unauthorized'))
                    refreshAccessToken();

                console.log('Restarting');
                setTimeout(start, 5000);
            }
        }

        connection.on('ReceiveMessage', (req) => {
            let sameUser = connection.connectionId === req.connectionId;
            setChat(prevState => [...prevState, {name: req.name, text: req.text, sameUser}]);
        });

        connection.on('GetCurrentTime', (time) => {
            setChat(prevState => [...prevState, {name: "Current time is:", text: time, sameUser: false}]);
        });

        connection.onreconnecting(() => {
            console.log('Reconnecting');
        });

        start();

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

    let getCurrentTime = function () {
        if (connection) {
            axios.get(`/home/${connection.connectionId}`)
                .then(response => {
                    console.log(response.data);
                })
                .catch(e => {
                    alert(e.response.statusText);
                });
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