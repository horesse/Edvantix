import type { TestConstants } from "../types";

/**
 * Константы и пороговые значения для тестов Edvantix
 */
export const CONSTANTS: TestConstants = {
	HTTP_OK: 200,
	HTTP_BAD_REQUEST: 400,
	HTTP_NOT_FOUND: 404,
	HTTP_UNAUTHORIZED: 401,
	RESPONSE_TIME_THRESHOLD_95: 1000, // миллисекунды — реалистично для среды разработки
	RESPONSE_TIME_THRESHOLD_99: 2000, // миллисекунды — мягкий порог
	CHECK_RATE_THRESHOLD: 0.9, // 90% успешных проверок
};

/**
 * Конфигурация K6 для нагрузочного тестирования Edvantix
 */
export const options = {
	maxVUs: 50,
	preAllocatedVUs: 10,
	timeUnit: "1s",
	startRate: 0,
	abortOnFail: false,
	scenarios: {
		// Сценарий 1: Просмотр списка организаций (лёгкая нагрузка)
		browse_organizations: {
			executor: "ramping-vus",
			env: { scenario: "browse_organizations" },
			startVUs: 0,
			stages: [
				{ duration: "30s", target: 3 },
				{ duration: "1m", target: 5 },
				{ duration: "30s", target: 0 },
			],
			gracefulRampDown: "30s",
		},
		// Сценарий 2: Поиск и фильтрация организаций (средняя нагрузка)
		search_filter: {
			executor: "ramping-vus",
			env: { scenario: "search_filter" },
			startVUs: 0,
			stages: [
				{ duration: "30s", target: 5 },
				{ duration: "1m", target: 8 },
				{ duration: "30s", target: 0 },
			],
			gracefulRampDown: "30s",
		},
		// Сценарий 3: Комплексное тестирование всех API-эндпоинтов
		api_comprehensive: {
			executor: "ramping-vus",
			env: { scenario: "api_comprehensive" },
			startVUs: 0,
			stages: [
				{ duration: "20s", target: 3 },
				{ duration: "40s", target: 6 },
				{ duration: "20s", target: 0 },
			],
			gracefulRampDown: "20s",
		},
		// Сценарий 4: Стресс-тест (высокая нагрузка)
		stress_test: {
			executor: "ramping-vus",
			env: { scenario: "stress_test" },
			startVUs: 0,
			stages: [
				{ duration: "30s", target: 10 },
				{ duration: "1m", target: 15 },
				{ duration: "30s", target: 0 },
			],
			gracefulRampDown: "30s",
		},
		// Сценарий 5: Спайк-тест (внезапный всплеск нагрузки)
		spike_test: {
			executor: "ramping-vus",
			env: { scenario: "spike_test" },
			startVUs: 0,
			stages: [
				{ duration: "20s", target: 2 },
				{ duration: "10s", target: 12 },
				{ duration: "20s", target: 12 }, // Поддерживаем пиковую нагрузку
				{ duration: "10s", target: 2 }, // Быстрое восстановление
				{ duration: "20s", target: 0 },
			],
			gracefulRampDown: "20s",
		},
	},
	thresholds: {
		// Пороги времени ответа
		http_req_duration: [
			`p(95)<${CONSTANTS.RESPONSE_TIME_THRESHOLD_95}`,
			`p(99)<${CONSTANTS.RESPONSE_TIME_THRESHOLD_99}`,
			"avg<500", // Среднее время ответа не более 500 мс
			"med<300", // Медиана не более 300 мс
		],
		// Порог ошибок — не более 5%
		http_req_failed: ["rate<0.05"],
		// Время обработки на сервере
		http_req_waiting: ["p(95)<800"],
		// Минимальный процент успешных проверок
		checks: [`rate>${CONSTANTS.CHECK_RATE_THRESHOLD}`],

		// Пороги по конкретным эндпоинтам
		"http_req_duration{name:organizations}": ["p(95)<1200", "avg<500"],
		"http_req_duration{name:organization_details}": ["p(95)<800", "avg<300"],
		"http_req_duration{name:members}": ["p(95)<1000", "avg<400"],
		"http_req_duration{name:skills}": ["p(95)<600", "avg<250"],
		"http_req_duration{name:notifications}": ["p(95)<800", "avg<300"],
		"http_req_duration{name:profile}": ["p(95)<600", "avg<250"],

		// Пороги по сценариям
		"http_req_duration{scenario:stress_test}": ["p(95)<2000"],
		"http_req_duration{scenario:spike_test}": ["p(95)<3000"],

		// Успешность проверок по сценариям
		"checks{scenario:browse_organizations}": ["rate>0.90"],
		"checks{scenario:search_filter}": ["rate>0.85"],
		"checks{scenario:api_comprehensive}": ["rate>0.88"],
	},
};
