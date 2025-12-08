import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { loginRequest } from "../services/authService";
import { useAuth } from "../context/AuthContext";

export default function LoginPage() {
    const [form, setForm] = useState({ username: "", password: "" });
    const [error, setError] = useState("");
    const navigate = useNavigate();
    const { login } = useAuth();

    async function handleSubmit(e) {
        e.preventDefault();
        setError("");
        try {
            const data = await loginRequest({
                username: form.username,
                password: form.password,
            });
            const token = data.token ?? data.Token;
            const role = data.role ?? data.Role ?? "user";
            if (!token) {
                setError("Токен не отримано від сервера."); // Токен не получен от сервера.
                return;
            }
            login(token, role);
            navigate("/");
        } catch (e) {
            console.error(e);
            setError("Помилка входу. Перевірте логін та пароль."); // Ошибка входа. Проверьте логин и пароль.
        }
    }

    function handleChange(e) {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    }

    return (
        <div className="card" style={{ maxWidth: 380, margin: "0 auto" }}>
            <h2>Вхід</h2> {/* Вход */}
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Логін</label> {/* Логин */}
                    <input
                        name="username"
                        value={form.username}
                        onChange={handleChange}
                        required
                    />
                </div>
                <div>
                    <label>Пароль</label> {/* Пароль */}
                    <input
                        type="password"
                        name="password"
                        value={form.password}
                        onChange={handleChange}
                        required
                    />
                </div>
                {error && (
                    <div style={{ color: "darkred", fontSize: 13, marginTop: 4 }}>
                        {error}
                    </div>
                )}
                <button className="btn" type="submit" style={{ marginTop: 8 }}>
                    Увійти {/* Войти */}
                </button>
            </form>
            <p style={{ marginTop: 8, fontSize: 13 }}>
                Немає акаунту? <Link to="/register">Реєстрація</Link> {/* Нет аккаунта? Регистрация */}
            </p>
        </div>
    );
}