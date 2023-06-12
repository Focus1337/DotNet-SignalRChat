import {Link} from "react-router-dom";
import {redirectToLogin} from "../../utils/globals";
import axios from "../../utils/axios";
import {ACCESS_TOKEN_KEY, REFRESH_TOKEN_KEY, USERNAME_KEY} from "../../utils/env";

export default function Header() {
    const onSignOut = async () => {
        await axios.post('/auth/logout');
        localStorage.removeItem(ACCESS_TOKEN_KEY);
        localStorage.removeItem(REFRESH_TOKEN_KEY);
        localStorage.removeItem(USERNAME_KEY);
        redirectToLogin();
    }

    return (
        <div className="header">
            <div className="header-item">
                <Link to={'/'}>Home</Link>
            </div>
            <div className="header-item">
                <Link to={'/login'}>Login</Link>
            </div>
            <div className="header-item">
                <Link to={'/register'}>Register</Link>
            </div>
            <div className="header-item">
                <button type="button" onClick={onSignOut}>Sign out</button>
            </div>
        </div>
    );
}