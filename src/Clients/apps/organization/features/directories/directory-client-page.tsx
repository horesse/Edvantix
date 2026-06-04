"use client";

import { DirectoryPage } from "./directory-page";
import { directoryRegistry } from "./registry";

interface DirectoryClientPageProps {
  code: string;
}

/**
 * Клиентская обёртка: принимает строковый code (сериализуем через server→client
 * границу), ищет конфиг в реестре и рендерит DirectoryPage.
 * Config содержит функции и не может быть передан напрямую из серверного компонента.
 */
export function DirectoryClientPage({
  code,
}: Readonly<DirectoryClientPageProps>) {
  const config = directoryRegistry[code];
  if (!config) return null;
  return <DirectoryPage config={config} />;
}
