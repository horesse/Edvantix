import axios, { type AxiosInstance, type AxiosResponse } from "axios";
import axiosRetry, { exponentialDelay } from "axios-retry";

import axiosConfig from "./config";
import { AxiosRequestConfig } from "./global";

const DEFAULT_MAX_RETRIES = Number(process.env.NEXT_PUBLIC_MAX_RETRIES) || 5;

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

    axiosRetry(this.client, {
      retries: maxRetries,
      retryDelay: exponentialDelay,
      retryCondition: (error) =>
        axiosRetry.isNetworkOrIdempotentRequestError(error) ||
        error.response?.status === 429,
    });
  }
  private setupInterceptors(instance: AxiosInstance): AxiosInstance {
    instance.interceptors.request.use(
      async (config) => {
        const accessToken = localStorage.getItem("access_token");

        if (accessToken) {
          config.headers["Authorization"] = `Bearer ${accessToken}`;
        }

        return config;
      },
      (error) => {
        console.error(`[request error] [${JSON.stringify(error)}]`);
        return Promise.reject(error);
      },
    );
    instance.interceptors.response.use(
      async (response) => response,
      (error) => {
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
