"use client";

import { useEffect, useState } from "react";

import Link from "next/link";

import { ChevronRight, GraduationCap, Menu, X } from "lucide-react";

import { Button } from "@workspace/ui/components/button";

const NAV_LINKS = [
  { label: "Возможности", href: "#features" },
  { label: "Как это работает", href: "#how-it-works" },
  { label: "Тарифы", href: "#pricing" },
  { label: "Отзывы", href: "#testimonials" },
] as const;

export function Navbar() {
  const [scrolled, setScrolled] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);

  useEffect(() => {
    const handleScroll = () => {
      setScrolled(window.scrollY > 16);
    };

    window.addEventListener("scroll", handleScroll, { passive: true });

    return () => {
      window.removeEventListener("scroll", handleScroll);
    };
  }, []);

  return (
    <header
      className={`fixed top-0 right-0 left-0 z-50 transition-all duration-500 ${
        scrolled
          ? "bg-background/90 border-border shadow-background/50 border-b shadow-lg backdrop-blur-xl"
          : "bg-transparent"
      }`}
    >
      {/* Skip to content */}
      <a
        href="#main-content"
        className="bg-primary text-primary-foreground sr-only z-50 rounded-md px-4 py-2 text-sm focus:not-sr-only focus:absolute focus:top-4 focus:left-4"
      >
        Перейти к содержимому
      </a>

      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="flex h-16 items-center justify-between">
          {/* Logo */}
          <Link
            href="/"
            className="group focus-visible:ring-ring flex items-center gap-2.5 rounded-md focus-visible:ring-2 focus-visible:outline-none"
            aria-label="Edvantix — на главную"
          >
            <div className="bg-primary shadow-primary/30 group-hover:shadow-primary/50 flex h-8 w-8 items-center justify-center rounded-lg shadow-lg transition-all duration-300 group-hover:scale-105">
              <GraduationCap
                className="text-primary-foreground h-4 w-4"
                aria-hidden="true"
              />
            </div>
            <span className="text-card-foreground text-lg font-bold tracking-tight">
              Edv<span className="text-primary">antix</span>
            </span>
          </Link>

          {/* Desktop navigation */}
          <nav
            className="hidden items-center gap-1 md:flex"
            aria-label="Основная навигация"
          >
            {NAV_LINKS.map((link) => (
              <Link
                key={link.href}
                href={link.href}
                className="text-muted-foreground hover:text-foreground hover:bg-accent focus-visible:ring-ring rounded-md px-4 py-2 text-sm transition-colors duration-200 focus-visible:ring-2 focus-visible:outline-none"
              >
                {link.label}
              </Link>
            ))}
          </nav>

          {/* Desktop CTA */}
          <div className="hidden items-center gap-3 md:flex">
            <Link
              href="/login"
              className="text-muted-foreground hover:text-foreground hover:bg-accent focus-visible:ring-ring rounded-md px-4 py-2 text-sm transition-colors focus-visible:ring-2 focus-visible:outline-none"
            >
              Войти
            </Link>
            <Button
              asChild
              className="bg-primary hover:bg-primary/90 text-primary-foreground shadow-primary/20 hover:shadow-primary/30 focus-visible:ring-ring focus-visible:ring-offset-background shadow-lg transition-all duration-200 focus-visible:ring-2 focus-visible:ring-offset-2"
            >
              <Link href="/signup">
                Начать бесплатно
                <ChevronRight
                  className="ml-0.5 h-3.5 w-3.5"
                  aria-hidden="true"
                />
              </Link>
            </Button>
          </div>

          {/* Mobile menu button */}
          <button
            className="text-muted-foreground hover:text-foreground hover:bg-accent focus-visible:ring-ring rounded-md p-2 transition-colors focus-visible:ring-2 focus-visible:outline-none md:hidden"
            onClick={() => setMenuOpen(!menuOpen)}
            aria-label={menuOpen ? "Закрыть меню" : "Открыть меню"}
            aria-expanded={menuOpen}
            aria-controls="mobile-menu"
            type="button"
          >
            {menuOpen ? (
              <X className="h-5 w-5" aria-hidden="true" />
            ) : (
              <Menu className="h-5 w-5" aria-hidden="true" />
            )}
          </button>
        </div>

        {/* Mobile menu */}
        <div
          id="mobile-menu"
          className={`overflow-hidden transition-all duration-300 md:hidden ${
            menuOpen ? "max-h-96 opacity-100" : "max-h-0 opacity-0"
          }`}
          aria-hidden={!menuOpen}
        >
          <div className="border-border mt-1 border-t pb-4">
            <nav
              className="flex flex-col gap-1 pt-3"
              aria-label="Мобильная навигация"
            >
              {NAV_LINKS.map((link) => (
                <Link
                  key={link.href}
                  href={link.href}
                  className="text-muted-foreground hover:text-foreground hover:bg-accent focus-visible:ring-ring rounded-md px-4 py-2.5 text-sm transition-colors focus-visible:ring-2 focus-visible:outline-none"
                  onClick={() => setMenuOpen(false)}
                >
                  {link.label}
                </Link>
              ))}
              <div className="mt-3 flex flex-col gap-2 px-1">
                <Link
                  href="/login"
                  className="text-muted-foreground hover:text-foreground hover:bg-accent rounded-md px-4 py-2.5 text-center text-sm transition-colors"
                  onClick={() => setMenuOpen(false)}
                >
                  Войти
                </Link>
                <Button
                  asChild
                  className="bg-primary hover:bg-primary/90 text-primary-foreground w-full"
                >
                  <Link href="/signup" onClick={() => setMenuOpen(false)}>
                    Начать бесплатно
                  </Link>
                </Button>
              </div>
            </nav>
          </div>
        </div>
      </div>
    </header>
  );
}
