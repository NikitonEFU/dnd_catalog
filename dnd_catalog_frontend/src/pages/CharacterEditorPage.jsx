import { useEffect, useMemo, useState } from "react";
import { getRaces } from "../services/racesService";
import { getClasses } from "../services/classesService";
import { createCharacter } from "../services/charactersService";

const STAT_KEYS = [
    { key: "STR", label: "Сила (STR)" },
    { key: "DEX", label: "Спритність (DEX)" }, // Ловкость (DEX)
    { key: "CON", label: "Витривалість (CON)" }, // Телосложение (CON)
    { key: "INT", label: "Інтелект (INT)" },
    { key: "WIS", label: "Мудрість (WIS)" }, // Мудрость (WIS)
    { key: "CHA", label: "Харизма (CHA)" },
];

export default function CharacterEditorPage() {
    const [name, setName] = useState("");
    const [gender, setGender] = useState("Чоловіча"); // Мужской
    const [raceId, setRaceId] = useState("");
    const [classId, setClassId] = useState("");
    const [stats, setStats] = useState(() =>
        STAT_KEYS.reduce((acc, s) => ({ ...acc, [s.key]: 10 }), {})
    );

    const [races, setRaces] = useState([]);
    const [classes, setClasses] = useState([]);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    // Для модалки здібностей
    const [abilityModalOpen, setAbilityModalOpen] = useState(false);
    const [modalAbility, setModalAbility] = useState(null);

    useEffect(() => {
        async function load() {
            setLoading(true);
            try {
                const [racesData, classesData] = await Promise.all([
                    getRaces(true), // includeAbilities = true
                    getClasses(true),
                ]);
                setRaces(racesData);
                setClasses(classesData);
            } catch (e) {
                console.error(e);
                setError("Помилка завантаження рас та класів."); // Ошибка загрузки рас и классов.
            } finally {
                setLoading(false);
            }
        }
        load();
    }, []);

    // Автоматичні здібності: раса + клас
    const combinedAbilities = useMemo(() => {
        const race = races.find((r) => r.id === raceId);
        const clazz = classes.find((c) => c.id === classId);

        const list = [];

        if (race?.abilities) {
            for (const ab of race.abilities) {
                list.push({ ...ab, source: "Раса" }); // Раса
            }
        }
        if (clazz?.abilities) {
            for (const ab of clazz.abilities) {
                list.push({ ...ab, source: "Клас" }); // Класс
            }
        }

        // усунемо дублі по Id, якщо що
        const map = new Map();
        for (const ab of list) {
            if (!ab.id) continue;
            if (!map.has(ab.id)) {
                map.set(ab.id, ab);
            }
        }
        return Array.from(map.values());
    }, [raceId, classId, races, classes]);

    function handleStatChange(key, value) {
        const num = parseInt(value, 10);
        if (Number.isNaN(num)) {
            setStats((prev) => ({ ...prev, [key]: "" }));
        } else {
            setStats((prev) => ({ ...prev, [key]: num }));
        }
    }

    async function handleSubmit(e) {
        e.preventDefault();
        setError("");
        setSuccess("");

        if (!name.trim() || !raceId || !classId || !gender.trim()) {
            setError("Заповніть ім'я, стать, расу та клас."); // Заполните имя, пол, расу и класс.
            return;
        }

        const payload = {
            name,
            gender,
            raceId,
            classId,
            stats,
        };

        try {
            setSaving(true);
            await createCharacter(payload);
            setSuccess("Персонажа успішно створено!"); // Персонаж успешно создан!
            // можна скинути форму
            setName("");
            setGender("Чоловіча"); // Мужской
            setRaceId("");
            setClassId("");
            setStats(STAT_KEYS.reduce((acc, s) => ({ ...acc, [s.key]: 10 }), {}));
        } catch (e) {
            console.error(e);
            setError("Помилка при створенні персонажа."); // Ошибка при создании персонажа.
        } finally {
            setSaving(false);
        }
    }

    function openAbilityModal(ability) {
        setModalAbility(ability);
        setAbilityModalOpen(true);
    }

    function closeAbilityModal() {
        setAbilityModalOpen(false);
        setModalAbility(null);
    }

    return (
        <div className="card">
            <h2>Створення персонажа</h2> {/* Создание персонажа */}

            {loading && <p>Завантаження даних...</p>} {/* Загрузка данных... */}

            {!loading && (
                <>
                    <form onSubmit={handleSubmit} style={{ marginTop: 10, gap: 14 }}>
                        <div>
                            <label>Ім'я персонажа</label> {/* Имя персонажа */}
                            <input
                                value={name}
                                onChange={(e) => setName(e.target.value)}
                                required
                            />
                        </div>

                        <div>
                            <label>Стать</label> {/* Пол */}
                            <select
                                value={gender}
                                onChange={(e) => setGender(e.target.value)}
                                required
                            >
                                <option value="Чоловіча">Чоловіча</option> {/* Мужской */}
                                <option value="Жіноча">Жіноча</option> {/* Женский */}
                                <option value="Інше">Інше</option> {/* Другое */}
                            </select>
                        </div>

                        <div>
                            <label>Раса</label> {/* Раса */}
                            <select
                                value={raceId}
                                onChange={(e) => setRaceId(e.target.value)}
                                required
                            >
                                <option value="">— виберіть расу —</option> {/* — выберите расу — */}
                                {races.map((race) => (
                                    <option key={race.id} value={race.id}>
                                        {race.name}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div>
                            <label>Клас</label> {/* Класс */}
                            <select
                                value={classId}
                                onChange={(e) => setClassId(e.target.value)}
                                required
                            >
                                <option value="">— виберіть клас —</option> {/* — выберите класс — */}
                                {classes.map((c) => (
                                    <option key={c.id} value={c.id}>
                                        {c.name} ({c.hitDie})
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div>
                            <h3 style={{ marginTop: 10, marginBottom: 6 }}>
                                Характеристики {/* Характеристики */}
                            </h3>
                            <div
                                style={{
                                    display: "grid",
                                    gridTemplateColumns: "repeat(auto-fit, minmax(140px, 1fr))",
                                    gap: 10,
                                }}
                            >
                                {STAT_KEYS.map((s) => (
                                    <div key={s.key}>
                                        <label>{s.label}</label>
                                        <input
                                            type="number"
                                            min={1}
                                            max={30}
                                            value={stats[s.key]}
                                            onChange={(e) => handleStatChange(s.key, e.target.value)}
                                        />
                                    </div>
                                ))}
                            </div>
                        </div>

                        {error && (
                            <div style={{ color: "darkred", marginTop: 4, fontSize: 13 }}>
                                {error}
                            </div>
                        )}
                        {success && (
                            <div style={{ color: "darkgreen", marginTop: 4, fontSize: 13 }}>
                                {success}
                            </div>
                        )}

                        <button className="btn" type="submit" disabled={saving}>
                            {saving ? "Збереження..." : "Створити персонажа"} {/* Сохранение... / Создать персонажа */}
                        </button>
                    </form>

                    <div style={{ marginTop: 20 }}>
                        <h3>Здібності</h3> {/* Способности персонажа */}
                        {combinedAbilities.length === 0 && (
                            <p style={{ fontSize: 14 }}>
                                Виберіть расу та клас, щоб побачити здібності. {/* Выберите расу и класс, чтобы увидеть способности. */}
                            </p>
                        )}
                        {combinedAbilities.length > 0 && (
                            <div className="grid" style={{ marginTop: 8 }}>
                                {combinedAbilities.map((ab) => (
                                    <div
                                        key={ab.id}
                                        className="card"
                                        style={{ padding: 10, marginBottom: 0 }}
                                    >
                                        <div
                                            style={{
                                                display: "flex",
                                                justifyContent: "space-between",
                                                alignItems: "center",
                                                gap: 8,
                                            }}
                                        >
                                            <div>
                                                <strong>{ab.name}</strong>
                                                <div
                                                    className="chip"
                                                    style={{ marginTop: 4, fontSize: 11 }}
                                                >
                                                    Джерело: {ab.source} {/* Источник: */}
                                                </div>
                                            </div>
                                            <button
                                                type="button"
                                                className="btn btn-outline"
                                                onClick={() => openAbilityModal(ab)}
                                            >
                                                Детальніше {/* Подробнее */}
                                            </button>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                </>
            )}

            {abilityModalOpen && modalAbility && (
                <div
                    style={{
                        position: "fixed",
                        inset: 0,
                        background: "rgba(0,0,0,0.5)",
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        zIndex: 999,
                    }}
                    onClick={closeAbilityModal}
                >
                    <div
                        className="card"
                        style={{
                            maxWidth: 480,
                            maxHeight: "80vh",
                            overflowY: "auto",
                            background: "#f5f0e6",
                        }}
                        onClick={(e) => e.stopPropagation()}
                    >
                        <h3>{modalAbility.name}</h3>
                        <p style={{ marginTop: 8, whiteSpace: "pre-wrap" }}>
                            {modalAbility.description}
                        </p>
                        <button
                            type="button"
                            className="btn"
                            style={{ marginTop: 12 }}
                            onClick={closeAbilityModal}
                        >
                            Закрити {/* Закрыть */}
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
}