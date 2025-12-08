import { createContext, useContext, useState, useEffect } from "react";
import { setAuthToken } from "../services/api";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
    const [token, setToken] = useState(localStorage.getItem("token"));
    const [role, setRole] = useState(localStorage.getItem("role"));

    useEffect(() => {
        setAuthToken(token);
    }, [token]);

    const isAuthenticated = !!token;
    const isAdmin = role === "admin";

    function login(tokenValue, roleValue) {
        setToken(tokenValue);
        setRole(roleValue);
        localStorage.setItem("token", tokenValue);
        localStorage.setItem("role", roleValue);
    }

    function logout() {
        setToken(null);
        setRole(null);
        localStorage.removeItem("token");
        localStorage.removeItem("role");
    }

    return (
        <AuthContext.Provider value={{ token, role, isAuthenticated, isAdmin, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    return useContext(AuthContext);
}
