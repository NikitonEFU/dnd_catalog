import { BrowserRouter, Routes, Route } from "react-router-dom";
import { AuthProvider, useAuth } from "./context/AuthContext";
import { setAuthToken } from "./services/api";
import Header from "./components/Header";
import ProtectedRoute from "./components/ProtectedRoute";

import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import RacesPage from "./pages/RacesPage";
import ClassesPage from "./pages/ClassesPage";
import AbilitiesPage from "./pages/AbilitiesPage";
import CharacterEditorPage from "./pages/CharacterEditorPage";
import AccountPage from "./pages/AccountPage";

function AppInner() {
    const { token } = useAuth();

    // каждый рендер будем обновлять токен в axios
    setAuthToken(token);

    return (
        <div className="app-root">
            <Header />
            <main className="app-content">
                <Routes>
                    <Route path="/" element={<HomePage />} />
                    <Route path="/login" element={<LoginPage />} />
                    <Route path="/register" element={<RegisterPage />} />

                    <Route
                        path="/characters"
                        element={
                            <ProtectedRoute>
                                <CharacterEditorPage />
                            </ProtectedRoute>
                        }
                    />

                    <Route path="/races" element={<RacesPage />} />
                    <Route path="/classes" element={<ClassesPage />} />
                    <Route path="/abilities" element={<AbilitiesPage />} />

                    <Route
                        path="/account"
                        element={
                            <ProtectedRoute>
                                <AccountPage />
                            </ProtectedRoute>
                        }
                    />
                </Routes>
            </main>
        </div>
    );
}

export default function App() {
    return (
        <AuthProvider>
            <BrowserRouter>
                <AppInner />
            </BrowserRouter>
        </AuthProvider>
    );
}
