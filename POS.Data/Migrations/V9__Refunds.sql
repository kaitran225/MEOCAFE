CREATE TABLE IF NOT EXISTS refunds (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    order_id      INTEGER NOT NULL REFERENCES orders(id),
    amount        REAL NOT NULL,
    method        TEXT NOT NULL DEFAULT 'Cash',
    reason        TEXT,
    created_at    TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS ix_refunds_order_id ON refunds(order_id);
