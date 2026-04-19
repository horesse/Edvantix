import type { TestData } from "../types";
import type { SeededRandom } from "./seeded-random";

/**
 * Тестовые данные для реалистичного моделирования нагрузки в Edvantix
 */
export const TEST_DATA: TestData = {
	searchTerms: [
		"школа",
		"университет",
		"лицей",
		"колледж",
		"гимназия",
		"центр",
		"онлайн",
		"технология",
		"образование",
		"академия",
	],
	skillQueries: [
		"JavaScript",
		"Python",
		"React",
		"TypeScript",
		"SQL",
		".NET",
		"C#",
		"Kotlin",
		"Английский",
		"Дизайн",
	],
	// OrganizationType: 0=EducationalInstitution, 1=GeneralEducationSchool, 2=Lyceum, ...
	organizationTypes: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16],
	// OrganizationStatus: 0=Active, 1=Archived
	organizationStatuses: [0, 1],
	pageSizes: [5, 10, 15, 20, 25],
};

/**
 * Генераторы случайных тестовых данных с фиксированным сидом для воспроизводимых прогонов
 */
export class TestDataGenerator {
	constructor(private readonly random: SeededRandom) {}

	getRandomSearchTerm(): string {
		return this.random.pick(TEST_DATA.searchTerms);
	}

	getRandomSkillQuery(): string {
		return this.random.pick(TEST_DATA.skillQueries);
	}

	getRandomOrganizationType(): number {
		return this.random.pick(TEST_DATA.organizationTypes);
	}

	getRandomOrganizationStatus(): number {
		return this.random.pick(TEST_DATA.organizationStatuses);
	}

	getRandomPageSize(): number {
		return this.random.pick(TEST_DATA.pageSizes);
	}

	getRandomPageIndex(): number {
		return this.random.int(1, 5);
	}
}
