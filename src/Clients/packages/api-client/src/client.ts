import axios, { type AxiosInstance, type AxiosResponse } from "axios";

import axiosConfig from "./config";
import { AxiosRequestConfig } from "./global";

type TokenProvider = () => Promise<string | null>;

export default class ApiClient {
  private readonly client: AxiosInstance;

  private static tokenProvider: TokenProvider | null = null;

  /**
   * Устанавливает глобальный провайдер токена авторизации.
   * Вызывается один раз при инициализации приложения.
   */
  public static setTokenProvider(provider: TokenProvider): void {
    ApiClient.tokenProvider = provider;
  }

  constructor(config = axiosConfig) {
    const axiosConfigs = "baseURL" in config ? config : axiosConfig;

    const instance = axios.create({
      ...axiosConfigs,
      headers: {
        ...(axiosConfigs.headers as Record<string, string>),
      },
    });

    this.client = this.setupInterceptors(instance);
  }

  private setupInterceptors(instance: AxiosInstance): AxiosInstance {
    instance.interceptors.request.use(
      async (config) => {
        if (ApiClient.tokenProvider) {
          const token = await ApiClient.tokenProvider();
          if (token) {
            config.headers.Authorization = `Bearer ${token}`;
          }
        }
        return config;
      },
      (error) => {
        console.error(`[request error] [${JSON.stringify(error)}]`);
        return Promise.reject(new Error(error));
      },
    );
    instance.interceptors.response.use(
      async (response) => response,
      (error) => {
        return Promise.reject(new Error(error));
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
