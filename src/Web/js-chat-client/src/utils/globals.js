import {ACCESS_TOKEN_KEY} from "./env";

export let redirectToLogin = () => window.location.replace('/login');
export let redirectToHome = () => window.location.replace('/');
export const userAuthorized = !!localStorage.getItem(ACCESS_TOKEN_KEY);