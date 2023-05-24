import {useState} from "react";
import axios from "../../utils/axios";
import {redirectToHome} from "../../utils/globals";
import {ACCESS_TOKEN_KEY, REFRESH_TOKEN_KEY} from "../../utils/env";

export default function Login() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');

    let inputChangeHandler = function (event) {
        switch (event.target.id) {
            case "email":
                setUsername(event.target.value);
                break;
            case "password":
                setPassword(event.target.value);
                break;
            default:
                return;
        }
    }

    let sendRequest = function (username, password) {
        let data = {
            username: username,
            password: password,
            grant_type: "password",
            scope: "offline_access"
        };

        axios.post('/connect/token', data, {
            headers: {"Content-Type": "application/x-www-form-urlencoded"}
        })
            .then(response => {
                console.log(response.data);
                localStorage.setItem(ACCESS_TOKEN_KEY, response.data.access_token);
                localStorage.setItem(REFRESH_TOKEN_KEY, response.data.refresh_token);
                redirectToHome();
            })
            .catch(e => {
                console.log(e.response.data);
                alert(e.response.statusText);
            });
    }

    let submitHandler = function (event) {
        event.preventDefault();

        if (username && username !== '' && password && password !== '') {
            sendRequest(username, password);
        } else {
            alert('Please, fill inputs.');
        }
    }

    return (
        <div id="login-container">
            <form onSubmit={submitHandler}>
                <div className="login-text">
                    <p className="form-text">Login</p>
                </div>

                <input id="email" type="email" placeholder="Email" className="form-input" value={username}
                       onChange={inputChangeHandler}/>
                <input id="password" type="password" placeholder="Password" className="form-input" value={password}
                       onChange={inputChangeHandler}/>
                <input type="submit" value="Login" className="form-submit"/>
            </form>

            <div className="login-bottom form-input">
                <p>Not registered yet?</p>
                <a href="/register">Register</a>
            </div>
        </div>
    );
}
