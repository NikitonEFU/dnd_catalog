import api from "./api";

export async function getAbilities() {
    const res = await api.get("/Abilities");
    return res.data;
}

export async function getAbility(id) {
    const res = await api.get(`/Abilities/${id}`);
    return res.data;
}

export async function createAbility(data) {
    const res = await api.post("/Abilities", data);
    return res.data;
}

export async function updateAbility(id, data) {
    await api.put(`/Abilities/${id}`, data);
}

export async function deleteAbility(id) {
    await api.delete(`/Abilities/${id}`);
}
