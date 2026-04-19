import http from "k6/http";
import { getBaseUrl } from "../utils/helpers";
import type { SeededRandom } from "../utils/seeded-random";
import type { TestDataGenerator } from "../utils/test-data";
import {
	testEndpointWithRetry,
	validatePagedResponse,
	validateResponse,
} from "../utils/validation";

export function searchFilterScenario(dataGen: TestDataGenerator, random: SeededRandom): void {
	try {
		const baseUrl = getBaseUrl();

		const searchTerm = dataGen.getRandomSearchTerm();
		const orgType = dataGen.getRandomOrganizationType();

		// Поиск организаций по тексту с комбинированными параметрами
		const searchParams = {
			search: searchTerm,
			pageIndex: 1,
			pageSize: dataGen.getRandomPageSize(),
		};

		const searchResponse = testEndpointWithRetry(
			`${baseUrl}/organisational/api/v1/organizations`,
			searchParams,
			"search"
		);
		if ("body" in searchResponse) {
			const searchData = validateResponse(searchResponse, "search", 200, 800);
			validatePagedResponse(searchData, "search");
		}

		// Фильтрация только по типу организации
		const typeFilterParams = {
			organizationType: orgType,
			pageIndex: 1,
			pageSize: 10,
		};

		const queryString = Object.entries(typeFilterParams)
			.map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`)
			.join("&");
		const typeFilterResponse = http.get(
			`${baseUrl}/organisational/api/v1/organizations?${queryString}`,
			{
				tags: { scenario: "search_filter", endpoint: "type_filter" },
			}
		);
		validateResponse(typeFilterResponse, "type_filter");

		// Фильтрация по статусу организации (50% вероятность)
		if (random.bool(0.5)) {
			const statusFilterParams = {
				status: dataGen.getRandomOrganizationStatus(),
				pageIndex: 1,
				pageSize: dataGen.getRandomPageSize(),
			};

			const statusQueryString = Object.entries(statusFilterParams)
				.map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`)
				.join("&");
			const statusFilterResponse = http.get(
				`${baseUrl}/organisational/api/v1/organizations?${statusQueryString}`,
				{
					tags: { scenario: "search_filter", endpoint: "status_filter" },
				}
			);
			validateResponse(statusFilterResponse, "status_filter");
		}

		// Пагинация с различными размерами страниц (50% вероятность)
		if (random.bool(0.5)) {
			const paginationParams = {
				pageIndex: random.int(1, 3),
				pageSize: dataGen.getRandomPageSize(),
			};

			const paginationQueryString = Object.entries(paginationParams)
				.map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`)
				.join("&");
			const paginationResponse = http.get(
				`${baseUrl}/organisational/api/v1/organizations?${paginationQueryString}`,
				{
					tags: { scenario: "search_filter", endpoint: "pagination" },
				}
			);
			validateResponse(paginationResponse, "pagination");
		}
	} catch (error) {
		const errorMsg = error instanceof Error ? error.message : "Неизвестная ошибка";
		console.error(`Ошибка в searchFilterScenario: ${errorMsg}`);
	}
}
