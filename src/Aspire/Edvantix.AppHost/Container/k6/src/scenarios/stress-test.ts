import http from "k6/http";
import { CONSTANTS } from "../config";
import { getBaseUrl } from "../utils/helpers";
import type { SeededRandom } from "../utils/seeded-random";
import type { TestDataGenerator } from "../utils/test-data";
import { validateResponse } from "../utils/validation";

export function stressTestScenario(dataGen: TestDataGenerator, random: SeededRandom): void {
	try {
		const baseUrl = getBaseUrl();

		// Тестирование под высокой нагрузкой с параллельными запросами
		const requests = [
			{
				url: `${baseUrl}/organisational/api/v1/organizations`,
				name: "organizations_stress",
			},
			{
				url: `${baseUrl}/persona/api/v1/skills`,
				name: "skills_stress",
			},
			{
				url: `${baseUrl}/notification/api/v1/notifications`,
				name: "notifications_stress",
			},
		];

		// Отправляем несколько параллельных запросов
		const responses = requests.map((req) => http.get(req.url));

		// Валидируем все ответы
		let index = 0;
		for (const response of responses) {
			// При стресс-тесте допускаем более высокое время ответа
			validateResponse(response, requests[index].name, CONSTANTS.HTTP_OK, 1000);
			index++;
		}

		// Сложный поиск организаций под нагрузкой
		const complexSearchParams = {
			search: dataGen.getRandomSearchTerm(),
			organizationType: dataGen.getRandomOrganizationType(),
			status: dataGen.getRandomOrganizationStatus(),
			pageIndex: random.int(1, 5),
			pageSize: dataGen.getRandomPageSize(),
		};

		const queryString = Object.entries(complexSearchParams)
			.map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`)
			.join("&");
		const requestUrl = `${baseUrl}/organisational/api/v1/organizations?${queryString}`;
		const stressSearchResponse = http.get(requestUrl, {
			tags: { scenario: "stress_test", endpoint: "stress_search" },
		});
		validateResponse(stressSearchResponse, "stress_search", CONSTANTS.HTTP_OK, 1200);
	} catch (error) {
		const errorMsg = error instanceof Error ? error.message : "Неизвестная ошибка";
		console.error(`Ошибка в stressTestScenario: ${errorMsg}`);
	}
}
