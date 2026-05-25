// Schedule view data — lessons of EN-B1-12 from Sep 2025 → May 2026
// Pattern: Пн / Ср · 18:00–19:30 · Каб. 204
// Today (in app): четверг, 14 мая 2026

const TODAY_GS_VIEW = '2026-05-14';

const SCHEDULE_PATTERN = {
  duration: 90,                       // min
  startTime: '18:00',
  endTime:   '19:30',
  weekdays:  [1, 3],                  // Mon, Wed
  weekdayLabels: ['Пн', 'Ср'],
  room: 'Каб. 204',
  starts: '2025-09-03',
  ends:   '2026-05-27',
};

const SCHEDULE_RU_MONTHS = ['Янв','Фев','Мар','Апр','Май','Июн',
  'Июл','Авг','Сен','Окт','Ноя','Дек'];
const SCHEDULE_RU_MONTHS_FULL = ['Январь','Февраль','Март','Апрель','Май','Июнь',
  'Июль','Август','Сентябрь','Октябрь','Ноябрь','Декабрь'];
const SCHEDULE_RU_WEEKDAYS_SHORT = ['Вс','Пн','Вт','Ср','Чт','Пт','Сб'];
const SCHEDULE_RU_WEEKDAYS_LONG  = ['воскресенье','понедельник','вторник','среда','четверг','пятница','суббота'];

// Cancellations & breaks (UI surfaces them as "пропуск")
// Format: { date: 'YYYY-MM-DD', reason }
const SCHEDULE_CANCELS = {
  '2025-12-29': 'Зимние каникулы',
  '2025-12-31': 'Зимние каникулы',
  '2026-01-05': 'Новогодние каникулы',
  '2026-01-07': 'Рождество Христово',
  '2026-02-23': 'День защитника Отечества',
  '2026-05-04': 'Майские праздники',
};

// Topics per lesson — by index (0-based), filling from program units in GRP_PROGRAM order.
// Each program unit gets N consecutive lessons; remaining go untitled (draft).
// We curate the first ~50 explicitly so the calendar reads "real".
// 71 topics — one per non-cancelled lesson (Sep 3 2025 → May 27 2026, Mon/Wed).
// Unit boundaries chosen so May 11+ falls on Unit 7 (matches GRP_RECENT_LESSONS).
const SCHEDULE_TOPICS = [
  // Sep 2025 — Unit 1 · Present tenses overview (idx 0–7, 8 lessons)
  'Course intro & needs analysis',
  'Present Simple — review',
  'Present Continuous — review',
  'State vs. action verbs',
  'Routines & habits',
  'Present Simple vs. Continuous · practice',
  'Speaking · daily routines',
  'Unit 1 — review & quiz',
  // Oct 2025 — Unit 2 · Past tenses & narrative (idx 8–16, 9 lessons)
  'Past Simple — overview',
  'Past Simple — irregular verbs',
  'Past Continuous · interrupted actions',
  'Used to / would',
  'Past Perfect',
  'Past Perfect Continuous',
  'Narrative tenses · mixed practice',
  'Storytelling project — prep',
  'Storytelling project — presentations',
  // Nov 2025 — Unit 3 · Future forms (idx 17–24, 8 lessons)
  'Will / be going to',
  'Present Continuous for future',
  'Future Simple vs. Continuous',
  'Future Continuous',
  'Future Perfect',
  'Predictions & arrangements',
  'Speaking · plans for the year',
  'Unit 3 — review',
  // Dec 2025 — Unit 4 · Modal verbs (idx 25–32, 8 lessons)
  'Ability & possibility',
  'Permission & obligation',
  'Advice & criticism',
  'Modals of deduction · present',
  'Modals of deduction · past',
  'Mixed practice',
  'Mid-course review',
  'Семестровая контрольная',
  // Jan 2026 — Unit 5 · Present Perfect Continuous (idx 33–38, 6 lessons)
  'Present Perfect — recap',
  'Present Perfect Continuous',
  'PPS vs. PPC',
  'For / since / how long',
  'Mixed practice & Q&A',
  'Unit 5 — review & quiz',
  // Feb–early May 2026 — Unit 6 · Conditionals (idx 39–64, 26 lessons)
  'Zero & first conditional — intro',
  'Zero conditional · practice',
  'First conditional — intro',
  'First conditional · practice',
  'Time clauses (when / as soon as / until)',
  'First conditional · speaking',
  'Second conditional — intro',
  'Second conditional · forms',
  'Second conditional · practice',
  'Wishes (I wish + past)',
  'If only / would rather',
  'Speaking · imaginary situations',
  'Third conditional — intro',
  'Third conditional · forms',
  'Third conditional · practice',
  'Past regrets · speaking',
  'Mixed conditionals — intro',
  'Mixed conditionals · practice',
  'Conditional review · grammar',
  'Conditional review · writing',
  'Mid-unit project',
  'Conditional review · listening',
  'Conditional review · reading',
  'Discussion',
  'Speaking practice',
  'Mini-project presentations',
  // May 2026 — Unit 7 · Reported speech (idx 65–70, 6 lessons)
  'Reported speech — overview',
  'Reported speech — intro',
  'Reported speech — practice',
  'Reported questions',
  'Reporting verbs',
  'Speaking · interviews',
];

// Unit index for each topic (parallel array). 71 entries.
const SCHEDULE_TOPIC_UNITS = [
  1,1,1,1,1,1,1,1,                     // 8 × Unit 1
  2,2,2,2,2,2,2,2,2,                   // 9 × Unit 2
  3,3,3,3,3,3,3,3,                     // 8 × Unit 3
  4,4,4,4,4,4,4,4,                     // 8 × Unit 4
  5,5,5,5,5,5,                         // 6 × Unit 5
  6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,  // 26 × Unit 6
  7,7,7,7,7,7,                         // 6 × Unit 7
];

// Per-lesson attendance (only completed lessons). Defaults to 11/11 if absent.
// Key: 'YYYY-MM-DD' → { present, late, absent }
const SCHEDULE_ATTENDANCE = {
  '2026-04-27': { present:10, late:0, absent:1 },
  '2026-04-29': { present:10, late:1, absent:0 },
  '2026-05-06': { present:9,  late:1, absent:1 },
  '2026-05-11': { present:10, late:0, absent:1 },
  '2026-05-13': { present:10, late:1, absent:0 },
};

// Lessons marked as "key" — for visual badge (★)
const SCHEDULE_KEY_LESSONS = new Set([
  '2025-09-29',  // Unit 1 review & quiz
  '2025-12-22',  // Mid-course review
  '2026-04-15',  // Mid-unit project
  '2026-05-06',  // Mini-project presentations
  '2026-05-27',  // Speaking · interviews (last lesson)
]);

// ── Generator ────────────────────────────────────────────────────────
function genScheduleLessons() {
  const start = new Date(SCHEDULE_PATTERN.starts + 'T00:00:00');
  const end   = new Date(SCHEDULE_PATTERN.ends + 'T00:00:00');
  const today = new Date(TODAY_GS_VIEW + 'T00:00:00');
  const cur = new Date(start);
  const out = [];
  let idx = 0;
  let safety = 0;
  while (cur <= end && safety < 500) {
    safety++;
    const dow = cur.getDay();
    if (SCHEDULE_PATTERN.weekdays.includes(dow)) {
      const y = cur.getFullYear();
      const m = String(cur.getMonth()+1).padStart(2,'0');
      const d = String(cur.getDate()).padStart(2,'0');
      const key = `${y}-${m}-${d}`;
      const cancelReason = SCHEDULE_CANCELS[key];
      // Fallback for safety if the curriculum array is ever shorter than the
      // generated lesson count — pick up where the last unit left off.
      const lastIdx = SCHEDULE_TOPICS.length - 1;
      const topic = SCHEDULE_TOPICS[idx] || `Practice & review · ${idx - lastIdx + 1}`;
      const unitN = SCHEDULE_TOPIC_UNITS[idx] || SCHEDULE_TOPIC_UNITS[lastIdx] || null;
      const att = SCHEDULE_ATTENDANCE[key];

      let status;
      if (cancelReason) status = 'cancelled';
      else if (cur < today) status = 'done';
      else if (key === TODAY_GS_VIEW) status = 'today';
      else status = 'upcoming';

      out.push({
        id: idx + 1,
        date: key,
        weekday: dow,
        startTime: SCHEDULE_PATTERN.startTime,
        endTime:   SCHEDULE_PATTERN.endTime,
        duration:  SCHEDULE_PATTERN.duration,
        room:      SCHEDULE_PATTERN.room,
        topic: cancelReason ? cancelReason : topic,
        unit: cancelReason ? null : unitN,
        unitTitle: cancelReason ? null : programUnitTitle(unitN),
        status,
        attendance: att || null,
        isKey: SCHEDULE_KEY_LESSONS.has(key),
        cancelReason: cancelReason || null,
      });
      if (!cancelReason) idx++;
    }
    cur.setDate(cur.getDate() + 1);
  }
  return out;
}

function programUnitTitle(n) {
  if (!n) return null;
  const titles = {
    1: 'Present tenses overview',
    2: 'Past tenses & narrative',
    3: 'Future forms',
    4: 'Modal verbs',
    5: 'Present Perfect Continuous',
    6: 'Conditionals',
    7: 'Reported speech',
    8: 'Passive voice',
    9: 'Articles & determiners',
    10:'Final review + exam',
  };
  return titles[n] || null;
}

const SCHEDULE_LESSONS = genScheduleLessons();

// ── Aggregates for the page ──────────────────────────────────────────
const SCHEDULE_STATS = (() => {
  const total = SCHEDULE_LESSONS.length;
  const done = SCHEDULE_LESSONS.filter(l => l.status === 'done').length;
  const cancelled = SCHEDULE_LESSONS.filter(l => l.status === 'cancelled').length;
  const today = SCHEDULE_LESSONS.filter(l => l.status === 'today').length;
  const upcoming = SCHEDULE_LESSONS.filter(l => l.status === 'upcoming').length + today;
  const heldTotal = total - cancelled;
  const hoursDone = done * SCHEDULE_PATTERN.duration / 60;
  const hoursTotal = heldTotal * SCHEDULE_PATTERN.duration / 60;
  // months covered for nav
  const monthSet = {};
  SCHEDULE_LESSONS.forEach(l => {
    const k = l.date.slice(0,7);
    if (!monthSet[k]) monthSet[k] = { key:k, count:0, done:0, cancelled:0 };
    monthSet[k].count++;
    if (l.status === 'done') monthSet[k].done++;
    if (l.status === 'cancelled') monthSet[k].cancelled++;
  });
  const months = Object.values(monthSet).sort((a,b) => a.key.localeCompare(b.key));
  return { total, done, cancelled, upcoming, heldTotal, hoursDone, hoursTotal, months };
})();

// Next upcoming lesson (today or future, not cancelled)
const SCHEDULE_NEXT = SCHEDULE_LESSONS.find(l =>
  (l.status === 'today' || l.status === 'upcoming')
);

Object.assign(window, {
  TODAY_GS_VIEW, SCHEDULE_PATTERN, SCHEDULE_LESSONS, SCHEDULE_STATS, SCHEDULE_NEXT,
  SCHEDULE_RU_MONTHS, SCHEDULE_RU_MONTHS_FULL,
  SCHEDULE_RU_WEEKDAYS_SHORT, SCHEDULE_RU_WEEKDAYS_LONG,
  SCHEDULE_CANCELS,
});
