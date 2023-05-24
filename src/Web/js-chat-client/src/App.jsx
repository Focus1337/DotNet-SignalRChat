import {BrowserRouter, Navigate, Route, Routes} from 'react-router-dom'
import './App.css';
import Chat from "./components/Chat/Chat";
import Login from "./components/Login/Login";
import Register from "./components/Register";
import Header from "./components/Header/Header";
import {userAuthorized} from "./utils/globals";
import React from "react";

function App() {
    const withAuth = (Component, navigatePath, isAuthorized) => {
        if (userAuthorized === isAuthorized) {
            return <Navigate to={navigatePath} replace/>;
        }
        return <Component/>;
    };

    return (
        <div className="App">
            <BrowserRouter>
                <Header/>
                <div id="container">
                    <Routes>
                        <Route path='/' element={withAuth(Chat, '/login', false)}/>
                        <Route path='/login' element={withAuth(Login, '/', true)}/>
                        <Route path='/register' element={withAuth(Register, '/', true)}/>
                    </Routes>
                </div>
            </BrowserRouter>
        </div>
    )
}

export default App;
