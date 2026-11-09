CREATE TABLE IF NOT EXISTS problem_reports (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title TEXT NOT NULL,
    description TEXT NOT NULL DEFAULT '',
    category TEXT NOT NULL DEFAULT '',
    priority TEXT NOT NULL DEFAULT 'Medium',
    status TEXT NOT NULL DEFAULT 'Open',
    reported_by TEXT NOT NULL DEFAULT '',
    reported_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    resolution TEXT,
    resolved_at TEXT
);
