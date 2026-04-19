import { check } from "k6";
import http from "k6/http";
import { CONSTANTS } from "../config";

/**
 * Проверяет доступность сервисов через health-эндпоинты шлюза.
 * Возвращает false при недоступности хотя бы одного сервиса.
 */
export function checkServiceAvailability(): boolean {
	const baseUrl = getBaseUrl();
	const services = ["persona", "organisational"];

	for (const service of services) {
		const response = http.get(`${baseUrl}/${service}/health`, {
			timeout: "5s",
		});

		if (response.status === 0) {
			console.error(
				`Сервис ${service} недоступен по адресу ${baseUrl}. Убедитесь, что сервис запущен.`
			);
			return false;
		}

		// Принимаем любой HTTP-ответ как признак работоспособности сервиса
		const acceptableStatuses = [CONSTANTS.HTTP_OK, 404];
		if (!acceptableStatuses.includes(response.status)) {
			console.warn(`Сервис ${service} вернул статус ${response.status}`);
		}
	}

	return true;
}

/**
 * Получает базовый URL шлюза из переменных окружения Aspire.
 */
export function getBaseUrl(): string {
	const baseUrl = __ENV.services__gateway__http__0;
	if (!baseUrl) {
		throw new Error(
			"services__gateway__http__0 не задан. Убедитесь, что шлюз запущен и привязан к K6."
		);
	}
	return baseUrl;
}

/**
 * Тестирует получение деталей конкретной организации по идентификатору.
 */
export function testOrganizationDetails(
	orgId: string,
	name: string = "organization_details"
): unknown {
	const response = http.get(`${getBaseUrl()}/organisational/api/v1/organizations/${orgId}`, {
		tags: { name },
		timeout: "10s",
	});
	const data = parseJsonResponse(response, name) as Record<string, unknown> | null;

	if (data && response.status === CONSTANTS.HTTP_OK) {
		check(response, {
			[`${name}: содержит id`]: () => Object.hasOwn(data, "id"),
			[`${name}: содержит name`]: () => Object.hasOwn(data, "name"),
		});
	}

	return data;
}

/**
 * Парсит JSON-тело ответа при успешном статусе.
 */
function parseJsonResponse(response: http.Response, name: string): unknown {
	if (response.status === CONSTANTS.HTTP_OK) {
		try {
			if (
				response.headers["Content-Type"]?.includes("application/json") &&
				response.body &&
				typeof response.body === "string"
			) {
				return JSON.parse(response.body);
			}
		} catch (error) {
			const errorMsg = error instanceof Error ? error.message : "Неизвестная ошибка";
			console.warn(`Ошибка парсинга JSON для ${name}:`, errorMsg);
		}
	}
	return null;
}
