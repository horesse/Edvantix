"use client";

import { useState } from "react";

import Link from "next/link";
import { Check, Sparkles, Zap } from "lucide-react";

import { Button } from "@workspace/ui/components/button";

interface PricingPlan {
  name: string;
  tagline: string;
  monthlyPrice: number | null;
  annualPrice: number | null;
  priceLabel?: string;
  featured: boolean;
  features: string[];
  cta: string;
  ctaHref: string;
  badge?: string;
}

const PLANS: PricingPlan[] = [
  {
    name: "Старт",
    tagline: "Идеально для старта",
    monthlyPrice: 0,
    annualPrice: 0,
    featured: false,
    cta: "Начать бесплатно",
    ctaHref: "/signup",
    features: [
      "До 100 студентов",
      "До 5 курсов",
      "Базовая аналитика",
      "Email-уведомления",
      "Приём платежей",
      "Сертификаты",
    ],
  },
  {
    name: "Профи",
    tagline: "Для растущих школ",
    monthlyPrice: 2990,
    annualPrice: 2390,
    featured: true,
    cta: "Попробовать 14 дней",
    ctaHref: "/signup?plan=pro",
    badge: "Популярный",
    features: [
      "До 1 000 студентов",
      "Неограниченно курсов",
      "Продвинутая аналитика",
      "Автоматические воронки",
      "Брендированный домен",
      "Расписание и вебинары",
      "Приоритетная поддержка",
      "API-доступ",
    ],
  },
  {
    name: "Корпорация",
    tagline: "Для крупных академий",
    monthlyPrice: null,
    annualPrice: null,
    priceLabel: "По запросу",
    featured: false,
    cta: "Связаться с нами",
    ctaHref: "/contact",
    features: [
      "Безлимитные студенты",
      "Белый лейбл",
      "Выделенный менеджер",
      "SLA 99.9%",
      "On-premise вариант",
      "Интеграции на заказ",
      "Обучение команды",
      "24/7 поддержка",
    ],
  },
];

function formatPrice(price: number): string {
  return new Intl.NumberFormat("ru-RU").format(price);
}

export function Pricing() {
  const [annual, setAnnual] = useState(false);

  return (
    <section
      id="pricing"
      aria-label="Тарифные планы"
      className="relative py-24 sm:py-32 bg-background"
    >
      <div
        className="absolute inset-0 pointer-events-none"
        aria-hidden="true"
        style={{
          background:
            "radial-gradient(ellipse 70% 50% at 50% 0%, color-mix(in oklch, var(--primary) 6%, transparent) 0%, transparent 60%)",
        }}
      />
      <div className="absolute inset-0 dot-pattern opacity-30" aria-hidden="true" />

      <div className="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="text-center mb-12">
          <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full border border-primary/20 bg-primary/5 text-primary text-xs font-medium mb-4">
            <Sparkles className="w-3 h-3" aria-hidden="true" />
            Тарифы
          </div>
          <h2 className="text-3xl sm:text-4xl lg:text-5xl font-bold text-card-foreground mb-4 tracking-tight">
            Прозрачные цены
            <span className="block text-primary">без скрытых платежей</span>
          </h2>
          <p className="text-muted-foreground text-lg max-w-xl mx-auto mb-8">
            Начните бесплатно и масштабируйтесь вместе с ростом вашей школы.
          </p>

          {/* Billing toggle */}
          <div
            className="inline-flex items-center gap-3 p-1 rounded-full bg-muted/40 border border-border"
            role="group"
            aria-label="Период оплаты"
          >
            <button
              type="button"
              onClick={() => setAnnual(false)}
              className={`px-5 py-2 rounded-full text-sm font-medium transition-all duration-200 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring ${
                !annual
                  ? "bg-card text-card-foreground shadow-sm"
                  : "text-muted-foreground hover:text-foreground"
              }`}
              aria-pressed={!annual}
            >
              Ежемесячно
            </button>
            <button
              type="button"
              onClick={() => setAnnual(true)}
              className={`flex items-center gap-2 px-5 py-2 rounded-full text-sm font-medium transition-all duration-200 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring ${
                annual
                  ? "bg-card text-card-foreground shadow-sm"
                  : "text-muted-foreground hover:text-foreground"
              }`}
              aria-pressed={annual}
            >
              Ежегодно
              <span className="px-1.5 py-0.5 rounded text-[10px] bg-emerald-500/15 text-emerald-500 font-semibold">
                −20%
              </span>
            </button>
          </div>
        </div>

        {/* Plans grid */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 items-start">
          {PLANS.map((plan) => {
            const price = annual ? plan.annualPrice : plan.monthlyPrice;

            return (
              <div
                key={plan.name}
                className={`relative rounded-2xl border p-7 transition-all duration-300 ${
                  plan.featured
                    ? "border-primary/30 bg-card shadow-2xl shadow-primary/10"
                    : "border-border bg-card/50 hover:bg-card"
                }`}
              >
                {/* Popular badge */}
                {plan.badge && (
                  <div className="absolute -top-3.5 left-1/2 -translate-x-1/2 inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-semibold bg-primary text-primary-foreground">
                    <Zap className="w-3 h-3" aria-hidden="true" />
                    {plan.badge}
                  </div>
                )}

                {/* Plan name */}
                <div className="mb-6">
                  <div className="inline-flex p-2 rounded-xl bg-primary/10 mb-3" aria-hidden="true">
                    <Sparkles className="w-4 h-4 text-primary" />
                  </div>
                  <h3 className="text-card-foreground font-bold text-xl mb-1">{plan.name}</h3>
                  <p className="text-muted-foreground text-sm">{plan.tagline}</p>
                </div>

                {/* Price */}
                <div className="mb-7 pb-7 border-b border-border">
                  {plan.priceLabel ? (
                    <div className="text-3xl font-extrabold text-card-foreground">
                      {plan.priceLabel}
                    </div>
                  ) : (
                    <div className="flex items-end gap-1">
                      <span className="text-4xl font-extrabold text-card-foreground">
                        {price === 0 ? "Бесплатно" : `₽${formatPrice(price!)}`}
                      </span>
                      {price !== 0 && (
                        <span className="text-muted-foreground text-sm mb-1.5">/мес</span>
                      )}
                    </div>
                  )}
                  {annual && plan.monthlyPrice !== 0 && !plan.priceLabel && (
                    <p className="text-muted-foreground text-xs mt-1">
                      Вместо ₽{formatPrice(plan.monthlyPrice!)}/мес
                    </p>
                  )}
                </div>

                {/* CTA */}
                <Button
                  asChild
                  className={`w-full mb-7 transition-all duration-200 focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 focus-visible:ring-offset-background ${
                    plan.featured
                      ? "bg-primary hover:bg-primary/90 text-primary-foreground shadow-lg shadow-primary/20"
                      : "bg-muted hover:bg-muted/80 text-foreground border border-border"
                  }`}
                >
                  <Link href={plan.ctaHref}>{plan.cta}</Link>
                </Button>

                {/* Features */}
                <ul className="flex flex-col gap-3" role="list">
                  {plan.features.map((feature) => (
                    <li key={feature} className="flex items-center gap-2.5 text-sm">
                      <Check
                        className={`w-4 h-4 shrink-0 ${plan.featured ? "text-primary" : "text-muted-foreground"}`}
                        aria-hidden="true"
                      />
                      <span className="text-foreground">{feature}</span>
                    </li>
                  ))}
                </ul>
              </div>
            );
          })}
        </div>

        <p className="mt-10 text-center text-muted-foreground text-sm">
          Все тарифы включают SSL, резервное копирование и базовую техническую
          поддержку. Цены указаны без НДС.
        </p>
      </div>
    </section>
  );
}
