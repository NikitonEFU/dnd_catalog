import { useEffect, useState } from "react";
import { getMyCharacters, deleteCharacter } from "../services/charactersService";
// Імпортуємо додаткові сервіси
import { getRaces } from "../services/racesService";
import { getClasses } from "../services/classesService";

export default function AccountPage() {
    const [characters, setCharacters] = useState([]);
    const [racesMap, setRacesMap] = useState({});
    const [classesMap, setClassesMap] = useState({});

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    // Функція-хелпер для отримання назви за ID
    const getNameById = (id, map) => {
        return map[id]?.name || id; // Якщо назву не знайдено, показуємо ID
    };

    async function load() {
        setLoading(true);
        setError("");
        try {
            // 1. Завантажуємо дані паралельно
            const [charactersData, racesData, classesData] = await Promise.all([
                getMyCharacters(),
                getRaces(true), // Припускаємо, що true включає здібності
                getClasses(true), // Припускаємо, що true включає здібності
            ]);

            // 2. Створюємо мапи для швидкого пошуку за ID
            const racesMap = racesData.reduce((acc, race) => ({ ...acc, [race.id]: race }), {});
            const classesMap = classesData.reduce((acc, cls) => ({ ...acc, [cls.id]: cls }), {});

            setCharacters(charactersData);
            setRacesMap(racesMap);
            setClassesMap(classesMap);
        } catch (e) {
            console.error(e);
            setError("Помилка завантаження даних персонажів, рас або класів.");
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        load();
    }, []);

    async function handleDelete(id) {
        if (!window.confirm("Видалити цього персонажа?")) return;
        try {
            await deleteCharacter(id);
            load();
        } catch (e) {
            console.error(e);
            setError("Помилка при видаленні персонажа.");
        }
    }

    return (
        <div className="card">
            <h2>Мої персонажі</h2>

            {error && (
                <div style={{ color: "darkred", fontSize: 13, marginBottom: 6 }}>
                    {error}
                </div>
            )}

            {loading && <p>Завантаження...</p>}

            {!loading && characters.length === 0 && (
                <p>У вас поки немає створених персонажів.</p>
            )}

            {!loading && characters.length > 0 && (
                <table className="table">
                    <thead>
                        <tr>
                            <th>Ім'я</th>
                            <th>Стать</th>
                            <th>Раса</th> {/* RaceId -> Раса */}
                            <th>Клас</th> {/* ClassId -> Клас */}
                            <th>Стати</th>
                            <th>Здібності</th> {/* Додано стовпець Здібності */}
                            <th />
                        </tr>
                    </thead>
                    <tbody>
                        {characters.map((ch) => {
                            const raceName = getNameById(ch.raceId, racesMap);
                            const className = getNameById(ch.classId, classesMap);

                            // Збираємо здібності раси та класу
                            const raceAbilities = racesMap[ch.raceId]?.abilities || [];
                            const classAbilities = classesMap[ch.classId]?.abilities || [];

                            // Об'єднуємо здібності, усуваючи дублікати
                            const allAbilities = [
                                ...raceAbilities,
                                ...classAbilities.filter(
                                    (classAb) => !raceAbilities.some(raceAb => raceAb.id === classAb.id)
                                )
                            ];

                            return (
                                <tr key={ch.id}>
                                    <td>{ch.name}</td>
                                    <td>{ch.gender}</td>
                                    <td>{raceName}</td> {/* Виводимо назву раси */}
                                    <td>{className}</td> {/* Виводимо назву класу */}
                                    <td>
                                        {ch.stats
                                            ? Object.entries(ch.stats).map(([k, v]) => (
                                                <span key={k} className="chip">
                                                    {k}: {v}
                                                </span>
                                            ))
                                            : "-"}
                                    </td>
                                    <td style={{ maxWidth: 300 }}>
                                        {allAbilities.length > 0 ? (
                                            allAbilities.map((ab) => (
                                                <span key={ab.id} className="chip">
                                                    {ab.name}
                                                </span>
                                            ))
                                        ) : (
                                            "-"
                                        )}
                                    </td>
                                    <td>
                                        <button
                                            className="btn btn-outline"
                                            onClick={() => handleDelete(ch.id)}
                                        >
                                            Видалити
                                        </button>
                                    </td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
            )}
        </div>
    );
}