import api from "./api";

export async function getClasses(includeAbilities = false) {
    const res = await api.get(`/Classes?includeAbilities=${includeAbilities}`);
    return res.data;
}

export async function getClassById(id, includeAbilities = false) {
    const res = await api.get(`/Classes/${id}?includeAbilities=${includeAbilities}`);
    return res.data;
}

export async function createClassItem(data) {
    const res = await api.post("/Classes", data);
    return res.data;
}

export async function updateClassItem(id, data) {
    await api.put(`/Classes/${id}`, data);
}

export async function deleteClassItem(id) {
    await api.delete(`/Classes/${id}`);
}
