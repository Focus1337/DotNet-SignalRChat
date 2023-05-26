import ChatWindow from "./ChatWindow/ChatWindow";
import ChatInput from "./ChatInput";
import {HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import {useEffect, useState} from "react";
import {ACCESS_TOKEN_KEY, API_URL, USERNAME_KEY} from "../../utils/env";
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

        connection.on('ReceiveMessage', (res) => {
            let sameUser = localStorage.getItem(USERNAME_KEY) === res.name;
            setChat(prevState => [...prevState, {name: res.name, text: res.text, sameUser}]);
        });

        connection.on('GetCurrentTime', (time) => {
            setChat(prevState => [...prevState, {name: "Current time is:", text: time, sameUser: false}]);
        });

        connection.onreconnecting(() => {
            console.log('Reconnecting');
        });

        start();

    }, [connection]);

    // let getStream = (name) =>
    //     connection.stream("Counter", 10, 500)
    //         .subscribe({
    //             next: async (item) => {
    //                 await connection.send('SendMessage', name, item.toString(), connection.connectionId);
    //             },
    //             complete: async () => {
    //                 await connection.send('SendMessage', name, "Stream completed", connection.connectionId);
    //             },
    //             error: async (err) => {
    //                 await connection.send('SendMessage', name, err.toString(), connection.connectionId);
    //             },
    //         });

    let sendMessage = async function (messageRequest) {
        if (!connection)
            throw new Error('No chat connection');
        try {
            await connection.send('SendMessage', messageRequest);
            console.log(await connection.invoke('GetTotalLength', {param1: messageRequest.text}));
        } catch (e) {
            console.log(e);
        }
    };

    let getCurrentTime = async () => {
        if (!connection)
            throw new Error('No chat connection');
        try {
            await axios.get(`/home`);
        } catch (e) {
            alert(e.response.statusText);
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