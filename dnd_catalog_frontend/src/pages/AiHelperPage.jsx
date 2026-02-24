import React, { useState } from "react";
import { askAiCharacterHelp } from "../services/aiService";

export default function AiHelperPage() {
  const [prompt, setPrompt] = useState("");
  const [answer, setAnswer] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const onSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setAnswer("");

    const trimmed = prompt.trim();
    if (!trimmed) {
      setError("Напиши запрос 🙂");
      return;
    }

    try {
      setLoading(true);
      const res = await askAiCharacterHelp(trimmed);
      setAnswer(res?.text ?? "");
    } catch (err) {
      const msg =
        err?.response?.data?.message ||
        err?.response?.data?.details ||
        err?.message ||
        "Помилка запиту до ІІ";
      setError(msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ maxWidth: 900, margin: "0 auto", padding: 16 }}>
      <h2>AI Помічник по створенню D&amp;D персонажа</h2>

      <form onSubmit={onSubmit} style={{ marginTop: 12 }}>
        <textarea
          value={prompt}
          onChange={(e) => setPrompt(e.target.value)}
          placeholder="Например: Створи дворфа-паладина 3 рівня, танк + підтримка. Дай розподіл статів, навички, спорядження."
          rows={7}
          style={{ width: "100%", padding: 10, fontSize: 14 }}
        />

        <div style={{ display: "flex", gap: 10, marginTop: 10 }}>
          <button className="btn" type="submit" disabled={loading}>
            {loading ? "Генерую..." : "Запитати"}
          </button>

          <button
            className="btn"
            type="button"
            disabled={loading}
            onClick={() => {
              setPrompt("");
              setAnswer("");
              setError("");
            }}
          >
            Очистити
          </button>
        </div>
      </form>

      {error && (
        <div style={{ color: "darkred", marginTop: 10, fontSize: 13 }}>
          {error}
        </div>
      )}

      {answer && (
        <div style={{ marginTop: 16 }}>
          <h3>Відповідь</h3>
          <pre style={{ whiteSpace: "pre-wrap", padding: 12, borderRadius: 8 }}>
            {answer}
          </pre>
        </div>
      )}
    </div>
  );
}
