import axios from "axios";

const api = axios.create({
    baseURL: "https://localhost:7013/api", // ПОДСТАВЬ СВОЙ ПОРТ
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
