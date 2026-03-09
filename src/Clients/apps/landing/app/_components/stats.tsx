"use client";

import { useEffect, useRef, useState } from "react";

interface Stat {
  value: number;
  suffix: string;
  label: string;
  description: string;
  isFloat?: boolean;
}

const STATS: Stat[] = [
  {
    value: 500,
    suffix: "+",
    label: "Школ",
    description: "уже используют платформу",
  },
  {
    value: 50000,
    suffix: "+",
    label: "Студентов",
    description: "обучаются прямо сейчас",
  },
  {
    value: 98,
    suffix: "%",
    label: "Довольны",
    description: "рекомендуют нас коллегам",
  },
  {
    value: 4.9,
    suffix: "★",
    label: "Рейтинг",
    description: "средняя оценка платформы",
    isFloat: true,
  },
];

function formatNumber(n: number, isFloat?: boolean): string {
  if (isFloat) return n.toFixed(1);
  return new Intl.NumberFormat("ru-RU").format(Math.floor(n));
}

function useCountUp(
  target: number,
  duration: number,
  trigger: boolean,
  isFloat?: boolean,
) {
  const [count, setCount] = useState(0);

  useEffect(() => {
    if (!trigger) return;

    let animFrame: number;
    const startTime = performance.now();

    const tick = (now: number) => {
      const elapsed = now - startTime;
      const progress = Math.min(elapsed / duration, 1);
      const eased = progress === 1 ? 1 : 1 - Math.pow(2, -10 * progress);
      const current = eased * target;

      setCount(isFloat ? Math.round(current * 10) / 10 : current);

      if (progress < 1) {
        animFrame = requestAnimationFrame(tick);
      } else {
        setCount(target);
      }
    };

    animFrame = requestAnimationFrame(tick);

    return () => {
      cancelAnimationFrame(animFrame);
    };
  }, [target, duration, trigger, isFloat]);

  return count;
}

function StatCard({ stat, trigger }: { stat: Stat; trigger: boolean }) {
  const count = useCountUp(stat.value, 2000, trigger, stat.isFloat);

  return (
    <div className="flex flex-col items-center p-6 text-center">
      <div
        className="mb-1 text-4xl font-extrabold tracking-tight sm:text-5xl"
        style={{ fontVariantNumeric: "tabular-nums" }}
        aria-live="polite"
      >
        <span className="text-primary">
          {formatNumber(count, stat.isFloat)}
          {stat.suffix}
        </span>
      </div>
      <div className="text-card-foreground mb-1 text-lg font-semibold">
        {stat.label}
      </div>
      <div className="text-muted-foreground text-sm">{stat.description}</div>
    </div>
  );
}

export function Stats() {
  const sectionRef = useRef<HTMLElement>(null);
  const [triggered, setTriggered] = useState(false);

  useEffect(() => {
    const el = sectionRef.current;
    if (!el) return;

    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry?.isIntersecting) {
          setTriggered(true);
          observer.disconnect();
        }
      },
      { threshold: 0.3 },
    );

    observer.observe(el);

    return () => {
      observer.disconnect();
    };
  }, []);

  return (
    <section
      ref={sectionRef}
      aria-label="Ключевые показатели"
      className="bg-background relative py-16"
    >
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        {/* Top divider */}
        <div
          className="mb-12 h-px w-full"
          style={{
            /* Secondary: analogous amber (H≈70°) instead of contrasting blue ring */
            background:
              "linear-gradient(90deg, transparent, color-mix(in oklch, var(--primary) 50%, transparent), color-mix(in oklch, oklch(0.83 0.14 70) 30%, transparent), transparent)",
          }}
          aria-hidden="true"
        />

        <div className="grid grid-cols-2 gap-4 lg:grid-cols-4">
          {STATS.map((stat, i) => (
            <div key={stat.label} className="relative">
              {i < STATS.length - 1 && (
                <div
                  className="bg-border absolute top-1/2 right-0 hidden h-16 w-px -translate-y-1/2 lg:block"
                  aria-hidden="true"
                />
              )}
              <StatCard stat={stat} trigger={triggered} />
            </div>
          ))}
        </div>

        {/* Bottom divider */}
        <div
          className="mt-12 h-px w-full"
          style={{
            background:
              "linear-gradient(90deg, transparent, color-mix(in oklch, var(--border) 150%, transparent), transparent)",
          }}
          aria-hidden="true"
        />
      </div>
    </section>
  );
}
