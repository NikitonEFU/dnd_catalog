import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

export default function Header() {
    const { isAuthenticated, user, logout, isAdmin } = useAuth();

    return (
        <header
            style={{
                borderBottom: "2px solid rgba(75,43,31,0.6)",
                background:
                    "linear-gradient(90deg, rgba(75,43,31,0.95), rgba(177,59,44,0.9))",
                color: "#fdf7eb",
                boxShadow: "0 4px 10px rgba(0,0,0,0.4)",
            }}
        >
            <div
                style={{
                    maxWidth: "1100px",
                    margin: "0 auto",
                    padding: "10px 16px",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "space-between",
                    gap: "16px",
                }}
            >
                {/* ЛІВА ЧАСТИНА: ЛОГО + ТЕКСТ */}
                <Link
                    to="/"
                    style={{
                        display: "flex",
                        alignItems: "center",
                        gap: "10px",
                        textDecoration: "none",
                        color: "inherit",
                    }}
                >
                    <img
                        src="https://www.nicepng.com/png/full/14-147008_d-d-5th-edition-logo-latest-dd-logo.png"
                        alt="DnD Logo"
                        style={{
                            height: "40px",
                            width: "auto",
                            objectFit: "contain",
                            filter: "drop-shadow(0 0 4px rgba(0,0,0,0.6))",
                        }}
                    />

                    <div>
                        <h1 style={{ fontSize: "20px", marginBottom: "2px" }}>
                            DnD Grimoire
                        </h1>
                        <div style={{ fontSize: "12px", opacity: 0.9 }}>
                            Креатор персонажів &amp; каталог
                        </div>
                    </div>
                </Link>

                {/* НАВІГАЦІЯ */}
                <nav style={{ display: "flex", gap: "10px", fontSize: "14px" }}>
                    <Link to="/characters">Персонажі</Link>
                    <Link to="/races">Раси</Link>
                    <Link to="/classes">Класи</Link>
                    <Link to="/abilities">Здібності</Link>
                    <Link to="/ai">ШІ</Link>
                    {isAuthenticated && <Link to="/account">Акаунт</Link>}
                    {isAdmin && <span style={{ fontStyle: "italic" }}>Адмін</span>}
                </nav>

                {/* ПРАВА ЧАСТИНА — АККАУНТ */}
                <div style={{ display: "flex", alignItems: "center", gap: "8px" }}>
                    {isAuthenticated ? (
                        <>
                            <span style={{ fontSize: "13px" }}>
                                {user?.username || user?.email}
                            </span>
                            <button className="btn btn-outline" onClick={logout}>
                                Вийти
                            </button>
                        </>
                    ) : (
                        <>
                            <Link to="/login" className="btn btn-outline">
                                Вхід
                            </Link>
                            <Link to="/register" className="btn">
                                Реєстрація
                            </Link>
                        </>
                    )}
                </div>
            </div>
        </header>
    );
}
