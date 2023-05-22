export let redirectToLogin = () => window.location.replace('/login');
export let redirectToHome = () => window.location.replace('/');
export const userAuthorized = !!localStorage.getItem('jwt');