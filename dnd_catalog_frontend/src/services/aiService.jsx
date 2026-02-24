import api from "./api";

export async function askAiCharacterHelp(prompt) {
  const { data } = await api.post("/ai/character-help", { prompt });
  return data; // { text: "..." }
}
