-- Drink recipes: default per menu item + optional variants by note (structured: ingredients + steps).

CREATE TABLE IF NOT EXISTS drink_recipes (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    menu_item_id  INTEGER NOT NULL REFERENCES menu_items(id) ON DELETE CASCADE,
    variant_note  TEXT,
    ingredients   TEXT NOT NULL DEFAULT '[]',
    steps         TEXT NOT NULL DEFAULT '[]'
);

CREATE UNIQUE INDEX IF NOT EXISTS uq_drink_recipes_menu_item_variant
  ON drink_recipes (menu_item_id, COALESCE(variant_note, ''));
CREATE INDEX IF NOT EXISTS ix_drink_recipes_menu_item_id ON drink_recipes(menu_item_id);

-- Seed a few default recipes for demo (Espresso, Latte, Americano)
INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Espresso 2 shot", "Water 60ml"]', '["Pull double shot", "Add hot water"]'
FROM menu_items m WHERE m.name = 'Americano' AND NOT EXISTS (SELECT 1 FROM drink_recipes r WHERE r.menu_item_id = m.id AND r.variant_note IS NULL) LIMIT 1;

INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Espresso 1 shot", "Steamed milk 180ml", "Foam"]', '["Pull shot", "Steam milk", "Pour milk, top with foam"]'
FROM menu_items m WHERE m.name = 'Latte' AND NOT EXISTS (SELECT 1 FROM drink_recipes r WHERE r.menu_item_id = m.id AND r.variant_note IS NULL) LIMIT 1;

INSERT INTO drink_recipes (menu_item_id, variant_note, ingredients, steps)
SELECT m.id, NULL, '["Espresso 2 shot"]', '["Grind 18g", "Tamp", "Pull double shot 25-30s"]'
FROM menu_items m WHERE m.name = 'Espresso' AND NOT EXISTS (SELECT 1 FROM drink_recipes r WHERE r.menu_item_id = m.id AND r.variant_note IS NULL) LIMIT 1;
