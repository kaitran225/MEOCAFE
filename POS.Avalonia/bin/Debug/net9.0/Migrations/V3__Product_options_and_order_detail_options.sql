-- Product option definitions and order line options (Section 4.2).

CREATE TABLE IF NOT EXISTS product_option_groups (
    id   SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    option_type TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS product_option_values (
    id             SERIAL PRIMARY KEY,
    option_group_id INTEGER NOT NULL REFERENCES product_option_groups(id) ON DELETE CASCADE,
    name           TEXT NOT NULL,
    extra_price    NUMERIC(18,2)
);

CREATE TABLE IF NOT EXISTS menu_item_option_groups (
    menu_item_id     INTEGER NOT NULL REFERENCES menu_items(id) ON DELETE CASCADE,
    option_group_id  INTEGER NOT NULL REFERENCES product_option_groups(id) ON DELETE CASCADE,
    PRIMARY KEY (menu_item_id, option_group_id)
);

CREATE TABLE IF NOT EXISTS order_detail_options (
    id             SERIAL PRIMARY KEY,
    order_detail_id INTEGER NOT NULL REFERENCES order_details(id) ON DELETE CASCADE,
    option_type    TEXT NOT NULL,
    option_value_id INTEGER REFERENCES product_option_values(id),
    note           TEXT
);

CREATE INDEX IF NOT EXISTS ix_order_detail_options_order_detail_id ON order_detail_options(order_detail_id);
