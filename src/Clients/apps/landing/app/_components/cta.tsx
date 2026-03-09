import Link from "next/link";

import { ArrowRight, Rocket } from "lucide-react";

import { Button } from "@workspace/ui/components/button";

export function Cta() {
  return (
    <section
      aria-label="Призыв к действию"
      className="bg-background relative overflow-hidden py-24 sm:py-32"
    >
      {/* Animated aurora — primary + secondary */}
      <div
        className="animate-aurora pointer-events-none absolute inset-0"
        aria-hidden="true"
        style={{
          /* Secondary: analogous amber (H≈70°) keeps the palette warm */
          background:
            "linear-gradient(135deg, color-mix(in oklch, var(--primary) 12%, transparent) 0%, color-mix(in oklch, oklch(0.83 0.14 70) 7%, transparent) 50%, color-mix(in oklch, var(--primary) 9%, transparent) 100%)",
          backgroundSize: "200% 200%",
        }}
      />

      {/* Grid overlay */}
      <div className="grid-bg absolute inset-0 opacity-40" aria-hidden="true" />

      {/* Glow orbs */}
      <div
        className="pointer-events-none absolute -top-24 left-1/2 h-96 w-96 -translate-x-1/2 rounded-full blur-3xl"
        style={{
          background: "color-mix(in oklch, var(--primary) 14%, transparent)",
        }}
        aria-hidden="true"
      />
      <div
        className="pointer-events-none absolute -bottom-24 left-1/4 h-64 w-64 rounded-full blur-3xl"
        style={{
          background:
            "color-mix(in oklch, oklch(0.83 0.14 70) 10%, transparent)",
        }}
        aria-hidden="true"
      />

      <div className="relative mx-auto max-w-4xl px-4 text-center sm:px-6 lg:px-8">
        {/* Icon */}
        <div
          className="bg-primary shadow-primary/30 animate-float mb-8 inline-flex h-16 w-16 items-center justify-center rounded-2xl shadow-2xl"
          style={{ animationDuration: "5s" }}
          aria-hidden="true"
        >
          <Rocket className="text-primary-foreground h-7 w-7" />
        </div>

        <h2 className="text-card-foreground mb-6 text-3xl leading-tight font-extrabold tracking-tight sm:text-4xl lg:text-5xl xl:text-6xl">
          Готовы трансформировать
          <span className="text-shimmer block">вашу онлайн-школу?</span>
        </h2>

        <p className="text-muted-foreground mx-auto mb-10 max-w-2xl text-lg leading-relaxed">
          Присоединяйтесь к&nbsp;500+ школам, которые уже управляют бизнесом
          эффективнее. Первые&nbsp;14&nbsp;дней — бесплатно, без кредитной
          карты.
        </p>

        {/* CTA buttons */}
        <div className="mb-10 flex flex-col items-center justify-center gap-4 sm:flex-row">
          <Button
            asChild
            className="bg-primary hover:bg-primary/90 text-primary-foreground shadow-primary/25 hover:shadow-primary/40 focus-visible:ring-ring focus-visible:ring-offset-background h-12 px-8 text-base shadow-xl transition-all duration-300 hover:scale-[1.02] focus-visible:ring-2 focus-visible:ring-offset-2"
          >
            <Link href="/signup">
              Начать бесплатно сегодня
              <ArrowRight className="ml-1 h-4 w-4" aria-hidden="true" />
            </Link>
          </Button>
          <Button
            asChild
            variant="outline"
            className="border-border bg-muted/30 hover:bg-muted/60 text-foreground hover:border-border focus-visible:ring-ring h-12 px-8 text-base transition-all duration-300 focus-visible:ring-2"
          >
            <Link href="/demo">Запросить демонстрацию</Link>
          </Button>
        </div>

        {/* Trust badges */}
        <div className="text-muted-foreground flex flex-wrap justify-center gap-6 text-sm">
          {[
            "14 дней бесплатного периода",
            "Без кредитной карты",
            "Отмена в любое время",
            "Данные принадлежат вам",
          ].map((item) => (
            <span key={item} className="flex items-center gap-1.5">
              <span
                className="bg-primary/60 h-1.5 w-1.5 rounded-full"
                aria-hidden="true"
              />
              {item}
            </span>
          ))}
        </div>
      </div>
    </section>
  );
}
