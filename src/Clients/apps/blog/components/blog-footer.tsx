import Link from "next/link";

export function BlogFooter() {
  return (
    <footer className="border-border mt-auto border-t">
      <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
        <div className="flex flex-col items-center justify-between gap-4 sm:flex-row">
          <div className="flex items-center gap-2">
            <span className="text-primary font-semibold">Edvantix</span>
            <span className="text-muted-foreground text-sm">Blog</span>
          </div>
          <nav className="flex flex-wrap justify-center gap-6">
            <Link
              href="/"
              className="text-muted-foreground hover:text-foreground text-sm transition-colors"
            >
              Blog
            </Link>
            <Link
              href="/category"
              className="text-muted-foreground hover:text-foreground text-sm transition-colors"
            >
              Categories
            </Link>
          </nav>
          <p className="text-muted-foreground text-xs">
            &copy; {new Date().getFullYear()} Edvantix. All rights reserved.
          </p>
        </div>
      </div>
    </footer>
  );
}
