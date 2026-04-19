import { check } from "k6";
import http from "k6/http";
import { CONSTANTS } from "../config";
import { getBaseUrl } from "../utils/helpers";
import type { SeededRandom } from "../utils/seeded-random";
import type { TestDataGenerator } from "../utils/test-data";
import { validateResponse } from "../utils/validation";

export function apiComprehensiveScenario(dataGen: TestDataGenerator, random: SeededRandom): void {
	try {
		const baseUrl = getBaseUrl();

		// Систематическое тестирование всех основных API-эндпоинтов
		const endpoints = [
			{
				url: `${baseUrl}/organisational/api/v1/organizations`,
				name: "organizations",
				maxDuration: 600,
			},
			{
				url: `${baseUrl}/persona/api/v1/skills`,
				name: "skills",
				maxDuration: 300,
			},
			{
				url: `${baseUrl}/notification/api/v1/notifications`,
				name: "notifications",
				maxDuration: 400,
			},
		];

		// Тестируем все базовые эндпоинты
		for (const endpoint of endpoints) {
			const response = http.get(endpoint.url, {
				tags: { scenario: "api_comprehensive", endpoint: endpoint.name },
			});
			validateResponse(response, endpoint.name, CONSTANTS.HTTP_OK, endpoint.maxDuration);

			// Для эндпоинта организаций тестируем различные комбинации параметров
			if (endpoint.name === "organizations") {
				const paramCombinations = [
					{ pageSize: 5, organizationType: dataGen.getRandomOrganizationType() },
					{ pageSize: 10, status: dataGen.getRandomOrganizationStatus() },
					{ search: dataGen.getRandomSearchTerm(), pageSize: 15 },
					{ pageIndex: 2, pageSize: dataGen.getRandomPageSize() },
					{
						search: dataGen.getRandomSearchTerm(),
						organizationType: dataGen.getRandomOrganizationType(),
					},
				];

				let index = 0;
				for (const params of paramCombinations) {
					const queryString = Object.entries(params)
						.map(
							([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`
						)
						.join("&");
					const requestUrl = `${endpoint.url}?${queryString}`;

					const paramsResponse = http.get(requestUrl, {
						tags: {
							scenario: "api_comprehensive",
							endpoint: `${endpoint.name}_params_${index}`,
						},
					});
					validateResponse(paramsResponse, `${endpoint.name}_with_params_${index}`);
					index++;
				}
			}

			// Для эндпоинта навыков тестируем поисковые запросы
			if (endpoint.name === "skills") {
				const skillParams = { query: dataGen.getRandomSkillQuery(), limit: 10 };
				const queryString = Object.entries(skillParams)
					.map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`)
					.join("&");
				const skillsResponse = http.get(`${endpoint.url}?${queryString}`, {
					tags: { scenario: "api_comprehensive", endpoint: "skills_search" },
				});
				validateResponse(skillsResponse, "skills_search", CONSTANTS.HTTP_OK, 300);
			}
		}

		// Граничные условия для эндпоинта организаций
		const edgeCases = [
			{ params: { pageSize: 1 }, name: "min_page_size" },
			{ params: { pageSize: 100 }, name: "max_page_size" },
			{ params: { pageIndex: 1000 }, name: "high_page_index" },
			{ params: { search: "" }, name: "empty_search" },
		];

		for (const testCase of edgeCases) {
			const queryString = Object.entries(testCase.params)
				.map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`)
				.join("&");
			const requestUrl = `${baseUrl}/organisational/api/v1/organizations?${queryString}`;

			const response = http.get(requestUrl, {
				tags: {
					scenario: "api_comprehensive",
					endpoint: `edge_case_${testCase.name}`,
				},
			});
			validateResponse(response, `edge_case_${testCase.name}`);
		}

		// Тестирование несуществующих ресурсов (20% вероятность)
		if (random.bool(0.2)) {
			const errorTests = [
				{
					url: `${baseUrl}/organisational/api/v1/organizations/00000000-0000-0000-0000-000000000000`,
					expectedStatus: CONSTANTS.HTTP_NOT_FOUND,
					name: "invalid_org_id",
				},
			];

			for (const test of errorTests) {
				const response = http.get(test.url, {
					tags: {
						scenario: "api_comprehensive",
						endpoint: `error_${test.name}`,
					},
				});
				validateResponse(response, test.name, test.expectedStatus, 1000);
			}
		}

		// Тестирование некорректных параметров (10% вероятность)
		if (random.bool(0.1)) {
			const malformedParams = { pageSize: "invalid", organizationType: "not_a_number" };
			const queryString = Object.entries(malformedParams)
				.map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`)
				.join("&");
			const requestUrl = `${baseUrl}/organisational/api/v1/organizations?${queryString}`;

			const malformedResponse = http.get(requestUrl, {
				tags: { scenario: "api_comprehensive", endpoint: "malformed_params" },
			});

			// API должен вернуть 400 Bad Request или обработать параметры корректно
			const acceptableStatuses = [CONSTANTS.HTTP_OK, CONSTANTS.HTTP_BAD_REQUEST];
			const isValidStatus = acceptableStatuses.includes(malformedResponse.status);

			check(
				malformedResponse,
				{
					"некорректные параметры обработаны корректно": () => isValidStatus,
				},
				{ scenario: "api_comprehensive", endpoint: "malformed_params" }
			);
		}
	} catch (error) {
		const errorMsg = error instanceof Error ? error.message : "Неизвестная ошибка";
		console.error(`Ошибка в apiComprehensiveScenario: ${errorMsg}`);
	}
}
