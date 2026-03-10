"use client";

import { useState } from "react";

import Link from "next/link";

import { Check, Sparkles, Zap } from "lucide-react";

import { Button } from "@workspace/ui/components/button";

import { SECTION_H2_CLASS, SectionBadge } from "./section-badge";

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
      className="bg-background relative py-24 sm:py-32"
    >
      <div
        className="pointer-events-none absolute inset-0"
        aria-hidden="true"
        style={{
          background:
            "radial-gradient(ellipse 70% 50% at 50% 0%, color-mix(in oklch, var(--primary) 6%, transparent) 0%, transparent 60%)",
        }}
      />
      <div
        className="dot-pattern absolute inset-0 opacity-30"
        aria-hidden="true"
      />

      <div className="relative mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-12 text-center">
          <SectionBadge icon={Sparkles}>Тарифы</SectionBadge>
          <h2 className={SECTION_H2_CLASS}>
            Прозрачные цены
            <span className="text-primary block">без скрытых платежей</span>
          </h2>
          <p className="text-muted-foreground mx-auto mb-8 max-w-xl text-lg">
            Начните бесплатно и масштабируйтесь вместе с ростом вашей школы.
          </p>

          {/* Billing toggle */}
          <div
            className="bg-muted/40 border-border inline-flex items-center gap-3 rounded-full border p-1"
            role="group"
            aria-label="Период оплаты"
          >
            <button
              type="button"
              onClick={() => setAnnual(false)}
              className={`focus-visible:ring-ring rounded-full px-5 py-2 text-sm font-medium transition-all duration-200 focus-visible:ring-2 focus-visible:outline-none ${
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
              className={`focus-visible:ring-ring flex items-center gap-2 rounded-full px-5 py-2 text-sm font-medium transition-all duration-200 focus-visible:ring-2 focus-visible:outline-none ${
                annual
                  ? "bg-card text-card-foreground shadow-sm"
                  : "text-muted-foreground hover:text-foreground"
              }`}
              aria-pressed={annual}
            >
              Ежегодно
              <span className="rounded bg-emerald-500/15 px-1.5 py-0.5 text-[10px] font-semibold text-emerald-500">
                −20%
              </span>
            </button>
          </div>
        </div>

        {/* Plans grid */}
        <div className="grid grid-cols-1 items-start gap-6 md:grid-cols-3">
          {PLANS.map((plan) => {
            const price = annual ? plan.annualPrice : plan.monthlyPrice;

            return (
              <div
                key={plan.name}
                className={`relative rounded-2xl border p-7 transition-all duration-300 ${
                  plan.featured
                    ? "border-primary/30 bg-card shadow-primary/10 shadow-2xl"
                    : "border-border bg-card/50 hover:bg-card"
                }`}
              >
                {/* Popular badge */}
                {plan.badge && (
                  <div className="bg-primary text-primary-foreground absolute -top-3.5 left-1/2 inline-flex -translate-x-1/2 items-center gap-1.5 rounded-full px-3 py-1 text-xs font-semibold">
                    <Zap className="h-3 w-3" aria-hidden="true" />
                    {plan.badge}
                  </div>
                )}

                {/* Plan name */}
                <div className="mb-6">
                  <div
                    className="bg-primary/10 mb-3 inline-flex rounded-xl p-2"
                    aria-hidden="true"
                  >
                    <Sparkles className="text-primary h-4 w-4" />
                  </div>
                  <h3 className="text-card-foreground mb-1 text-xl font-bold">
                    {plan.name}
                  </h3>
                  <p className="text-muted-foreground text-sm">
                    {plan.tagline}
                  </p>
                </div>

                {/* Price */}
                <div className="border-border mb-7 border-b pb-7">
                  {plan.priceLabel ? (
                    <div className="text-card-foreground text-3xl font-extrabold">
                      {plan.priceLabel}
                    </div>
                  ) : (
                    <div className="flex items-end gap-1">
                      <span className="text-card-foreground text-4xl font-extrabold">
                        {price === 0 ? "Бесплатно" : `₽${formatPrice(price!)}`}
                      </span>
                      {price !== 0 && (
                        <span className="text-muted-foreground mb-1.5 text-sm">
                          /мес
                        </span>
                      )}
                    </div>
                  )}
                  {annual && plan.monthlyPrice !== 0 && !plan.priceLabel && (
                    <p className="text-muted-foreground mt-1 text-xs">
                      Вместо ₽{formatPrice(plan.monthlyPrice!)}/мес
                    </p>
                  )}
                </div>

                {/* CTA */}
                <Button
                  asChild
                  className={`focus-visible:ring-ring focus-visible:ring-offset-background mb-7 w-full transition-all duration-200 focus-visible:ring-2 focus-visible:ring-offset-2 ${
                    plan.featured
                      ? "bg-primary hover:bg-primary/90 text-primary-foreground shadow-primary/20 shadow-lg"
                      : "bg-muted hover:bg-muted/80 text-foreground border-border border"
                  }`}
                >
                  <Link href={plan.ctaHref}>{plan.cta}</Link>
                </Button>

                {/* Features */}
                <ul className="flex flex-col gap-3" role="list">
                  {plan.features.map((feature) => (
                    <li
                      key={feature}
                      className="flex items-center gap-2.5 text-sm"
                    >
                      <Check
                        className={`h-4 w-4 shrink-0 ${plan.featured ? "text-primary" : "text-muted-foreground"}`}
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

        <p className="text-muted-foreground mt-10 text-center text-sm">
          Все тарифы включают SSL, резервное копирование и базовую техническую
          поддержку. Цены указаны без НДС.
        </p>
      </div>
    </section>
  );
}
