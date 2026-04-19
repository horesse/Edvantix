import { textSummary } from "https://jslib.k6.io/k6-summary/0.0.1/index.js";
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";
import { sleep } from "k6";
import { apiComprehensiveScenario } from "./scenarios/api-comprehensive";
import { browseOrganizationsScenario } from "./scenarios/browse-organizations";
import { searchFilterScenario } from "./scenarios/search-filter";
import { spikeTestScenario } from "./scenarios/spike-test";
import { stressTestScenario } from "./scenarios/stress-test";
import type { K6SummaryData } from "./types";
import { checkServiceAvailability } from "./utils/helpers";
import { SeededRandom } from "./utils/seeded-random";
import { TestDataGenerator } from "./utils/test-data";

// Экспортируем конфигурацию K6
export { options } from "./config";

// Генератор псевдослучайных чисел с фиксированным сидом для воспроизводимости прогонов
const testRandom = new SeededRandom(
	__ENV.RANDOM_SEED ? Number.parseInt(__ENV.RANDOM_SEED, 10) : 12345
);
const dataGenerator = new TestDataGenerator(testRandom);

export const setup = () => {
	return {
		testStartTime: Date.now(),
	};
};

export default function main() {
	// Базовая проверка доступности сервисов перед выполнением сценария
	if (!checkServiceAvailability()) {
		console.error("Проверка доступности сервисов провалилась. Пропускаем итерацию.");
		sleep(5); // Ждём перед следующей попыткой
		return;
	}

	const scenario = __ENV.scenario || "browse_organizations";

	switch (scenario) {
		case "browse_organizations":
			browseOrganizationsScenario(dataGenerator, testRandom);
			break;
		case "search_filter":
			searchFilterScenario(dataGenerator, testRandom);
			break;
		case "api_comprehensive":
			apiComprehensiveScenario(dataGenerator, testRandom);
			break;
		case "stress_test":
			stressTestScenario(dataGenerator, testRandom);
			break;
		case "spike_test":
			spikeTestScenario(dataGenerator, testRandom);
			break;
		default:
			browseOrganizationsScenario(dataGenerator, testRandom);
	}

	// Случайная задержка для имитации поведения реальных пользователей
	sleep(testRandom.next() * 2 + 1); // 1–3 секунды
}

export function handleSummary(data: K6SummaryData) {
	// Формируем итоговый отчёт нагрузочного теста
	const customSummary = {
		"summary.html": htmlReport(data),
		"summary.json": JSON.stringify(data, null, 2),
		stdout: textSummary(data, { indent: " ", enableColors: true }),
	};

	// Выводим сводную статистику по завершении теста
	if (data.metrics) {
		console.log("\n=== Edvantix K6 Отчёт о нагрузочном тестировании ===");
		console.log(`Длительность теста: ${data.state.testRunDurationMs}ms`);
		console.log(`Всего запросов: ${data.metrics.http_reqs?.values?.count ?? 0}`);
		console.log(`Неудачных запросов: ${data.metrics.http_req_failed?.values?.fails ?? 0}`);
		console.log(
			`Среднее время ответа: ${Math.round(data.metrics.http_req_duration?.values?.avg ?? 0)}ms`
		);
		console.log(
			`95-й перцентиль времени ответа: ${Math.round(
				data.metrics.http_req_duration?.values?.["p(95)"] ?? 0
			)}ms`
		);
		const passes = data.metrics.checks?.values?.passes ?? 0;
		const fails = data.metrics.checks?.values?.fails ?? 0;
		const total = passes + fails;
		const successRate = total > 0 ? Math.round((passes / total) * 100) : 0;
		console.log(`Процент успешных проверок: ${successRate}%`);
		console.log("=====================================================\n");
	}

	return customSummary;
}
