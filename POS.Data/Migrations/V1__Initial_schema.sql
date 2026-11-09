-- Flyway-style migration V1: Initial schema for HienXanh POS
-- Run once per database; DbUp tracks applied scripts in schemaversions.

CREATE TABLE IF NOT EXISTS categories (
    id   INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS menu_items (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    name         TEXT NOT NULL,
    sell_price   REAL NOT NULL,
    capital_price REAL NOT NULL,
    quantity     INTEGER NOT NULL DEFAULT 0,
    image        BLOB,
    category_id  INTEGER NOT NULL REFERENCES categories(id)
);

CREATE TABLE IF NOT EXISTS combo_menu_items (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    name        TEXT NOT NULL,
    combo_price REAL NOT NULL,
    description TEXT,
    quantity    INTEGER NOT NULL DEFAULT 0,
    image       BLOB
);

CREATE TABLE IF NOT EXISTS combo_items (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    combo_id     INTEGER NOT NULL REFERENCES combo_menu_items(id) ON DELETE CASCADE,
    menu_item_id INTEGER NOT NULL REFERENCES menu_items(id)
);

CREATE TABLE IF NOT EXISTS discounts (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    name        TEXT NOT NULL,
    description TEXT,
    percentage  REAL NOT NULL,
    is_disabled INTEGER NOT NULL DEFAULT 0,
    start_date  TEXT NOT NULL,
    end_date    TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS menu_item_discount (
    menu_item_id INTEGER NOT NULL REFERENCES menu_items(id) ON DELETE CASCADE,
    discount_id  INTEGER NOT NULL REFERENCES discounts(id) ON DELETE CASCADE,
    PRIMARY KEY (menu_item_id, discount_id)
);

CREATE TABLE IF NOT EXISTS employee (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    fullname     TEXT NOT NULL,
    username     TEXT NOT NULL UNIQUE,
    password     TEXT NOT NULL,
    phone_number TEXT,
    gender       TEXT,
    address      TEXT,
    dob          TEXT,
    role         TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS customer (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    name         TEXT,
    phone_number TEXT NOT NULL UNIQUE,
    point        INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS orders (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    total_price REAL NOT NULL,
    phone_number TEXT,
    created_at  TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    note        TEXT
);

CREATE TABLE IF NOT EXISTS order_details (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    order_id     INTEGER NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    menu_item_id INTEGER NOT NULL REFERENCES menu_items(id),
    quantity     INTEGER NOT NULL,
    price        REAL NOT NULL,
    is_combo     INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS shifts (
    _day               TEXT NOT NULL,
    shift              INTEGER NOT NULL,
    employees_username TEXT NOT NULL DEFAULT '',
    PRIMARY KEY (_day, shift)
);

CREATE TABLE IF NOT EXISTS "ShiftCheck" (
    id                INTEGER PRIMARY KEY AUTOINCREMENT,
    _day              TEXT NOT NULL,
    employees_username TEXT NOT NULL,
    checkin           TEXT,
    checkout          TEXT,
    status            TEXT
);

CREATE INDEX IF NOT EXISTS ix_order_details_order_id ON order_details(order_id);
CREATE INDEX IF NOT EXISTS ix_orders_created_at ON orders(created_at);
CREATE INDEX IF NOT EXISTS ix_employee_username ON employee(username);
CREATE INDEX IF NOT EXISTS ix_ShiftCheck_day_username ON "ShiftCheck"(_day, employees_username);
