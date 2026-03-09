import { Settings2, BookPlus, TrendingUp, ArrowRight } from "lucide-react";

const STEPS = [
  {
    number: "01",
    icon: Settings2,
    title: "Создайте школу",
    description:
      "Зарегистрируйтесь и настройте профиль вашей школы за 5 минут: название, брендинг, домен. Никаких технических знаний не требуется.",
    details: [
      "Личный кабинет с брендингом",
      "Кастомный поддомен",
      "Настройка оплаты",
    ],
  },
  {
    number: "02",
    icon: BookPlus,
    title: "Добавьте курсы",
    description:
      "Создавайте курсы с видео, текстом и тестами. Загружайте материалы, настраивайте расписание и устанавливайте цены. Всё интуитивно просто.",
    details: [
      "Конструктор курсов",
      "Загрузка видео и файлов",
      "Автоматические тесты",
    ],
  },
  {
    number: "03",
    icon: TrendingUp,
    title: "Зарабатывайте",
    description:
      "Запустите продажи, подключите автоматические рассылки и отслеживайте рост школы через встроенную аналитику в реальном времени.",
    details: [
      "Страница продаж",
      "Автоматические воронки",
      "Аналитика и отчёты",
    ],
  },
] as const;

export function HowItWorks() {
  return (
    <section
      id="how-it-works"
      aria-label="Как это работает"
      className="relative py-24 sm:py-32 bg-background overflow-hidden"
    >
      {/* Background decoration */}
      <div
        className="absolute inset-0 pointer-events-none"
        aria-hidden="true"
        style={{
          background:
            "radial-gradient(ellipse 70% 50% at 50% 100%, color-mix(in oklch, var(--primary) 7%, transparent) 0%, transparent 60%)",
        }}
      />
      <div className="absolute inset-0 grid-bg opacity-30" aria-hidden="true" />

      <div className="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Section header */}
        <div className="text-center mb-20">
          <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full border border-primary/20 bg-primary/5 text-primary text-xs font-medium mb-4">
            <ArrowRight className="w-3 h-3" aria-hidden="true" />
            Как это работает
          </div>
          <h2 className="text-3xl sm:text-4xl lg:text-5xl font-bold text-card-foreground mb-4 tracking-tight">
            Запустите школу
            <span className="block text-primary">за 3 простых шага</span>
          </h2>
          <p className="text-muted-foreground text-lg max-w-xl mx-auto">
            От регистрации до первых продаж — всего несколько часов. Без
            разработчиков и лишних затрат.
          </p>
        </div>

        {/* Steps */}
        <div className="relative grid grid-cols-1 lg:grid-cols-3 gap-8 lg:gap-6">
          {/* Desktop connector line */}
          <div
            className="hidden lg:block absolute top-16 left-[calc(33%+2rem)] right-[calc(33%+2rem)] h-px"
            style={{
              background:
                "linear-gradient(90deg, color-mix(in oklch, var(--primary) 50%, transparent), color-mix(in oklch, var(--primary) 30%, transparent))",
            }}
            aria-hidden="true"
          />

          {STEPS.map((step, index) => {
            const Icon = step.icon;

            return (
              <div
                key={step.number}
                className="relative flex flex-col items-center text-center lg:items-start lg:text-left"
              >
                {/* Step number + icon */}
                <div className="relative mb-6 flex flex-col items-center lg:items-start">
                  {/* Large decorative number */}
                  <div
                    className="text-6xl font-black text-primary/10 absolute -top-4 -left-2 select-none leading-none"
                    aria-hidden="true"
                  >
                    {step.number}
                  </div>

                  {/* Icon circle */}
                  <div className="relative z-10 w-14 h-14 rounded-2xl bg-primary/10 border border-primary/20 flex items-center justify-center shadow-lg mb-4">
                    <Icon className="w-6 h-6 text-primary" aria-hidden="true" />
                  </div>

                  {/* Mobile arrow */}
                  {index < STEPS.length - 1 && (
                    <div className="lg:hidden absolute -bottom-12 left-1/2 -translate-x-1/2 text-muted-foreground">
                      <ArrowRight className="w-4 h-4 rotate-90" aria-hidden="true" />
                    </div>
                  )}
                </div>

                <h3 className="text-card-foreground text-xl font-semibold mb-3">
                  {step.title}
                </h3>
                <p className="text-muted-foreground text-sm leading-relaxed mb-5">
                  {step.description}
                </p>

                <ul className="flex flex-col gap-2 w-full" role="list">
                  {step.details.map((detail) => (
                    <li key={detail} className="flex items-center gap-2 text-sm text-muted-foreground">
                      <div
                        className="w-1.5 h-1.5 rounded-full shrink-0 bg-primary"
                        aria-hidden="true"
                      />
                      {detail}
                    </li>
                  ))}
                </ul>
              </div>
            );
          })}
        </div>

        {/* Bottom note */}
        <div className="mt-20 text-center">
          <p className="text-muted-foreground text-sm">
            Среднее время до первого курса —{" "}
            <span className="text-card-foreground font-medium">47 минут</span>
          </p>
        </div>
      </div>
    </section>
  );
}
