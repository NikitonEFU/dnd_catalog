import { useEffect, useState } from "react";
import {
    getAbilities,
    createAbility,
    deleteAbility,
} from "../services/abilitiesService";
import { useAuth } from "../context/AuthContext";

export default function AbilitiesPage() {
    const { isAdmin } = useAuth();

    const [abilities, setAbilities] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [newName, setNewName] = useState("");
    const [newDescription, setNewDescription] = useState("");

    async function load() {
        setLoading(true);
        setError("");
        try {
            const data = await getAbilities();
            setAbilities(data);
        } catch (e) {
            console.error(e);
            setError("Помилка завантаження здібностей."); // Помилка загрузки способностей.
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        load();
    }, []);

    async function handleCreate(e) {
        e.preventDefault();
        if (!newName.trim() || !newDescription.trim()) return;

        const payload = {
            name: newName,
            description: newDescription,
        };

        try {
            await createAbility(payload);
            setNewName("");
            setNewDescription("");
            load();
        } catch (e) {
            console.error(e);
            setError("Помилка при додаванні здібності (потрібні права адміна?)."); // Ошибка при добавлении способности (нужны права админа?).
        }
    }

    async function handleDelete(id) {
        if (!window.confirm("Видалити цю здібність?")) return; // Удалить эту способность?
        try {
            await deleteAbility(id);
            load();
        } catch (e) {
            console.error(e);
            setError("Помилка при видаленні здібності (потрібні права адміна?)."); // Ошибка при удалении способности (нужны права админа?).
        }
    }

    return (
        <div className="card">
            <h2>Здібності</h2> {/* Способности */}

            {error && (
                <div style={{ color: "darkred", fontSize: 13, marginBottom: 6 }}>
                    {error}
                </div>
            )}

            {loading && <p>Завантаження...</p>} {/* Загрузка... */}

            {!loading && (
                <>
                    <table className="table">
                        <thead>
                            <tr>
                                <th>Назва</th> {/* Название */}
                                <th>Опис</th> {/* Описание */}
                                {isAdmin && <th />}
                            </tr>
                        </thead>
                        <tbody>
                            {abilities.map((ab) => (
                                <tr key={ab.id}>
                                    <td>{ab.name}</td>
                                    <td style={{ maxWidth: 400 }}>{ab.description}</td>
                                    {isAdmin && (
                                        <td>
                                            <button
                                                className="btn btn-outline"
                                                onClick={() => handleDelete(ab.id)}
                                            >
                                                Видалити {/* Удалить */}
                                            </button>
                                        </td>
                                    )}
                                </tr>
                            ))}
                        </tbody>
                    </table>

                    {isAdmin && (
                        <div style={{ marginTop: 18 }}>
                            <h3>Додати нову здібність</h3> {/* Добавить новую способность */}
                            <form onSubmit={handleCreate}>
                                <div>
                                    <label>Назва</label> {/* Название */}
                                    <input
                                        value={newName}
                                        onChange={(e) => setNewName(e.target.value)}
                                        required
                                    />
                                </div>
                                <div>
                                    <label>Опис</label> {/* Описание */}
                                    <textarea
                                        rows={3}
                                        value={newDescription}
                                        onChange={(e) => setNewDescription(e.target.value)}
                                        required
                                    />
                                </div>
                                <button className="btn" type="submit" style={{ marginTop: 8 }}>
                                    Додати здібність {/* Добавить способность */}
                                </button>
                            </form>
                        </div>
                    )}
                </>
            )}
        </div>
    );
}