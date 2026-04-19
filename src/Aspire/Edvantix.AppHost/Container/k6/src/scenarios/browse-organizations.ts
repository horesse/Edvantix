import http from "k6/http";
import { getBaseUrl, testOrganizationDetails } from "../utils/helpers";
import type { SeededRandom } from "../utils/seeded-random";
import type { TestDataGenerator } from "../utils/test-data";
import {
	testEndpointWithRetry,
	validatePagedResponse,
	validateResponse,
} from "../utils/validation";

export function browseOrganizationsScenario(
	dataGen: TestDataGenerator,
	random: SeededRandom
): void {
	try {
		const baseUrl = getBaseUrl();

		// Запрос списка организаций — основной сценарий просмотра
		const orgsResponse = http.get(`${baseUrl}/organisational/api/v1/organizations`, {
			tags: { scenario: "browse_organizations", endpoint: "organizations" },
			timeout: "10s",
		});
		const orgsData = validateResponse(orgsResponse, "organizations", 200, 1200) as {
			items?: Array<{ id?: string }>;
		} | null;
		validatePagedResponse(orgsData, "organizations");

		// Поиск организаций по тексту (70% вероятность)
		if (random.bool(0.7)) {
			const searchParams = {
				search: dataGen.getRandomSearchTerm(),
				pageSize: dataGen.getRandomPageSize(),
				pageIndex: 1,
			};
			const searchResponse = testEndpointWithRetry(
				`${baseUrl}/organisational/api/v1/organizations`,
				searchParams,
				"organizations_search"
			);
			if ("body" in searchResponse) {
				validateResponse(searchResponse, "organizations_search");
			}
		}

		// Просмотр конкретной организации (30% вероятность)
		if (
			random.bool(0.3) &&
			orgsData?.items &&
			Array.isArray(orgsData.items) &&
			orgsData.items.length > 0
		) {
			const randomOrg = orgsData.items[random.int(0, orgsData.items.length - 1)];
			if (randomOrg?.id) {
				testOrganizationDetails(randomOrg.id, "browse_org_details");
			}
		}

		// Пагинация по страницам (40% вероятность)
		if (random.bool(0.4)) {
			const paginationParams = {
				pageIndex: dataGen.getRandomPageIndex(),
				pageSize: dataGen.getRandomPageSize(),
			};
			const queryString = Object.entries(paginationParams)
				.map(([key, value]) => `${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`)
				.join("&");
			const paginationResponse = http.get(
				`${baseUrl}/organisational/api/v1/organizations?${queryString}`,
				{
					tags: { scenario: "browse_organizations", endpoint: "organizations_pagination" },
					timeout: "10s",
				}
			);
			validateResponse(paginationResponse, "pagination_browse");
		}
	} catch (error) {
		const errorMsg = error instanceof Error ? error.message : "Неизвестная ошибка";
		console.error(`Ошибка в browseOrganizationsScenario: ${errorMsg}`);
	}
}
