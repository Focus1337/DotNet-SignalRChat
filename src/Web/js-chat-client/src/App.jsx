import {BrowserRouter, Navigate, Route, Routes} from 'react-router-dom'
import './App.css';
import Chat from "./components/Chat/Chat";
import Login from "./components/Login/Login";
import Register from "./components/Register";
import Header from "./components/Header/Header";
import {userAuthorized} from "./utils/globals";
import React from "react";

function App() {
    return (
        <div className="App">

            <BrowserRouter>
                <Header/>
                <div id="container">
                    <Routes>
                        <Route path='/' element={!userAuthorized ? <Navigate to="/login" replace/> : <Chat/>}/>
                        <Route path='/login' element={userAuthorized ? <Navigate to="/" replace/> : <Login/>}/>
                        <Route path='/register' element={userAuthorized ? <Navigate to="/" replace/> : <Register/>}/>
                    </Routes>
                </div>
            </BrowserRouter>

        </div>
    )
}

export default App;
