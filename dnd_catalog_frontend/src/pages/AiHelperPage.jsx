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
        err?.response?.data?.details ||
        err?.response?.data?.message ||
        err?.message ||
        "Помилка запиту до ІІ";
      setError(msg);
    } finally {
      setLoading(false);
    }
  };

  const styles = {
    page: {
      maxWidth: 980,
      margin: "0 auto",
      padding: "22px 16px 40px",
    },

    card: {
      border: "2px solid rgba(75,43,31,0.6)",
      borderRadius: 16,
      background:
        "linear-gradient(180deg, rgba(75,43,31,0.82), rgba(177,59,44,0.72))",
      boxShadow: "0 10px 30px rgba(0,0,0,0.35)",
      padding: 18,
      color: "#fdf7eb",
    },

    title: {
      margin: 0,
      fontSize: 26,
      textShadow: "0 2px 8px rgba(0,0,0,0.35)",
    },

    subtitle: {
      marginTop: 6,
      marginBottom: 0,
      opacity: 0.95,
      fontSize: 13,
    },

    paper: {
      marginTop: 14,
      background: "rgba(253, 247, 235, 0.92)", // пергамент
      color: "#2b1b12",
      borderRadius: 12,
      padding: 14,
      boxShadow: "inset 0 0 0 1px rgba(75,43,31,0.25)",
    },

    textarea: {
      width: "100%",
      padding: 12,
      fontSize: 14,
      borderRadius: 10,
      border: "1px solid rgba(75,43,31,0.35)",
      outline: "none",
      background: "rgba(255,255,255,0.75)",
      color: "#2b1b12",
      resize: "vertical",
      boxSizing: "border-box",
    },

    buttonsRow: {
      display: "flex",
      gap: 10,
      marginTop: 10,
      flexWrap: "wrap",
    },

    hint: {
      marginTop: 10,
      fontSize: 12,
      opacity: 0.95,
      lineHeight: 1.35,
    },

    error: {
      marginTop: 10,
      padding: "10px 12px",
      borderRadius: 10,
      background: "rgba(120, 20, 20, 0.15)",
      border: "1px solid rgba(120, 20, 20, 0.35)",
      color: "#ffd5d5",
      fontSize: 13,
      whiteSpace: "pre-wrap",
    },

    answerTitle: {
      marginTop: 0,
      marginBottom: 10,
      fontSize: 18,
      color: "#2b1b12",
    },

    pre: {
      whiteSpace: "pre-wrap",
      margin: 0,
      padding: 12,
      borderRadius: 10,
      background: "rgba(255,255,255,0.72)",
      border: "1px solid rgba(75,43,31,0.28)",
      color: "#2b1b12",
      lineHeight: 1.45,
      overflowWrap: "anywhere",
    },
  };

  return (
    <div style={styles.page}>
      <div style={styles.card}>
        <h2 style={styles.title}>AI Помічник по створенню D&amp;D персонажа</h2>
        <p style={styles.subtitle}>
          Опиши концепт: раса/клас/роль у групі/рівень — а я зберу лист персонажа.
        </p>

        <div style={styles.paper}>
          <form onSubmit={onSubmit}>
            <textarea
              value={prompt}
              onChange={(e) => setPrompt(e.target.value)}
              placeholder="Наприклад: Створи ельфа чаклуна 3 рівня, який кидається камінням і вогнем. Дай стати, навички, спорядження."
              rows={7}
              style={styles.textarea}
            />

            <div style={styles.buttonsRow}>
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

            <div style={styles.hint}>
              Порада: додай “рівень”, “роль у бою (танк/дпс/підтримка)” і “стиль
              (темний/героїчний/комедійний)”, щоб відповідь була точніше.
            </div>
          </form>

          {error && <div style={styles.error}>{error}</div>}

          {answer && (
            <div style={{ marginTop: 16 }}>
              <h3 style={styles.answerTitle}>Відповідь</h3>
              <pre style={styles.pre}>{answer}</pre>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
