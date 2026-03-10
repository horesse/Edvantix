import { ArrowRight, BookPlus, Settings2, TrendingUp } from "lucide-react";

import { SECTION_H2_CLASS, SectionBadge } from "./section-badge";

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
      className="bg-background relative overflow-hidden py-24 sm:py-32"
    >
      {/* Background decoration */}
      <div
        className="pointer-events-none absolute inset-0"
        aria-hidden="true"
        style={{
          background:
            "radial-gradient(ellipse 70% 50% at 50% 100%, color-mix(in oklch, var(--primary) 7%, transparent) 0%, transparent 60%)",
        }}
      />
      <div className="grid-bg absolute inset-0 opacity-30" aria-hidden="true" />

      <div className="relative mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        {/* Section header */}
        <div className="mb-20 text-center">
          <SectionBadge icon={ArrowRight}>Как это работает</SectionBadge>
          <h2 className={SECTION_H2_CLASS}>
            Запустите школу
            <span className="text-primary block">за 3 простых шага</span>
          </h2>
          <p className="text-muted-foreground mx-auto max-w-xl text-lg">
            От регистрации до первых продаж — всего несколько часов. Без
            разработчиков и лишних затрат.
          </p>
        </div>

        {/* Steps */}
        <div className="relative grid grid-cols-1 gap-8 lg:grid-cols-3 lg:gap-6">
          {/* Desktop connector line */}
          <div
            className="absolute top-16 right-[calc(33%+2rem)] left-[calc(33%+2rem)] hidden h-px lg:block"
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
                    className="text-primary/10 absolute -top-4 -left-2 text-6xl leading-none font-black select-none"
                    aria-hidden="true"
                  >
                    {step.number}
                  </div>

                  {/* Icon circle */}
                  <div className="bg-primary/10 border-primary/20 relative z-10 mb-4 flex h-14 w-14 items-center justify-center rounded-2xl border shadow-lg">
                    <Icon className="text-primary h-6 w-6" aria-hidden="true" />
                  </div>

                  {/* Mobile arrow */}
                  {index < STEPS.length - 1 && (
                    <div className="text-muted-foreground absolute -bottom-12 left-1/2 -translate-x-1/2 lg:hidden">
                      <ArrowRight
                        className="h-4 w-4 rotate-90"
                        aria-hidden="true"
                      />
                    </div>
                  )}
                </div>

                <h3 className="text-card-foreground mb-3 text-xl font-semibold">
                  {step.title}
                </h3>
                <p className="text-muted-foreground mb-5 text-sm leading-relaxed">
                  {step.description}
                </p>

                <ul className="flex w-full flex-col gap-2" role="list">
                  {step.details.map((detail) => (
                    <li
                      key={detail}
                      className="text-muted-foreground flex items-center gap-2 text-sm"
                    >
                      <div
                        className="bg-primary h-1.5 w-1.5 shrink-0 rounded-full"
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
