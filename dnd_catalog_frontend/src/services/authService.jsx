import api from "./api";

export async function registerRequest(data) {
    const res = await api.post("/Auth/register", data);
    return res.data; // { Message, Token, Role } или в camelCase, смотря настройки
}

export async function loginRequest(data) {
    const res = await api.post("/Auth/login", data);
    return res.data;
}

