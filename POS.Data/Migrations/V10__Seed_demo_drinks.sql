-- Seed demo categories and drinks for HienXanh POS (SQLite).

INSERT INTO categories (name) SELECT 'Coffee' WHERE NOT EXISTS (SELECT 1 FROM categories WHERE name = 'Coffee');
INSERT INTO categories (name) SELECT 'Tea' WHERE NOT EXISTS (SELECT 1 FROM categories WHERE name = 'Tea');
INSERT INTO categories (name) SELECT 'Cold Drinks' WHERE NOT EXISTS (SELECT 1 FROM categories WHERE name = 'Cold Drinks');
INSERT INTO categories (name) SELECT 'Smoothies & Blended' WHERE NOT EXISTS (SELECT 1 FROM categories WHERE name = 'Smoothies & Blended');
INSERT INTO categories (name) SELECT 'Other' WHERE NOT EXISTS (SELECT 1 FROM categories WHERE name = 'Other');

INSERT INTO menu_items (name, sell_price, capital_price, quantity, image, category_id, reorder_level)
SELECT 'Espresso', 35000, 12000, 100, NULL, c.id, 10 FROM categories c WHERE c.name = 'Coffee' AND NOT EXISTS (SELECT 1 FROM menu_items WHERE name = 'Espresso');
INSERT INTO menu_items (name, sell_price, capital_price, quantity, image, category_id, reorder_level)
SELECT 'Americano', 38000, 14000, 100, NULL, c.id, 10 FROM categories c WHERE c.name = 'Coffee' AND NOT EXISTS (SELECT 1 FROM menu_items WHERE name = 'Americano');
INSERT INTO menu_items (name, sell_price, capital_price, quantity, image, category_id, reorder_level)
SELECT 'Latte', 42000, 16000, 100, NULL, c.id, 10 FROM categories c WHERE c.name = 'Coffee' AND NOT EXISTS (SELECT 1 FROM menu_items WHERE name = 'Latte');
INSERT INTO menu_items (name, sell_price, capital_price, quantity, image, category_id, reorder_level)
SELECT 'Cappuccino', 42000, 16000, 100, NULL, c.id, 10 FROM categories c WHERE c.name = 'Coffee' AND NOT EXISTS (SELECT 1 FROM menu_items WHERE name = 'Cappuccino');
INSERT INTO menu_items (name, sell_price, capital_price, quantity, image, category_id, reorder_level)
SELECT 'Mocha', 45000, 18000, 100, NULL, c.id, 10 FROM categories c WHERE c.name = 'Coffee' AND NOT EXISTS (SELECT 1 FROM menu_items WHERE name = 'Mocha');
INSERT INTO menu_items (name, sell_price, capital_price, quantity, image, category_id, reorder_level)
SELECT 'Matcha Latte', 48000, 20000, 100, NULL, c.id, 10 FROM categories c WHERE c.name = 'Tea' AND NOT EXISTS (SELECT 1 FROM menu_items WHERE name = 'Matcha Latte');
INSERT INTO menu_items (name, sell_price, capital_price, quantity, image, category_id, reorder_level)
SELECT 'Cà phê sữa đá', 35000, 12000, 100, NULL, c.id, 10 FROM categories c WHERE c.name = 'Coffee' AND NOT EXISTS (SELECT 1 FROM menu_items WHERE name = 'Cà phê sữa đá');
INSERT INTO menu_items (name, sell_price, capital_price, quantity, image, category_id, reorder_level)
SELECT 'Trà đào cam sả', 45000, 16000, 100, NULL, c.id, 10 FROM categories c WHERE c.name = 'Tea' AND NOT EXISTS (SELECT 1 FROM menu_items WHERE name = 'Trà đào cam sả');
INSERT INTO menu_items (name, sell_price, capital_price, quantity, image, category_id, reorder_level)
SELECT 'Bạc xỉu', 38000, 13000, 100, NULL, c.id, 10 FROM categories c WHERE c.name = 'Coffee' AND NOT EXISTS (SELECT 1 FROM menu_items WHERE name = 'Bạc xỉu');

INSERT INTO orders (total_price, phone_number, created_at, note, kitchen_status)
SELECT 126000, '0901234567', datetime('now', '-2 days'), NULL, 'done'
WHERE NOT EXISTS (SELECT 1 FROM orders);

INSERT INTO order_details (order_id, menu_item_id, quantity, price, is_combo)
SELECT o.id, m.id, 2, 42000, 0
FROM orders o, menu_items m
WHERE o.id = (SELECT id FROM orders ORDER BY id LIMIT 1)
  AND m.name = 'Latte'
  AND NOT EXISTS (SELECT 1 FROM order_details);
