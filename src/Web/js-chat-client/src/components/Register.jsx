import {useState} from "react";
import {redirectToLogin} from "../utils/globals";
import axios from "../utils/axios";

export default function Register() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    
    let inputChangeHandler = function (event) {
        switch (event.target.id) {
            case "email":
                setEmail(event.target.value);
                break;
            case "password":
                setPassword(event.target.value);
                break;
            default:
                return;
        }
    }

    let sendRequest = function (email, password) {
        let data = {
            email: email,
            password: password
        };

        axios.post('/auth/register', data)
            .then(response => {
                console.log(response.data);
                redirectToLogin();
            })
            .catch(e => {
                console.log(e.response.data);
                alert(e.response.statusText);
            });
    }

    let submitHandler = function (event) {
        event.preventDefault();

        if (email && email !== '' && password && password !== '') {
            sendRequest(email, password);
        } else {
            alert('Please, fill inputs.');
        }
    }

    return (
        <div id="login-container">
            <form onSubmit={submitHandler}>
                <div className="login-text">
                    <p className="form-text">Register</p>
                </div>

                <input id="email" type="email" placeholder="Email" className="form-input" value={email}
                       onChange={inputChangeHandler}/>
                <input id="password" type="password" placeholder="Password" className="form-input" value={password}
                       onChange={inputChangeHandler}/>
                <input type="submit" value="Register" className="form-submit"/>
            </form>

            <div className="login-bottom form-input">
                <p>Already registered?</p>
                <a href="/login">Login</a>
            </div>
        </div>
    );
}