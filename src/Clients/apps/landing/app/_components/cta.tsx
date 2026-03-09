import Link from "next/link";
import { ArrowRight, Rocket } from "lucide-react";

import { Button } from "@workspace/ui/components/button";

export function Cta() {
  return (
    <section
      aria-label="Призыв к действию"
      className="relative py-24 sm:py-32 bg-background overflow-hidden"
    >
      {/* Animated aurora — primary + secondary */}
      <div
        className="absolute inset-0 animate-aurora pointer-events-none"
        aria-hidden="true"
        style={{
          /* Secondary: analogous amber (H≈70°) keeps the palette warm */
          background:
            "linear-gradient(135deg, color-mix(in oklch, var(--primary) 12%, transparent) 0%, color-mix(in oklch, oklch(0.83 0.14 70) 7%, transparent) 50%, color-mix(in oklch, var(--primary) 9%, transparent) 100%)",
          backgroundSize: "200% 200%",
        }}
      />

      {/* Grid overlay */}
      <div className="absolute inset-0 grid-bg opacity-40" aria-hidden="true" />

      {/* Glow orbs */}
      <div
        className="absolute -top-24 left-1/2 -translate-x-1/2 w-96 h-96 rounded-full blur-3xl pointer-events-none"
        style={{ background: "color-mix(in oklch, var(--primary) 14%, transparent)" }}
        aria-hidden="true"
      />
      <div
        className="absolute -bottom-24 left-1/4 w-64 h-64 rounded-full blur-3xl pointer-events-none"
        style={{ background: "color-mix(in oklch, oklch(0.83 0.14 70) 10%, transparent)" }}
        aria-hidden="true"
      />

      <div className="relative max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
        {/* Icon */}
        <div
          className="inline-flex w-16 h-16 rounded-2xl bg-primary items-center justify-center shadow-2xl shadow-primary/30 mb-8 animate-float"
          style={{ animationDuration: "5s" }}
          aria-hidden="true"
        >
          <Rocket className="w-7 h-7 text-primary-foreground" />
        </div>

        <h2 className="text-3xl sm:text-4xl lg:text-5xl xl:text-6xl font-extrabold text-card-foreground mb-6 tracking-tight leading-tight">
          Готовы трансформировать
          <span className="block text-shimmer">вашу онлайн-школу?</span>
        </h2>

        <p className="text-lg text-muted-foreground mb-10 max-w-2xl mx-auto leading-relaxed">
          Присоединяйтесь к&nbsp;500+ школам, которые уже управляют бизнесом
          эффективнее. Первые&nbsp;14&nbsp;дней — бесплатно, без кредитной карты.
        </p>

        {/* CTA buttons */}
        <div className="flex flex-col sm:flex-row gap-4 justify-center items-center mb-10">
          <Button
            asChild
            className="bg-primary hover:bg-primary/90 text-primary-foreground px-8 h-12 text-base shadow-xl shadow-primary/25 hover:shadow-primary/40 transition-all duration-300 hover:scale-[1.02] focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 focus-visible:ring-offset-background"
          >
            <Link href="/signup">
              Начать бесплатно сегодня
              <ArrowRight className="w-4 h-4 ml-1" aria-hidden="true" />
            </Link>
          </Button>
          <Button
            asChild
            variant="outline"
            className="border-border bg-muted/30 hover:bg-muted/60 text-foreground px-8 h-12 text-base hover:border-border transition-all duration-300 focus-visible:ring-2 focus-visible:ring-ring"
          >
            <Link href="/demo">Запросить демонстрацию</Link>
          </Button>
        </div>

        {/* Trust badges */}
        <div className="flex flex-wrap justify-center gap-6 text-sm text-muted-foreground">
          {[
            "14 дней бесплатного периода",
            "Без кредитной карты",
            "Отмена в любое время",
            "Данные принадлежат вам",
          ].map((item) => (
            <span key={item} className="flex items-center gap-1.5">
              <span className="w-1.5 h-1.5 rounded-full bg-primary/60" aria-hidden="true" />
              {item}
            </span>
          ))}
        </div>
      </div>
    </section>
  );
}
