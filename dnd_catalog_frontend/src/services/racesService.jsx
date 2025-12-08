import api from "./api";

export async function getRaces(includeAbilities = false) {
    const res = await api.get(`/Races?includeAbilities=${includeAbilities}`);
    return res.data;
}

export async function getRace(id, includeAbilities = false) {
    const res = await api.get(`/Races/${id}?includeAbilities=${includeAbilities}`);
    return res.data;
}

export async function createRace(race) {
    const res = await api.post("/Races", race);
    return res.data;
}

export async function updateRace(id, race) {
    await api.put(`/Races/${id}`, race);
}

export async function deleteRace(id) {
    await api.delete(`/Races/${id}`);
}
