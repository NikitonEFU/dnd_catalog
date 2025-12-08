import { useEffect, useState } from "react";
import {
    getClasses,
    createClassItem,
    deleteClassItem,
} from "../services/classesService";
import { getAbilities } from "../services/abilitiesService";
import { useAuth } from "../context/AuthContext";

export default function ClassesPage() {
    const { isAdmin } = useAuth();

    const [classes, setClasses] = useState([]);
    const [abilities, setAbilities] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [newName, setNewName] = useState("");
    const [hitDie, setHitDie] = useState("d8");
    const [selectedAbilityIds, setSelectedAbilityIds] = useState([]);

    async function load() {
        setLoading(true);
        setError("");
        try {
            const [classesData, abilitiesData] = await Promise.all([
                getClasses(true),
                getAbilities(),
            ]);
            setClasses(classesData);
            setAbilities(abilitiesData);
        } catch (e) {
            console.error(e);
            setError("Помилка завантаження класів."); // Ошибка загрузки классов.
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        load();
    }, []);

    function toggleAbilitySelection(id) {
        setSelectedAbilityIds((prev) =>
            prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
        );
    }

    async function handleCreate(e) {
        e.preventDefault();
        if (!newName.trim() || !hitDie.trim()) return;

        const payload = {
            name: newName,
            hitDie,
            abilityIds: selectedAbilityIds,
        };

        try {
            await createClassItem(payload);
            setNewName("");
            setHitDie("d8");
            setSelectedAbilityIds([]);
            load();
        } catch (e) {
            console.error(e);
            setError("Помилка при додаванні класу (потрібні права адміна?)."); // Ошибка при добавлении класса (нужны права админа?).
        }
    }

    async function handleDelete(id) {
        if (!window.confirm("Видалити цей клас?")) return; // Удалить этот класс?
        try {
            await deleteClassItem(id);
            load();
        } catch (e) {
            console.error(e);
            setError("Помилка при видаленні класу (потрібні права адміна?)."); // Ошибка при удалении класса (нужны права админа?).
        }
    }

    return (
        <div className="card">
            <h2>Класи</h2> {/* Классы */}

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
                                <th>Кістка хітів</th> {/* Кость хитов */}
                                <th>Здібності</th> {/* Способности */}
                                {isAdmin && <th />}
                            </tr>
                        </thead>
                        <tbody>
                            {classes.map((c) => (
                                <tr key={c.id}>
                                    <td>{c.name}</td>
                                    <td>{c.hitDie}</td>
                                    <td style={{ maxWidth: 260 }}>
                                        {c.abilities && c.abilities.length > 0 ? (
                                            c.abilities.map((ab) => (
                                                <span key={ab.id} className="chip">
                                                    {ab.name}
                                                </span>
                                            ))
                                        ) : (
                                            <span style={{ fontSize: 12, opacity: 0.7 }}>
                                                Немає здібностей {/* Нет способностей */}
                                            </span>
                                        )}
                                    </td>
                                    {isAdmin && (
                                        <td>
                                            <button
                                                className="btn btn-outline"
                                                onClick={() => handleDelete(c.id)}
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
                            <h3>Додати новий клас</h3> {/* Добавить новый класс */}
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
                                    <label>Кістка хітів</label> {/* Кость хитов */}
                                    <input
                                        value={hitDie}
                                        onChange={(e) => setHitDie(e.target.value)}
                                        placeholder="Наприклад: d8, d10, d12" // Например: d8, d10, d12
                                        required
                                    />
                                </div>

                                <div>
                                    <label>Здібності</label> {/* Способности */}
                                    <div
                                        style={{
                                            display: "grid",
                                            gridTemplateColumns:
                                                "repeat(auto-fit, minmax(180px, 1fr))",
                                            gap: 4,
                                            marginTop: 4,
                                        }}
                                    >
                                        {abilities.map((ab) => (
                                            <label
                                                key={ab.id}
                                                style={{
                                                    fontSize: 13,
                                                    display: "flex",
                                                    alignItems: "center",
                                                    gap: 4,
                                                }}
                                            >
                                                <input
                                                    type="checkbox"
                                                    checked={selectedAbilityIds.includes(ab.id)}
                                                    onChange={() => toggleAbilitySelection(ab.id)}
                                                />
                                                {ab.name}
                                            </label>
                                        ))}
                                    </div>
                                </div>

                                <button className="btn" type="submit" style={{ marginTop: 8 }}>
                                    Додати клас {/* Добавить класс */}
                                </button>
                            </form>
                        </div>
                    )}
                </>
            )}
        </div>
    );
}