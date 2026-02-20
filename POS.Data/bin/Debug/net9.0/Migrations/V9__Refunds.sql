CREATE TABLE IF NOT EXISTS refunds (
    id            SERIAL PRIMARY KEY,
    order_id      INTEGER NOT NULL REFERENCES orders(id),
    amount        NUMERIC(18,2) NOT NULL,
    method        TEXT NOT NULL DEFAULT 'Cash',
    reason        TEXT,
    created_at    TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS ix_refunds_order_id ON refunds(order_id);
