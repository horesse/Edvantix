import axios, {
  AxiosHeaders,
  type AxiosInstance,
  type AxiosResponse,
  type InternalAxiosRequestConfig,
} from "axios";
import axiosRetry, { exponentialDelay } from "axios-retry";

import axiosConfig from "./config";
import { AxiosRequestConfig } from "./global";

const DEFAULT_MAX_RETRIES = Number(process.env.NEXT_PUBLIC_MAX_RETRIES) || 5;

/** Function that returns a fresh access token, or null if refresh failed. */
type TokenRefresher = () => Promise<string | null>;

/**
 * Module-level refresher registered by the auth layer (e.g. AuthGuard).
 * Shared across all ApiClient instances — there is only one auth session per tab.
 */
let tokenRefresher: TokenRefresher | null = null;

/**
 * Registers the callback used by the 401 interceptor to silently refresh tokens.
 * The provided function is responsible for persisting the new token.
 * Call once after the user authenticates (e.g. inside AuthGuard).
 */
export function registerTokenRefresher(fn: TokenRefresher): void {
  tokenRefresher = fn;
}

/** Clears the registered refresher. Call on sign-out. */
export function unregisterTokenRefresher(): void {
  tokenRefresher = null;
}

/** Extends InternalAxiosRequestConfig to track per-request retry state. */
type RetryableConfig = InternalAxiosRequestConfig & { _retry?: boolean };

export default class ApiClient {
  private readonly client: AxiosInstance;

  constructor(config = axiosConfig, maxRetries = DEFAULT_MAX_RETRIES) {
    const axiosConfigs = "baseURL" in config ? config : axiosConfig;

    this.client = axios.create({
      ...axiosConfigs,
      headers: {
        ...(axiosConfigs.headers as Record<string, string>),
      },
    });

    this.setupInterceptors(this.client);

    axiosRetry(this.client, {
      retries: maxRetries,
      retryDelay: exponentialDelay,
      // Only retry network errors, idempotent requests, and rate-limits.
      // 401 is handled separately by the response interceptor below.
      retryCondition: (error) =>
        axiosRetry.isNetworkOrIdempotentRequestError(error) ||
        error.response?.status === 429,
    });
  }

  private setupInterceptors(instance: AxiosInstance): AxiosInstance {
    // Attach the current access token from localStorage to every outgoing request.
    // localStorage.getItem is synchronous — no async needed here.
    instance.interceptors.request.use(
      (config) => {
        const accessToken =
          typeof window !== "undefined"
            ? window.localStorage.getItem("access_token")
            : null;

        if (accessToken) {
          const headers = AxiosHeaders.from(config.headers);
          headers.set("Authorization", `Bearer ${accessToken}`);
          config.headers = headers;
        }

        return config;
      },
      (error) => {
        console.error(`[api-client] Request error: ${JSON.stringify(error)}`);
        return Promise.reject(error);
      },
    );

    // On 401: attempt a silent token refresh and retry the original request once.
    // The refresher (fetchFreshToken in AuthGuard) handles persisting the new token.
    //
    // On 403 PROFILE_NOT_REGISTERED: the user is authenticated but their token
    // doesn't yet include the `profile_id` claim (stale token case). Refresh and
    // retry once. If the retry still fails, redirect to the registration page.
    instance.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config as RetryableConfig | undefined;

        if (
          error.response?.status === 401 &&
          originalRequest &&
          !originalRequest._retry &&
          tokenRefresher
        ) {
          originalRequest._retry = true;

          try {
            const newToken = await tokenRefresher();

            if (newToken) {
              const headers = AxiosHeaders.from(originalRequest.headers);
              headers.set("Authorization", `Bearer ${newToken}`);
              originalRequest.headers = headers;

              return instance(originalRequest);
            }
          } catch (refreshError) {
            console.error("[api-client] Token refresh failed:", refreshError);
          }
        }

        if (
          error.response?.status === 403 &&
          error.response?.data?.code === "PROFILE_NOT_REGISTERED" &&
          originalRequest &&
          !originalRequest._retry &&
          tokenRefresher
        ) {
          originalRequest._retry = true;

          try {
            const newToken = await tokenRefresher();

            if (newToken) {
              const headers = AxiosHeaders.from(originalRequest.headers);
              headers.set("Authorization", `Bearer ${newToken}`);
              originalRequest.headers = headers;

              // Retry — if profile_id is now in the new token, this succeeds.
              return instance(originalRequest);
            }
          } catch (refreshError) {
            console.error(
              "[api-client] Token refresh after PROFILE_NOT_REGISTERED failed:",
              refreshError,
            );
          }

          // Token refreshed but profile_id still missing — user hasn't registered a profile.
          if (
            typeof window !== "undefined" &&
            !window.location.pathname.includes("/profile/register")
          ) {
            window.location.href = "/profile/register";
          }
        }

        return Promise.reject(error);
      },
    );

    return instance;
  }

  public async get<T>(
    url: string,
    config?: AxiosRequestConfig,
  ): Promise<AxiosResponse<T>> {
    return this.client.get<T>(url, config);
  }

  public async post<T>(
    url: string,
    data?: unknown,
    config?: AxiosRequestConfig,
  ): Promise<AxiosResponse<T>> {
    return this.client.post<T>(url, data, config);
  }

  public async put<T>(
    url: string,
    data?: unknown,
    config?: AxiosRequestConfig,
  ): Promise<AxiosResponse<T>> {
    return this.client.put<T>(url, data, config);
  }

  public async patch<T>(
    url: string,
    data?: unknown,
    config?: AxiosRequestConfig,
  ): Promise<AxiosResponse<T>> {
    return this.client.patch<T>(url, data, config);
  }

  public async delete<T>(
    url: string,
    config?: AxiosRequestConfig,
  ): Promise<AxiosResponse<T>> {
    return this.client.delete<T>(url, config);
  }
}

export const apiClient = new ApiClient();
