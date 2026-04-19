import http from "k6/http";
import { CONSTANTS } from "../config";
import { getBaseUrl } from "../utils/helpers";
import type { SeededRandom } from "../utils/seeded-random";
import type { TestDataGenerator } from "../utils/test-data";
import { validateResponse } from "../utils/validation";

export function spikeTestScenario(_dataGen: TestDataGenerator, random: SeededRandom): void {
	try {
		// Имитируем внезапный всплеск трафика — проверяем устойчивость сервисов
		const spikeEndpoints = [
			`${getBaseUrl()}/organisational/api/v1/organizations`,
			`${getBaseUrl()}/persona/api/v1/skills`,
		];

		const randomEndpoint = spikeEndpoints[random.int(0, spikeEndpoints.length - 1)];
		const response = http.get(randomEndpoint);

		// При спайк-тесте допускаем более высокое время ответа
		validateResponse(response, "spike_test", CONSTANTS.HTTP_OK, 1500);

		// Быстрые последовательные запросы (30% вероятность — имитирует многократные клики)
		if (random.bool(0.3)) {
			const rapidResponse = http.get(randomEndpoint);
			validateResponse(rapidResponse, "rapid_request", CONSTANTS.HTTP_OK, 1500);
		}
	} catch (error) {
		const errorMsg = error instanceof Error ? error.message : "Неизвестная ошибка";
		console.error(`Ошибка в spikeTestScenario: ${errorMsg}`);
	}
}
