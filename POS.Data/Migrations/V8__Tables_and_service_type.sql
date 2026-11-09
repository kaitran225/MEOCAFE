CREATE TABLE IF NOT EXISTS tables (
    id       INTEGER PRIMARY KEY AUTOINCREMENT,
    name     TEXT NOT NULL,
    capacity INTEGER NOT NULL DEFAULT 2,
    zone     TEXT,
    status   TEXT NOT NULL DEFAULT 'empty'
);

ALTER TABLE orders ADD COLUMN table_id INTEGER REFERENCES tables(id);
ALTER TABLE orders ADD COLUMN service_type TEXT DEFAULT 'Dine-in';
ALTER TABLE orders ADD COLUMN delivery_address TEXT;
ALTER TABLE orders ADD COLUMN delivery_fee REAL DEFAULT 0;
