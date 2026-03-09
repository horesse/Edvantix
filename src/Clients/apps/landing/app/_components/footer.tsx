import Link from "next/link";

import { ExternalLink, GraduationCap } from "lucide-react";

const FOOTER_LINKS = [
  {
    heading: "Продукт",
    links: [
      { label: "Возможности", href: "#features" },
      { label: "Тарифы", href: "#pricing" },
      { label: "Интеграции", href: "/integrations" },
      { label: "Безопасность", href: "/security" },
      { label: "Changelog", href: "/changelog" },
    ],
  },
  {
    heading: "Компания",
    links: [
      { label: "О нас", href: "/about" },
      { label: "Блог", href: "/blog" },
      { label: "Партнёры", href: "/partners" },
      { label: "Вакансии", href: "/careers" },
      { label: "Контакты", href: "/contact" },
    ],
  },
  {
    heading: "Ресурсы",
    links: [
      { label: "Документация", href: "/docs" },
      { label: "API", href: "/api" },
      { label: "Статус сервиса", href: "/status" },
      { label: "Поддержка", href: "/support" },
      { label: "Сообщество", href: "/community" },
    ],
  },
  {
    heading: "Правовое",
    links: [
      { label: "Условия использования", href: "/terms" },
      { label: "Политика конфиденциальности", href: "/privacy" },
      { label: "Обработка данных", href: "/dpa" },
      { label: "Cookie", href: "/cookies" },
    ],
  },
] as const;

function SocialIcon({ path }: { path: string }) {
  return (
    <svg
      viewBox="0 0 24 24"
      className="h-4 w-4 fill-current"
      aria-hidden="true"
    >
      <path d={path} />
    </svg>
  );
}

const SOCIAL_LINKS = [
  {
    label: "Telegram",
    href: "https://t.me/edvantix",
    icon: (
      <SocialIcon path="M11.944 0A12 12 0 0 0 0 12a12 12 0 0 0 12 12 12 12 0 0 0 12-12A12 12 0 0 0 12 0a12 12 0 0 0-.056 0zm4.962 7.224c.1-.002.321.023.465.14a.506.506 0 0 1 .171.325c.016.093.036.306.02.472-.18 1.898-.96 6.502-1.36 8.627-.168.9-.499 1.201-.82 1.23-.696.065-1.225-.46-1.9-.902-1.056-.693-1.653-1.124-2.678-1.8-1.185-.78-.417-1.21.258-1.91.177-.184 3.247-2.977 3.307-3.23.007-.032.014-.15-.056-.212s-.174-.041-.249-.024c-.106.024-1.793 1.14-5.061 3.345-.48.33-.913.49-1.302.48-.428-.008-1.252-.241-1.865-.44-.752-.245-1.349-.374-1.297-.789.027-.216.325-.437.893-.663 3.498-1.524 5.83-2.529 6.998-3.014 3.332-1.386 4.025-1.627 4.476-1.635z" />
    ),
  },
  {
    label: "ВКонтакте",
    href: "https://vk.com/edvantix",
    icon: (
      <SocialIcon path="M15.684 0H8.316C1.592 0 0 1.592 0 8.316v7.368C0 22.408 1.592 24 8.316 24h7.368C22.408 24 24 22.408 24 15.684V8.316C24 1.592 22.391 0 15.684 0zm3.692 17.123h-1.744c-.66 0-.864-.525-2.05-1.727-1.033-1.01-1.49-.854-1.49.506v1.22c0 .606-.213.862-1.197.862-1.83 0-3.81-1.07-5.222-3.086C5.044 11.66 4.1 8.763 4.1 8.3c0-.49.197-.75.586-.75h1.747c.44 0 .6.193.767.653 1.017 2.764 2.71 5.176 3.4 5.176.266 0 .38-.118.38-.762V9.1c-.073-1.367-.794-1.482-.794-1.97 0-.24.193-.49.5-.49h2.754c.37 0 .504.194.504.643v3.47c0 .37.166.5.27.5.265 0 .49-.13.993-.64 1.54-1.73 2.64-4.4 2.64-4.4.147-.297.39-.576.845-.576h1.747c.526 0 .64.265.526.643-.22 1.02-2.365 4.05-2.365 4.05-.186.302-.254.437 0 .775.182.253.783.776 1.184 1.244.74.853 1.307 1.572 1.46 2.068.15.486-.1.733-.587.733z" />
    ),
  },
  {
    label: "YouTube",
    href: "https://youtube.com/edvantix",
    icon: (
      <SocialIcon path="M23.498 6.186a3.016 3.016 0 0 0-2.122-2.136C19.505 3.545 12 3.545 12 3.545s-7.505 0-9.377.505A3.017 3.017 0 0 0 .502 6.186C0 8.07 0 12 0 12s0 3.93.502 5.814a3.016 3.016 0 0 0 2.122 2.136c1.871.505 9.376.505 9.376.505s7.505 0 9.377-.505a3.015 3.015 0 0 0 2.122-2.136C24 15.93 24 12 24 12s0-3.93-.502-5.814zM9.545 15.568V8.432L15.818 12l-6.273 3.568z" />
    ),
  },
] as const;

export function Footer() {
  const currentYear = new Date().getFullYear();

  return (
    <footer
      className="bg-card border-border relative border-t"
      role="contentinfo"
    >
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        {/* Main grid */}
        <div className="grid grid-cols-2 gap-8 py-16 md:grid-cols-6 lg:gap-12">
          {/* Brand */}
          <div className="col-span-2">
            <Link
              href="/"
              className="group focus-visible:ring-ring mb-4 flex w-fit items-center gap-2.5 rounded-md focus-visible:ring-2 focus-visible:outline-none"
              aria-label="Edvantix — на главную"
            >
              <div className="bg-primary shadow-primary/20 group-hover:shadow-primary/35 flex h-8 w-8 items-center justify-center rounded-lg shadow-lg transition-shadow">
                <GraduationCap
                  className="text-primary-foreground h-4 w-4"
                  aria-hidden="true"
                />
              </div>
              <span className="text-card-foreground text-lg font-bold tracking-tight">
                Edv<span className="text-primary">antix</span>
              </span>
            </Link>

            <p className="text-muted-foreground mb-6 max-w-[220px] text-sm leading-relaxed">
              Платформа управления онлайн-школой нового поколения. Всё в одном
              месте.
            </p>

            <div
              className="flex gap-2"
              role="list"
              aria-label="Социальные сети"
            >
              {SOCIAL_LINKS.map((social) => (
                <a
                  key={social.label}
                  href={social.href}
                  target="_blank"
                  rel="noopener noreferrer"
                  aria-label={social.label}
                  role="listitem"
                  className="bg-muted/50 border-border text-muted-foreground hover:text-foreground hover:bg-muted hover:border-border focus-visible:ring-ring flex h-9 w-9 items-center justify-center rounded-lg border transition-all duration-200 focus-visible:ring-2 focus-visible:outline-none"
                >
                  {social.icon}
                </a>
              ))}
            </div>
          </div>

          {/* Link columns */}
          {FOOTER_LINKS.map((group) => (
            <nav
              key={group.heading}
              className="col-span-1"
              aria-label={group.heading}
            >
              <h3 className="text-foreground mb-4 text-sm font-semibold">
                {group.heading}
              </h3>
              <ul className="flex flex-col gap-2.5" role="list">
                {group.links.map((link) => (
                  <li key={link.label}>
                    <Link
                      href={link.href}
                      className="text-muted-foreground hover:text-foreground group focus-visible:ring-ring flex items-center gap-1 rounded text-sm transition-colors duration-200 focus-visible:ring-2 focus-visible:outline-none"
                    >
                      {link.label}
                      {link.href.startsWith("http") && (
                        <ExternalLink
                          className="h-3 w-3 opacity-0 transition-opacity group-hover:opacity-60"
                          aria-hidden="true"
                        />
                      )}
                    </Link>
                  </li>
                ))}
              </ul>
            </nav>
          ))}
        </div>

        {/* Bottom bar */}
        <div className="border-border flex flex-col items-center justify-between gap-4 border-t py-6 sm:flex-row">
          <p className="text-muted-foreground text-sm">
            &copy; {currentYear} Edvantix. Все права защищены.
          </p>
          <div className="text-muted-foreground flex items-center gap-1.5 text-sm">
            <span>Сделано в&nbsp;</span>
            <span aria-label="России">🇷🇺</span>
            <span>с любовью к образованию</span>
          </div>
        </div>
      </div>
    </footer>
  );
}
