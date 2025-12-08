import { useEffect, useState } from "react";
import { getRaces, createRace, deleteRace } from "../services/racesService";
import { getAbilities } from "../services/abilitiesService";
import { useAuth } from "../context/AuthContext";

export default function RacesPage() {
    const { isAdmin } = useAuth();

    const [races, setRaces] = useState([]);
    const [abilities, setAbilities] = useState([]);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    // форма створення
    const [newName, setNewName] = useState("");
    const [newDescription, setNewDescription] = useState("");
    const [selectedAbilityIds, setSelectedAbilityIds] = useState([]);

    async function load() {
        setLoading(true);
        setError("");
        try {
            const [racesData, abilitiesData] = await Promise.all([
                getRaces(true), // includeAbilities = true
                getAbilities(),
            ]);
            setRaces(racesData);
            setAbilities(abilitiesData);
        } catch (e) {
            console.error(e);
            setError("Помилка завантаження рас."); // Ошибка загрузки рас.
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
            abilityIds: selectedAbilityIds,
        };

        try {
            await createRace(payload);
            setNewName("");
            setNewDescription("");
            setSelectedAbilityIds([]);
            load();
        } catch (e) {
            console.error(e);
            setError("Помилка при додаванні раси (потрібні права адміна?)."); // Ошибка при добавлении расы (нужны права админа?).
        }
    }

    async function handleDelete(id) {
        if (!window.confirm("Видалити цю расу?")) return; // Удалить эту расу?
        try {
            await deleteRace(id);
            load();
        } catch (e) {
            console.error(e);
            setError("Помилка при видаленні раси (потрібні права адміна?)."); // Ошибка при удалении расы (нужны права админа?).
        }
    }

    function toggleAbilitySelection(id) {
        setSelectedAbilityIds((prev) =>
            prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]
        );
    }

    return (
        <div className="card">
            <h2>Раси</h2> {/* Расы */}

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
                                <th>Здібності</th> {/* Способности */}
                                {isAdmin && <th />}
                            </tr>
                        </thead>
                        <tbody>
                            {races.map((race) => (
                                <tr key={race.id}>
                                    <td>{race.name}</td>
                                    <td style={{ maxWidth: 320 }}>{race.description}</td>
                                    <td style={{ maxWidth: 260 }}>
                                        {race.abilities && race.abilities.length > 0 ? (
                                            race.abilities.map((ab) => (
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
                                                onClick={() => handleDelete(race.id)}
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
                            <h3>Додати нову расу</h3> {/* Добавить новую расу */}
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
                                    Додати расу {/* Добавить расу */}
                                </button>
                            </form>
                        </div>
                    )}
                </>
            )}
        </div>
    );
}