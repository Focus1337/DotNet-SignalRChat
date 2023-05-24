import axios from "axios";
import {ACCESS_TOKEN_KEY, API_URL, REFRESH_TOKEN_KEY} from "./env"
import {redirectToLogin} from "./globals";

const instance = axios.create({
    baseURL: API_URL
});

instance.interceptors.request.use(
    config => {
        let accessToken = localStorage.getItem(ACCESS_TOKEN_KEY);
        if (accessToken) {
            config.headers.Authorization = `Bearer ${accessToken}`;
        }
        return config;
    },
    error => Promise.reject(error)
);

instance.interceptors.response.use(
    response => response,
    error => {
        const originalRequest = error.config;
        if (error.response.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;
            console.log('Refreshing access token');
            refreshAccessToken();
            return instance(originalRequest);
        }
        return Promise.reject(error);
    }
);

export const refreshAccessToken = () => {
    let data = {
        grant_type: "refresh_token",
        refresh_token: localStorage.getItem(REFRESH_TOKEN_KEY),
        access_token: localStorage.getItem(ACCESS_TOKEN_KEY)
    };

    instance.post('/connect/token', data, {
        headers: {"Content-Type": "application/x-www-form-urlencoded"}
    })
        .then(response => {
            localStorage.setItem(ACCESS_TOKEN_KEY, response.data.access_token);
            instance.defaults.headers.common['Authorization'] = 'Bearer ' + response.data.access_token;
        })
        .catch(() => {
            axios.post('/auth/logout');
            localStorage.removeItem(ACCESS_TOKEN_KEY);
            localStorage.removeItem(REFRESH_TOKEN_KEY);
            redirectToLogin();
        });
};

export default instance;