import ChatWindow from "./ChatWindow/ChatWindow";
import ChatInput from "./ChatInput";
import {useEffect, useState} from "react";
import {ACCESS_TOKEN_KEY, API_URL, USERNAME_KEY} from "../../utils/env";
import axios, {refreshAccessToken} from "../../utils/axios";
import IMessage from "../../models/IMessage";

export default function Chat() {
    // const [connection, setConnection] = useState(null);
    const [chat, setChat] = useState<IMessage[]>([]);

    // useEffect(() => {
    //     const connect = new HubConnectionBuilder()
    //         .withUrl(`${API_URL}/chat`, {
    //             accessTokenFactory: () => localStorage.getItem(ACCESS_TOKEN_KEY)
    //         })
    //         .configureLogging(LogLevel.Information)
    //         .withAutomaticReconnect({
    //             nextRetryDelayInMilliseconds: retryContext => {
    //                 if (retryContext.retryReason.message.includes('Unauthorized'))
    //                     refreshAccessToken();
    //
    //                 if (retryContext.elapsedMilliseconds < 60000)
    //                     return Math.random() * 10000;
    //
    //                 return null;
    //             }
    //         })
    //         .build();
    //
    //     setConnection(connect);
    // }, []);

    // useEffect(() => {
    //     if (!connection)
    //         return;
    //
    //     axios.get('/messages')
    //         .then(response => {
    //             const curUsername = localStorage.getItem(USERNAME_KEY);
    //             for (let message of response.data) {
    //                 setChat(prevState => [...prevState,
    //                     new Message(message.username, message.text, message.sentTime, message.username === curUsername)]);
    //             }
    //
    //             setTimeout(() => {
    //                 let chatElement = document.getElementById('chat');
    //                 chatElement.scroll(0, chatElement.scrollHeight);
    //             });
    //         })
    //         .catch(e => console.log(e));
    //
    //     async function start() {
    //         try {
    //             await connection.start();
    //             console.log('Connected');
    //         } catch (e) {
    //             console.log(e.message);
    //
    //             if (e.message.includes('Unauthorized'))
    //                 refreshAccessToken();
    //
    //             console.log('Restarting');
    //             setTimeout(start, 5000);
    //         }
    //     }
    //
    //     connection.on('ReceiveMessage', (res) => {
    //         let sameUser = localStorage.getItem(USERNAME_KEY) === res.name;
    //         setChat(prevState => [...prevState, new Message(res.name, res.text, res.sentTime, sameUser)]);
    //     });
    //
    //     connection.on('GetCurrentTime', (time) => {
    //         setChat(prevState => [...prevState, new Message("Current time is:", time, '', false)]);
    //     });
    //
    //     connection.onreconnecting(() => {
    //         console.log('Reconnecting');
    //     });
    //
    //     start();
    //
    // }, [connection]);

    let sendMessage = async function () { //(messageRequest) {
        // if (!connection)
        //     throw new Error('No chat connection');
        // try {
        //     let res = await connection.invoke('SendMessage', messageRequest);
        //     console.log(res);
        //     await axios.post('/messages', messageRequest);
        // } catch (e) {
        //     console.log(e);
        // }
    };

    let getCurrentTime = async () => {
        // if (!connection)
        //     throw new Error('No chat connection');
        // try {
        //     await axios.get(`/home`);
        // } catch (e) {
        //     alert(e.response.statusText);
        // }
    }

    return (
        <div id="chat-container">
            <ChatWindow chat={chat}></ChatWindow>

            <ChatInput sendMessage={sendMessage}></ChatInput>
            <button onClick={getCurrentTime} className="form-submit">Get current time!</button>
        </div>
    );
}