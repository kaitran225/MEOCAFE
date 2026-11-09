-- Shift sessions: clock in/out with opening/closing cash
CREATE TABLE IF NOT EXISTS shift_sessions (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    username      TEXT NOT NULL,
    start_at      TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    end_at        TEXT,
    opening_cash  REAL NOT NULL DEFAULT 0,
    closing_cash  REAL
);

CREATE INDEX IF NOT EXISTS ix_shift_sessions_username ON shift_sessions(username);
CREATE INDEX IF NOT EXISTS ix_shift_sessions_start ON shift_sessions(start_at);

-- Audit log for void, refund, override
CREATE TABLE IF NOT EXISTS audit_log (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    username    TEXT NOT NULL,
    action      TEXT NOT NULL,
    entity_type TEXT,
    entity_id   INTEGER,
    details     TEXT,
    created_at  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS ix_audit_log_username ON audit_log(username);
CREATE INDEX IF NOT EXISTS ix_audit_log_created ON audit_log(created_at);
