import axios from "axios";

// *** ИСПРАВЛЕНО: Чтение URL из переменной окружения Render/Vite ***
const API_BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:7013"; 
// Мы используем import.meta.env.VITE_API_URL, а localhost оставляем как запасной вариант для локальной разработки.

const api = axios.create({
    // Используем переменную окружения
    baseURL: `${API_BASE_URL}/api`, 
});

// ставим JWT токен
export function setAuthToken(token) {
    if (token) {
        api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
    } else {
        delete api.defaults.headers.common["Authorization"];
    }
}

export default api;
