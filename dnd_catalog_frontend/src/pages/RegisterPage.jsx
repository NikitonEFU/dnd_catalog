import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { registerRequest } from "../services/authService"; // loginRequest не використовується в логіці, але залишаю імпорт, якщо це потрібно для подальшого коду
import { useAuth } from "../context/AuthContext";

export default function RegisterPage() {
    const [form, setForm] = useState({
        username: "",
        password: "",
    });

    const [error, setError] = useState("");
    const navigate = useNavigate();
    const { login } = useAuth();

    function handleChange(e) {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    }

    async function handleSubmit(e) {
        e.preventDefault();
        setError("");

        try {
            // 1) РЕЄСТРАЦІЯ
            const reg = await registerRequest({
                username: form.username,
                password: form.password,
            });

            // 2) ОТРИМУЄМО ТОКЕН + РОЛЬ
            const token = reg.token ?? reg.Token;
            const role = reg.role ?? reg.Role ?? "user";

            if (!token) {
                setError("Сервер не повернув токен."); // Сервер не вернул токен.
                return;
            }

            // 3) ЛОГІНИМО КОРИСТУВАЧА
            login(token, role);

            // 4) ПЕРЕНОСИМО НА ГОЛОВНУ
            navigate("/");
        } catch (err) {
            console.error(err);
            setError("Помилка реєстрації."); // Ошибка регистрации.
        }
    }

    return (
        <div className="card" style={{ maxWidth: 420, margin: "0 auto" }}>
            <h2>Реєстрація</h2> {/* Регистрация */}
            <form onSubmit={handleSubmit}>
                <div>
                    <label>Ім'я користувача</label> {/* Имя пользователя */}
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
                    <div style={{ color: "darkred", fontSize: 13 }}>
                        {error}
                    </div>
                )}

                <button className="btn" type="submit">
                    Створити акаунт {/* Создать аккаунт */}
                </button>
            </form>

            <p style={{ marginTop: 8, fontSize: 13 }}>
                Вже маєте акаунт? <Link to="/login">Увійти</Link> {/* Уже есть аккаунт? Войти */}
            </p>
        </div>
    );
}