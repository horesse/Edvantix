"use client";

import { useRef } from "react";

import { Quote, Star } from "lucide-react";

import { SECTION_H2_CLASS } from "./section-badge";

interface Testimonial {
  text: string;
  author: string;
  role: string;
  school: string;
  avatarColor: string;
}

/** All testimonials have 5-star rating — no need to store it per item. */
const TESTIMONIALS: Testimonial[] = [
  {
    text: "Edvantix полностью изменил работу нашей школы. Раньше мы вели студентов в Excel, теперь всё автоматически. Сэкономили 20 часов в неделю.",
    author: "Алёна Морозова",
    role: "Основатель",
    school: "Digital Skills Academy",
    avatarColor: "from-pink-500 to-rose-400",
  },
  {
    text: "Лучшая платформа для онлайн-школы на рынке. Удобный интерфейс, быстрая поддержка и всё что нужно из коробки. Рекомендую всем коллегам.",
    author: "Сергей Кузнецов",
    role: "CEO",
    school: "ProSchool",
    avatarColor: "from-blue-500 to-cyan-400",
  },
  {
    text: "Запустила свою школу за день. Всё интуитивно: курсы, оплата, уведомления. Студенты в восторге от качества платформы.",
    author: "Мария Соколова",
    role: "Директор",
    school: "BeautyPro Edu",
    avatarColor: "from-orange-500 to-amber-400",
  },
  {
    text: "Аналитика в Edvantix помогла нам понять, где теряем студентов, и поднять конверсию на 35%. Данные, которые реально работают.",
    author: "Дмитрий Волков",
    role: "Продюсер",
    school: "Volkov Academy",
    avatarColor: "from-emerald-500 to-green-400",
  },
  {
    text: "Перешли с другой платформы и не пожалели. Переезд занял 2 дня, а результаты видны уже с первого месяца. Выручка выросла на 40%.",
    author: "Наталья Иванова",
    role: "Владелец",
    school: "EduMaster",
    avatarColor: "from-violet-500 to-purple-400",
  },
  {
    text: "Модуль расписания и напоминаний — просто бомба. Студенты перестали пропускать вебинары, посещаемость выросла с 60% до 89%.",
    author: "Антон Петров",
    role: "Методист",
    school: "TechLearn",
    avatarColor: "from-teal-500 to-cyan-400",
  },
  {
    text: "Финансовые отчёты экономят бухгалтеру несколько часов каждый месяц. Всё выгружается в нужном формате автоматически.",
    author: "Елена Захарова",
    role: "CFO",
    school: "SmartEd Group",
    avatarColor: "from-indigo-500 to-blue-400",
  },
  {
    text: "Поддержка отвечает в течение 10 минут. За год работы ни одного критического сбоя. Стабильная платформа — это бесценно.",
    author: "Виктор Смирнов",
    role: "Технический директор",
    school: "CodeAcademy RU",
    avatarColor: "from-rose-500 to-pink-400",
  },
];

const FIRST_ROW = [...TESTIMONIALS.slice(0, 4), ...TESTIMONIALS.slice(0, 4)];
const SECOND_ROW = [...TESTIMONIALS.slice(4), ...TESTIMONIALS.slice(4)];

const STAR_ROW = Array.from({ length: 5 });

function initials(author: string) {
  return author
    .split(" ")
    .map((w) => w[0] ?? "")
    .join("")
    .slice(0, 2);
}

function TestimonialCard({ item }: { item: Testimonial }) {
  return (
    <article className="border-border bg-card/60 mx-2 w-80 flex-shrink-0 rounded-2xl border p-6">
      {/* 5-star rating */}
      <div className="mb-4 flex gap-0.5" aria-label="Оценка: 5 из 5">
        {STAR_ROW.map((_, i) => (
          <Star
            key={i}
            className="h-3.5 w-3.5 fill-yellow-400 text-yellow-400"
            aria-hidden="true"
          />
        ))}
      </div>

      <Quote className="text-primary/40 mb-3 h-5 w-5" aria-hidden="true" />

      <p className="text-foreground mb-5 text-sm leading-relaxed">
        &ldquo;{item.text}&rdquo;
      </p>

      <div className="flex items-center gap-3">
        <div
          className={`h-9 w-9 rounded-full bg-gradient-to-br ${item.avatarColor} flex shrink-0 items-center justify-center`}
          aria-hidden="true"
        >
          <span className="text-xs font-bold text-white">
            {initials(item.author)}
          </span>
        </div>
        <div className="min-w-0">
          <div className="text-card-foreground truncate text-sm font-semibold">
            {item.author}
          </div>
          <div className="text-muted-foreground truncate text-xs">
            {item.role}, {item.school}
          </div>
        </div>
      </div>
    </article>
  );
}

function MarqueeRow({
  items,
  reverse = false,
}: {
  items: Testimonial[];
  reverse?: boolean;
}) {
  const trackRef = useRef<HTMLDivElement>(null);

  return (
    <div
      className="flex overflow-hidden"
      onMouseEnter={() => {
        if (trackRef.current)
          trackRef.current.style.animationPlayState = "paused";
      }}
      onMouseLeave={() => {
        if (trackRef.current)
          trackRef.current.style.animationPlayState = "running";
      }}
    >
      <div
        ref={trackRef}
        className={`flex ${reverse ? "animate-marquee-reverse" : "animate-marquee"}`}
        aria-hidden="true"
      >
        {items.map((item, i) => (
          <TestimonialCard key={`${item.author}-${i}`} item={item} />
        ))}
      </div>
    </div>
  );
}

export function Testimonials() {
  return (
    <section
      id="testimonials"
      aria-label="Отзывы клиентов"
      className="bg-background relative overflow-hidden py-24 sm:py-32"
    >
      <div
        className="pointer-events-none absolute inset-0"
        aria-hidden="true"
        style={{
          background:
            "radial-gradient(ellipse 80% 50% at 50% 50%, color-mix(in oklch, var(--primary) 4%, transparent) 0%, transparent 70%)",
        }}
      />

      <div className="mx-auto mb-16 max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="text-center">
          <div className="mb-4 inline-flex items-center gap-2 rounded-full border border-yellow-500/20 bg-yellow-500/5 px-3 py-1 text-xs font-medium text-yellow-500">
            <Star
              className="h-3 w-3 fill-yellow-500 text-yellow-500"
              aria-hidden="true"
            />
            Отзывы клиентов
          </div>
          <h2 className={SECTION_H2_CLASS}>
            Их школы уже
            <span className="text-primary block">работают по-новому</span>
          </h2>
          <p className="text-muted-foreground mx-auto max-w-xl text-lg">
            Более&nbsp;500&nbsp;школ выбрали Edvantix. Вот что они говорят.
          </p>
        </div>
      </div>

      {/* Marquee rows */}
      <div className="relative space-y-4">
        {/* Fade masks */}
        <div
          className="from-background pointer-events-none absolute inset-y-0 left-0 z-10 w-32 bg-gradient-to-r to-transparent"
          aria-hidden="true"
        />
        <div
          className="from-background pointer-events-none absolute inset-y-0 right-0 z-10 w-32 bg-gradient-to-l to-transparent"
          aria-hidden="true"
        />

        <MarqueeRow items={FIRST_ROW} />
        <MarqueeRow items={SECOND_ROW} reverse />
      </div>

      {/* Screen-reader list */}
      <div className="sr-only">
        <h3>Все отзывы</h3>
        <ul>
          {TESTIMONIALS.map((t) => (
            <li key={t.author}>
              <blockquote>{t.text}</blockquote>
              <cite>
                {t.author}, {t.role} — {t.school}
              </cite>
            </li>
          ))}
        </ul>
      </div>
    </section>
  );
}
