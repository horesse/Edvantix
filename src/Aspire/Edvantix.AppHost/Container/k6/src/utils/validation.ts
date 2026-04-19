import { sleep } from "k6";
import http from "k6/http";
import { CONSTANTS } from "../config";

/**
 * Парсит JSON-тело ответа безопасно, возвращая null при ошибке.
 */
function parseJsonBody(body: string | ArrayBuffer | null): unknown {
	if (typeof body !== "string") {
		return null;
	}

	try {
		const data = JSON.parse(body);
		return data && typeof data === "object" ? data : null;
	} catch {
		return null;
	}
}

/**
 * Логирует ошибку парсинга JSON с контекстом запроса.
 */
function logParsingError(name: string, error: unknown, response: http.Response): void {
	const errorMsg = error instanceof Error ? error.message : "Неизвестная ошибка";
	console.warn(`Ошибка парсинга JSON для ${name}:`, errorMsg);

	const bodyPreview =
		typeof response.body === "string" ? response.body.substring(0, 200) : "binary data";
	console.warn(`Статус ответа: ${response.status}, Body: ${bodyPreview}...`);
}

/**
 * Проверяет, является ли ответ JSON-типом.
 */
function isJsonResponse(response: http.Response): boolean {
	return !!response.headers["Content-Type"]?.includes("application/json") && !!response.body;
}

/**
 * Валидирует HTTP-ответ: проверяет статус и парсит JSON при успехе.
 */
export function validateResponse(
	response: http.Response,
	name: string,
	expectedStatus: number = CONSTANTS.HTTP_OK,
	_maxDuration: number = CONSTANTS.RESPONSE_TIME_THRESHOLD_95
): unknown {
	// Логируем неожиданный статус
	if (response.status !== expectedStatus) {
		console.warn(
			`${name} вернул неожиданный статус: ${response.status} (ожидался: ${expectedStatus})`
		);
		return null;
	}

	// Парсим JSON только для успешных ответов с JSON-контентом
	if (expectedStatus !== CONSTANTS.HTTP_OK || !isJsonResponse(response)) {
		return null;
	}

	const data = parseJsonBody(response.body);
	if (data) {
		return data;
	}

	logParsingError(name, new Error("Ошибка парсинга JSON"), response);
	return null;
}

/**
 * Валидирует структуру постраничного ответа PagedResult.
 */
export function validatePagedResponse(data: unknown, name: string): boolean {
	if (!data || typeof data !== "object") {
		console.warn(`${name} — нет данных для валидации пагинации`);
		return false;
	}

	try {
		const record = data as Record<string, unknown>;
		const hasItems = Object.hasOwn(record, "items") && Array.isArray(record.items);
		const hasPageIndex = Object.hasOwn(record, "pageIndex") && typeof record.pageIndex === "number";
		const hasPageSize = Object.hasOwn(record, "pageSize") && typeof record.pageSize === "number";
		const hasTotalItems =
			Object.hasOwn(record, "totalItems") && typeof record.totalItems === "number";

		if (!hasItems) {
			console.warn(`${name} — отсутствует или некорректен массив 'items'`);
			return false;
		}

		if (!hasPageIndex || (record.pageIndex as number) < 0) {
			console.warn(`${name} — некорректный pageIndex: ${record.pageIndex}`);
			return false;
		}

		if (!hasPageSize || (record.pageSize as number) <= 0) {
			console.warn(`${name} — некорректный pageSize: ${record.pageSize}`);
			return false;
		}

		if (!hasTotalItems || (record.totalItems as number) < 0) {
			console.warn(`${name} — некорректный totalItems: ${record.totalItems}`);
			return false;
		}

		return true;
	} catch (error) {
		const errorMsg = error instanceof Error ? error.message : "Неизвестная ошибка";
		console.error(`Ошибка валидации постраничного ответа для ${name}:`, errorMsg);
		return false;
	}
}

/**
 * Формирует URL с query-параметрами.
 */
function buildUrlWithParams(url: string, params: Record<string, unknown>): string {
	if (!params || Object.keys(params).length === 0) {
		return url;
	}

	const queryString = Object.entries(params)
		.map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`)
		.join("&");
	return `${url}${url.includes("?") ? "&" : "?"}${queryString}`;
}

/**
 * Выполняет GET-запрос с автоматическими повторными попытками при ошибке.
 */
export function testEndpointWithRetry(
	url: string,
	params: Record<string, unknown>,
	name: string,
	headers: Record<string, string> = {},
	maxRetries: number = 2
): http.Response | { status: number; timings: { duration: number } } {
	let response: http.Response | { status: number; timings: { duration: number } };
	let retries = 0;
	const requestUrl = buildUrlWithParams(url, params);

	do {
		try {
			response = http.get(requestUrl, {
				headers,
				tags: { name, retry: retries.toString() },
				timeout: "10s",
			});
			if (response.status === CONSTANTS.HTTP_OK || retries >= maxRetries) break;
		} catch (error) {
			const errorMsg = error instanceof Error ? error.message : "Неизвестная ошибка";
			console.warn(`Запрос ${name} не удался (попытка ${retries + 1}): ${errorMsg}`);
			response = { status: 0, timings: { duration: 10000 } };
		}

		retries++;
		if (retries <= maxRetries) {
			console.warn(`Повтор ${name} (попытка ${retries + 1}/${maxRetries + 1})`);
			sleep(0.5 * retries); // Экспоненциальный откат
		}
	} while (retries <= maxRetries);

	if (response.status !== CONSTANTS.HTTP_OK && retries > 0) {
		console.warn(
			`${name} не удался после ${retries} попыток. Финальный статус: ${response.status}`
		);
	}

	return response;
}
