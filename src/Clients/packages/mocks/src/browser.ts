import { setupWorker } from "msw/browser";

export const worker = setupWorker();

export const startMocking = async () => {
  await worker.start({
    onUnhandledRequest: "warn",
    serviceWorker: {
      url: "/mockServiceWorker.js",
    },
    quiet: false,
  });
};
