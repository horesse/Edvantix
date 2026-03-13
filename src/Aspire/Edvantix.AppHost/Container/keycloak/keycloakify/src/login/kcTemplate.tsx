import { useEffect } from "react";
import { kcSanitize } from "keycloakify/lib/kcSanitize";
import type { TemplateProps } from "keycloakify/login/TemplateProps";
import type { KcContext } from "./KcContext";
import type { I18n } from "./i18n";

/** SVG-иконка стопки слоёв (логотип Edvantix) */
function LayersIcon({ size = 24, strokeWidth = 1.8 }: { size?: number; strokeWidth?: number }) {
    return (
        <svg width={size} height={size} fill="none" viewBox="0 0 24 24" aria-hidden="true">
            <path
                d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5"
                stroke="white"
                strokeWidth={strokeWidth}
                strokeLinecap="round"
                strokeLinejoin="round"
            />
        </svg>
    );
}

/** Иконка галочки для списка преимуществ на панели регистрации */
function CheckIcon() {
    return (
        <svg
            width="12"
            height="12"
            fill="none"
            viewBox="0 0 24 24"
            stroke="white"
            strokeWidth={2.5}
            aria-hidden="true"
        >
            <path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" />
        </svg>
    );
}

/** Нижний контент левой панели для страницы входа — статистика */
function LoginPanelContent() {
    return (
        <>
            <h2 className="text-white text-3xl font-bold leading-tight mb-3">
                Управляйте обучением
                <br />
                эффективно
            </h2>
            <p className="text-sm leading-relaxed mb-8" style={{ color: "rgba(255,255,255,0.7)" }}>
                Единая платформа для преподавателей и организаций —
                посещаемость, аналитика, участники.
            </p>
            <div className="grid grid-cols-3 gap-3">
                {[
                    { value: "12k+", label: "Участников" },
                    { value: "98%", label: "Точность" },
                    { value: "350+", label: "Организаций" },
                ].map((stat) => (
                    <div
                        key={stat.label}
                        className="rounded-xl p-3"
                        style={{
                            background: "rgba(255,255,255,0.1)",
                            backdropFilter: "blur(4px)",
                            border: "1px solid rgba(255,255,255,0.15)",
                        }}
                    >
                        <div className="text-white font-bold text-xl">{stat.value}</div>
                        <div className="text-xs mt-0.5" style={{ color: "rgba(255,255,255,0.6)" }}>
                            {stat.label}
                        </div>
                    </div>
                ))}
            </div>
        </>
    );
}

/** Нижний контент левой панели для страницы регистрации — преимущества */
function RegisterPanelContent() {
    const features = [
        "Бесплатный доступ на 30 дней",
        "Настройка за 5 минут",
        "Карта не требуется",
    ];

    return (
        <>
            <h2 className="text-white text-3xl font-bold leading-tight mb-3">
                Начните бесплатно
                <br />
                уже сегодня
            </h2>
            <p className="text-sm leading-relaxed mb-8" style={{ color: "rgba(255,255,255,0.7)" }}>
                Зарегистрируйтесь и получите полный доступ к инструментам
                управления обучением.
            </p>
            <ul className="space-y-3">
                {features.map((text) => (
                    <li key={text} className="flex items-center gap-3">
                        <div
                            className="w-6 h-6 rounded-full flex items-center justify-center shrink-0"
                            style={{ background: "rgba(255,255,255,0.2)" }}
                        >
                            <CheckIcon />
                        </div>
                        <span className="text-sm" style={{ color: "rgba(255,255,255,0.8)" }}>
                            {text}
                        </span>
                    </li>
                ))}
            </ul>
        </>
    );
}

/**
 * Кастомный шаблон страниц аутентификации Edvantix.
 * Разделён на левую брендинговую панель и правую панель с формой.
 * Левая панель адаптируется под тип страницы (login / register).
 */
export default function Template(props: TemplateProps<KcContext, I18n>) {
    const { displayMessage = true, documentTitle, kcContext, i18n, children } = props;

    const { msgStr } = i18n;
    const { message, realm } = kcContext;
    const isRegisterPage = kcContext.pageId === "register.ftl";

    useEffect(() => {
        document.title = documentTitle ?? msgStr("loginTitle", realm.displayName);
    }, [documentTitle, msgStr, realm.displayName]);

    return (
        <div className="flex min-h-screen" style={{ background: "#f8fafc" }}>
            {/* ═══════════ ЛЕВАЯ ПАНЕЛЬ — БРЕНДИНГ ═══════════ */}
            <div
                className="hidden lg:flex lg:w-[480px] xl:w-[540px] relative overflow-hidden flex-col justify-between p-12 shrink-0"
                style={{
                    background: "linear-gradient(135deg, #4338ca 0%, #6366f1 50%, #818cf8 100%)",
                }}
            >
                {/* Фон: сетка точек */}
                <div
                    className="absolute inset-0"
                    style={{
                        backgroundImage:
                            "radial-gradient(circle, rgba(255,255,255,0.15) 1px, transparent 1px)",
                        backgroundSize: "32px 32px",
                        opacity: 0.4,
                    }}
                />

                {/* Декоративные концентрические кольца */}
                <div
                    className="absolute"
                    style={{
                        top: "50%",
                        left: "50%",
                        width: 480,
                        height: 480,
                        marginLeft: -240,
                        marginTop: -240,
                    }}
                >
                    {[200, 300, 400, 480].map((size, i) => (
                        <div
                            key={size}
                            className="absolute rounded-full"
                            style={{
                                width: size,
                                height: size,
                                top: "50%",
                                left: "50%",
                                border: "1px solid rgba(255,255,255,0.12)",
                                animation: "pulse-ring 5s ease-in-out infinite",
                                animationDelay: `${i * 0.8}s`,
                            }}
                        />
                    ))}
                </div>

                {/* Парящая иконка по центру */}
                <div
                    className="absolute"
                    style={{
                        top: "50%",
                        left: "50%",
                        animation: "float 6s ease-in-out infinite",
                    }}
                >
                    <div
                        className="w-24 h-24 rounded-3xl flex items-center justify-center"
                        style={{
                            background: "rgba(255,255,255,0.15)",
                            backdropFilter: "blur(8px)",
                            border: "1px solid rgba(255,255,255,0.25)",
                            boxShadow: "0 25px 50px -12px rgba(0,0,0,0.25)",
                        }}
                    >
                        <LayersIcon size={48} strokeWidth={1.8} />
                    </div>
                </div>

                {/* Логотип вверху */}
                <div className="relative z-10 flex items-center gap-2.5">
                    <div
                        className="w-9 h-9 rounded-xl flex items-center justify-center"
                        style={{
                            background: "rgba(255,255,255,0.2)",
                            border: "1px solid rgba(255,255,255,0.3)",
                        }}
                    >
                        <LayersIcon size={20} strokeWidth={2} />
                    </div>
                    <span className="text-white font-bold text-lg tracking-tight">Edvantix</span>
                </div>

                {/* Нижний блок — разный контент в зависимости от страницы */}
                <div className="relative z-10">
                    {isRegisterPage ? <RegisterPanelContent /> : <LoginPanelContent />}
                </div>
            </div>

            {/* ═══════════ ПРАВАЯ ПАНЕЛЬ — ФОРМА ═══════════ */}
            <div className="flex-1 flex items-center justify-center p-6 sm:p-10">
                <div className="w-full max-w-[400px]">
                    {/* Глобальное системное сообщение Keycloak (сессия истекла и т.п.) */}
                    {displayMessage && message !== undefined && (
                        <div
                            className={[
                                "mb-6 rounded-xl p-4 flex items-start gap-3 border text-sm",
                                message.type === "error"
                                    ? "bg-red-50 border-red-200 text-red-700"
                                    : message.type === "success"
                                      ? "bg-green-50 border-green-200 text-green-700"
                                      : message.type === "warning"
                                        ? "bg-amber-50 border-amber-200 text-amber-700"
                                        : "bg-blue-50 border-blue-200 text-blue-700",
                            ].join(" ")}
                            role="alert"
                            aria-live="polite"
                        >
                            <span
                                dangerouslySetInnerHTML={{
                                    __html: kcSanitize(message.summary),
                                }}
                            />
                        </div>
                    )}

                    {children}
                </div>
            </div>
        </div>
    );
}
