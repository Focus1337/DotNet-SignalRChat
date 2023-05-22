import axios from "axios";
import {API_URL} from "./env"

const jwt = localStorage.getItem('jwt');

const instance = axios.create({
    baseURL: API_URL,
    headers: {Authorization: `Bearer ${jwt}`}
});

export default instance;