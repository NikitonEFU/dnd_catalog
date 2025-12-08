import api from "./api";

export async function createCharacter(character) {
    const res = await api.post("/Character", character);
    return res.data;
}

export async function getMyCharacters() {
    const res = await api.get("/Character/my");
    return res.data; // список персонажей
}

export async function deleteCharacter(id) {
    await api.delete(`/Character/${id}`);
}
